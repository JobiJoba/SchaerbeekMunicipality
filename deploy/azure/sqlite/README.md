# Azure deploy — SQLite (default production profile)

Ephemeral SQLite on Azure Container Apps: no PostgreSQL, no persistent volumes. Each cold start recreates the database and re-seeds reference data.

## Prerequisites

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) (`az login`)
- A GHCR image published from this repo (see [CD workflow](../../.github/workflows/cd.yml) publish job)
- **GitHub:** repo **Settings → Actions → General → Workflow permissions → Read and write**
- **GHCR package:** set the container package visibility to **Public** (easiest for ACA pull), or configure a registry secret (see parent [README](../README.md))

## Configure

1. Edit [parameters.json](./parameters.json) — set `containerImage` to your GHCR URL (this is the default).
2. Optionally override via environment variables:

```bash
export CONTAINER_IMAGE=ghcr.io/YOUR_GITHUB_USER/schaerbeek-municipality-web:latest  # overrides parameters.json
export RESOURCE_GROUP=schaerbeek-rg
export LOCATION=westeurope
```

## Deploy

```bash
chmod +x deploy.sh
./deploy.sh
```

On success, `deploy.sh` prints the app URL and updates the **Try it live** link in the root [README.md](../../README.md). Set `UPDATE_README_URL=0` to skip that step.

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
