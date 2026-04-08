# Mandatory Rule-Loading Workflow

Follow this flow for every backend-related request:

1. Read this `copilot-instructions.md` file first.
2. Identify the task category from the Backend Rules Index (architecture, api, application, infrastructure, cross-cutting).
3. Open and read the specific referenced rule file(s) before proposing or writing code.
4. If multiple rule files apply, follow the most restrictive requirement.
5. If any rule conflicts with generated code, fix the code to match the rule.
6. Do not skip rule-file lookup, even for small changes.

Rule source location to read from:

- `c:\Users\rishi.alluri\OneDrive - Paltech Consulting Private Limited\Documents\Projects\ai-rules\backend\`

Compliance output requirement for backend tasks:

- Briefly state which rule file(s) were applied.
- If a needed rule file is empty, state that and continue with the closest applicable rule.

# Backend Rules Index

This index provides a short purpose summary for every file in `backend/`.

## 01-architecture

- `core-type-rules.md`: Defines when to use records vs classes across commands, DTOs, entities, and components.
- `cqrs-overview.md`: Establishes CQRS structure, forbidden patterns, pipeline expectations, and implementation order.
- `dentova-branch-implmentation.md`: Dentova-specific tenant-branch architecture rules, entity classification, authorization order, validation requirements, and database design constraints.
- `dentova-backend-rules-short.md`: Condensed backend checklist for type rules, validation, IDs, transactions, and dependency usage.
- `dependency-injection-rules.md`: Defines tenant-aware DI rules, scoped lifetimes, and required repository/provider registrations.
- `final-mandatory-implementation-summary.md`: Final must-follow implementation checklist for core backend standards.
- `tenant-architecture.md`: Defines Civet multi-tenancy model, tenant resolution flow, repository enforcement points, risks, and implementation guardrails.
- `three-layer-architecture.md` (empty): Should cover API/Application/Infrastructure layer boundaries and allowed dependencies.

## 02-api

- `api-contract-rules.md`: Defines API contracts, request/response conventions, and thin-controller expectations.
- `authentication-jwt-flow.md`: Defines first-party JWT auth flow using identifier + password, refresh sessions, and tenant scoping.
- `authorization-rules.md`: Defines tenant-aware authorization, JWT checks, and role revalidation against current DB role.

## 03-application

- `application-service-vs-domain-service-rule.md`: Clarifies when to use application service vs domain service.
- `command-rules.md`: Defines command shape, handler expectations, and thin orchestration boundaries.
- `dto-rules.md`: Defines DTO type guidance and placement references.
- `handler-rules.md`: Defines handler pipeline behavior and execution responsibilities.
- `query-rules.md`: Defines query patterns for read-only flows and projection rules.
- `repository-injection-rules.md`: Defines explicit repository injection by use-case need.
- `service-usage-rules.md`: Defines when services should and should not be introduced.
- `transaction-rules.md`: Defines transaction behavior and save pattern expectations.
- `user-management-rules.md`: Defines tenant/user/invite/session management flows and rules.
- `validation-rules.md`: Defines split between input validation and business-state validation.

## 04-infrastructure

- `dbcontext-rules.md`: Defines soft-delete global filters, repository-driven tenant filtering, and direct DbContext caveats.
- `ef-core-rules.md` (empty): Should cover EF Core usage standards, performance practices, and mapping conventions.
- `migration-rules.md` (empty): Should cover migration naming, generation workflow, review, and deployment safety.
- `password-hashing-rules.md`: Defines secure hashing standards for passwords/tokens and prohibited practices.
- `repository-rules.md`: Defines tenant-safe repository boundaries, required query behavior, and mutation safety caveats.
- `savechanges-rules.md`: Defines SaveChanges usage patterns plus interceptor guardrails for soft-delete, tracking, and tenant auto-fill.
- `unit-of-work-rules.md`: Defines scoped UnitOfWork with intentional mixed tenant/global repositories and required tenant-safe usage.

## 05-cross-cutting

- `exception-handling-rules.md`: Defines centralized exception handling and standardized error responses.
- `logging-rules.md`: Defines structured logging expectations, levels, and sensitive-data restrictions.

## Empty file backlog

The following files are currently empty and should be filled next:

- `01-architecture/three-layer-architecture.md`
- `04-infrastructure/ef-core-rules.md`
- `04-infrastructure/migration-rules.md`

