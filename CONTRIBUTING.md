# Contributing to SpendWise

This document defines the Git workflow and contribution standards for SpendWise. Quality and consistency are enforced through CI automation, not manual review gates.

---

## Rule 1 — Branching: Trunk-Based

**There is only one permanent branch: `main`**

Forbidden:
- Creating `develop`, `staging`, `release/*`
- Git Flow or similar multi-branch models

Allowed:
- All temporary branches are cut directly from `main` and merged directly back into `main`

### Branch Naming Convention

Choose the pattern that matches your work:

| Branch Type | Pattern | Example |
|---|---|---|
| **Personal feature work** | `<initials>-<short-hyphenated-description>` | `amg-add-caching`, `ja-fix-transaction-hash`, `sr-optimize-queries` |
| **Automated file sync** | `sync-files` | `sync-files` |
| **Automated bot/CI update** | `auto-<tool>-update/<branch>` | `auto-dependabot-update/main` |
| **Dependabot PR** | Auto-generated | `dependabot/npm/lodash-4.17.21` |

Replace `<initials>` with your initials (e.g., `amg`, `ja`, `sr`).

### Creating Your Branch

```bash
git checkout main
git pull origin main
git checkout -b ja-add-backup-script
```

---

## Rule 2 — Commits

**Required:** Commit message must be an **imperative sentence** — direct, short, no prefixes.

| ✅ Correct | ❌ Wrong |
|---|---|
| `Add transaction integrity verification` | `feat(transactions): added integrity checks` |
| `Fix SQL Server backup timeout` | `chore: backup improvements` |
| `Optimize account balance query` | `refactor(db): optimize query` |
| `Update GitHub Actions workflow` | `ci: update workflow` |

### Commit Best Practices

```bash
git commit -m "Add transaction integrity verification"
git commit -m "Fix SQL Server backup timeout issue"
git commit -m "Optimize account balance query with index"
```

### Additional Rules

- **Do not manually add the PR number** — GitHub automatically appends `(#N)` when squash-merging.
- **Signing commits is optional** — commits pushed locally do not require GPG signatures.
- **Direct pushes to `main` are allowed** for small, urgent fixes (e.g., fixing a typo in README). Use good judgment; pair with colleagues for substantial changes.
- **Automated cross-repo sync commits** use this exact format:
  ```
  Sync files from Productive-Build-Cycle/shared@<commit-sha>
  ```

---

## Rule 3 — Merge Strategy: Squash Only

`main` must maintain a **linear, readable history**.

### GitHub Repository Settings

| Setting | Value |
|---|---|
| Allow squash merging | ✅ Enabled |
| Allow merge commits | ❌ **Disabled** |
| Allow rebase merging | ❌ **Disabled** |

When your PR is merged, GitHub will squash all commits into a single commit with the format:

```
<PR Title> (#42)
```

Example:
```
Add transaction integrity verification (#42)
```

### Local Equivalent

If you're squash-merging locally:

```bash
git checkout main
git pull origin main
git merge --squash ja-add-backup-script
git commit -m "Add transaction integrity verification (#42)"
git push origin main
```

---

## Rule 4 — Updating Your Branch: Merge, Never Rebase

When `main` updates while you're working on your branch:

```bash
git fetch origin
git merge origin/main
git push origin ja-add-backup-script
```

**Forbidden:**
```bash
git rebase origin/main  # ❌ Do not do this
git push -f             # ❌ Never force-push to shared branches
```

### Why?

- Rebasing rewrites history and creates merge conflicts for teammates.
- Since we squash-merge, your branch's history is temporary anyway.
- Seeing `Merge branch 'main' into ja-add-backup-script` commits is **normal and expected** — don't worry about it.

---

## Rule 5 — Pull Requests & Code Review

### What Requires a PR?

**Always use a PR for:**
- New features
- Bug fixes
- Infrastructure changes (Terraform, workflows, Dockerfile)
- Database schema/migration changes
- Documentation updates

**Can push directly to `main` (no PR):**
- README typos or minor clarifications
- Comment updates
- Non-breaking documentation fixes

### PR Process

1. **Push your branch to GitHub:**
   ```bash
   git push origin ja-add-backup-script
   ```

2. **Open a PR** with a clear title:
   - ✅ `Add database backup verification script`
   - ✅ `Fix Docker image build timeout`
   - ❌ `WIP: stuff` or `Update`

3. **Wait for CI to pass:**
   - All GitHub Actions workflows must pass (lint, tests, security scans)
   - No manual approvals required — CI is your approval gate
   - If CI fails, fix the issues and push again

4. **Merge when ready:**
   - Click "Squash and merge"
   - Confirm the commit message is clear
   - Delete the branch after merge

### Code Review Guidelines

- **Code reviews are collaborative, not gatekeeping** — ask questions, suggest improvements, share knowledge
- **Review for:** correctness, clarity, maintainability, security
- **Do not require:** perfect style, every optimization, or exhaustive test coverage (CI validates those)
- **For DevOps/DB changes:** review for: security hardening, backup strategy, disaster recovery impact

---

## Rule 6 — Quality Control via CI, Not Process

**Forbidden:**
- `CODEOWNERS` files
- `PULL_REQUEST_TEMPLATE.md`
- Mandatory commit signing
- Mandatory DCO (Developer Certificate of Origin)

**Required:** All quality gates are enforced by GitHub Actions workflows.

### Minimum Required Workflows

Every repository must have these workflows in `.github/workflows/`:

| Workflow | Trigger | Purpose |
|---|---|---|
| `ci-lint.yml` | `pull_request`, `push to main` | Run linters, format checks, static analysis |
| `ci-tests.yml` | `pull_request`, `push to main` | Run unit and integration tests |
| `ci-security.yml` | `pull_request`, `push to main` | Security scanning (dependencies, SAST) |

**Example structure:**
```yaml
name: CI - Lint & Test

on:
  pull_request:
    types: [opened, reopened, synchronize]
  push:
    branches: [main]

jobs:
  lint:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0'
      - run: dotnet build
      - run: dotnet test

  security:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: github/super-linter@v5
```

If a workflow needs to auto-fix and commit (e.g., code formatting):

```yaml
permissions:
  contents: write
  pull-requests: write
```

---

## Rule 7 — Tagging and Releases

### For SpendWise (Main Application)

SpendWise has not reached a stable `v1.0.0` release yet.

**Current Rule:** **Do not create any tags or GitHub releases.**

Version will be managed through:
- Docker image tags: `main-<commit-sha>`, `staging`, `latest`
- Deployment tracking in CD pipelines

### For Dependent Libraries (Future)

When SpendWise becomes a library others depend on:

| Stage | Version Example | When to Tag |
|---|---|---|
| Early development | `v0.1.0-alpha.1` | Significant milestones |
| Pre-release | `v0.1.0-beta.1`, `v0.1.0-beta.2` | Ready for testing |
| Release candidate | `v1.0.0-rc.1` | Feature-complete, testing |
| Stable release | `v1.0.0` | Ready for production |

Tag format: `v<major>.<minor>.<patch>-<prerelease>`

```bash
git tag v1.0.0-beta.1
git push origin v1.0.0-beta.1
```

**Forbidden:** Date-based tags (`2026-10-11`), auto-generated changelogs, manual version files.

---

## Rule 8 — Multi-Repo Coordination (Future)

If/when SpendWise expands to multiple repositories (e.g., shared CI/CD, shared infrastructure code):

1. Create a `shared` repository to hold shared files (workflows, Terraform modules, scripts)
2. Build a bot that automatically syncs changes from `shared` to other repos as PRs
3. Sync PRs use this exact title and commit format:
   ```
   Sync files from Productive-Build-Cycle/shared@<commit-sha>
   ```
4. Sync PRs go through normal squash-merge — no exceptions or fast-tracks

---

## Practical Examples

### Example 1: Adding a Backup Script

```bash
# Create branch
git checkout main && git pull
git checkout -b amg-add-database-backup-script

# Make changes
# ... write backup-database.go, update .github/workflows/backup.yml ...

# Commit (imperative message, no "feat:" prefix)
git add scripts/backup-database.go
git commit -m "Add automated database backup script"

git add .github/workflows/daily-backup.yml
git commit -m "Add workflow to trigger daily backups"

# Push and create PR
git push origin amg-add-database-backup-script
# (Open PR on GitHub)

# Wait for CI to pass, then squash-merge
# Result: single commit on main: "Add automated database backup script (#47)"
```

### Example 2: Fixing a Bug

```bash
# Create branch
git checkout main && git pull
git checkout -b sr-fix-transaction-hash-verification

# Fix the bug
# ... edit Transaction service ...

git commit -m "Fix transaction hash verification for large amounts"
git push origin sr-fix-transaction-hash-verification
# (Open PR, wait for CI, merge)
```

### Example 3: Updating When main Changes

```bash
# While you're working on your branch, main gets updated
git fetch origin
git merge origin/main  # ✅ Correct
# Do not: git rebase origin/main ❌

git push origin ja-optimize-account-query
# (if there were conflicts, resolve and push)
```

---

## DevOps & Database Responsibilities

### For DevOps Engineers (You)

- **CI/CD Pipeline:** Ensure workflows are fast, reliable, and secure
  - No direct main pushes for pipeline changes (use PR + merge)
  - Test workflow changes on a branch before merging
  
- **Infrastructure Changes:** Always use PR for:
  - Terraform / IaC changes
  - Dockerfile updates
  - Kubernetes manifests
  - Deployment scripts
  
- **Secrets & Security:** 
  - Never commit secrets, even accidentally
  - Use GitHub Secrets for CI/CD
  - Document secret rotation procedures

- **Backup & Recovery Scripts:**
  - Write in Go (as per project standards)
  - Test locally before pushing to `main`
  - Include in PR for peer review

### For Database Engineers (You)

- **Schema Changes:** Always use PR for:
  - SQL migration scripts
  - EF Core migration files
  - Index definitions
  - Stored procedures
  
- **Backup/Restore Procedures:**
  - Document in PRs
  - Test restore procedures on staging before main merge
  
- **Performance Tuning:**
  - Include query execution plans in PR description
  - Show before/after metrics
  
- **Data Governance:**
  - Audit logging changes require PR + review
  - GDPR/compliance scripts reviewed before deployment

---

## Commit Message Examples by Role

### DevOps Examples
- `Add GitHub Actions workflow for CI`
- `Configure Docker image scanning with Trivy`
- `Update Terraform configuration for staging environment`
- `Implement health check monitoring dashboard`
- `Add WAF rules for SQL injection prevention`

### Database Examples
- `Create Users table with auth fields`
- `Add index for transaction date queries`
- `Implement automated backup to Azure Blob Storage`
- `Create audit logging for financial transactions`
- `Optimize monthly report query with materialized view`

### Full-Stack Examples
- `Add API health endpoint with database connectivity check`
- `Implement structured JSON logging to ELK stack`
- `Create migration for encrypted password storage`

---

## Questions?

If anything is unclear:
1. **Check this file first** — most answers are here
2. **Ask your mentor** — Microsoft engineer is available to clarify workflow and best practices
3. **Check recent PRs** — look at how others followed these rules
4. **Open an issue** — if you think the workflow needs updating

---

## Checklist for New Contributors

- [ ] You have Git installed and `main` branch cloned locally
- [ ] You created your branch: `<initials>-<description>`
- [ ] You made changes and committed with imperative messages
- [ ] You pushed to GitHub and opened a PR
- [ ] CI workflows passed (lint, tests, security scans)
- [ ] You used "Squash and merge" when merging
- [ ] You deleted your branch after merge
- [ ] You're ready to start the next task!

---

**Last Updated:** October 2026  
**Owner:** Productive Build Cycle  
**Status:** Active
