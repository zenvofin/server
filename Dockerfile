FROM mcr.microsoft.com/dotnet/sdk:11.0@sha256:bfabf51e74c5a7cf2fc4eac852145db86f4867d475257d809518d6a2b0fe71e1 AS build
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

FROM mcr.microsoft.com/dotnet/aspnet:11.0-azurelinux3.0-distroless@sha256:3830dc72852e3517140642a7d8ae91b3bdf076cad72811fb81475743864a25b7 AS runtime
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Zenvofin.dll"]