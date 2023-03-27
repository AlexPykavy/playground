FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .

RUN mv *.csproj App.csproj \
    && dotnet publish -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app

ARG USERNAME=dotnet
RUN groupadd --gid 10001 $USERNAME \
    && useradd --uid 10000 --gid $USERNAME --shell /bin/bash --create-home $USERNAME
USER $USERNAME

COPY --from=build /publish .

ENTRYPOINT ["dotnet", "App.dll"]
