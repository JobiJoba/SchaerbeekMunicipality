#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

RESOURCE_GROUP="${RESOURCE_GROUP:-schaerbeek-rg}"
LOCATION="${LOCATION:-westeurope}"
CONTAINER_IMAGE="${CONTAINER_IMAGE:-}"
POSTGRES_SERVER_NAME="${POSTGRES_SERVER_NAME:-schaerbeek-pg-$(openssl rand -hex 3)}"
POSTGRES_ADMIN_LOGIN="${POSTGRES_ADMIN_LOGIN:-schaerbeekadmin}"
POSTGRES_ADMIN_PASSWORD="${POSTGRES_ADMIN_PASSWORD:-$(openssl rand -base64 24 | tr -d '/+=' | head -c 24)}"

if [[ -z "${CONTAINER_IMAGE}" ]]; then
  echo "Set CONTAINER_IMAGE to your GHCR image, for example:"
  echo "  export CONTAINER_IMAGE=ghcr.io/YOUR_GITHUB_USER/schaerbeek-municipality-web:latest"
  exit 1
fi

echo "Creating resource group ${RESOURCE_GROUP} in ${LOCATION} (if needed)..."
az group create --name "${RESOURCE_GROUP}" --location "${LOCATION}" --output none

echo "Deploying Azure Container Apps + PostgreSQL Flexible Server..."
echo "PostgreSQL server name: ${POSTGRES_SERVER_NAME}"
echo "Save the generated admin password if you did not set POSTGRES_ADMIN_PASSWORD."

az deployment group create \
  --resource-group "${RESOURCE_GROUP}" \
  --template-file "${SCRIPT_DIR}/main.bicep" \
  --parameters @"${SCRIPT_DIR}/parameters.json" \
  --parameters \
    "location=${LOCATION}" \
    "containerImage=${CONTAINER_IMAGE}" \
    "postgresServerName=${POSTGRES_SERVER_NAME}" \
    "postgresAdminLogin=${POSTGRES_ADMIN_LOGIN}" \
    "postgresAdminPassword=${POSTGRES_ADMIN_PASSWORD}" \
  --output table

echo ""
echo "PostgreSQL admin password used for this deployment:"
echo "  ${POSTGRES_ADMIN_PASSWORD}"
echo ""
echo "App URL:"
echo "  az containerapp show --name schaerbeek-web --resource-group ${RESOURCE_GROUP} --query properties.configuration.ingress.fqdn -o tsv"
