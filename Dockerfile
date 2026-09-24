FROM mcr.microsoft.com/dotnet/sdk:11.0@sha256:ab11199f8a0cded1d667111a0d4f0906567c3a17348a7181f9781d12cc5eb6f9 AS build
WORKDIR /src

COPY *.slnx ./
COPY Zenvofin/*.csproj ./Zenvofin/
COPY Zenvofin.AppHost/*.csproj ./Zenvofin.AppHost/
COPY Zenvofin.ServiceDefaults/*.csproj ./Zenvofin.ServiceDefaults/
COPY Zenvofin.Tests/*.csproj ./Zenvofin.Tests/

RUN dotnet restore

COPY Zenvofin/ ./Zenvofin/
COPY Zenvofin.AppHost/ ./Zenvofin.AppHost/
COPY Zenvofin.ServiceDefaults/ ./Zenvofin.ServiceDefaults/
COPY Zenvofin.Tests/ ./Zenvofin.Tests/

WORKDIR /src/Zenvofin
RUN dotnet publish -c Release -o /app --no-restore \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

FROM mcr.microsoft.com/dotnet/aspnet:11.0-azurelinux3.0-distroless@sha256:9e955f30a71e50557397046f5f965d170dd1d1f294f1d3bc0126c25b5844324b AS runtime
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Zenvofin.dll"]