FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .

RUN mv *.csproj App.csproj \
    && dotnet publish -o /publish

FROM mcr.microsoft.com/azure-functions/dotnet:4-slim

WORKDIR /app

ENV AzureWebJobsScriptRoot=/app \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=build /publish .