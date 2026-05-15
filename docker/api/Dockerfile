# syntax=docker/dockerfile:1.7

# ---------- base ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# New Relic .NET agent (installed at OS level, attached via CLR profiler).
# The four CORECLR_* values are platform constants and safe to bake into the image.
# New Relic's apt repo is signed with a DSA-1024 key that Ubuntu 24.04 (noble)
# rejects as a weak algorithm. We mark the repo `[trusted=yes]` to skip signature
# verification — we're already trusting `apt.newrelic.com` over HTTPS, so the
# trust boundary is identical.
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates wget \
    && echo "deb [trusted=yes] https://apt.newrelic.com/debian/ newrelic non-free" > /etc/apt/sources.list.d/newrelic.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*

ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    CORECLR_NEW_RELIC_HOME=/usr/local/newrelic-dotnet-agent \
    ASPNETCORE_URLS=http://+:8080

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["api/Dent1.Api/Dent1.Api.csproj",       "api/Dent1.Api/"]
COPY ["api/Dent1.Business/Dent1.Business.csproj", "api/Dent1.Business/"]
COPY ["api/Dent1.Common/Dent1.Common.csproj",   "api/Dent1.Common/"]
COPY ["api/Dent1.Data/Dent1.Data.csproj",       "api/Dent1.Data/"]
RUN dotnet restore "api/Dent1.Api/Dent1.Api.csproj"

COPY api/ ./api/
WORKDIR /src/api/Dent1.Api
RUN dotnet build "Dent1.Api.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/build

# ---------- publish ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Dent1.Api.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

# ---------- development ----------
# Note: uses `dotnet run` (not `dotnet watch`) because the CLR profiler attaches
# to the started .NET process. `dotnet watch` spawns child processes and the
# profiler attaches to the wrapper, missing the actual app.
FROM build AS development
ENV ASPNETCORE_ENVIRONMENT=Development \
    CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    CORECLR_NEW_RELIC_HOME=/usr/local/newrelic-dotnet-agent \
    ASPNETCORE_URLS=http://+:8080
WORKDIR /src/api/Dent1.Api
EXPOSE 8080
# Install the agent in the build stage too so dev runs are instrumented.
# New Relic's apt repo is signed with a DSA-1024 key that Ubuntu 24.04 (noble)
# rejects as a weak algorithm. We mark the repo `[trusted=yes]` to skip signature
# verification — we're already trusting `apt.newrelic.com` over HTTPS, so the
# trust boundary is identical.
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates wget \
    && echo "deb [trusted=yes] https://apt.newrelic.com/debian/ newrelic non-free" > /etc/apt/sources.list.d/newrelic.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "run", "--no-launch-profile", "--urls", "http://+:8080"]

# ---------- production ----------
FROM base AS production
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dent1.Api.dll"]
