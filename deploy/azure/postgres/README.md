# Azure deploy — PostgreSQL (optional production-like profile)

Managed PostgreSQL Flexible Server (B1ms) plus Azure Container Apps running the same GHCR image with `ConnectionStrings__schaerbeek` pointing at the database. EF migrations run on startup.

## Prerequisites

Same as the [SQLite profile](../sqlite/README.md), plus budget for PostgreSQL (~**$16–17/month** for B1ms + 32 GiB storage).

## Configure

1. Edit [parameters.json](./parameters.json) — set `containerImage` (required unless you use `CONTAINER_IMAGE` env var).
2. Optionally override:

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

The script prints the PostgreSQL admin password when it generates one. Store it securely if you need direct database access.

## What gets created

Everything in the SQLite profile, plus:

- PostgreSQL Flexible Server 16 (Burstable B1ms, 32 GiB)
- Database `schaerbeek`
- Firewall rule allowing Azure services
- Container App secret `ConnectionStrings__schaerbeek` (Npgsql)
- `minReplicas: 1` (avoids double cold-start with the database)

## Teardown

```bash
az group delete --name schaerbeek-rg --yes --no-wait
```

## When to use this profile

- You want **persistent** registration cases across restarts
- You want to exercise **EF migrations** in a production-like environment
- You accept the higher monthly cost versus ephemeral SQLite
