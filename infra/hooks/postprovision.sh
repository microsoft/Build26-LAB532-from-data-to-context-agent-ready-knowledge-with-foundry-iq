#!/bin/sh
set -e

echo "Running postprovision hook..."

# Fetch API keys (not available as Bicep outputs)
SEARCH_ADMIN_KEY=$(az search admin-key show \
    --resource-group "$AZURE_RESOURCE_GROUP" \
    --service-name "$AZURE_SEARCH_SERVICE_NAME" \
    --query primaryKey -o tsv)

OPENAI_KEY=$(az cognitiveservices account keys list \
    --resource-group "$AZURE_RESOURCE_GROUP" \
    --name "$AZURE_OPENAI_SERVICE_NAME" \
    --query key1 -o tsv)

# Get tenant ID for notebooks that need it (Parts 3, 4, 5)
TENANT_ID=$(az account show --query tenantId -o tsv)

# Write .env file for notebooks
cat > .env << EOF
# Azure AI Search Configuration
AZURE_SEARCH_SERVICE_ENDPOINT=${AZURE_SEARCH_SERVICE_ENDPOINT}
AZURE_SEARCH_ADMIN_KEY=${SEARCH_ADMIN_KEY}

# Azure OpenAI Configuration
AZURE_OPENAI_ENDPOINT=${AZURE_OPENAI_ENDPOINT}
AZURE_OPENAI_KEY=${OPENAI_KEY}
AZURE_OPENAI_CHATGPT_DEPLOYMENT=${AZURE_OPENAI_CHATGPT_DEPLOYMENT}
AZURE_OPENAI_CHATGPT_MODEL_NAME=gpt-5.4
AZURE_OPENAI_EMBEDDING_DEPLOYMENT=${AZURE_OPENAI_EMBEDDING_DEPLOYMENT}

# Tenant and project configuration
AZURE_TENANT_ID=${TENANT_ID}

# Fabric configuration (populated by lakehouse setup if capacity was deployed)
FABRIC_CAPACITY_ID=${FABRIC_CAPACITY_ID}

# GitHub Token (optional for Part 5 MCP server scenarios)
# GITHUB_TOKEN=
EOF

echo "Created .env file"

# Create indexes and upload data
echo "Creating search indexes and uploading data..."
if [ -f "infra/deploy-yourself/create-indexes.py" ]; then
    python3 -m pip install -r notebooks/requirements.txt --quiet 2>/dev/null
    python3 infra/deploy-yourself/create-indexes.py
    echo "Indexes created and data uploaded"
elif [ -f "infra/create-knowledge.py" ]; then
    python3 -m pip install -r notebooks/requirements.txt --quiet 2>/dev/null
    python3 infra/create-knowledge.py
    echo "Knowledge base setup complete"
else
    echo "No index creation script found, skipping data upload"
fi

echo "Postprovision complete! Open notebooks/ to start the lab."

# Set up Fabric Lakehouse (if capacity was deployed)
if [ -n "$FABRIC_CAPACITY_ID" ]; then
    echo "Setting up Fabric Lakehouse..."
    if [ -f "infra/create-lakehouse.py" ]; then
        # create-lakehouse.py reads config from .env via load_dotenv()
        python3 infra/create-lakehouse.py
        echo "Fabric Lakehouse setup complete"
    else
        echo "WARNING: create-lakehouse.py not found, skipping lakehouse setup"
    fi
fi

# Note: Email seeding (seed-emails.ps1) requires a service principal with
# Mail.Send application permission and is only used in the Skillable hosted lab.
# For self-deploy, Part 4 (Work IQ) will use your own mailbox data.
