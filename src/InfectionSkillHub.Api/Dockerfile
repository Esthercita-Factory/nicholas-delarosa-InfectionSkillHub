# REST API (ASP.NET Core + JWT + Scalar). Built from the repository root:
#   docker build -f src/InfectionSkillHub.Api/Dockerfile -t infectionskillhub-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Project files first: the restore layer stays cached while dependencies do not change.
COPY src/InfectionSkillHub.Domain/InfectionSkillHub.Domain.csproj src/InfectionSkillHub.Domain/
COPY src/InfectionSkillHub.Application/InfectionSkillHub.Application.csproj src/InfectionSkillHub.Application/
COPY src/InfectionSkillHub.Infrastructure/InfectionSkillHub.Infrastructure.csproj src/InfectionSkillHub.Infrastructure/
COPY src/InfectionSkillHub.Api/InfectionSkillHub.Api.csproj src/InfectionSkillHub.Api/
RUN dotnet restore src/InfectionSkillHub.Api/InfectionSkillHub.Api.csproj

COPY src/ src/
RUN dotnet publish src/InfectionSkillHub.Api/InfectionSkillHub.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
# libgssapi-krb5-2 avoids the "libgssapi_krb5.so.2: cannot open shared object file" warning of Npgsql.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "InfectionSkillHub.Api.dll"]
