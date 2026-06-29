"""
Create hrdocs + healthdocs indexes and upload data via REST.
Works around azure-search-documents SDK breakage (SearchIndex.deserialize removed).
"""
import json
import os
import sys
from pathlib import Path
import requests
from dotenv import load_dotenv

load_dotenv(override=True)

ENDPOINT = os.environ["AZURE_SEARCH_SERVICE_ENDPOINT"].rstrip("/")
KEY      = os.environ["AZURE_SEARCH_ADMIN_KEY"]
OAI_EP   = os.environ["AZURE_OPENAI_ENDPOINT"].rstrip("/")
OAI_KEY  = os.environ["AZURE_OPENAI_KEY"]
API      = "2024-07-01"

DATA = Path(__file__).resolve().parents[4] / "data" / "index-data"
HEAD = {"api-key": KEY, "Content-Type": "application/json"}

def restore(index_name: str, records_file: str) -> bool:
    print(f"\n=== {index_name} ===")
    with open(DATA / "index.json", "r", encoding="utf-8") as f:
        idx = json.load(f)
    idx["name"] = index_name
    v = idx["vectorSearch"]["vectorizers"][0]["azureOpenAIParameters"]
    v["resourceUri"] = OAI_EP + "/"
    v["apiKey"] = OAI_KEY

    r = requests.put(
        f"{ENDPOINT}/indexes/{index_name}?api-version={API}",
        headers=HEAD, data=json.dumps(idx))
    if not r.ok:
        print(f"  ERROR creating index: {r.status_code} {r.text[:500]}")
        return False
    print(f"  Index created/updated")

    batch, uploaded, batches = [], 0, 0
    with open(DATA / records_file, "r", encoding="utf-8") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            batch.append(json.loads(line))
            if len(batch) >= 100:
                batches += 1
                uploaded += _upload(index_name, batch)
                batch = []
    if batch:
        batches += 1
        uploaded += _upload(index_name, batch)
    print(f"  Uploaded {uploaded} docs in {batches} batches")
    return True

def _upload(index_name: str, docs: list) -> int:
    body = {"value": [{**d, "@search.action": "upload"} for d in docs]}
    r = requests.post(
        f"{ENDPOINT}/indexes/{index_name}/docs/index?api-version={API}",
        headers=HEAD, data=json.dumps(body))
    if not r.ok:
        print(f"  ERROR upload: {r.status_code} {r.text[:500]}")
        return 0
    return len(docs)

if __name__ == "__main__":
    ok1 = restore("hrdocs",     "hrdocs-exported.jsonl")
    ok2 = restore("healthdocs", "healthdocs-exported.jsonl")
    sys.exit(0 if (ok1 and ok2) else 1)
