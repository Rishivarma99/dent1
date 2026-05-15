# Docker + New Relic Integration

How the .NET API and the Angular frontend are containerized, and how the New Relic agents are wired up.

## What is instrumented

| Service | Type | Instrumentation |
|---|---|---|
| `api` (.NET 10) | APM (backend) | New Relic .NET agent attached via CLR profiler |
| `frontend` (Angular) | Browser (RUM) | New Relic Browser snippet in `frontend/src/index.html` |

---

## Layout

```
docker/
  docker-compose.yml         # Brings up db, api, frontend
  api/Dockerfile             # Multi-stage: base / build / publish / development / production
  frontend/Dockerfile        # Multi-stage: build / development / production (nginx)
  frontend/nginx.conf        # SPA fallback config for prod image
  .env.newrelic              # SECRET — gitignored, holds NEW_RELIC_LICENSE_KEY
  .env.newrelic.example      # Template, safe to commit
.env                         # Project-wide DEV defaults (tracked in Git)
.dockerignore                # Excludes build output / env files from build context
```

---

## .NET API — how the agent attaches

The .NET agent is **not** a NuGet package. It's a native shared library installed at the OS level via the New Relic apt repo (`apt.newrelic.com`). The CLR loads it as a profiler at startup when four env vars are present:

| Variable | Value | Purpose |
|---|---|---|
| `CORECLR_ENABLE_PROFILING` | `1` | Tells the CLR to load a profiler |
| `CORECLR_PROFILER` | `{36032161-FFC0-4B61-B559-F6C5D41BAE5A}` | New Relic's profiler GUID |
| `CORECLR_PROFILER_PATH` | `/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so` | Profiler shared object path |
| `CORECLR_NEW_RELIC_HOME` | `/usr/local/newrelic-dotnet-agent` | Agent config + extensions |

Plus two runtime values from `docker/.env.newrelic`:

| Variable | Purpose |
|---|---|
| `NEW_RELIC_LICENSE_KEY` | Account license key (secret) |
| `NEW_RELIC_APP_NAME` | Service name shown in the NR UI |

### Why `dotnet watch run` is not used

The CLR profiler attaches to the .NET process that starts the app. `dotnet watch` is a wrapper that spawns and restarts child `dotnet` processes — the profiler attaches to the wrapper, not the actual app. The `development` stage uses `dotnet run` instead, trading hot reload for instrumentation.

---

## Angular frontend — Browser agent

The Browser snippet is embedded directly in [frontend/src/index.html](../frontend/src/index.html). The `NRJS-*` `licenseKey` is **public** by design (it identifies the app to the beacon) and is not the same value as the .NET agent's `NEW_RELIC_LICENSE_KEY`. To rotate it, edit `index.html` and rebuild the image.

---

## Env var injection — three layers, ordered by precedence (highest wins)

1. `environment:` block in `docker-compose.yml`
2. Last `env_file:` entry (later files override earlier)
3. Earlier `env_file:` entries
4. `ENV` directives in the Dockerfile

| Layer | Used for | Example |
|---|---|---|
| Dockerfile `ENV` | Platform constants — never vary by env | `CORECLR_*` |
| Root `.env` (tracked) | Non-secret DEV defaults | DB host/port, JWT issuer |
| `docker/.env.newrelic` (gitignored) | Secrets / per-developer overrides | `NEW_RELIC_LICENSE_KEY` |

The `api` service in `docker-compose.yml` loads both:

```yaml
api:
  env_file:
    - ../.env
    - .env.newrelic
```

### Why Windows `[Environment]::SetEnvironmentVariable(..., "Machine")` does not work

Host vars live only on the host. Containers are isolated Linux environments — they don't inherit the Windows host's env. Everything must come through one of the three layers above.

---

## Adding a new observability variable — checklist

1. **Constant?** → `ENV` in the Dockerfile.
2. **Config that varies by env but isn't secret?** → root `.env` + `env_file`.
3. **Secret?** → gitignored env file (`.env.newrelic` pattern), and add the filename to `.gitignore`.
4. Reference it from the relevant service in `docker-compose.yml` via `env_file`.
5. Rebuild only if you changed `ENV` in the Dockerfile. For runtime env files: `docker compose up -d --force-recreate <service>`.

---

## Running locally

```powershell
# 1. Fill in your New Relic license key
Copy-Item docker/.env.newrelic.example docker/.env.newrelic
# (edit docker/.env.newrelic, set NEW_RELIC_LICENSE_KEY)

# 2. Bring everything up
docker compose -f docker/docker-compose.yml up --build

# Defaults: API on http://localhost:5003, frontend on http://localhost:4200, MySQL on 3306.
```

---

## Verification commands

```powershell
# Confirm vars are inside the running container
docker exec dent1-api env | Select-String "NEW_RELIC|CORECLR"

# Confirm the agent is installed
docker exec dent1-api ls /usr/local/newrelic-dotnet-agent/

# Confirm the profiler attached (log files appear after the first profiled request)
docker exec dent1-api ls /usr/local/newrelic-dotnet-agent/logs/

# Look for the handshake with New Relic's collector
docker exec dent1-api sh -c "grep -iE 'reporting to|license|invalid|connect' /usr/local/newrelic-dotnet-agent/logs/*.log | tail -n 20"
```

After hitting the API a few times, `NEW_RELIC_APP_NAME` appears in **APM → Services** within ~2 minutes.
