#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

RESOURCE_GROUP="${RESOURCE_GROUP:-schaerbeek-rg}"
LOCATION="${LOCATION:-westeurope}"
CONTAINER_IMAGE="${CONTAINER_IMAGE:-}"

if [[ -z "${CONTAINER_IMAGE}" ]]; then
  echo "Set CONTAINER_IMAGE to your GHCR image, for example:"
  echo "  export CONTAINER_IMAGE=ghcr.io/YOUR_GITHUB_USER/schaerbeek-municipality-web:latest"
  exit 1
fi

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
