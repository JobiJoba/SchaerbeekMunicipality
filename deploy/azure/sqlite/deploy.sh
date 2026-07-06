#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PARAMETERS_FILE="${SCRIPT_DIR}/parameters.json"

RESOURCE_GROUP="${RESOURCE_GROUP:-schaerbeek-rg}"
LOCATION="${LOCATION:-westeurope}"
CONTAINER_IMAGE="${CONTAINER_IMAGE:-}"

read_parameter() {
  local name="$1"
  python3 -c "import json, sys; print(json.load(open(sys.argv[1]))['parameters'][sys.argv[2]]['value'])" \
    "${PARAMETERS_FILE}" "${name}"
}

if [[ -z "${CONTAINER_IMAGE}" && -f "${PARAMETERS_FILE}" ]]; then
  CONTAINER_IMAGE="$(read_parameter containerImage)"
fi

if [[ -z "${CONTAINER_IMAGE}" ]] || [[ "${CONTAINER_IMAGE}" == *"YOUR_GITHUB_USER"* ]]; then
  echo "Set containerImage in ${PARAMETERS_FILE} or export CONTAINER_IMAGE, for example:"
  echo "  export CONTAINER_IMAGE=ghcr.io/YOUR_GITHUB_USER/schaerbeek-municipality-web:latest"
  exit 1
fi

if [[ -f "${PARAMETERS_FILE}" ]]; then
  LOCATION="${LOCATION:-$(read_parameter location)}"
fi

echo "Using container image: ${CONTAINER_IMAGE}"

echo "Creating resource group ${RESOURCE_GROUP} in ${LOCATION} (if needed)..."
az group create --name "${RESOURCE_GROUP}" --location "${LOCATION}" --output none

echo "Deploying Azure Container Apps (SQLite / ephemeral production profile)..."
az deployment group create \
  --resource-group "${RESOURCE_GROUP}" \
  --template-file "${SCRIPT_DIR}/main.bicep" \
  --parameters @"${SCRIPT_DIR}/parameters.json" \
  --parameters "location=${LOCATION}" "containerImage=${CONTAINER_IMAGE}" \
  --output table

echo ""
echo "Deployment complete. Fetch the app URL with:"
echo "  az containerapp show --name schaerbeek-web --resource-group ${RESOURCE_GROUP} --query properties.configuration.ingress.fqdn -o tsv"
