# Runs the unit tests. docker compose starts the database and the API only if they pass.
#   docker build -f src/InfectionSkillHub.Tests/Dockerfile -t infectionskillhub-tests .

FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /source

COPY src/InfectionSkillHub.Domain/InfectionSkillHub.Domain.csproj src/InfectionSkillHub.Domain/
COPY src/InfectionSkillHub.Application/InfectionSkillHub.Application.csproj src/InfectionSkillHub.Application/
COPY src/InfectionSkillHub.Infrastructure/InfectionSkillHub.Infrastructure.csproj src/InfectionSkillHub.Infrastructure/
COPY src/InfectionSkillHub.Tests/InfectionSkillHub.Tests.csproj src/InfectionSkillHub.Tests/
RUN dotnet restore src/InfectionSkillHub.Tests/InfectionSkillHub.Tests.csproj

COPY src/ src/
RUN dotnet build src/InfectionSkillHub.Tests/InfectionSkillHub.Tests.csproj -c Release --no-restore

ENTRYPOINT ["dotnet", "test", "src/InfectionSkillHub.Tests/InfectionSkillHub.Tests.csproj", "-c", "Release", "--no-build", "--logger", "console;verbosity=normal"]
