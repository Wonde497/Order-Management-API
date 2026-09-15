# Order Management API

A production-grade, RESTful Order Management Web API built with **ASP.NET Core 10**, **Entity Framework Core 10**, and **PostgreSQL**. The application is designed using Domain-Driven Design (DDD) principles with aggregate protection, optimistic concurrency control, and rate limiting.

---

## Key Features

* **Architecture:** 3-Tier Layered Architecture (Controllers $\rightarrow$ Services $\rightarrow$ Repositories).
* **Aggregate Boundaries:** Nested routing for order items (`/api/v1/orders/{orderId}/items`) protecting business invariants.
* **Optimistic Concurrency:** RowVersion timestamp tokens on mutable entities (`Product`, `Order`) to handle concurrent updates cleanly.
* **Authentication & Authorization:** JWT Bearer authentication and role-based policies.
* **API Versioning:** URL and header-based versioning integrated with Version-Aware Swagger UI.
* **Resilience & Rate Limiting:** Fixed-window rate limiting and EF Core execution strategies with automatic connection retries.
* **Deployment Ready:** Containerized via Docker (using .NET 10 SDK & Runtime images) and configured for Vercel deployment with CI/CD via GitHub Actions.

---

## Tech Stack

* **Framework:** .NET 10 (ASP.NET Core Web API)
* **Database:** PostgreSQL via Entity Framework Core 10
* **Documentation:** Swagger 
* **Authentication:** JWT (JSON Web Tokens)
* **Containerization:** Docker (.NET 10 Base Images)
* **Deployment:** Vercel / GitHub Actions

Client Requests
│
▼
┌─────────────────────────┐
│   Controllers Layer     │ ── API Versioning, Routing & Response Formatting
└────────────┬────────────┘
│
▼
┌─────────────────────────┐
│     Services Layer      │ ── Business Validation, Stock Checks & Total Recalculation
└────────────┬────────────┘
│
▼
┌─────────────────────────┐
│   Repositories Layer    │ ── EF Core Data Access & Optimistic Concurrency Checks
└────────────┬────────────┘
│
▼
┌─────────────────────────┐
│   PostgreSQL Database   │
└─────────────────────────┘


---

## Environment Variables Configuration

Set up these key-value pairs in your local `appsettings.Development.json`, user secrets, or host environment variables (Vercel/GitHub Secrets):

| Variable Name | Description | Example |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Application Environment | `Development` / `Production` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=localhost;Database=OrderDb;Username=postgres;Password=secret` |
| `Jwt__Key` | Secret key for signing JWT tokens | `YourSuperSecretProductionKeyThatIsAtLeast32BytesLong!` |
| `Jwt__Issuer` | Valid JWT Issuer URL | `https://api.yourdomain.com` |
| `Jwt__Audience` | Valid JWT Audience URL | `https://yourdomain.com` |

---

## Local Development Setup

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [PostgreSQL](https://www.postgresql.org/)
* [EF Core CLI Tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)

### Steps

1. **Clone the Repository:**
   ```bash
   git clone [https://github.com/Wonde497/OrderManagementAPI.git](https://github.com/Wonde497/OrderManagementAPI.git)
   cd OrderManagementAPI

---

## System Architecture
