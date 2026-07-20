 # 01-update-projects: Update project target frameworks to net8.0

 Update each project file's TargetFramework/TargetFrameworks to net8.0. For Azure Functions projects, follow the Functions V2 model guidance surfaced in the assessment.

 Research:
 - Identified project: src/Management.Dashboard.ChangeListener/Management.Dashboard.ChangeListener.csproj
 - Current TargetFramework: net6.0
 - Changes required: update <TargetFramework> to net8.0. For Azure Functions projects, further adjustments may be required in subsequent tasks (update Functions SDK, binding versions, and App Insights).

 Planned actions:
 1. Update TargetFramework in project files to net8.0
 2. Run `dotnet restore` and `dotnet build` for the solution to detect issues
 3. If build errors, record them in progress-details.md and address in task 04-build-fix-tests

 **Do not edit other project properties in this task.**

 **Done when**: All project files have net8.0 as the target framework and the solution loads in the IDE without project-file errors.
