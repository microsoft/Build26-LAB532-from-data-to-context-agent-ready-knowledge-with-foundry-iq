"""
Create Fabric Lakehouse and load Zava DIY dataset as Delta tables.

This script:
1. Creates a lakehouse in Microsoft Fabric using the REST API
2. Downloads the Zava DIY dataset (product_data.json, reference_data.json) from GitHub
3. Flattens JSON data into CSV files
4. Uploads CSVs to OneLake (lakehouse Files section)
5. Loads CSVs as Delta tables using the Load Table API

Environment variables (from .env):
  FABRIC_WORKSPACE_ID  - Your Fabric workspace GUID (required)
  LAKEHOUSE_NAME       - Name for the lakehouse (default: zava-diy-lakehouse)
  INCLUDE_EMBEDDINGS   - Include vector embeddings in products table (default: false)
"""

import csv
import io
import json
import os
import sys
import time
import traceback
from datetime import datetime
from pathlib import Path

import requests
from azure.identity import DefaultAzureCredential
from azure.storage.filedatalake import DataLakeServiceClient
from dotenv import load_dotenv

load_dotenv(override=True)

# Configuration
FABRIC_API_BASE = "https://api.fabric.microsoft.com/v1"
ONELAKE_DFS_URL = "https://onelake.dfs.fabric.microsoft.com"
FABRIC_SCOPE = "https://api.fabric.microsoft.com/.default"
STORAGE_SCOPE = "https://storage.azure.com/.default"

GITHUB_RAW_BASE = (
    "https://raw.githubusercontent.com/microsoft/ai-tour-26-zava-diy-dataset-plus-mcp"
    "/main/data/database"
)

WORKSPACE_ID = os.getenv("FABRIC_WORKSPACE_ID", "")
LAKEHOUSE_NAME = os.getenv("LAKEHOUSE_NAME", "ZavaDIYLakehouse")
WORKSPACE_NAME = os.getenv("FABRIC_WORKSPACE_NAME", "ZavaDIYWorkspace")
FABRIC_CAPACITY_ID = os.getenv("FABRIC_CAPACITY_ID", "")
INCLUDE_EMBEDDINGS = os.getenv("INCLUDE_EMBEDDINGS", "false").lower() == "true"

# Logging
LOG_FILE = os.path.join(os.path.dirname(os.path.abspath(__file__)), "create-lakehouse.log")


def log_message(message: str):
    """Write message to log file and stdout with timestamp."""
    os.makedirs(os.path.dirname(LOG_FILE), exist_ok=True)
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    line = f"[{timestamp}] {message}"
    print(line)
    with open(LOG_FILE, "a", encoding="utf-8") as f:
        f.write(line + "\n")


def get_fabric_token() -> str:
    """Get an access token for Fabric API."""
    credential = DefaultAzureCredential()
    token = credential.get_token(FABRIC_SCOPE)
    return token.token


def get_storage_token() -> str:
    """Get an access token for OneLake (Azure Storage)."""
    credential = DefaultAzureCredential()
    token = credential.get_token(STORAGE_SCOPE)
    return token.token


def fabric_headers() -> dict:
    """Return headers for Fabric API calls."""
    return {
        "Authorization": f"Bearer {get_fabric_token()}",
        "Content-Type": "application/json",
    }


def resolve_capacity_id(capacity_id_or_arm: str) -> str:
    """Resolve ARM resource ID or Fabric capacity GUID to the Fabric GUID."""
    # If it's already a GUID (no slashes), return as-is
    if "/" not in capacity_id_or_arm:
        return capacity_id_or_arm

    # It's an ARM resource ID — look up the Fabric GUID via the capacities API
    log_message("Resolving ARM capacity ID to Fabric GUID...")
    url = f"{FABRIC_API_BASE}/capacities"
    resp = requests.get(url, headers=fabric_headers())
    resp.raise_for_status()

    # Extract capacity name from ARM ID (last segment)
    arm_name = capacity_id_or_arm.rstrip("/").split("/")[-1]
    for cap in resp.json().get("value", []):
        if cap["displayName"] == arm_name:
            log_message(f"Resolved capacity: {cap['id']} ({cap['displayName']})")
            return cap["id"]

    log_message(f"ERROR: Could not find Fabric capacity matching '{arm_name}'")
    sys.exit(1)


def create_workspace(name: str, capacity_id: str) -> dict:
    """Create a Fabric workspace assigned to the given capacity."""
    url = f"{FABRIC_API_BASE}/workspaces"
    payload = {"displayName": name, "capacityId": capacity_id}
    log_message(f"Creating workspace '{name}' on capacity {capacity_id[:12]}...")
    resp = requests.post(url, headers=fabric_headers(), json=payload)

    if resp.status_code == 201:
        data = resp.json()
        log_message(f"Workspace created: {data['id']}")
        return data
    elif resp.status_code == 409:
        log_message(f"Workspace '{name}' already exists. Fetching existing...")
        return get_existing_workspace(name)
    else:
        log_message(f"ERROR: Failed to create workspace: {resp.status_code} - {resp.text}")
        sys.exit(1)


def get_existing_workspace(name: str) -> dict:
    """Find an existing workspace by name."""
    url = f"{FABRIC_API_BASE}/workspaces"
    resp = requests.get(url, headers=fabric_headers())
    resp.raise_for_status()
    for ws in resp.json().get("value", []):
        if ws["displayName"] == name:
            log_message(f"Found existing workspace: {ws['id']}")
            return ws
    log_message(f"ERROR: Workspace '{name}' not found.")
    sys.exit(1)


def create_lakehouse(workspace_id: str, name: str) -> dict:
    """Create a lakehouse in the specified workspace."""
    url = f"{FABRIC_API_BASE}/workspaces/{workspace_id}/lakehouses"
    payload = {"displayName": name}
    log_message(f"Creating lakehouse '{name}'...")
    resp = requests.post(url, headers=fabric_headers(), json=payload)

    if resp.status_code == 201:
        data = resp.json()
        log_message(f"Lakehouse created: {data['id']}")
        return data
    elif resp.status_code == 409:
        log_message(f"Lakehouse '{name}' already exists. Fetching existing...")
        return get_existing_lakehouse(workspace_id, name)
    else:
        log_message(f"ERROR: Failed to create lakehouse: {resp.status_code} - {resp.text}")
        sys.exit(1)


def get_existing_lakehouse(workspace_id: str, name: str) -> dict:
    """Find an existing lakehouse by name."""
    url = f"{FABRIC_API_BASE}/workspaces/{workspace_id}/lakehouses"
    resp = requests.get(url, headers=fabric_headers())
    resp.raise_for_status()
    for lh in resp.json().get("value", []):
        if lh["displayName"] == name:
            log_message(f"Found existing lakehouse: {lh['id']}")
            return lh
    log_message(f"ERROR: Lakehouse '{name}' not found in workspace.")
    sys.exit(1)


def get_lakehouse_properties(workspace_id: str, lakehouse_id: str) -> dict:
    """Get lakehouse properties including OneLake paths."""
    url = f"{FABRIC_API_BASE}/workspaces/{workspace_id}/lakehouses/{lakehouse_id}"
    resp = requests.get(url, headers=fabric_headers())
    resp.raise_for_status()
    return resp.json()


def download_json(filename: str) -> dict:
    """Download a JSON file from the GitHub repository."""
    url = f"{GITHUB_RAW_BASE}/{filename}"
    log_message(f"Downloading {filename}...")
    resp = requests.get(url, timeout=120)
    resp.raise_for_status()
    return resp.json()


def flatten_products(product_data: dict) -> list[dict]:
    """Flatten nested product_data.json into a flat list of product records."""
    rows = []
    categories = product_data.get("main_categories", {})
    for category_name, category_data in categories.items():
        seasonal = category_data.get("washington_seasonal_multipliers", [])
        seasonal_str = ";".join(str(s) for s in seasonal) if seasonal else ""

        for product_type_name, products in category_data.items():
            if product_type_name == "washington_seasonal_multipliers":
                continue
            if not isinstance(products, list):
                continue
            for product in products:
                row = {
                    "category": category_name,
                    "product_type": product_type_name,
                    "name": product.get("name", ""),
                    "sku": product.get("sku", ""),
                    "price": product.get("price", 0),
                    "description": product.get("description", ""),
                    "stock_level": product.get("stock_level", 0),
                    "image_path": product.get("image_path", ""),
                    "seasonal_multipliers": seasonal_str,
                }
                if INCLUDE_EMBEDDINGS:
                    img_emb = product.get("image_embedding", [])
                    desc_emb = product.get("description_embedding", [])
                    row["image_embedding"] = json.dumps(img_emb) if img_emb else ""
                    row["description_embedding"] = (
                        json.dumps(desc_emb) if desc_emb else ""
                    )
                rows.append(row)
    return rows


def flatten_stores(reference_data: dict) -> list[dict]:
    """Flatten stores from reference_data.json."""
    rows = []
    for store_name, config in reference_data.get("stores", {}).items():
        rows.append(
            {
                "store_name": store_name,
                "rls_user_id": config.get("rls_user_id", ""),
                "customer_distribution_weight": config.get(
                    "customer_distribution_weight", 0
                ),
                "order_frequency_multiplier": config.get(
                    "order_frequency_multiplier", 0
                ),
                "order_value_multiplier": config.get("order_value_multiplier", 0),
            }
        )
    return rows


def flatten_year_weights(reference_data: dict) -> list[dict]:
    """Flatten year weights from reference_data.json."""
    rows = []
    for year, weight in reference_data.get("year_weights", {}).items():
        rows.append({"year": int(year), "weight": weight})
    return rows


def flatten_categories(product_data: dict) -> list[dict]:
    """Extract unique categories with their seasonal multipliers."""
    rows = []
    categories = product_data.get("main_categories", {})
    for category_name, category_data in categories.items():
        seasonal = category_data.get("washington_seasonal_multipliers", [])
        row = {"category_name": category_name}
        for i, month in enumerate(
            ["jan", "feb", "mar", "apr", "may", "jun",
             "jul", "aug", "sep", "oct", "nov", "dec"]
        ):
            row[f"multiplier_{month}"] = seasonal[i] if i < len(seasonal) else 1.0
        rows.append(row)
    return rows


def to_csv_bytes(rows: list[dict]) -> bytes:
    """Convert a list of dicts to CSV bytes."""
    if not rows:
        return b""
    output = io.StringIO()
    writer = csv.DictWriter(output, fieldnames=rows[0].keys())
    writer.writeheader()
    writer.writerows(rows)
    return output.getvalue().encode("utf-8")


def upload_to_onelake(
    workspace_id: str, lakehouse_id: str, filename: str, data: bytes
):
    """Upload a file to the lakehouse Files section via OneLake ADLS SDK."""
    credential = DefaultAzureCredential()
    service_client = DataLakeServiceClient(
        account_url=ONELAKE_DFS_URL, credential=credential
    )

    filesystem_name = workspace_id
    directory_path = f"{lakehouse_id}/Files"

    file_system_client = service_client.get_file_system_client(filesystem_name)
    directory_client = file_system_client.get_directory_client(directory_path)
    file_client = directory_client.get_file_client(filename)

    log_message(f"Uploading {filename} ({len(data):,} bytes)...")
    file_client.upload_data(data, overwrite=True)
    log_message(f"Uploaded {filename}")


def load_table(
    workspace_id: str, lakehouse_id: str, table_name: str, filename: str
) -> str:
    """Submit a Load Table request and return the full polling URL."""
    url = (
        f"{FABRIC_API_BASE}/workspaces/{workspace_id}"
        f"/lakehouses/{lakehouse_id}/tables/{table_name}/load"
    )
    payload = {
        "relativePath": f"Files/{filename}",
        "pathType": "File",
        "mode": "Overwrite",
        "formatOptions": {"header": True, "delimiter": ",", "format": "Csv"},
    }
    log_message(f"Loading table '{table_name}' from {filename}...")
    resp = requests.post(url, headers=fabric_headers(), json=payload)

    if resp.status_code == 202:
        location = resp.headers.get("Location", "")
        log_message(f"Load initiated for '{table_name}'")
        return location
    else:
        log_message(f"ERROR: Load failed for '{table_name}': {resp.status_code} - {resp.text}")
        return ""


def poll_operation(
    workspace_id: str, lakehouse_id: str, poll_url: str, timeout: int = 300
) -> bool:
    """Poll a load operation using the Location URL until complete or timeout."""
    if not poll_url:
        return False

    start = time.time()
    while time.time() - start < timeout:
        resp = requests.get(poll_url, headers=fabric_headers())
        if resp.status_code == 200:
            data = resp.json()
            status = data.get("status", data.get("Status", ""))
            percent = data.get("percentComplete", data.get("PercentComplete", 0))

            if status in ("Succeeded", "succeeded", 3):
                log_message(f"  Operation complete (100%)")
                return True
            elif status in ("Failed", "failed", 4):
                error = data.get("error", data.get("Error", "Unknown error"))
                log_message(f"  Operation FAILED: {error}")
                return False
            else:
                time.sleep(10)
        elif resp.status_code == 202:
            # Still in progress
            time.sleep(10)
        else:
            log_message(f"  Poll response: {resp.status_code}")
            time.sleep(10)

    log_message(f"  Operation TIMEOUT after {timeout}s")
    return False


def main():
    """Main execution flow."""
    log_message("=" * 60)
    log_message("Fabric Lakehouse Creator - Zava DIY Dataset")
    log_message("=" * 60)

    # Resolve workspace: use provided ID, or create one on the given capacity
    workspace_id = WORKSPACE_ID
    if not workspace_id and FABRIC_CAPACITY_ID:
        log_message("No workspace ID provided. Creating workspace on Fabric capacity...")
        capacity_guid = resolve_capacity_id(FABRIC_CAPACITY_ID)
        ws = create_workspace(WORKSPACE_NAME, capacity_guid)
        workspace_id = ws["id"]
    elif not workspace_id:
        workspace_id = input("Enter your Fabric Workspace ID: ").strip()
        if not workspace_id:
            log_message("ERROR: Workspace ID is required (or set FABRIC_CAPACITY_ID to auto-create).")
            sys.exit(1)

    log_message(f"Workspace ID: {workspace_id}")
    log_message(f"Lakehouse Name: {LAKEHOUSE_NAME}")
    log_message(f"Include Embeddings: {INCLUDE_EMBEDDINGS}")

    try:
        # Step 1: Create Lakehouse
        log_message("\n[1/5] Creating Lakehouse")
        lakehouse = create_lakehouse(workspace_id, LAKEHOUSE_NAME)
        lakehouse_id = lakehouse["id"]

        # Step 2: Get lakehouse properties
        log_message("\n[2/5] Getting Lakehouse Properties")
        props = get_lakehouse_properties(workspace_id, lakehouse_id)
        onelake_files = props.get("properties", {}).get("oneLakeFilesPath", "")
        log_message(f"OneLake Files Path: {onelake_files}")

        # Step 3: Download and process data
        log_message("\n[3/5] Downloading and Processing Dataset")
        product_data = download_json("product_data.json")
        reference_data = download_json("reference_data.json")

        tables = {
            "products": (flatten_products(product_data), "products.csv"),
            "categories": (flatten_categories(product_data), "categories.csv"),
            "stores": (flatten_stores(reference_data), "stores.csv"),
            "year_weights": (flatten_year_weights(reference_data), "year_weights.csv"),
        }

        for table_name, (rows, _) in tables.items():
            log_message(f"  {table_name}: {len(rows)} rows")

        # Step 4: Upload CSVs to OneLake
        log_message("\n[4/5] Uploading CSVs to OneLake")
        for table_name, (rows, filename) in tables.items():
            csv_data = to_csv_bytes(rows)
            upload_to_onelake(workspace_id, lakehouse_id, filename, csv_data)

        # Step 5: Load tables
        log_message("\n[5/5] Loading Delta Tables")
        operations = {}
        for table_name, (_, filename) in tables.items():
            poll_url = load_table(workspace_id, lakehouse_id, table_name, filename)
            if poll_url:
                operations[table_name] = poll_url

        # Poll all operations
        log_message("\nWaiting for table loads to complete...")
        results = {}
        for table_name, poll_url in operations.items():
            log_message(f"Polling '{table_name}'...")
            results[table_name] = poll_operation(workspace_id, lakehouse_id, poll_url)

        # Summary
        log_message("\n" + "=" * 60)
        log_message("SUMMARY")
        log_message("=" * 60)
        log_message(f"Lakehouse: {LAKEHOUSE_NAME} ({lakehouse_id})")
        log_message(f"Workspace: {workspace_id}")
        log_message("Tables loaded:")
        for table_name, success in results.items():
            status = "SUCCESS" if success else "FAILED"
            log_message(f"  {status}: {table_name}")
        log_message("=" * 60)

        if all(results.values()):
            log_message("\nAll tables loaded successfully!")
            return True
        else:
            log_message("\nWARNING: Some tables failed to load.")
            return False

    except Exception as e:
        log_message(f"ERROR: {type(e).__name__}: {str(e)}")
        log_message(f"Traceback:\n{traceback.format_exc()}")
        return False


if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)
