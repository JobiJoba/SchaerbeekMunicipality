#!/usr/bin/env bash
set -euo pipefail

RESOURCE_GROUP="${RESOURCE_GROUP:-schaerbeek-rg}"
CONTAINER_APP_NAME="${CONTAINER_APP_NAME:-schaerbeek-web}"
README_FILE="${README_FILE:-}"

usage() {
  echo "Usage: $(basename "$0") [--readme PATH] [--resource-group NAME] [--container-app NAME]"
  echo ""
  echo "Fetches the deployed Container App URL and updates the Try it live link in README.md."
  echo "Set UPDATE_README_URL=0 to skip when running deploy.sh."
  exit 1
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --readme)
      README_FILE="$2"
      shift 2
      ;;
    --resource-group)
      RESOURCE_GROUP="$2"
      shift 2
      ;;
    --container-app)
      CONTAINER_APP_NAME="$2"
      shift 2
      ;;
    -h|--help)
      usage
      ;;
    *)
      echo "Unknown argument: $1" >&2
      usage
      ;;
  esac
done

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if [[ -z "${README_FILE}" ]]; then
  README_FILE="$(cd "${SCRIPT_DIR}/../.." && pwd)/README.md"
fi

if [[ ! -f "${README_FILE}" ]]; then
  echo "README not found: ${README_FILE}" >&2
  exit 1
fi

FQDN="$(az containerapp show \
  --name "${CONTAINER_APP_NAME}" \
  --resource-group "${RESOURCE_GROUP}" \
  --query properties.configuration.ingress.fqdn -o tsv)"

if [[ -z "${FQDN}" ]]; then
  echo "Could not resolve FQDN for ${CONTAINER_APP_NAME} in ${RESOURCE_GROUP}." >&2
  exit 1
fi

APP_URL="https://${FQDN}/"

python3 - "${README_FILE}" "${APP_URL}" <<'PY'
import sys
from pathlib import Path

readme_path = Path(sys.argv[1])
url = sys.argv[2]
if not url.endswith("/"):
    url += "/"

content = readme_path.read_text(encoding="utf-8")
start = "<!-- deploy-live-url-start -->"
end = "<!-- deploy-live-url-end -->"

if start not in content or end not in content:
    raise SystemExit(
        f"{readme_path} is missing {start} / {end} markers around the live URL."
    )

before, rest = content.split(start, 1)
_, after = rest.split(end, 1)
new_link = f"\n**[{url}]({url})**\n"
new_content = before + start + new_link + end + after

if new_content == content:
    print(f"README live URL already up to date: {url}")
else:
    readme_path.write_text(new_content, encoding="utf-8")
    print(f"Updated {readme_path} live URL to {url}")
PY
