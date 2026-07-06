# Azure deploy — SQLite (default production profile)

Ephemeral SQLite on Azure Container Apps: no PostgreSQL, no persistent volumes. Each cold start recreates the database and re-seeds reference data.

## Prerequisites

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) (`az login`)
- A GHCR image published from this repo (see [../../.github/workflows/container-publish.yml](../../.github/workflows/container-publish.yml))
- **GitHub:** repo **Settings → Actions → General → Workflow permissions → Read and write**
- **GHCR package:** set the container package visibility to **Public** (easiest for ACA pull), or configure a registry secret (see parent [README](../README.md))

## Configure

1. Edit [parameters.json](./parameters.json) or pass environment variables.
2. Set your image:

```bash
export CONTAINER_IMAGE=ghcr.io/YOUR_GITHUB_USER/schaerbeek-municipality-web:latest
export RESOURCE_GROUP=schaerbeek-rg
export LOCATION=westeurope
```

## Deploy

```bash
chmod +x deploy.sh
./deploy.sh
```

## What gets created

- Log Analytics workspace (Container Apps logs)
- Container Apps Environment
- Container App (`schaerbeek-web`) with:
  - `ASPNETCORE_ENVIRONMENT=Production`
  - SQLite at `/tmp/schaerbeek.db` (from `appsettings.Production.json`)
  - `minReplicas: 0`, `maxReplicas: 1`
  - HTTP ingress with WebSocket transport (`auto`)

## Expected behaviour

- First request after idle may take 15–45 seconds (cold start + EF `EnsureCreated` + seeding).
- All registration cases and uploads are lost when the container is replaced.
- Reference data (streets, national register seed) is restored on each startup.

## Teardown

```bash
az group delete --name schaerbeek-rg --yes --no-wait
```

## Cost

Roughly **$0–5/month** when scaled to zero most of the time (no PostgreSQL, no ACR if using public GHCR).
