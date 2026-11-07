FROM mcr.microsoft.com/dotnet/sdk:11.0@sha256:59d092a889fbad09c1a0b0c5dd072c6315125b0294a5cddfa48ec892f63b2ab3 AS build
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

FROM mcr.microsoft.com/dotnet/aspnet:11.0-azurelinux3.0-distroless@sha256:b9c94386b917b099f055881feaf612c49d3fcee37eda554b75de16192ac4e46c AS runtime
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Zenvofin.dll"]