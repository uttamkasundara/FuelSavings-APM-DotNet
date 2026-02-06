# FuelSavings-APM-DotNet

Sample ASP.NET Core minimal API that calculates fuel differences between driving styles.

Quick run (requires .NET 8 SDK):

```powershell
dotnet run --project src/FuelSavings.Api
```

Build and run with Docker:

```powershell
docker build -t fuelsavings:local .
docker run -p 5000:80 fuelsavings:local
```

The API exposes `POST /api/v1/fuel/savings` with JSON body. See `postman/FuelSavings.postman_collection.json` for a sample request.
# FuelSavings API

Important setup notes
- Prerequisites: Docker Desktop (Linux containers) or Docker Engine. For local testing with Datadog APM, you will need a Datadog account and an API key.
- Create a `.env` file in the repo root with your Datadog API key:

```env
DD_API_KEY=your_datadog_api_key_here
```

Overview
- Purpose: small ASP.NET Core sample REST API that compares fuel consumption for two driving styles (continuous driving vs. driving with unnecessary accelerations/stops). Intended as an educational/demo project and a starting point for instrumentation and tracing.
- API Spec (summary):
	- POST /api/v1/fuel/savings
		- Request JSON: `distanceKm` (number, required), `model` (string: `physics|mid|simple`, optional, default `physics`), `accelEvents` (int), plus model-specific parameters such as `vehicleMassKg`, `avgDeltaVmS`, `baseLPer100Km`, `engineEfficiency`.
		- Response JSON: `totalNoStopL`, `totalWithBadL`, `savingsL`, `savingsPct`, and a `breakdown` object.
	- Purpose: compute an estimate of additional fuel used due to acceleration events using a physics-based default model (with sensible defaults). This is an estimate only.

No guarantee / disclaimer
- The calculations are illustrative estimates only and are not guaranteed to be accurate for any specific vehicle, use case, or regulatory requirement. Use at your own risk. The author makes no warranty of correctness, fitness for a particular purpose, or suitability for commercial use.

Usage notice
- This repository is provided under the Apache License 2.0 (see LICENSE). You may use, modify, and distribute this project for personal or commercial purposes consistent with that license. The author is not responsible for how you use this code or for any outcomes from its use.

Running locally (Docker)

1) Build and run the app image (no local .NET SDK required):

```powershell
docker build -t fuelsavings:local .
docker run --rm -p 5000:80 --name fuelsavings fuelsavings:local
```

2) Or run with docker-compose (recommended when using Datadog agent):

```powershell
# make sure .env contains DD_API_KEY
.\run.ps1 -UseCompose
```

Testing the API

Example request (PowerShell):

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5000/api/v1/fuel/savings -ContentType 'application/json' -Body '{
	"distanceKm": 20,
	"model": "physics",
	"vehicleMassKg": 1500,
	"accelEvents": 8,
	"avgDeltaVmS": 2.5,
	"engineEfficiency": 0.25,
	"baseLPer100Km": 6.8
}'
```

Postman
- Import `postman/FuelSavings.postman_collection.json` and point requests to `http://localhost:5000`.

Datadog tracing / notes
- The repository includes a Dockerfile that installs the Datadog .NET tracer and a `docker-compose.yml` with a `datadog` agent service (APM enabled). The compose file expects `DD_API_KEY` to be supplied via `.env` or environment.
- IMPORTANT: replace the placeholder `DD_API_KEY` with your actual Datadog API key. Do NOT commit your real API key into source control.
- If you enable tracing and do not see traces:
	- Ensure the `datadog` service starts without hostname/permission errors (see container logs: `docker logs -f datadog`).
	- Ensure the app environment variables point to the agent: `DD_AGENT_HOST=datadog` and `DD_TRACE_AGENT_PORT=8126` (these are set in `docker-compose.yml`).
	- For debugging, set `DD_TRACE_DEBUG=true` in the app service environment to see tracer startup logs.

License
- This project is licensed under the Apache License 2.0 — see `LICENSE`.

Notice
- See `NOTICE.txt` for a short usage and responsibility notice.
