$ErrorActionPreference = "Stop"

Write-Host "Running postprovision hook..."

# Install Python dependencies first (needed for key fetching below)
python -m pip install -r notebooks\requirements.txt --quiet 2>$null

# Write .env and fetch API keys using azure-identity (no az CLI needed)
python infra\setup-env.py

# Create indexes and upload data
Write-Host "Creating search indexes and uploading data..."
python infra\create-knowledge.py
Write-Host "Indexes created and data uploaded"

# Set up Fabric Lakehouse (if capacity was deployed)
if ($env:FABRIC_CAPACITY_ID) {
    Write-Host "Setting up Fabric Lakehouse..."
    python infra\create-lakehouse.py
    Write-Host "Fabric Lakehouse setup complete"
}

Write-Host "Postprovision complete! Open notebooks/ to start the lab."
