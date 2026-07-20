# 02-update-packages: Update NuGet packages to versions compatible with net8.0

Audit packages identified in assessment.md, update packages that have compatible releases for .NET 8, and remove packages now provided by framework references. Prefer stable releases. Record any packages with no compatible version as blocking issues in the task's progress details.

**Done when**: All updated projects restore and build; no package-compatibility blockers remain unresolved.
