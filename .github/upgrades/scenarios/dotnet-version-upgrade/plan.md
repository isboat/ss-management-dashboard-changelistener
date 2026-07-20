# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade projects in this solution to net8.0
**Scope**: Single solution (Management.Dashboard.ChangeListener.sln) with one primary project targeting .NET 6; changes include project TFM updates, NuGet package updates, and Azure Functions migration adjustments if applicable.

## Tasks

### 01-update-projects: Update project target frameworks to net8.0

Update each project file's TargetFramework/TargetFrameworks to net8.0. For Azure Functions projects, follow the Functions V2 model guidance surfaced in the assessment.

**Done when**: All project files have net8.0 as the target framework and the solution loads in the IDE without project-file errors.

---

### 02-update-packages: Update NuGet packages to versions compatible with net8.0

Audit packages identified in assessment.md, update packages that have compatible releases for .NET 8, and remove packages now provided by framework references. Prefer stable releases. Record any packages with no compatible version as blocking issues in the task's progress details.

**Done when**: All updated projects restore and build; no package-compatibility blockers remain unresolved.

---

### 03-azure-functions-migration: Migrate Azure Functions specifics

If the project is an Azure Functions project, apply required changes: adopt Functions V2 model where indicated, enable Application Insights integration per assessment recommendation, and update function bindings or SDK packages as needed.

**Done when**: Functions compile under net8.0 and local function host (where applicable) starts without critical errors.

---

### 04-build-fix-tests: Build solution and fix compile/test issues

After TFMs and package updates, perform a full build. Address API breaking changes, update code where framework or package APIs changed, and run unit/integration tests. Fix any warnings introduced by the upgrade.

**Done when**: Solution builds without errors and tests pass locally (if tests exist).

---

### 05-finalize: Validation, cleanup, and commits

Run final verification: ensure no warnings in modified projects, run static analyzers if configured, update README or docs if TFMs are surfaced, and commit all changes according to the commit strategy.

**Done when**: All changes are committed on the working branch and tasks.md shows all tasks completed.
