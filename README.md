# SpendWise

[![Build Status](https://img.shields.io/github/actions/workflow/status/Productive-Build-Cycle/SpendWise/ci.yml?branch=main)](https://github.com/Productive-Build-Cycle/SpendWise/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=.net)](https://dotnet.microsoft.com/download/dotnet/10.0)

> A modern, AI-enhanced personal finance management system built with Clean Architecture principles and enterprise-grade patterns.

## 📋 Overview

SpendWise is a comprehensive personal finance platform that empowers users to manage, analyze, and optimize their financial situation through intelligent transaction tracking, goal setting, and AI-driven insights. Built with maintainability, scalability, and extensibility as first-class concerns, SpendWise demonstrates production-ready software engineering practices.

### Core Features

- **Transaction Management** — Record and categorize income, expenses, and transfers with integrity verification
- **Multi-Account Support** — Manage multiple account types (Cash, Digital Wallet, Credit Card, Savings, Investment, Chequing)
- **Financial Goals & Budgets** — Set targets, track progress, and monitor budget health across spending categories
- **Comprehensive Reporting** — Income vs. expense analysis, monthly summaries, and trend visualization
- **AI-Powered Insights** — Voice-to-text transaction input and intelligent financial recommendations
- **Secure Authentication** — Token-based authentication with refresh and logout mechanisms

## 🏗️ Architecture

SpendWise follows **Clean Architecture** principles, ensuring separation of concerns and testability across four independent layers:

```
┌─────────────────────────────────────────────────────────────┐
│           SpendWise.Api (Presentation Layer)                │
│  ASP.NET Core Web API | Controllers | Middleware | DTOs     │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│       SpendWise.Application (Business Logic Layer)          │
│  MediatR Handlers | Validation | Use Cases | Abstractions   │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│      SpendWise.Domain (Core Business Rules Layer)           │
│  Entities | Value Objects | Domain Logic | Interfaces      │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│      SpendWise.Infrastructure (Data Access Layer)           │
│  Entity Framework Core | Repositories | Persistence         │
└─────────────────────────────────────────────────────────────┘
```

### Design Patterns

- **CQRS (Command Query Responsibility Segregation)** — Queries and commands separated for clarity and optimization
- **MediatR Pipeline** — Cross-cutting concerns via behaviors (logging, validation)
- **Dependency Injection** — Composition root at API tier; extension methods for testability
- **Repository Pattern** — Abstracted data access with SQL Server via EF Core
- **Domain-Driven Design** — Rich entities with validation and business logic

### Tech Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Runtime** | .NET | 10.0 |
| **Web Framework** | ASP.NET Core | 10.0.12 |
| **Database** | SQL Server | (configurable) |
| **ORM** | Entity Framework Core | 10.0.12 |
| **CQRS/Mediation** | MediatR | 14.2.0 |
| **Validation** | FluentValidation | 12.1.1 |

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- SQL Server 2019+ (local or remote instance)
- Git

### Local Development Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Productive-Build-Cycle/SpendWise.git
   cd SpendWise
   ```

2. **Configure the database connection:**
   
   Create `src/SpendWise.Api/appsettings.Local.json` (git-ignored):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(local);Database=SpendWiseDb;Trusted_Connection=true;Encrypt=false;"
     },
     "MediatR": {
       "LicenseKey": "your-mediatr-community-license-key"
     }
   }
   ```
   
   **Note:** MediatR 13+ requires a free [Community License](https://mediatrsoftware.com/licensing).

3. **Restore dependencies and build:**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Apply database migrations:**
   ```bash
   cd src/SpendWise.Infrastructure
   dotnet ef database update --project ../SpendWise.Infrastructure --startup-project ../SpendWise.Api
   ```

5. **Run the API:**
   ```bash
   cd src/SpendWise.Api
   dotnet run
   ```
   
   The API will be available at `https://localhost:5001` (or the configured port).

6. **Verify health:**
   ```bash
   curl https://localhost:5001/api/health --insecure
   ```
   Expected response: `{"status":"healthy","timestamp":"2026-10-11T...","version":"1.0.0"}`

### Docker Setup (Coming Soon)

A production-ready Dockerfile and docker-compose configuration will be added to support containerized deployment.

## 📚 API Documentation

### Available Endpoints

#### Authentication
- `POST /api/auth/register` — User registration
- `POST /api/auth/login` — User login (returns JWT)
- `POST /api/auth/refresh-token` — Refresh expired tokens
- `POST /api/auth/logout` — Revoke session

#### Accounts
- `GET /api/accounts` — List current user's accounts
- `GET /api/accounts/{accountId}` — Retrieve account details
- `GET /api/accounts/{accountId}/balance` — Get account balance
- `POST /api/accounts` — Create new account
- `PUT /api/accounts/{accountId}` — Update account
- `DELETE /api/accounts/{accountId}` — Delete account

#### Transactions
- `GET /api/transactions` — List transactions (paginated, filterable)
- `GET /api/transactions/{transactionId}` — Retrieve transaction
- `POST /api/transactions` — Record transaction
- `POST /api/transactions/income` — Record income
- `POST /api/transactions/expense` — Record expense

**Query Parameters:** `pageNumber`, `pageSize`, `fromDate`, `toDate`, `categoryId`, `type`

#### Transfers
- `GET /api/transfers` — List transfers
- `GET /api/transfers/{transferId}` — Retrieve transfer details
- `POST /api/transfers` — Create transfer between accounts

#### Categories
- `GET /api/categories` — List expense/income categories
- `GET /api/categories/{categoryId}` — Retrieve category
- `POST /api/categories` — Create category
- `PUT /api/categories/{categoryId}` — Update category
- `DELETE /api/categories/{categoryId}` — Delete category

#### Budgets
- `GET /api/budgets` — List budgets
- `GET /api/budgets/{budgetId}` — Retrieve budget
- `GET /api/budgets/{budgetId}/status` — Check remaining budget
- `POST /api/budgets` — Create budget

#### Financial Goals
- `GET /api/financial-goals` — List goals
- `POST /api/financial-goals` — Create goal
- `PUT /api/financial-goals/{id}` — Update goal
- `DELETE /api/financial-goals/{id}` — Delete goal
- `GET /api/financial-goals/{id}/progress` — Track goal progress
- `POST /api/financial-goals/{id}/progress` — Add progress to goal

#### Reports
- `GET /api/reports/income-vs-expense` — Income and expense analysis
- `GET /api/reports/monthly-summary` — Monthly financial summary

#### AI Features
- `POST /api/ai/voice-transaction` — Convert voice to transaction
- `POST /api/ai/voice-transaction/{id}/confirm` — Confirm transaction
- `GET /api/ai/insights` — Get AI-powered financial insights

### OpenAPI / Swagger

Navigate to `https://localhost:5001/openapi/v1.json` for the OpenAPI specification. Swagger UI will be available in development environments.

## 🧪 Testing

### Running Tests

Unit and integration tests will be organized in a dedicated `tests/` directory. To run all tests:

```bash
dotnet test
```

### Testing Strategy

- **Unit Tests** — Application layer (handlers, validators)
- **Integration Tests** — Infrastructure layer (database, repositories)
- **API Contract Tests** — Controller endpoints

## 📋 Project Structure

```
SpendWise/
├── src/
│   ├── SpendWise.Api/                    # Presentation layer
│   │   ├── Controllers/                  # HTTP endpoint handlers
│   │   ├── Middleware/                   # Cross-cutting concerns
│   │   ├── Program.cs                    # Startup & DI configuration
│   │   └── appsettings*.json             # Environment configuration
│   │
│   ├── SpendWise.Application/            # Business logic layer
│   │   ├── Features/                     # CQRS handlers (Queries/Commands)
│   │   ├── Common/                       # Shared behaviors, DTOs
│   │   ├── Interfaces/                   # Service contracts
│   │   └── Services/                     # Business logic
│   │
│   ├── SpendWise.Domain/                 # Domain model layer
│   │   ├── Entities/                     # Domain entities (User, Account, etc.)
│   │   └── Common/                       # Shared domain logic
│   │
│   └── SpendWise.Infrastructure/         # Data access layer
│       ├── Persistence/                  # DbContext, migrations, repositories
│       └── DependencyInjection.cs        # Infrastructure service registration
│
├── tests/                                # Test projects (planned)
├── .github/                              # GitHub Actions, issue templates
├── SpendWise.slnx                        # Solution manifest
├── README.md                             # This file
└── .gitignore
```

## 🔒 Security Considerations

### Current State
- JWT-based authentication with refresh tokens
- Entity-level ownership checks (users can only access their own data)
- Input validation via FluentValidation

### Planned Enhancements
- Role-based access control (RBAC)
- Rate limiting on API endpoints
- Encrypted sensitive data fields (passwords, card details)
- HTTPS enforcement in production
- CORS policy configuration
- Audit logging for financial transactions

## 📈 Roadmap

### Phase 1: MVP Foundation (Current)
- [x] Clean Architecture setup
- [x] Domain entity definitions
- [x] Authentication scaffolding
- [ ] Core entity implementations (User, Account, Transaction)
- [ ] Repository implementations
- [ ] CRUD endpoints for all entities

### Phase 2: Feature Richness
- [ ] Advanced reporting and analytics
- [ ] Budget tracking and alerts
- [ ] Financial goal progress visualization
- [ ] Data export (PDF, CSV)

### Phase 3: AI & Intelligence
- [ ] Voice transaction recording
- [ ] Transaction categorization via ML
- [ ] Spending pattern analysis
- [ ] Personalized financial insights
- [ ] Anomaly detection

### Phase 4: Scale & Performance
- [ ] Distributed caching (Redis)
- [ ] Event sourcing for transactions
- [ ] Background job processing (Hangfire)
- [ ] Database query optimization
- [ ] Load testing and benchmarking

## 🤝 Contributing

SpendWise is mentored by a Microsoft engineer and follows enterprise software engineering practices.

### Before You Start
1. Familiarize yourself with [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) and [CQRS patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
2. Review existing handlers in `SpendWise.Application/Features/`
3. Ensure all code follows C# coding conventions and runs through formatters

### Development Workflow

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/my-feature
   ```

2. **Make your changes** following Clean Architecture principles:
   - Domain logic in `Domain/Entities/`
   - Use cases in `Application/Features/Queries` or `Application/Features/Commands/`
   - Validation in `Application/Common/Validators/`
   - Data access in `Infrastructure/Persistence/`

3. **Build and test locally:**
   ```bash
   dotnet build
   dotnet test
   ```

4. **Commit with clear messages:**
   ```bash
   git commit -m "feat(transactions): add transfer integrity validation"
   ```

5. **Push and create a Pull Request:**
   ```bash
   git push origin feature/my-feature
   ```

### Code Standards

- **Naming:** PascalCase for classes, properties; camelCase for parameters
- **Validation:** Use FluentValidation for all DTOs and commands
- **Error Handling:** Centralized via ExceptionHandlingMiddleware
- **Logging:** Use ILogger<T> injected via DI
- **Documentation:** XML comments on public API methods

### Pull Request Checklist

- [ ] Tests pass locally (`dotnet test`)
- [ ] Code builds without warnings (`dotnet build`)
- [ ] Follows Clean Architecture principles
- [ ] Includes XML documentation for public members
- [ ] Updates CHANGELOG.md if applicable
- [ ] Addresses any linting issues

## 📄 License

SpendWise is licensed under the [MIT License](LICENSE).

## 👥 Team
- **Organization:** [Productive Build Cycle](https://github.com/Productive-Build-Cycle)

## 📞 Support

For issues, questions, or feature requests:

1. **Check [existing issues](https://github.com/Productive-Build-Cycle/SpendWise/issues)** to avoid duplicates
2. **[Create a new issue](https://github.com/Productive-Build-Cycle/SpendWise/issues/new)** with:
   - Clear title and description
   - Steps to reproduce (for bugs)
   - Expected vs. actual behavior
   - Environment details (.NET version, OS, SQL Server version)

## 📚 Additional Resources

- [.NET Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)

---

**Last Updated:** October 2026  
**Status:** Active Development
