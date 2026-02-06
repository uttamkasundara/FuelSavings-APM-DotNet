FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/FuelSavings.Api/FuelSavings.Api.csproj src/FuelSavings.Api/
COPY src/FuelSavings.Core/FuelSavings.Core.csproj src/FuelSavings.Core/
RUN dotnet restore src/FuelSavings.Api/FuelSavings.Api.csproj

COPY src/ ./src/
WORKDIR /src/src/FuelSavings.Api
RUN dotnet publish -c Release -o /app/publish

# Please select the corresponding download for your Linux containers https://github.com/DataDog/dd-trace-dotnet/releases/latest

# Download and install the Tracer
RUN mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && rm ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app/publish ./
# Copy tracer files installed in the build stage into the runtime image
COPY --from=build /opt/datadog /opt/datadog

# Enable the Datadog .NET tracer at runtime
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog

ENTRYPOINT ["dotnet", "FuelSavings.Api.dll"]
