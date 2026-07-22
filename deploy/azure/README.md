# Azure deployment

Two Bicep-based profiles deploy the same **GHCR** container image to **Azure Container Apps**. Local development continues to use **Aspire + PostgreSQL** (`dotnet run --project src/SchaerbeekMunicipality.AppHost`).

| Profile | Path | Database | Persist data | Typical idle cost |
|---------|------|----------|--------------|-------------------|
| **Default production** | [sqlite/](./sqlite/) | Ephemeral SQLite (`/tmp/schaerbeek.db`) | No | ~$0–5/mo |
| **Optional** | [postgres/](./postgres/) | PostgreSQL Flexible Server | Yes | ~$16–35/mo |

## Image source — GitHub Container Registry (GHCR)

On green CI for `main`, [`.github/workflows/cd.yml`](../../.github/workflows/cd.yml) builds the image, scans it with Trivy, and pushes:

```text
ghcr.io/<github-owner>/schaerbeek-municipality-web:latest
ghcr.io/<github-owner>/schaerbeek-municipality-web:<short-git-sha>
```

### GitHub configuration (required once)

1. **Settings → Actions → General → Workflow permissions:** enable **Read and write permissions**.
2. After the first successful publish job, open **Packages** → `schaerbeek-municipality-web` → **Package settings** → set visibility to **Public** (simplest ACA pull).

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

## Continuous deploy (SQLite profile)

After publish, **CD** can update the existing Container App to the **SHA-tagged** image via **Azure OIDC** (no long-lived client secret). README live-URL rewrite is **not** done from CD.

See [docs/CI.md — Continuous deploy](../../docs/CI.md#continuous-deploy-azure-oidc) for federated credential setup and the `AZURE_DEPLOY_ENABLED` variable.

Until that variable is set, only the image is published; Azure is unchanged.

## Manual deploy flow (first-time / infra changes)

Use this for the **first** deploy, Bicep/parameter changes, or the optional PostgreSQL profile.

1. Push to `main` (CI then CD) — or run the **CD** workflow via `workflow_dispatch` — so GHCR has an image.
2. `az login` and pick a subscription.
3. Run `./deploy.sh` from [sqlite/](./sqlite/) or [postgres/](./postgres/).

Pin `CONTAINER_IMAGE` (or `parameters.json`) to a **SHA tag** when possible, for example:

```bash
export CONTAINER_IMAGE=ghcr.io/jobijoba/schaerbeek-municipality-web:a80d2b2
```

After a successful manual deploy, the script prints the live app URL and updates the **Try it live** link in the repo [README.md](../../README.md). Set `UPDATE_README_URL=0` to skip the README edit (CD never rewrites it). You can also refresh the link manually:

```bash
chmod +x deploy/azure/update-readme-url.sh
./deploy/azure/update-readme-url.sh
```

## Health checks

Production exposes `/health` (readiness, includes EF database check) and `/alive` (liveness). Optionally set `HEALTH_CHECK_API_KEY` on the Container App and send header `X-Health-Check-Key` to restrict access. This key is **not** configured in Bicep by default (ACA probes would also need the header).

## Related docs

- [CI.md](../../docs/CI.md) — pipelines, Trivy, OIDC CD
- [Phase 10 delivery notes](../../docs/phases/phase-10-azure-deployment.md)
- [ADR-0005](../../docs/adr/0005-deployment-database-by-environment.md)
