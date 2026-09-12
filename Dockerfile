FROM mcr.microsoft.com/dotnet/sdk:11.0@sha256:335dfd6679df1025209964f3e2592c0f1808501d17b9295640fd1a41d132c117 AS build
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