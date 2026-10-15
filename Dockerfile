FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:0d99ebf1ccad15037b5c98c9fe8979d6b8dbac23812feae2277a0b277b5c5ffa AS build
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

FROM mcr.microsoft.com/dotnet/aspnet:10.0-azurelinux3.0-distroless@sha256:42f3714eaa91aad7843633f943221f3a36f7894b7d93f512177e9757861c024f AS runtime
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Zenvofin.dll"]