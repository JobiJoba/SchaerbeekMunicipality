# Phase 10 — Azure deployment

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Deploy the web app to Azure Container Apps with production hardening; local dev stays on Aspire + PostgreSQL.

---

## Summary

Phase 10 adds a containerized Azure deployment path via **GitHub Container Registry (GHCR)** and **Bicep** templates. **Production defaults to ephemeral SQLite** (fresh demo on each cold start). **Local development is unchanged** — AppHost + PostgreSQL. An **optional PostgreSQL** Azure profile provisions Flexible Server for persistent data.

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| `Dockerfile` + `.dockerignore` | Done | Multi-stage; includes `Directory.Build.props` |
| GHCR publish workflow | Done | `.github/workflows/container-publish.yml` |
| Azure SQLite deploy (default) | Done | `deploy/azure/sqlite/` |
| Azure PostgreSQL deploy (optional) | Done | `deploy/azure/postgres/` |
| `appsettings.Production.json` | Done | SQLite `Data Source=/tmp/schaerbeek.db` |
| `MigrateAsync` for Npgsql (all envs) | Done | `InitializeDatabaseAsync` |
| `AddDbContextCheck` on `/health` | Done | `HealthCheckExtensions` |
| `/health` + `/alive` in Production | Done | `MapDefaultEndpoints` + optional API key |
| OpenAPI in Production | Done | `/openapi/v1.json` |
| [ADR-0005](../adr/0005-deployment-database-by-environment.md) | Done | Database-by-environment decision |

### Deferred

| Item | Reason |
|------|--------|
| Full integration suite on Testcontainers PG | ADR-0002 follow-up; migration job still validates PG |
| `aspire publish` manifest | Bicep + GHCR replaces ACR-based publish for this project |
| Fly.io profile | Out of scope |

---

## Database profiles

| Profile | Where | Connection string | Data survives restart? |
|---------|-------|-------------------|------------------------|
| Local dev | AppHost | Aspire-injected Npgsql | Yes |
| CI tests | `WebApplicationFactory` | `Data Source=:memory:` | No |
| Azure default | ACA + Production | `/tmp/schaerbeek.db` | No |
| Azure optional | ACA + Flexible Server | Secret `ConnectionStrings__schaerbeek` | Yes |

---

## GHCR setup (one-time)

1. **GitHub → Settings → Actions → General → Workflow permissions:** **Read and write**.
2. Push to `main` or run **Container publish** workflow manually.
3. **Packages → schaerbeek-municipality-web → Package settings → Public** (easiest ACA pull).

Image: `ghcr.io/<owner>/schaerbeek-municipality-web:latest`

---

## Deploy — SQLite (default)

```bash
export CONTAINER_IMAGE=ghcr.io/YOUR_USER/schaerbeek-municipality-web:latest
cd deploy/azure/sqlite
./deploy.sh
```

See [deploy/azure/sqlite/README.md](../../deploy/azure/sqlite/README.md).

**Demo:** Open the ACA URL → cold start may take 15–45s → empty case list with seeded reference data → open a case → scale-to-zero or restart → data gone, seeds restored.

---

## Deploy — PostgreSQL (optional)

```bash
export CONTAINER_IMAGE=ghcr.io/YOUR_USER/schaerbeek-municipality-web:latest
cd deploy/azure/postgres
./deploy.sh
```

See [deploy/azure/postgres/README.md](../../deploy/azure/postgres/README.md).

---

## Local verification

```bash
docker build -t schaerbeek-web:local .
docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production schaerbeek-web:local
curl http://localhost:8080/health
```

---

## Cost (rough)

| Profile | Idle |
|---------|------|
| SQLite, `minReplicas: 0` | ~$0–5/mo |
| PostgreSQL + `minReplicas: 1` | ~$16–35/mo |

No ACR when using public GHCR.

---

## Teardown

```bash
az group delete --name schaerbeek-rg --yes --no-wait
```

---

## Carries forward

- Phase 11 optional features deploy with the same image and either profile.
- Expand Testcontainers PG coverage when PR CI time allows.
