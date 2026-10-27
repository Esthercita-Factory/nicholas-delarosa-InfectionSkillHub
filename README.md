# InfectionSkillHub

REST API for **InfectionSkillHub**, a training center that offers short in-person workshops (photography, cooking, public
speaking, design, music, marketing...). It replaces the spreadsheets InfectionSkillHub used to track enrollments, which
ended up with overbooked workshops, people enrolled twice in the same workshop and participants enrolled in two
workshops at the same time.

- Participants sign up, browse the workshops, enroll, cancel and download a PDF enrollment certificate.
- Employees (the instructors) publish workshops, see who is enrolled, cancel any enrollment, bulk enroll people
  from an Excel file and check which workshops have the most enrollments.
- Any visitor, signed in or not, can browse the upcoming workshops and their available seats.

The participant application is a separate Angular client:
[InfectionSkillHub.Client](https://github.com/Esthercita-Factory/nicholas-delarosa-InfectionSkillHub.Client)
(presentation option B: it consumes this API with the JWT). Employee operations are performed directly on the API
through its **Scalar** reference.

## Business rules

All the rules live in the domain and application layers, never in the controllers.

| Rule | Where | Result |
| --- | --- | --- |
| A workshop is created with a future start and an end later than the start | `Workshop.Schedule` | 422 |
| The workshop code is unique (stored in uppercase, for example `FOT-001`) | `WorkshopService` + unique index | 409 |
| The instructor is given by email and must be registered | `WorkshopService` | 400 |
| The catalog only shows workshops that have not started, with the seats calculated by the system | `WorkshopRepository` + `EnrollmentPolicy.AvailableSeats` | |
| **Capacity**: only active enrollments count | `EnrollmentPolicy` | 422 |
| **No duplicates**: one active enrollment per participant and workshop | `EnrollmentPolicy` + partial unique index | 409 |
| **No schedule overlaps** with another active enrollment (back-to-back workshops are allowed) | `EnrollmentPolicy` / `Workshop.OverlapsWith` | 409 |
| **Only future workshops** accept enrollments | `EnrollmentPolicy` | 422 |
| **Cancellation**: participants up to 24 hours before the start; employees any time before the start. It records who cancelled and when | `Enrollment.Cancel` | 422 |
| **Re-enrollment**: a cancelled enrollment stays as history; enrolling again creates a new one if every rule is met | `EnrollmentPolicy` | |
| A participant only sees and manages their own enrollments | `EnrollmentAccess` | 403 |
| Only active enrollments have a certificate | `CertificateService` | 422 |

Two simultaneous requests cannot break the capacity or overlap rules: every enrollment runs inside a transaction
that locks the workshop row and then the participant row (`SELECT ... FOR UPDATE`, always in that order), and the
database keeps a partial unique index on `(participant_id, workshop_id) WHERE status = 'Active'` as a last line
of defense.

## Architecture

Clean Architecture: dependencies point towards the domain.

```
src/
├── InfectionSkillHub.Domain          Entities, business rules, domain errors and roles. No dependencies.
├── InfectionSkillHub.Application     Use cases (services), DTOs, Result pattern and abstractions
│                                     (repositories, unit of work, clock, token, PDF and Excel). Depends on Domain.
├── InfectionSkillHub.Infrastructure  EF Core + PostgreSQL, ASP.NET Core Identity, repositories, JWT issuing,
│                                     QuestPDF certificate, ClosedXML reader, migrations and initial data.
├── InfectionSkillHub.Api             Controllers, JWT authentication, authorization policies, ProblemDetails,
│                                     OpenAPI and Scalar. Composition root.
└── InfectionSkillHub.Tests           xUnit + NSubstitute unit tests.
```

```
Api ──► Application ──► Domain
 │           ▲
 └──► Infrastructure
```

Patterns applied:

- **Dependency injection** with the native .NET container: every class depends on interfaces
  (`IWorkshopRepository`, `IUnitOfWork`, `ICertificateGenerator`, `IEnrollmentSheetReader`, `IClock`...).
- **Repository** and **Unit of Work** over EF Core.
- **DTOs** for every request and response; entities never leave the Application layer.
- **Result pattern** (`Result<T>` + `Error`) for expected failures, translated to HTTP status codes and
  ProblemDetails (RFC 9457) by the API.
- **Domain service** (`EnrollmentPolicy`) for the rules that involve several entities, and rich entities
  with private setters and factory methods.
- **Code First**: the model is defined in code and the database is created from EF Core migrations.

## Technologies

- .NET 10, ASP.NET Core Web API
- Entity Framework Core 10 + Npgsql (PostgreSQL)
- ASP.NET Core Identity (users and roles) + JWT Bearer authentication
- Microsoft.AspNetCore.OpenApi + Scalar (interactive API reference)
- QuestPDF (enrollment certificate)
- ClosedXML (Excel import and initial data)
- xUnit + NSubstitute (unit tests)
- Docker and Docker Compose
- PostgreSQL 16/17

## Getting started

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- PostgreSQL 16 or later running locally (or Docker, see below)

### 1. Configure the database

The development settings live in `src/InfectionSkillHub.Api/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=infection_skillhub;Username=postgres;Password=postgres"
}
```

Change the user and password if your PostgreSQL uses different ones. The database does not need to exist: it is
created on the first run. `appsettings.json` has no connection string or signing key; outside Development they
come from environment variables (`ConnectionStrings__DefaultConnection`, `Jwt__SigningKey`).

### 2. Run the API

```bash
dotnet restore
dotnet run --project src/InfectionSkillHub.Api --launch-profile http
```

On startup the API:

1. Applies the EF Core migrations (creates the database and its tables).
2. Creates the `Employee` and `Participant` roles.
3. Loads the **Instructors**, **Participants** and **Workshops** sheets of `InfectionSkillHub_Data.xlsx` (the file is
   embedded in the Infrastructure project). The load is idempotent: existing emails and codes are skipped.

Then open the API reference at **http://localhost:5180/scalar**.

The **Enrollments** sheet is never loaded on startup: it goes through `POST /api/v1/enrollments/import`.

Migrations can also be applied by hand with the local tool:

```bash
dotnet tool restore
dotnet ef database update --project src/InfectionSkillHub.Infrastructure --startup-project src/InfectionSkillHub.Api
```

### 3. Run with Docker

The client repository has to be cloned next to this one (`../InfectionSkillHub.Client`; change it with
`CLIENT_BUILD_CONTEXT` in `.env`).

```bash
cp .env.example .env    # optional locally: change passwords, ports and the JWT key
docker compose up -d --build
```

```
tests ──(exit code 0)──▶ db ──(healthy)──▶ api ──▶ client
```

Compose first runs the unit tests; if any fails nothing else starts. Then PostgreSQL, the API (which applies the
migrations and loads the initial data) and the client, whose Nginx forwards `/api` to the API container, so the
browser sees a single origin.

| Service | URL |
| --- | --- |
| Participant client | http://localhost:8667 |
| API reference (Scalar) | http://localhost:8666/scalar |
| PostgreSQL | only reachable inside Docker |

Stop it with `docker compose down` (data stays in the `db-data` volume; `docker compose down -v` removes it).

### 4. Database script

`db/InfectionSkillHub.sql` is the final database (structure and data) exported with `pg_dump`, after the initial
data load and the import of the Enrollments sheet. To restore it on an empty database:

```bash
createdb -U postgres infection_skillhub
psql -U postgres -d infection_skillhub -f db/InfectionSkillHub.sql
```

### 5. Run the tests

```bash
dotnet test
```

## Deployment (esthercita server)

The clan server only runs Docker through `sudo`, and projects get ports between 8000 and 8999 (80 and 443 belong
to the server's Nginx). InfectionSkillHub uses **8666** (API) and **8667** (client).

### First time

```bash
ssh esthercita
git clone https://github.com/Esthercita-Factory/nicholas-delarosa-InfectionSkillHub.git ~/InfectionSkillHub
git clone https://github.com/Esthercita-Factory/nicholas-delarosa-InfectionSkillHub.Client.git ~/InfectionSkillHub.Client
cd ~/InfectionSkillHub
cp .env.example .env
nano .env
```

In `.env`, change at least `POSTGRES_PASSWORD` and `JWT_SIGNING_KEY` (32+ characters, for example the output of
`openssl rand -base64 48`). Then:

```bash
sudo docker compose up -d --build   # builds, runs the tests and, if they pass, starts everything
sudo docker compose ps              # db healthy, api and client up, tests exited (0)
curl -s -o /dev/null -w "client %{http_code}\n" http://127.0.0.1:8667/
curl -s -o /dev/null -w "api %{http_code}\n" http://127.0.0.1:8666/health
```

The project is then available at `http://<server ip>:8667` (client) and `http://<server ip>:8666/scalar` (API).
If the browser cannot reach those ports, the server firewall has to open them (ask the TL).

### Optional: domains with HTTPS

The TL creates the DNS records (Cloudflare, DNS only). The names below are an example; use the ones assigned.
The site file is written without `sudo` (the `deployers` group owns `sites-available` and `sites-enabled`):

```bash
tee /etc/nginx/sites-available/infectionskillhub > /dev/null <<'EOF'
server {
    listen 80;
    server_name infectionskillhub.clandev.com;

    location / {
        proxy_pass http://127.0.0.1:8667;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        client_max_body_size 6m;
    }
}

server {
    listen 80;
    server_name services.infectionskillhub.clandev.com;

    location / {
        proxy_pass http://127.0.0.1:8666;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        client_max_body_size 6m;
    }
}
EOF
ln -s /etc/nginx/sites-available/infectionskillhub /etc/nginx/sites-enabled/
chmod 644 /etc/nginx/sites-available/infectionskillhub
sudo nginx -t && sudo systemctl reload nginx
sudo certbot --nginx -d infectionskillhub.clandev.com -d services.infectionskillhub.clandev.com
```

`'EOF'` stays quoted so the shell does not expand Nginx variables such as `$host`. Once the domains work, the
ports can be closed to the outside with `API_PORT=127.0.0.1:8666` and `CLIENT_PORT=127.0.0.1:8667` in `.env`,
followed by `sudo docker compose up -d`.

### Updating

```bash
cd ~/InfectionSkillHub.Client && git pull
cd ~/InfectionSkillHub && git pull
sudo docker compose up -d --build
```

Only the containers whose image changed are recreated; the database volume is kept.

### Useful commands and common problems

```bash
sudo docker compose logs -f api     # API logs (Ctrl+C to leave)
sudo docker compose logs tests      # result of the last test run
sudo docker compose down            # stop everything, data is kept
```

| Symptom | Likely cause |
| --- | --- |
| `service "tests" didn't complete successfully` | A test failed: `sudo docker compose logs tests` shows which one. |
| `port is already allocated` | Another project took the port: change `API_PORT` or `CLIENT_PORT` in `.env`. |
| `502 Bad Gateway` on a domain | The port in `proxy_pass` does not match `.env`, or the container is down (`sudo docker compose ps`). |
| The API does not start: `Jwt:SigningKey` | `JWT_SIGNING_KEY` is missing in `.env` or shorter than 32 characters. |
| `unable to prepare context` for `client` | The client repository is not cloned at `../InfectionSkillHub.Client` (or set `CLIENT_BUILD_CONTEXT`). |

## Credentials

### Employees (instructors)

Every InfectionSkillHub employee is an instructor. Their accounts exist from the first start of the application.

| Name | Email | Password | Specialty |
| --- | --- | --- | --- |
| Andrés Felipe Mejía Arango | andres.mejia@correo.com | `Instructor2026*` | Fotografía |
| Rosa Elena Barrios Polo | rosa.barrios@correo.com | `Instructor2026*` | Cocina |
| Mauricio Jiménez Cárdenas | mauricio.jimenez@correo.com | `Instructor2026*` | Oratoria |
| Natalia Herrera Castaño | natalia.herrera@correo.com | `Instructor2026*` | Diseño gráfico |
| Hernán Darío Quintero Gil | hernan.quintero@correo.com | `Instructor2026*` | Música |
| Paola Andrea Vargas Mora | paola.vargas@correo.com | `Instructor2026*` | Marketing digital |

### Participants

The 30 participants of the **Participants** sheet use the password `Skillhub2026*`, for example
`jorge.perez@correo.com` / `Skillhub2026*`. Anyone can also create a new participant account with
`POST /api/v1/auth/register`.

## Using the API

1. Open **Scalar** (`/scalar`).
2. Call `POST /api/v1/auth/login` with an employee or participant account.
3. Copy the `accessToken` and paste it in the **Authentication** section (Bearer token). Scalar keeps it while
   you navigate.

To bulk enroll, call `POST /api/v1/enrollments/import` as an employee with a `multipart/form-data` field named
`file`. The `InfectionSkillHub_Data.xlsx` file works as it is: if the workbook has a sheet named `Enrollments` that one is
read; otherwise the first sheet. The first row must contain the `ParticipantEmail` and `WorkshopCode` columns.
Example of a response with rejected rows:

```json
{
  "totalRows": 70,
  "enrolledCount": 68,
  "rejectedCount": 2,
  "rejectedRows": [
    { "rowNumber": 5, "participantEmail": "nobody@correo.com", "workshopCode": "COC-001", "reason": "There is no participant registered with the email nobody@correo.com." },
    { "rowNumber": 9, "participantEmail": "juan.restrepo@correo.com", "workshopCode": "COC-002", "reason": "Workshop COC-002 is full: all 10 seats are taken." }
  ]
}
```

`rowNumber` is the row inside the spreadsheet (the header is row 1).

### Endpoints

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| POST | `/api/v1/auth/register` | Public | Registers a new participant |
| POST | `/api/v1/auth/login` | Public | Signs in and returns the JWT |
| GET | `/api/v1/auth/me` | Authenticated | Profile of the signed in user |
| GET | `/api/v1/workshops` | Public | Upcoming workshops with their available seats |
| GET | `/api/v1/workshops/{id}` | Public | Workshop detail with its available seats |
| POST | `/api/v1/workshops` | Employee | Creates a workshop |
| GET | `/api/v1/workshops/{id}/enrollments` | Employee | Enrolled participants (`?includeCancelled=true` adds the cancelled ones) |
| GET | `/api/v1/instructors` | Employee | Registered instructors |
| POST | `/api/v1/enrollments` | Participant | Enrolls the authenticated participant in a workshop |
| GET | `/api/v1/enrollments/mine` | Participant | Enrollments of the authenticated participant |
| GET | `/api/v1/enrollments/{id}` | Owner or employee | Enrollment detail |
| PATCH | `/api/v1/enrollments/{id}/cancel` | Owner or employee | Cancels an enrollment |
| POST | `/api/v1/enrollments/import` | Employee | Enrolls participants from an Excel file |
| GET | `/api/v1/enrollments/{id}/certificate` | Owner or employee | Downloads the enrollment certificate (PDF) |
| GET | `/api/v1/reports/top-workshops` | Employee | Workshops with the most active enrollments (`?limit=10`) |
| GET | `/health` | Public | Database connectivity check |

### Errors

Every error is returned as `application/problem+json` with a clear `detail`:

| Status | When |
| --- | --- |
| 400 | Invalid data (validation errors come in `errors` by field) |
| 401 | Missing, invalid or expired token; wrong credentials |
| 403 | The role cannot perform the operation, or the enrollment belongs to another participant |
| 404 | The workshop or enrollment does not exist |
| 409 | Duplicated code or email, duplicated enrollment or schedule overlap |
| 422 | Business rule: full workshop, workshop already started, past dates, cancellation deadline, cancelled enrollment without certificate |

### Dates and time zone

Dates are stored and returned in UTC (`timestamptz`). InfectionSkillHub works on Bogotá time (`America/Bogota`, UTC-05:00,
configurable in `InfectionSkillHub:TimeZoneId`): the dates of the initial data are read in that zone and the certificate
prints them in it. When creating a workshop send the dates with their offset, for example
`2027-03-06T09:00:00-05:00`.

## Author

- **Full name:** Nicholas Andres De la Rosa Rivera
- **Clan:** Esthercita
- **Email:** nicholasandresdelarosarivera@hotmail.com
- **ID document:** 1044630341
