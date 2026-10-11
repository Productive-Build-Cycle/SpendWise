# SpendWise DevOps & Database Engineering Roadmap

**Owner:** Amir-Golmoradi (DevOps + DB Engineer)  
**Status:** Active Planning  
**Last Updated:** October 2026

---

## 📋 Executive Summary

This document defines the complete DevOps and Database Engineering responsibilities for SpendWise. As the sole DevOps + DB Engineer, you are responsible for:

- **Infrastructure as Code** — deployment pipelines, environments, secrets management
- **CI/CD Automation** — build, test, deploy workflows
- **Database Design & Optimization** — schema design, migrations, backups, performance tuning
- **Monitoring & Observability** — health checks, logging, alerting, metrics
- **Disaster Recovery** — backup strategy, failover procedures, data retention
- **Security & Compliance** — secrets management, access control, audit logging

---

## 🔧 DevOps Responsibilities

### 1. **CI/CD Pipeline (GitHub Actions)**

#### What You Must Do:

**1.1 Build Workflow**
- Trigger on: `push` to `main`, `develop`; `pull_request` to any branch
- Steps:
  - Check out code
  - Set up .NET 10 SDK
  - Restore dependencies
  - Build solution
  - Run unit tests
  - Publish artifacts (Docker image, binaries, reports)
  - Generate code coverage report
  - Upload to artifact storage (GitHub Artifacts or Docker Registry)

**1.2 Code Quality Gates**
- Run linting (StyleCop, FxCop via dotnet analyzers)
- Check test coverage (threshold: 70%+)
- Security scanning (SonarQube, or GitHub Advanced Security)
- Dependency vulnerability scanning (Dependabot)

**1.3 What You'll Create:**
```
.github/workflows/
  ├── ci-build.yml           # Build & test on every PR
  ├── cd-deploy-staging.yml  # Deploy to staging on merge to main
  ├── cd-deploy-prod.yml     # Deploy to production (manual trigger)
  └── db-migrations.yml      # Run EF Core migrations on deployment
```

**Tools:** GitHub Actions (free), Docker, .NET CLI, dotnet-format

---

### 2. **Containerization**

#### What You Must Do:

**2.1 Docker Image for API**
- Create `Dockerfile` (multi-stage build):
  - Stage 1: Build stage (.NET SDK)
  - Stage 2: Runtime stage (.NET Runtime only, smaller image)
- Build layers:
  - Copy `.csproj` files
  - `dotnet restore`
  - Copy source code
  - `dotnet build` and `dotnet publish`
  - Final image runs `dotnet SpendWise.Api.dll`

**2.2 Docker Compose for Local Development**
- Services:
  - `api` (SpendWise.Api container)
  - `sqlserver` (SQL Server container)
  - `volumes` (persistent database storage)
- Environment variables for local overrides
- Port mappings: `5001:8080` for API, `1433:1433` for SQL Server

**2.3 Image Registry**
- Push to Docker Hub / GitHub Container Registry / Azure Container Registry
- Tag strategy: `latest`, `v1.0.0`, `main-sha-abc123`
- Scan images for vulnerabilities (Trivy, Snyk)

**2.4 What You'll Create:**
```
├── Dockerfile              # Multi-stage build
├── docker-compose.yml      # Local dev environment
├── .dockerignore           # Exclude build artifacts, .git, etc.
└── docker/
    ├── entrypoint.sh       # Health checks, migration runner
    └── healthcheck.sh      # Kubernetes/Docker readiness probe
```

**Tools:** Docker, Docker Compose, Trivy (vulnerability scanning)

---

### 3. **Environment Management**

#### What You Must Do:

**3.1 Secrets Management**
- Store secrets in **GitHub Secrets** (per environment):
  - `SQL_CONNECTION_STRING_DEV`
  - `SQL_CONNECTION_STRING_STAGING`
  - `SQL_CONNECTION_STRING_PROD`
  - `MEDIATR_LICENSE_KEY`
  - `JWT_SECRET_KEY`
  - `API_KEYS` (for third-party services like AI APIs)

- For production: migrate to **Azure Key Vault** / **Vault by HashiCorp**

**3.2 Environment Variables by Tier**
```
Development:
  - LocalDB or SQL Server container
  - ASPNETCORE_ENVIRONMENT=Development
  - OpenAPI (Swagger) enabled
  - Debug logging enabled

Staging:
  - Azure SQL Database / RDS
  - ASPNETCORE_ENVIRONMENT=Staging
  - OpenAPI disabled
  - Structured logging
  - Same data schema as prod

Production:
  - Managed SQL Server (Azure SQL / AWS RDS)
  - ASPNETCORE_ENVIRONMENT=Production
  - HTTPS enforced
  - Rate limiting enabled
  - Minimal logging (critical only)
```

**3.3 Configuration Strategy**
- Use `appsettings.json` for defaults
- `appsettings.{Environment}.json` for env-specific overrides
- Environment variables override both (12-factor app principle)
- Avoid hardcoding secrets in config files
- Document required env vars in `docs/ENVIRONMENT_VARIABLES.md`

**3.4 What You'll Create:**
```
docs/
  ├── ENVIRONMENT_VARIABLES.md    # Required vars per environment
  ├── SECRETS_MANAGEMENT.md        # How to rotate, audit secrets
  └── DEPLOYMENT_CHECKLIST.md      # Pre-deploy validation steps

.github/
  └── environments/
      ├── development              # Auto-deploy on main
      ├── staging                  # Manual approval required
      └── production               # Manual approval + sign-off
```

**Tools:** GitHub Secrets, Azure Key Vault, Vault, GitHub Environments

---

### 4. **Deployment Orchestration**

#### What You Must Do:

**4.1 Deployment Targets (Choose One or Multiple)**

**Option A: Azure App Service (Recommended for .NET)**
- Deployment slot strategy: `production` → `staging` → swap on success
- Auto-scaling: CPU > 70% scale up; CPU < 30% scale down
- Health checks: `/api/health` endpoint (Azure built-in)
- Managed SQL Server (Azure SQL Database)
- Application Insights integration

**Option B: Kubernetes (AKS / EKS)**
- Helm charts for SpendWise API
- Services: ClusterIP for internal, LoadBalancer for external
- Deployment strategy: Rolling update (25% at a time)
- Pod disruption budgets for high availability
- Horizontal Pod Autoscaler (HPA) based on CPU/memory

**Option C: Docker Compose (Development Only)**
- Local containers for entire stack
- Quick feedback loop for developers

**4.2 Deployment Flow**
```
1. Code merged to main
   ↓
2. GitHub Actions builds Docker image
   ↓
3. Image pushed to registry + tagged
   ↓
4. Deploy to staging (auto)
   ↓
5. Run smoke tests on staging
   ↓
6. Manual approval for production
   ↓
7. Deploy to production (blue-green or rolling)
   ↓
8. Run health checks + monitoring
   ↓
9. Alert on-call if issues detected
```

**4.3 Rollback Strategy**
- Keep previous 3 image versions in registry
- Instant rollback: revert to previous deployment
- Database rollback: EF Core migration reversions (downward migrations)
- Communication plan: notify stakeholders of rollback

**4.4 What You'll Create:**
```
.github/workflows/
  ├── deploy-staging.yml      # Runs after merge to main
  └── deploy-production.yml   # Requires manual approval

k8s/ (if using Kubernetes)
  ├── deployment.yaml         # Pod spec, replicas, resource limits
  ├── service.yaml            # ClusterIP or LoadBalancer
  ├── configmap.yaml          # Non-secret env vars
  ├── secret.yaml             # Secrets mounted as volumes
  ├── ingress.yaml            # Routing rules (if using ingress)
  └── helm/
      └── spendwise/          # Helm chart for templating

docs/
  └── DEPLOYMENT_GUIDE.md     # Step-by-step deployment
```

**Tools:** GitHub Actions, Azure CLI / AWS CLI, kubectl (Kubernetes), Helm

---

### 5. **Monitoring, Logging & Observability**

#### What You Must Do:

**5.1 Health Checks**
- API endpoint: `GET /api/health` returns:
  ```json
  {
    "status": "healthy",
    "timestamp": "2026-10-11T10:30:00Z",
    "version": "1.0.0",
    "database": {
      "status": "connected",
      "responseTime": "45ms"
    }
  }
  ```
- Kubernetes: liveness probe every 10s, readiness probe every 5s
- Timeout: 3s; failure threshold: 2

**5.2 Structured Logging**
- Implement JSON structured logs (not plain text)
- Log levels: Debug (dev only), Info (important events), Warning (non-fatal issues), Error (failures)
- Include trace ID for request correlation
- Log fields: timestamp, level, service, method, status_code, duration_ms, user_id, error_details
- Aggregation platform: ELK Stack, Splunk, or Azure Monitor

**5.3 Metrics & Monitoring**
- Application metrics:
  - Request count (by endpoint, status code)
  - Request latency (p50, p95, p99)
  - Error rate
  - Database query duration
  - Transaction hash verification failures
  - Active connections

- Infrastructure metrics:
  - CPU, memory, disk usage
  - Network I/O
  - Container restarts

- Database metrics:
  - Query execution time
  - Lock wait time
  - Index fragmentation
  - Connection pool usage

- Tool: Prometheus + Grafana / Azure Monitor / CloudWatch

**5.4 Alerting**
- PagerDuty / OpsGenie integration for on-call
- Conditions:
  - API down (health check fails for 1 min)
  - Error rate > 5% for 5 minutes
  - P99 latency > 2s for 10 minutes
  - Database connection pool exhausted
  - Disk usage > 80%
  - Deployment failure

**5.5 What You'll Create:**
```
src/SpendWise.Api/
  ├── Controllers/HealthController.cs  # Implement health endpoint

.github/workflows/
  └── monitoring-setup.yml   # Deploy monitoring infrastructure

docs/
  ├── MONITORING.md          # Dashboard URLs, alert thresholds
  ├── LOGGING.md             # Log format, aggregation pipeline
  └── RUNBOOK.md             # How to respond to common alerts

docker/
  └── entrypoint.sh          # Health check probe script

k8s/
  ├── probes.yaml            # Liveness & readiness probe config
  └── servicemonitor.yaml    # Prometheus scrape config (if using Prometheus)
```

**Tools:** ELK Stack, Prometheus + Grafana, Azure Monitor, Splunk, PagerDuty

---

### 6. **Backup & Disaster Recovery**

#### What You Must Do:

**6.1 Backup Strategy**
- Frequency:
  - Full backup daily at 2 AM UTC
  - Incremental backups every 6 hours
  - Transaction log backups every 15 minutes (for RTO < 15 min)

- Retention:
  - Daily backups: 7 days
  - Weekly backups: 4 weeks
  - Monthly backups: 1 year
  - Compliance backups: As per data retention policy

- Location:
  - Primary: Azure Blob Storage / S3 with geo-redundancy
  - Secondary: Different region (for disaster recovery)

**6.2 Recovery Testing**
- Monthly: Test full restore from backup (point-in-time recovery)
- Document RTO (Recovery Time Objective): < 1 hour
- Document RPO (Recovery Point Objective): < 15 minutes
- Alert on backup failures immediately

**6.3 Disaster Recovery Plan**
- **Scenario 1: Database corruption**
  - Step 1: Detect via health check
  - Step 2: Notify team, trigger incident response
  - Step 3: Restore from latest clean backup
  - Step 4: Verify data integrity
  - Step 5: Resume service

- **Scenario 2: Entire region down (cloud provider outage)**
  - Failover to secondary region
  - Update DNS / load balancer
  - Verify API endpoints operational

- **Scenario 3: Data breach / ransomware**
  - Isolate affected environment
  - Restore from immutable backup (air-gapped)
  - Audit access logs for breach timeline

**6.4 What You'll Create:**
```
docs/
  ├── BACKUP_STRATEGY.md           # Backup schedule, retention, testing
  ├── DISASTER_RECOVERY_PLAN.md    # Step-by-step recovery procedures
  └── INCIDENT_RUNBOOK.md          # Response playbooks

scripts/
  ├── backup-database.go           # Backup orchestration (in Go!)
  ├── restore-database.go          # Restore procedure
  ├── test-backup-integrity.go     # Monthly test automation
  └── verify-replication.go        # Cross-region backup verification

.github/workflows/
  └── backup-verification.yml      # Weekly backup restoration test
```

**Tools:** Azure Backup / AWS Backup, SQL Server native backups, Velero (Kubernetes)

---

### 7. **Infrastructure as Code (IaC)**

#### What You Must Do:

**7.1 Infrastructure Definition**
- Define all cloud resources in code (no manual clicking in portals)
- Include:
  - App Service / Container App instance
  - SQL Server / Database
  - Key Vault
  - Storage Account (for backups, logs)
  - Virtual Network / Security Groups
  - CDN (optional, for frontend assets)
  - Load Balancer / Application Gateway

**7.2 IaC Tool (Choose One)**
- **Terraform** (recommended for multi-cloud)
- **Bicep** (native Azure)
- **CloudFormation** (AWS)
- **Pulumi** (programmatic, can use Go!)

**7.3 Environment Parity**
- Same resource definitions for dev, staging, prod
- Only scale/SKU differs (prod has more replicas, larger DB)
- All environments deployed from same code

**7.4 What You'll Create:**
```
infrastructure/
  ├── terraform/ (or bicep/)
  │   ├── main.tf                    # Resource definitions
  │   ├── variables.tf               # Input variables (region, env name)
  │   ├── outputs.tf                 # Endpoint URLs, resource IDs
  │   ├── databases.tf               # SQL Server + database config
  │   ├── networking.tf              # VNet, NSG, DNS
  │   └── environments/
  │       ├── dev.tfvars             # dev-specific values
  │       ├── staging.tfvars         # staging-specific values
  │       └── prod.tfvars            # prod-specific values
  │
  └── scripts/
      ├── init-terraform.sh          # Initialize Terraform backend
      └── apply-infrastructure.sh    # Apply changes with approval
```

**Tools:** Terraform, Bicep, Pulumi, Azure Resource Manager

---

### 8. **Security Hardening**

#### What You Must Do:

**8.1 Network Security**
- API only accessible via HTTPS (TLS 1.2+)
- Database only accessible from API subnet (no internet exposure)
- Web Application Firewall (WAF) rules:
  - Block SQL injection patterns
  - Block XSS payloads
  - Rate limit per IP (10,000 requests/hour)
- IP whitelisting for admin access (optional)

**8.2 Secrets Management**
- Never commit secrets to Git
- Use GitHub Secrets or Azure Key Vault
- Rotate credentials every 90 days
- Audit secret access logs
- Alert on secret rotation failures

**8.3 Access Control**
- Production deployments require approval from 2+ authorized users
- Database backups encrypted at rest (AES-256)
- Enable SQL Server Transparent Data Encryption (TDE)
- Enable audit logging for all data modifications

**8.4 What You'll Create:**
```
docs/
  ├── SECURITY_POLICY.md             # Security standards & compliance
  ├── INCIDENT_RESPONSE.md           # Security incident procedures
  └── SECRETS_ROTATION_SCHEDULE.md   # When/how to rotate credentials

scripts/
  └── security-audit.go              # Automated security checks (in Go!)
```

**Tools:** Azure Security Center, WAF rules, SSH key management, encrypted secrets

---

## 💾 Database Engineering Responsibilities

### 1. **Database Schema Design**

#### What You Must Do:

**1.1 Entity Relationships & Schema**

Create SQL Server tables for all domain entities:

```sql
-- Users
CREATE TABLE Users (
  UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  FirstName NVARCHAR(100) NOT NULL,
  LastName NVARCHAR(100) NOT NULL,
  UserName NVARCHAR(100) NOT NULL UNIQUE,
  Email NVARCHAR(255) NOT NULL UNIQUE,
  PasswordHash NVARCHAR(MAX) NOT NULL,
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  IsActive BIT NOT NULL DEFAULT 1
);

-- Accounts
CREATE TABLE Accounts (
  AccountId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId UNIQUEIDENTIFIER NOT NULL,
  Name NVARCHAR(100) NOT NULL,
  AccountType NVARCHAR(50) NOT NULL, -- Cash, DigitalWallet, CreditCard, Savings, Investment, Chequing
  Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
  Currency NVARCHAR(3) NOT NULL DEFAULT 'USD',
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Categories
CREATE TABLE Categories (
  CategoryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId UNIQUEIDENTIFIER NOT NULL,
  Name NVARCHAR(100) NOT NULL,
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
  UNIQUE(UserId, Name)
);

-- Transactions
CREATE TABLE Transactions (
  TransactionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  AccountId UNIQUEIDENTIFIER NOT NULL,
  UserId UNIQUEIDENTIFIER NOT NULL,
  CategoryId UNIQUEIDENTIFIER,
  GoalId UNIQUEIDENTIFIER, -- Optional, for goal-linked transactions
  Amount DECIMAL(18,2) NOT NULL,
  TransactionType NVARCHAR(50) NOT NULL, -- Income, Expense
  TransactionHash NVARCHAR(256) NOT NULL UNIQUE, -- SHA-256 hash for integrity
  TransactionDate DATETIME2 NOT NULL,
  Description NVARCHAR(500),
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
  FOREIGN KEY (UserId) REFERENCES Users(UserId),
  FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId) ON DELETE SET NULL
);

-- Transfers
CREATE TABLE Transfers (
  TransferId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId UNIQUEIDENTIFIER NOT NULL,
  FromAccountId UNIQUEIDENTIFIER NOT NULL,
  ToAccountId UNIQUEIDENTIFIER NOT NULL,
  Amount DECIMAL(18,2) NOT NULL,
  TransactionDate DATETIME2 NOT NULL,
  Description NVARCHAR(500),
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(UserId),
  FOREIGN KEY (FromAccountId) REFERENCES Accounts(AccountId),
  FOREIGN KEY (ToAccountId) REFERENCES Accounts(AccountId)
);

-- Budgets
CREATE TABLE Budgets (
  BudgetId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId UNIQUEIDENTIFIER NOT NULL,
  CategoryId UNIQUEIDENTIFIER,
  Amount DECIMAL(18,2) NOT NULL,
  StartDate DATETIME2 NOT NULL,
  EndDate DATETIME2 NOT NULL,
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(UserId),
  FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId) ON DELETE SET NULL
);

-- Financial Goals
CREATE TABLE FinancialGoals (
  GoalId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId UNIQUEIDENTIFIER NOT NULL,
  Name NVARCHAR(200) NOT NULL,
  TargetAmount DECIMAL(18,2) NOT NULL,
  CurrentAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
  TargetDate DATETIME2 NOT NULL,
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- AI Conversations (for voice transaction & insights)
CREATE TABLE AIConversations (
  ConversationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId UNIQUEIDENTIFIER NOT NULL,
  VoiceTranscript NVARCHAR(MAX),
  ParsedTransaction NVARCHAR(MAX), -- JSON: amount, category, description
  Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Confirmed, Rejected
  CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
```

**1.2 Indexes for Query Performance**

```sql
-- Frequently queried: transactions by user, date range, category
CREATE INDEX IX_Transactions_UserId_TransactionDate 
  ON Transactions(UserId, TransactionDate DESC) 
  INCLUDE (Amount, CategoryId, AccountId);

CREATE INDEX IX_Transactions_CategoryId 
  ON Transactions(CategoryId, TransactionDate DESC);

-- Account balance lookups
CREATE INDEX IX_Accounts_UserId 
  ON Accounts(UserId);

-- Budget tracking
CREATE INDEX IX_Budgets_UserId_CategoryId 
  ON Budgets(UserId, CategoryId);

-- Financial goal progress
CREATE INDEX IX_FinancialGoals_UserId_TargetDate 
  ON FinancialGoals(UserId, TargetDate DESC);

-- Category lookups
CREATE INDEX IX_Categories_UserId 
  ON Categories(UserId);
```

**1.3 What You'll Create:**
```
database/
  ├── schema/
  │   ├── 001_initial_schema.sql          # All table definitions
  │   ├── 002_indexes.sql                 # Index creation
  │   ├── 003_stored_procedures.sql       # Reporting queries
  │   └── 004_views.sql                   # Materialized views for reporting
  │
  ├── migrations/
  │   └── EF Core migrations (generated by .NET team)
  │
  └── seeds/
      └── 001_initial_seed_data.sql       # Sample users, categories for dev
```

**Tools:** SQL Server Management Studio (SSMS), Azure Data Studio, EF Core migrations

---

### 2. **Entity Framework Core Migrations**

#### What You Must Do:

**2.1 Manage Migration Workflow**
- .NET developers create migrations: `dotnet ef migrations add MigrationName`
- You review migration SQL (ensure no performance issues)
- You test migrations on staging database
- You execute migrations on production (via CD pipeline)

**2.2 Migration Strategy**
- Always reversible (support `dotnet ef migrations remove` / `dotnet ef database update 0`)
- Include both `Up()` and `Down()` methods
- Large tables: use `ONLINE = ON` for index operations (SQL Server)
- Add `NVARCHAR(MAX)` columns with `NOT NULL DEFAULT ''` to avoid sparse column issues

**2.3 What You'll Create:**
```
database/
  └── ef-core-migrations/
      ├── migration-review-checklist.md  # Questions to ask .NET team
      └── deployment-validation.sql      # Pre/post-migration checks

docs/
  └── MIGRATION_PROCESS.md               # How migrations are deployed
```

**Tools:** EF Core CLI, SQL Server Management Studio

---

### 3. **Backup & Recovery**

#### What You Must Do:

**3.1 Automated Backup Pipeline (in Go!)**

Create a Go tool to orchestrate SQL Server backups:

```go
// backup-database.go
package main

import (
  "database/sql"
  "fmt"
  "log"
  "os"
  "time"

  _ "github.com/denisenkom/go-mssqldb"
  "github.com/Azure/azure-sdk-for-go/sdk/storage/azblob"
)

func main() {
  // 1. Connect to SQL Server
  // 2. Perform full/incremental backup to local disk
  // 3. Upload to Azure Blob Storage
  // 4. Verify backup integrity
  // 5. Delete old backups (retention policy)
  // 6. Log results to monitoring system
}
```

**3.2 Backup Types**
- **Full Backup:** Daily at 2 AM UTC (entire database snapshot)
- **Incremental Backup:** Every 6 hours (only changed pages)
- **Transaction Log Backup:** Every 15 minutes (point-in-time recovery)

**3.3 Retention Policy**
- Daily full backups: 7 days
- Weekly full backups: 4 weeks
- Monthly full backups: 1 year
- Compliance hold: As per data retention policy (check with legal)

**3.4 Restore Testing**
- Automated: Monthly full restore to staging database
- Manual: On-demand point-in-time recovery
- Verify: Data integrity checks post-restore

**3.5 What You'll Create:**
```
scripts/
  ├── backup-database.go              # Full backup orchestration
  ├── backup-config.yaml              # Backup schedule, retention
  ├── restore-database.go             # Restore procedure
  ├── verify-backup-integrity.go      # Test restore monthly
  └── go.mod / go.sum                 # Go module dependencies

.github/workflows/
  ├── daily-backup.yml                # Trigger backup-database.go daily
  └── monthly-restore-test.yml        # Trigger restore test

docs/
  └── BACKUP_RECOVERY_GUIDE.md        # Detailed procedures
```

**Tools:** Go, Azure SDK, AWS SDK, SQL Server T-SQL

---

### 4. **Performance Tuning & Optimization**

#### What You Must Do:

**4.1 Query Performance Analysis**
- Monitor slow queries (> 1 second execution time)
- Analyze query plans using SSMS / Azure Data Studio
- Identify missing indexes via DMVs
- Review execution statistics: logical reads, CPU time

**4.2 Index Maintenance**
- Weekly: Rebuild fragmented indexes (> 30% fragmentation)
- Weekly: Reorganize moderately fragmented indexes (10-30%)
- Monthly: Update index statistics for query optimizer

**4.3 Connection Pool Optimization**
- Monitor active connections vs. max pool size
- Adjust pool size based on concurrent API requests
- Alert if pool exhaustion detected

**4.4 Reporting Query Optimization**
- Income vs. Expense: Pre-aggregate daily totals (denormalization)
- Monthly Summary: Use indexed views for fast aggregation
- Cache frequently accessed reports (Redis, if needed)

**4.5 What You'll Create:**
```
scripts/
  ├── index-maintenance.go            # Rebuild/reorganize fragmented indexes
  ├── slow-query-analyzer.go          # Identify slow queries from extended events
  ├── statistics-update.go            # Update index stats
  └── connection-pool-monitor.go      # Track connection usage

docs/
  ├── QUERY_OPTIMIZATION.md           # Index strategy, execution plans
  └── PERFORMANCE_BASELINES.md        # Target metrics (p99 latency, throughput)
```

**Tools:** SQL Server Extended Events, DMVs (Dynamic Management Views), Query Store, Go

---

### 5. **Disaster Recovery & High Availability**

#### What You Must Do:

**5.1 High Availability Architecture**
- Option 1: Azure SQL Database (geo-redundancy, auto-failover)
- Option 2: SQL Server AlwaysOn Availability Groups (on-premises)
- Goal: < 5 minute RTO, < 5 minute RPO

**5.2 Data Replication**
- Primary database: Active (accepts reads/writes)
- Secondary database: Warm standby (ready for failover)
- Async replication to third region (for disaster recovery)

**5.3 Failover Testing**
- Quarterly: Planned failover from primary to secondary
- Document failover procedure: DNS update, connection string changes
- Alert team of failover completion

**5.4 What You'll Create:**
```
docs/
  ├── HIGH_AVAILABILITY_ARCHITECTURE.md
  ├── FAILOVER_RUNBOOK.md              # Step-by-step failover procedure
  └── REGIONAL_RECOVERY_PLAN.md        # Multi-region strategy

scripts/
  ├── test-failover.go                # Automated failover test
  └── verify-replication.go            # Check replication lag
```

**Tools:** Azure SQL Database, SQL Server AlwaysOn, automated failover

---

### 6. **Monitoring & Alerting**

#### What You Must Do:

**6.1 Database Metrics to Monitor**
- **Query Performance:** Average execution time, slow query count
- **Connections:** Active connections, pool utilization
- **Disk I/O:** Read/write throughput, latency
- **Indexing:** Fragmentation %, unused index count
- **Backup:** Last successful backup time, backup duration
- **Replication:** Replication lag (for secondary replicas)
- **Locks:** Lock wait time, deadlock count

**6.2 Alerting Thresholds**
- P99 query latency > 2000ms → Page on-call
- Connection pool usage > 80% → Warning
- Backup failure → Critical alert
- Replication lag > 5 minutes → Warning
- Disk usage > 80% → Warning
- Deadlock detected → Investigate

**6.3 What You'll Create:**
```
scripts/
  ├── db-monitoring.go                # Collect metrics via DMVs
  ├── metrics-exporter.go             # Export to Prometheus / Azure Monitor
  └── alert-rules.yaml                # Alert threshold configuration

docs/
  └── DATABASE_MONITORING.md          # Dashboard URLs, alert definitions
```

**Tools:** SQL Server DMVs, Azure Monitor, Prometheus, Grafana

---

### 7. **Data Governance & Compliance**

#### What You Must Do:

**7.1 Data Retention Policy**
- User data: Retained while account active + 30 days after deletion
- Audit logs: Retained for 1 year
- Backups: Retained per backup strategy above

**7.2 GDPR Compliance (if applicable)**
- Right to deletion: Script to anonymize user data
- Data portability: Export user data as JSON/CSV
- Audit trail: Log all access to sensitive data

**7.3 Encryption**
- Data at rest: SQL Server Transparent Data Encryption (TDE)
- Data in transit: HTTPS / TLS 1.2+
- Sensitive fields: Password hashes (bcrypt), financial data (encrypted column)

**7.4 Audit Logging**
- Track: User account creation/deletion, permission changes
- Log: All transactions (immutable audit table)
- Retention: 1 year (compliant with financial regulations)

**7.5 What You'll Create:**
```
database/
  └── audit/
      ├── create-audit-tables.sql      # Immutable transaction log
      └── audit-procedures.sql         # Trigger-based audit capture

docs/
  ├── DATA_RETENTION_POLICY.md
  ├── GDPR_COMPLIANCE.md              # Right to deletion, data export
  └── ENCRYPTION_STRATEGY.md          # TDE, encrypted columns
```

**Tools:** SQL Server Transparent Data Encryption, audit tables, row-level security

---

## 📊 Implementation Roadmap

### Phase 1: Foundation (Weeks 1-2)
**Priority: P0 (Critical)**

- [ ] Set up GitHub Actions CI/CD workflow
  - Build on every PR
  - Run tests
  - Publish Docker image
  
- [ ] Create Dockerfile and docker-compose.yml
  - Multi-stage build
  - Local dev environment
  
- [ ] Design and implement database schema
  - Create all tables (Users, Accounts, Transactions, etc.)
  - Define indexes
  - Create EF Core migrations
  
- [ ] Set up secrets management
  - GitHub Secrets for dev
  - Environment variables documentation

**Deliverables:**
- Working CI pipeline
- Runnable local environment (docker-compose up)
- SQL Server schema deployed

---

### Phase 2: Deployment & Monitoring (Weeks 3-4)
**Priority: P1 (High)**

- [ ] Deploy to staging environment
  - App Service / Container App
  - Managed SQL Server
  - Health checks
  
- [ ] Implement health endpoint
  - Database connectivity check
  - API responsiveness
  
- [ ] Set up structured logging
  - JSON logs to ELK / Azure Monitor
  - Request correlation IDs
  
- [ ] Create monitoring dashboards
  - Request rate, latency, errors
  - Database metrics
  - Infrastructure metrics

**Deliverables:**
- Staging environment fully operational
- Monitoring dashboards live
- Health checks passing

---

### Phase 3: Reliability & Recovery (Weeks 5-6)
**Priority: P1 (High)**

- [ ] Implement automated backups (Go script)
  - Full backup daily
  - Incremental backup every 6 hours
  - Upload to Azure Blob Storage
  
- [ ] Set up disaster recovery
  - Database replication to secondary region
  - Failover testing procedure
  - RTO/RPO documentation
  
- [ ] Implement alerting
  - PagerDuty integration
  - Alert rules for critical issues
  - On-call rotation setup

**Deliverables:**
- Backup pipeline operational
- Disaster recovery plan tested
- Alerts configured and tested

---

### Phase 4: Production Hardening (Weeks 7-8)
**Priority: P2 (Medium)**

- [ ] Infrastructure as Code (Terraform)
  - All cloud resources defined
  - Reusable for all environments
  
- [ ] Security hardening
  - WAF rules
  - Network isolation
  - Secrets rotation schedule
  
- [ ] Performance optimization
  - Index maintenance script
  - Slow query monitoring
  - Query plan analysis

- [ ] Compliance & governance
  - Audit logging
  - Data retention policy
  - GDPR compliance (if needed)

**Deliverables:**
- Production environment ready
- IaC fully implemented
- Security audit passed

---

### Phase 5: AI & Scale (Weeks 9+)
**Priority: P3 (Lower)**

- [ ] Optimize reporting queries
  - Materialized views
  - Pre-aggregated data
  
- [ ] Add caching layer (Redis)
  - Cache frequently accessed endpoints
  - Invalidation strategy
  
- [ ] Auto-scaling policies
  - Scale based on CPU/memory
  - Database connection pool tuning

**Deliverables:**
- Sub-second reporting queries
- 99.9% uptime SLA
- Horizontal scaling validated

---

## 🛠️ Tools & Technologies You'll Use

### DevOps Tools
| Tool | Purpose | Where |
|------|---------|-------|
| **GitHub Actions** | CI/CD automation | `.github/workflows/` |
| **Docker** | Containerization | `Dockerfile`, `docker-compose.yml` |
| **Azure App Service / AKS** | Hosting | Cloud provider |
| **Terraform / Bicep** | Infrastructure as Code | `infrastructure/` |
| **Prometheus + Grafana** | Monitoring | Cloud provider |
| **ELK / Splunk** | Logging | Cloud provider |
| **PagerDuty** | Incident alerting | SaaS |

### Database Tools
| Tool | Purpose | Where |
|------|---------|-------|
| **SQL Server** | Database engine | Azure SQL / on-premises |
| **EF Core** | ORM & migrations | `src/SpendWise.Infrastructure/` |
| **Azure Data Studio** | Query editor | Local dev machine |
| **SQL Server Extended Events** | Query profiling | Database |
| **Azure Backup** | Automated backups | Cloud provider |

### Go Utilities (Your Code!)
| Script | Purpose | File |
|--------|---------|------|
| **backup-database.go** | Orchestrate SQL Server backups | `scripts/` |
| **restore-database.go** | Restore from backup | `scripts/` |
| **verify-backup-integrity.go** | Test restore monthly | `scripts/` |
| **db-monitoring.go** | Collect DB metrics | `scripts/` |
| **index-maintenance.go** | Rebuild fragmented indexes | `scripts/` |
| **slow-query-analyzer.go** | Find performance issues | `scripts/` |
| **security-audit.go** | Automated security checks | `scripts/` |

---

## 📚 Key Decisions You Need to Make

### 1. **Deployment Target**
- [ ] Azure App Service (easiest for .NET, PaaS)
- [ ] Azure Container Apps (containers, simpler than K8s)
- [ ] Azure Kubernetes Service (complex, but scalable)
- [ ] AWS ECS / EC2
- [ ] On-premises servers

### 2. **Database Solution**
- [ ] Azure SQL Database (managed, geo-redundant)
- [ ] AWS RDS for SQL Server
- [ ] Self-hosted SQL Server on VMs

### 3. **Monitoring Platform**
- [ ] Azure Monitor (native if using Azure)
- [ ] Prometheus + Grafana (open-source)
- [ ] Splunk (enterprise)
- [ ] DataDog

### 4. **Backup Destination**
- [ ] Azure Blob Storage (if using Azure)
- [ ] AWS S3 (if using AWS)
- [ ] On-premises NAS / tape drive

---

## 📖 How to Work with Your Team

### With .NET Developers
- You define: Deployment requirements (ports, environment variables, health check endpoints)
- They implement: Health endpoint, structured logging, database models
- You review: Their code for logging/monitoring best practices

### With .NET Team Lead / Architect
- Weekly sync on deployment progress, infrastructure requirements
- Discuss: Scaling needs, performance targets, SLA requirements

### With Your Microsoft Mentor
- Ask for reviews on: CI/CD pipeline, IaC code, disaster recovery plan
- Discuss: Best practices for monitoring, alerting, incident response

---

## 🎯 Success Metrics

By end of Week 8, you should have:

✅ **DevOps Checklist**
- [ ] CI/CD pipeline builds on every PR
- [ ] Deployment to staging on main merge
- [ ] Manual approval for production deploy
- [ ] Docker images published to registry
- [ ] Health checks passing (green lights)
- [ ] Monitoring dashboards live
- [ ] Alerts configured
- [ ] Runbook for common incidents

✅ **Database Checklist**
- [ ] Schema deployed to all environments
- [ ] Indexes optimized for queries
- [ ] Backups running daily
- [ ] Restore testing passing monthly
- [ ] Replication lag < 5 minutes
- [ ] Audit logging enabled
- [ ] Encryption at rest configured
- [ ] GDPR compliance verified

---

## 📞 Questions to Ask Your Team

1. **Deployment:** "Which cloud platform are we using? Azure, AWS, or on-premises?"
2. **Database:** "What's our recovery time objective (RTO)? 1 hour? 15 minutes?"
3. **Scale:** "What's the expected user base? 100 users? 10,000 users? 1M users?"
4. **Compliance:** "Do we need to meet GDPR, SOC2, or PCI-DSS?"
5. **Budget:** "Are there cloud cost constraints? Should we optimize for cost or performance?"
6. **On-call:** "Who's on-call? How are incidents escalated?"
7. **SLA:** "What uptime SLA are we targeting? 99%? 99.9%? 99.99%?"

---

**Owner:** Amir-Golmoradi (DevOps + DB Engineer)  
**Last Updated:** October 2026  
**Next Review:** November 2026
