# VoteMe 🗳️

A secure, scalable **multi-organization voting platform** built with ASP.NET Core 9. VoteMe enables organizations to create and manage elections, register voters, and collect votes — all through a clean REST API.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [Authentication](#authentication)
- [Background Jobs](#background-jobs)
- [Caching](#caching)
- [Event System](#event-system)
- [Security](#security)
- [Database](#database)

---

## Overview

VoteMe allows multiple organizations to run independent elections on a single platform. Each organization manages its own members, elections, categories, and candidates. Voters can only participate in elections belonging to organizations they are approved members of.

### Core Flow

```
Register Organization
        ↓
Members join via UniqueKey → Pending
        ↓
Admin approves members → Approved
        ↓
Admin creates Election → Pending
        ↓
Admin adds Categories + Candidates
        ↓
Admin opens Election → Active (sets EndDate)
        ↓
Approved members vote (one vote per category)
        ↓
Election auto-closes at EndDate via Hangfire
        ↓
Results available
```

---

## Architecture

VoteMe follows **Clean Architecture** with 4 projects:

```
VoteMe/
├── VoteMe.API/              → Controllers, Middleware, Program.cs
├── VoteMe.Application/      → Services, DTOs, Interfaces, Mappers, Events
├── VoteMe.Domain/           → Entities, Enums, Exceptions, Base classes
└── VoteMe.Infrastructure/   → EF Core, Repositories, Redis, RabbitMQ, Hangfire, Cloudinary
```

### Design Patterns Used

- **Repository Pattern** with Generic Repository base
- **Unit of Work** for transaction management
- **CQRS-lite** — services handle reads and writes separately
- **Domain Events** via RabbitMQ message bus
- **Soft Deletes** — no data is permanently deleted (global query filter)

---

## Features

### Organizations
- Register an organization with an admin account
- Auto-generated unique key for member invitations
- Logo upload via Cloudinary
- Admin can approve/reject/ban members
- Admin can promote/demote members

### Elections
- Create elections under an organization
- Add categories (e.g. President, Vice President)
- Add candidates per category with optional photo
- Manually open elections (sets real StartDate)
- Auto-close elections at EndDate via Hangfire background job
- Soft delete elections

### Voting
- Approved members only can vote
- One vote per category per voter
- Voters can change their vote while election is active
- Real-time tie detection in results

### Results
- Per-category results with vote counts and percentages
- Winner detection with tie handling
- Results cached for closed elections

### Users
- JWT authentication with token versioning
- Role-based access: `SuperAdmin`, `OrgAdmin`, `Voter`
- SuperAdmin can delete any user
- Audit logging for key actions

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server (LocalDB / SQL Express) |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Caching | Redis (StackExchange.Redis) |
| Background Jobs | Hangfire |
| Message Broker | RabbitMQ |
| Image Storage | Cloudinary |
| Logging | AWS CloudWatch |
| API Docs | Swagger / Scalar |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Express)
- [Redis](https://redis.io/download)
- [RabbitMQ](https://www.rabbitmq.com/download.html)
- [Cloudinary Account](https://cloudinary.com/)

### Installation

```bash
# Clone the repository
git clone https://github.com/your-username/VoteMe.git
cd VoteMe

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project VoteMe.Infrastructure --startup-project VoteMe.API

# Run the application
dotnet run --project VoteMe.API
```

### API Documentation

Once running, visit:
```
https://localhost:7251/swagger
```

Hangfire Dashboard:
```
https://localhost:7251/hangfire
```

---

## Configuration

Create an `appsettings.json` file in `VoteMe.API/` based on the template below. **Never commit this file to source control.**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=VoteMe;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Key": "YOUR_JWT_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "VoteMeServer",
    "Audience": "VoteMeAudience",
    "ExpiryMinutes": 60
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "CloudinarySettings": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  },
  "SuperAdmin": {
    "FirstName": "Super",
    "LastName": "Admin",
    "DisplayName": "Super Admin",
    "Email": "superadmin@yourdomain.com",
    "Password": "YourSecurePassword123!"
  },
  "AWS": {
    "AccessKeyId": "YOUR_ACCESS_KEY",
    "SecretAccessKey": "YOUR_SECRET_KEY",
    "Region": "us-west-1",
    "LogGroupName": "VoteMeGroup",
    "LogStreamName": "VoteMeStream"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  }
}
```

> ⚠️ Add `appsettings.json` and `appsettings.Development.json` to your `.gitignore`

---

## API Reference

### Auth
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Register a voter with UniqueKey | Public |
| POST | `/api/auth/login` | Login and receive JWT | Public |
| POST | `/api/auth/logout` | Invalidate token | Authenticated |
| POST | `/api/auth/change-password` | Change password | Authenticated |
| POST | `/api/auth/register-organization` | Register org + admin | Public |

### Organizations
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/organization/{id}` | Get organization details | Authenticated |
| PUT | `/api/organization/{id}` | Update organization | OrgAdmin |
| DELETE | `/api/organization/{id}` | Delete organization | OrgAdmin |

### Organization Members
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/organizationmember/{orgId}/pending` | Get pending members | OrgAdmin |
| POST | `/api/organizationmember/{orgId}/{userId}/approve` | Approve member | OrgAdmin |
| POST | `/api/organizationmember/{orgId}/{userId}/reject` | Reject member | OrgAdmin |
| POST | `/api/organizationmember/join` | Join org with UniqueKey | Authenticated |
| POST | `/api/organizationmember/leave/{orgId}` | Leave organization | Authenticated |
| GET | `/api/organizationmember/my-organizations` | Get user's organizations | Authenticated |

### Elections
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/election` | Create election | OrgAdmin |
| GET | `/api/election/{id}` | Get election details | Authenticated |
| PUT | `/api/election/{id}` | Update election | OrgAdmin |
| DELETE | `/api/election/{id}` | Delete election | OrgAdmin |
| POST | `/api/election/{id}/open` | Open election | OrgAdmin |
| GET | `/api/election/{id}/results` | Get election results | Authenticated |
| GET | `/api/election/organization/{orgId}` | Get org elections | Authenticated |

### Election Categories
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/electioncategory` | Create category | OrgAdmin |
| GET | `/api/electioncategory/{id}` | Get category | Authenticated |
| PATCH | `/api/electioncategory/{id}` | Update category | OrgAdmin |
| DELETE | `/api/electioncategory/{id}` | Delete category | OrgAdmin |

### Candidates
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/candidate` | Add candidate | OrgAdmin |
| GET | `/api/candidate/{id}` | Get candidate | Authenticated |
| PATCH | `/api/candidate/{id}` | Update candidate | OrgAdmin |
| DELETE | `/api/candidate/{id}` | Delete candidate | OrgAdmin |

### Votes
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/votes/{candidateId}` | Cast or change vote | Voter |

### Users
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/users/{id}` | Get user profile | Authenticated |
| DELETE | `/api/users/{id}` | Delete user | SuperAdmin |

---

## Authentication

VoteMe uses **JWT Bearer tokens** with token versioning to support instant token invalidation.

### How it works

1. Login returns a signed JWT token
2. Include it in every request:
```
Authorization: Bearer <your_token>
```
3. On logout or password change, `TokenVersion` is incremented in the DB
4. Every request validates the token version — stale tokens are rejected even before expiry

### Roles

| Role | Description |
|---|---|
| `SuperAdmin` | Platform-level admin. Can delete users |
| `OrgAdmin` | Organization admin. Manages elections and members |
| `Voter` | Regular member. Can vote in approved elections |

---

## Background Jobs

Hangfire is used to automatically close elections at their scheduled `EndDate`.

```
Admin opens election → sets EndDate
        ↓
Hangfire schedules CloseElectionAsync for EndDate
        ↓
At EndDate → election status set to Closed
           → results computed
           → members notified via RabbitMQ
```

Monitor jobs at `/hangfire`.

---

## Caching

Redis is used to cache frequently read data and reduce database load.

| Cache Key | TTL | Invalidated When |
|---|---|---|
| `election-{id}` | 10 mins | Election updated, opened, deleted |
| `organization-elections-{orgId}` | 10 mins | Election created, updated, deleted |
| `election-results-{id}` | 1 hr | Never (closed elections don't change) |
| `user-{id}` | 15 mins | User updated or deleted |
| `election-category-candidates-{id}` | 10 mins | Candidate added, updated, deleted |

Cache failures degrade gracefully — the app falls through to the database without returning errors.

---

## Event System

VoteMe uses RabbitMQ to publish domain events asynchronously. Consumers handle side effects like sending emails.

| Event | Trigger | Consumer Action |
|---|---|---|
| `user-registered` | New voter registers | Send welcome email |
| `organization-created` | Org registered | Send confirmation with UniqueKey |
| `election-created` | Election created | Notify org members |
| `election-opened` | Election goes active | Notify org members |
| `election-closed` | Hangfire closes election | Send results to members |
| `vote-cast` | Voter casts vote | Send vote confirmation |
| `vote-changed` | Voter changes vote | Send change confirmation |
| `password-changed` | User changes password | Send security alert |
| `member-joined` | User joins org | Notify admin |

---

## Security

- **JWT token versioning** — tokens invalidated on logout/password change
- **Soft deletes** — data is never permanently removed
- **Membership approval** — members must be approved before voting
- **Organization isolation** — voters can only vote in their approved orgs
- **Role-based policies** — `OrgAdmin`, `Voter`, `SuperAdmin` policies enforced per endpoint
- **Audit logging** — key actions (login, logout, reads, updates) are logged

---

## Database

VoteMe uses SQL Server with EF Core Code First migrations.

### Key Entities

```
AppUser
    └── OrganizationMember (many)
            └── Organization
                    └── Election (many)
                            └── ElectionCategory (many)
                                    └── Candidate (many)
                                            └── Vote (many)
```

### Soft Deletes

All entities inherit from `BaseEntity` which includes `IsDeleted`, `DeletedAt`. A global EF Core query filter ensures deleted records are never returned in queries.

### Running Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project VoteMe.Infrastructure --startup-project VoteMe.API

# Apply migrations
dotnet ef database update --project VoteMe.Infrastructure --startup-project VoteMe.API

# Rollback last migration
dotnet ef migrations remove --project VoteMe.Infrastructure --startup-project VoteMe.API
```

---

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

---

## License

This project is licensed under the MIT License.