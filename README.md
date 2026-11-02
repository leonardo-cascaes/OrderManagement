# OrderManagement

A RESTful order management API built with **C# and .NET 10**, applying layered architecture, Domain-Driven Design principles, CQRS, and Entity Framework Core.

The project focuses on maintainable code, separation of concerns, business rule enforcement, automated testing, and reliable data persistence.

> **Project status:** In development.

## Overview

OrderManagement is a backend application designed to manage customers, products, and orders.

Its main use case is order creation, including customer validation, product availability checks, stock management, and order persistence.

The application enforces business rules within the Domain layer and uses the Application layer to orchestrate use cases, keeping infrastructure concerns isolated.

## Features

### Order Management
- Create orders for existing customers.
- Validate customer and product existence.
- Validate requested quantities.
- Check product stock availability.
- Automatically decrease product stock when an order is created.
- Calculate order totals based on item quantities and unit prices.
- Manage order status transitions.
- Handle insufficient stock and concurrency conflicts.

### Data Persistence
- Entity Framework Core with PostgreSQL.
- Repository Pattern.
- Unit of Work.
- Code-first database migrations.
- Optimistic concurrency control for product stock.
- Transactional persistence of orders and stock changes.

### Application Architecture
- Layered architecture.
- Domain-Driven Design principles.
- CQRS with MediatR.
- FluentValidation.
- Dependency Injection.
- Centralized validation pipeline.
- Consistent HTTP error handling.

### Automated Testing
- Unit tests with xUnit and FluentAssertions.
- Integration tests for repositories and persistence.
- SQLite in-memory database for isolated integration tests.
- API integration tests using `WebApplicationFactory`.
- Tests covering business rules, stock management, and concurrency scenarios.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| .NET 10 | Application platform |
| ASP.NET Core Web API | HTTP API |
| Entity Framework Core 10 | Object-relational mapping |
| PostgreSQL 16 | Relational database |
| Npgsql | PostgreSQL provider |
| MediatR | CQRS and request handling |
| FluentValidation | Input validation |
| xUnit | Testing framework |
| FluentAssertions | Test assertions |
| SQLite | Isolated integration testing |
| Docker | Database containerization |

## Architecture

The solution is organized into four main projects:

```text
OrderManagement/
│
├── src/
│   ├── OrderManagement.Api/
│   ├── OrderManagement.Application/
│   ├── OrderManagement.Domain/
│   └── OrderManagement.Infrastructure/
│
├── tests/
│   ├── OrderManagement.Domain.Tests/
│   ├── OrderManagement.Application.Tests/
│   └── OrderManagement.IntegrationTests/
│
├── .config/
│   └── dotnet-tools.json
│
├── global.json
├── .gitignore
└── OrderManagement.slnx
```

### Domain

Contains the core business model and business rules.

Main entities:

- `Customer`
- `Product`
- `Order`
- `OrderItem`

The Domain layer does not depend on Application, Infrastructure, or API.

### Application

Contains application use cases and orchestration logic.

Responsibilities include:

- Commands and handlers.
- Repository interfaces.
- Unit of Work abstraction.
- Validation rules.
- Application-specific exceptions.
- Result handling.

### Infrastructure

Contains technical implementations required by the application.

Responsibilities include:

- Entity Framework Core DbContext.
- Entity configurations.
- Repository implementations.
- PostgreSQL integration.
- Database migrations.
- Unit of Work implementation.

### API

Provides the HTTP interface to the application.

Responsibilities include:

- Controllers and endpoints.
- Request and response models.
- Dependency injection composition root.
- HTTP status code mapping.
- Exception handling.

## Domain Model

The main relationships are:

```text
Customer
   |
   | 1:N
   |
 Order
   |
   | 1:N
   |
OrderItem
   |
   | N:1
   |
 Product
```

### Business Rules

**Customer**
- Name and email are required.
- Email must satisfy the domain's basic validity checks.

**Product**
- Price must be greater than zero.
- Stock cannot be negative.
- Stock additions and removals must use positive quantities.
- Stock removal cannot exceed available stock.

**Order**
- Every order starts with `Pending` status.
- Only pending orders can be confirmed or cancelled.
- Only confirmed orders can be completed.
- Items can only be added to pending orders.
- The order total is calculated from its items.

**OrderItem**
- Product ID must be valid.
- Quantity must be greater than zero.
- Unit price must be greater than zero.
- Unit price is stored as a historical snapshot of the product price when the order is created.

## API

### Currently implemented endpoint

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/orders` | Create an order |

**Example request:**

```json
{
  "customerId": "00000000-0000-0000-0000-000000000001",
  "productId": "00000000-0000-0000-0000-000000000002",
  "quantity": 3
}
```

**Successful response: `201 Created`**

```json
{
  "id": "00000000-0000-0000-0000-000000000003",
  "customerId": "00000000-0000-0000-0000-000000000001",
  "status": 0,
  "total": 10500.00,
  "orderItems": [
    {
      "id": "00000000-0000-0000-0000-000000000004",
      "productId": "00000000-0000-0000-0000-000000000002",
      "quantity": 3,
      "unitPrice": 3500.00
    }
  ]
}
```

Order status values:

| Value | Status |
|---:|---|
| 0 | Pending |
| 1 | Confirmed |
| 2 | Completed |
| 3 | Cancelled |

### HTTP Status Codes

| Status | Description |
|---|---|
| 201 | Order successfully created |
| 400 | Request validation failure |
| 404 | Customer or product not found |
| 409 | Insufficient stock or concurrency conflict |
| 500 | Unexpected server error |

*The current Create Order use case accepts one product per request. Additional customer, product, and order management endpoints are planned.*

## Getting Started

### Prerequisites

- .NET 10 SDK.
- PostgreSQL 16.
- Docker (recommended for running PostgreSQL).
- Git.

### 1. Clone the repository

```bash
git clone https://github.com/leonardo-cascaes/OrderManagement.git

cd OrderManagement
```

### 2. Restore dependencies and local tools

```bash
dotnet tool restore

dotnet restore
```

### 3. Start PostgreSQL

You can use an existing PostgreSQL instance or start one with Docker:

```bash
docker run -d \
  --name ordermanagement-postgres \
  -e POSTGRES_DB=ordermanagement \
  -e POSTGRES_USER=ordermanagement \
  -e POSTGRES_PASSWORD=change-me \
  -p 5433:5432 \
  -v ordermanagement_postgres_data:/var/lib/postgresql/data \
  postgres:16-alpine
```

The database will be available at `localhost:5433`.

### 4. Configure the connection string

User Secrets are used to store local development credentials outside the repository.

Initialize User Secrets for the API project:

```bash
dotnet user-secrets init --project src/OrderManagement.Api
```

Set the connection string:

```bash
dotnet user-secrets set "ConnectionStrings:OrderManagement" \
  "Host=localhost;Port=5433;Database=ordermanagement;Username=ordermanagement;Password=change-me" \
  --project src/OrderManagement.Api
```

Replace the example password with the password configured for your PostgreSQL instance.

The API must run in the `Development` environment to load User Secrets automatically.

### 5. Apply database migrations

```bash
dotnet ef database update \
  --context OrderManagementDbContext \
  --project src/OrderManagement.Infrastructure \
  --startup-project src/OrderManagement.Api
```

### 6. Run the API

```bash
dotnet run --project src/OrderManagement.Api
```

## Running Tests

Restore dependencies before running the test suite:

```bash
dotnet restore
```

Run all tests:

```bash
dotnet test
```

The test suite includes:

- Domain business rule validation.
- Application use case testing.
- Repository integration tests.
- EF Core persistence tests.
- API integration tests.
- Stock concurrency scenarios.

SQLite in-memory databases are used to isolate persistence integration tests from external database dependencies.

## Design Decisions

Some architectural decisions were made to keep the application maintainable:

**
