#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PARAMETERS_FILE="${SCRIPT_DIR}/parameters.json"

RESOURCE_GROUP="${RESOURCE_GROUP:-schaerbeek-rg}"
LOCATION="${LOCATION:-westeurope}"
CONTAINER_IMAGE="${CONTAINER_IMAGE:-}"
POSTGRES_SERVER_NAME="${POSTGRES_SERVER_NAME:-}"
POSTGRES_ADMIN_LOGIN="${POSTGRES_ADMIN_LOGIN:-}"
POSTGRES_ADMIN_PASSWORD="${POSTGRES_ADMIN_PASSWORD:-}"

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
  POSTGRES_SERVER_NAME="${POSTGRES_SERVER_NAME:-$(read_parameter postgresServerName)}"
  POSTGRES_ADMIN_LOGIN="${POSTGRES_ADMIN_LOGIN:-$(read_parameter postgresAdminLogin)}"
  if [[ -z "${POSTGRES_ADMIN_PASSWORD}" ]]; then
    param_password="$(read_parameter postgresAdminPassword)"
    if [[ "${param_password}" != "REPLACE_ME_OR_PASS_VIA_CLI" ]]; then
      POSTGRES_ADMIN_PASSWORD="${param_password}"
    fi
  fi
fi

POSTGRES_SERVER_NAME="${POSTGRES_SERVER_NAME:-schaerbeek-pg-$(openssl rand -hex 3)}"
POSTGRES_ADMIN_LOGIN="${POSTGRES_ADMIN_LOGIN:-schaerbeekadmin}"
POSTGRES_ADMIN_PASSWORD="${POSTGRES_ADMIN_PASSWORD:-$(openssl rand -base64 24 | tr -d '/+=' | head -c 24)}"

echo "Using container image: ${CONTAINER_IMAGE}"

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

CONTAINER_APP_NAME="${CONTAINER_APP_NAME:-$(read_parameter containerAppName)}"
FQDN="$(az containerapp show \
  --name "${CONTAINER_APP_NAME}" \
  --resource-group "${RESOURCE_GROUP}" \
  --query properties.configuration.ingress.fqdn -o tsv)"
APP_URL="https://${FQDN}/"

echo ""
echo "PostgreSQL admin password used for this deployment:"
echo "  ${POSTGRES_ADMIN_PASSWORD}"
echo ""
echo "Deployment complete."
echo "App URL: ${APP_URL}"

if [[ "${UPDATE_README_URL:-1}" != "0" ]]; then
  REPO_ROOT="$(cd "${SCRIPT_DIR}/../../.." && pwd)"
  "${SCRIPT_DIR}/../update-readme-url.sh" \
    --readme "${REPO_ROOT}/README.md" \
    --resource-group "${RESOURCE_GROUP}" \
    --container-app "${CONTAINER_APP_NAME}"
else
  echo "Skipped README update (UPDATE_README_URL=0)."
fi
