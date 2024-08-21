FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build -c Release

ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "run", "--urls", "http://0.0.0.0:80"]