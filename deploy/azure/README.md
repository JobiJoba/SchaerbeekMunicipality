# Azure deployment

Two Bicep-based profiles deploy the same **GHCR** container image to **Azure Container Apps**. Local development continues to use **Aspire + PostgreSQL** (`dotnet run --project src/SchaerbeekMunicipality.AppHost`).

| Profile | Path | Database | Persist data | Typical idle cost |
|---------|------|----------|--------------|-------------------|
| **Default production** | [sqlite/](./sqlite/) | Ephemeral SQLite (`/tmp/schaerbeek.db`) | No | ~$0–5/mo |
| **Optional** | [postgres/](./postgres/) | PostgreSQL Flexible Server | Yes | ~$16–35/mo |

## Image source — GitHub Container Registry (GHCR)

Images are built by [.github/workflows/container-publish.yml](../../.github/workflows/container-publish.yml) and pushed to:

```text
ghcr.io/<github-owner>/schaerbeek-municipality-web:latest
ghcr.io/<github-owner>/schaerbeek-municipality-web:<git-sha>
```

### GitHub configuration (required once)

1. **Settings → Actions → General → Workflow permissions:** enable **Read and write permissions**.
2. After the first workflow run, open **Packages** → `schaerbeek-municipality-web` → **Package settings** → set visibility to **Public** (simplest ACA pull).

### Private GHCR package (optional)

If the package stays private, grant ACA pull access before deploy:

```bash
az containerapp registry set \
  --name schaerbeek-web \
  --resource-group schaerbeek-rg \
  --server ghcr.io \
  --username YOUR_GITHUB_USER \
  --password YOUR_GITHUB_PAT_WITH_read_packages
```

Use a [fine-grained PAT](https://github.com/settings/tokens) with `read:packages` on this repository.

## Deploy flow

1. Push to `main` (or run the **Container publish** workflow manually) so GHCR has an image.
2. `az login` and pick a subscription.
3. Run `./deploy.sh` from [sqlite/](./sqlite/) or [postgres/](./postgres/).

## Health checks

Production exposes `/health` (readiness, includes EF database check) and `/alive` (liveness). Optionally set `HEALTH_CHECK_API_KEY` on the Container App and send header `X-Health-Check-Key` to restrict access.

## Related docs

- [Phase 10 delivery notes](../../docs/phases/phase-10-azure-deployment.md)
- [ADR-0005](../../docs/adr/0005-deployment-database-by-environment.md)
