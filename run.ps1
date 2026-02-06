<#
Simple helper to build and run the FuelSavings Docker image.
Usage:
  .\run.ps1            # build image and run (foreground)
  .\run.ps1 -UseCompose # run via docker-compose (requires docker-compose.yml)
  .\run.ps1 -Detach     # run container in background
#>

param(
    [switch]$UseCompose,
    [switch]$Detach
)

$ErrorActionPreference = 'Stop'

if ($UseCompose) {
    Write-Host "Using docker-compose to build and run..."
    if ($Detach) {
        docker-compose up --build -d
    }
    else {
        docker-compose up --build
    }
    return
}

Write-Host "Building Docker image 'fuelsavings:local'..."
docker build -t fuelsavings:local .

if ($Detach) {
    Write-Host "Stopping any existing 'fuelsavings' container..."
    docker rm -f fuelsavings -ErrorAction SilentlyContinue | Out-Null
    Write-Host "Running container in background (host port 5000 -> container 80)..."
    docker run -d --name fuelsavings -p 5000:80 fuelsavings:local
    Write-Host "Container started. Use 'docker logs -f fuelsavings' to view logs."
}
else {
    Write-Host "Stopping any existing 'fuelsavings' container..."
    docker rm -f fuelsavings -ErrorAction SilentlyContinue | Out-Null
    Write-Host "Running container (host port 5000 -> container 80)..."
    docker run --rm --name fuelsavings -p 5000:80 fuelsavings:local
}
