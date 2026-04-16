# BuildStatus Database File Map (Excluding .dbschema)

Generated:
2026-04-16

Scope:
- Repository scanned: `q:\dd\BuildStatus`
- Excluded completely: all `*.dbschema` files
- Goal: list database names used in BuildStatus, plus the SQL files and C# files where they appear

## Database Inventory

The following database names were found in BuildStatus after excluding `*.dbschema` files:

1. `LabStatus`
2. `BFD`
3. `CDBurn2`
4. `devdiv_General`
5. `Addax_whby`
6. `LaunchPad`
7. `FeedStore`

---

## 1. LabStatus

### Connection / config files

- `BuildStatus/src/BuildStatus/Web.Config`
- `BuildStatus/src/BuildStatusRestAPI/Web.config`
- `BuildStatus/src/BuildStatus.Tests/App.config`
- `BuildStatus/src/LabStatus.DataBase.Test/App.config`
- `BuildStatus/src/BuildStatus.Components/app.config`

### SQL files

Notes:
- `BuildStatus/src/LabStatus.Database/**` is the owned SQL asset tree for the `LabStatus` database.
- Many SQL files inside that project reference `LabStatus..*` explicitly because they are authored as self-referencing stored procedures/functions.

Representative files:
- `BuildStatus/src/LabStatus.Database/LabStatus.Database.dbproj`
- `BuildStatus/src/LabStatus.Database/LabStatus.Database.sqlproj`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/Integration/Programmability/Stored Procedures/uspLogIntegrationHistory.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_GetCurrentSessionProcess.function.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetOrg.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetMachineAccessRights.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/xls_BranchData.proc.sql`

### C# files

Notes:
- `BuildStatusDB.cs` is the central runtime wrapper over the `LabStatus` database.
- Most application-side usage fans out through `BuildStatusDBFactory.Create()`.

Core wrapper files:
- `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs`
- `BuildStatus/src/BuildStatus.Components/IBuildStatusDB.cs`
- `BuildStatus/src/BuildStatus.Components/BuildStatusDBFactory.cs`

Representative callers:
- `BuildStatus/src/BuildStatus.Components/Build.cs`
- `BuildStatus/src/BuildStatus.Components/Issue.cs`
- `BuildStatus/src/BuildStatus.Components/Fix.cs`
- `BuildStatus/src/BuildStatus.Components/Lab.cs`
- `BuildStatus/src/BuildStatus.Components/Org.cs`
- `BuildStatus/src/BuildStatus.Components/SessionGroup.cs`
- `BuildStatus/src/BuildStatus.Components/ProcessGroup.cs`
- `BuildStatus/src/BuildStatus.Components/Process.cs`
- `BuildStatus/src/BuildStatus.Components/PreservationRequest.cs`
- `BuildStatus/src/BuildStatus.Components/ProcessLog.cs`
- `BuildStatus/src/BuildStatus.Components/BuildLog.cs`
- `BuildStatus/src/BuildStatus.Components/IssueLog.cs`
- `BuildStatus/src/BuildStatus.Components/Utils.cs`
- `BuildStatus/src/BuildStatus.Components/Triage.cs`
- `BuildStatus/src/BuildStatus.Components/TicketSystem.cs`
- `BuildStatus/src/BuildStatus.Components/Source.cs`
- `BuildStatus/src/BuildStatus.Components/Severity.cs`
- `BuildStatus/src/BuildStatus.Components/MachineAccessRight.cs`
- `BuildStatus/src/BuildStatus.Components/LocaleSet.cs`
- `BuildStatus/src/BuildStatus.Components/Layout.cs`
- `BuildStatus/src/BuildStatus.Components/Flavor.cs`
- `BuildStatus/src/BuildStatus.Components/FixType.cs`
- `BuildStatus/src/BuildStatus.Components/BuildFixLog.cs`
- `BuildStatus/src/BuildStatus/Services/BuildService.cs`
- `BuildStatus/src/BuildStatus/Services/AjaxService.cs`
- `BuildStatus/src/BuildStatus/ExternalServices/BuildStatusService.cs`
- `BuildStatus/src/BuildStatusRestAPI/Controllers/BuildStatusController.cs`
- `BuildStatus/src/LabStatus.DataBase.Test/Tests/SessionGroupDependencyTest.cs`

---

## 2. BFD

### Connection / config files

- `BuildStatus/src/BuildStatus/Web.Config`
- `BuildStatus/src/BuildStatus.Tests/App.config`

### SQL files

- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetBFDGroups.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetLabDetails.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetLabBFDs.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_UpdateLabWithGit.sproc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_UpdateLabWithGit.proc.sql`

### C# files

- `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs`
- `BuildStatus/src/BuildStatus.Components/IBuildStatusDB.cs`

Note:
- `BuildStatusDB.GetBuildBFD(int sessionId)` is the clearest direct runtime access path to database `BFD`.

---

## 3. CDBurn2

### Connection / config files

- `BuildStatus/src/BuildStatus/Web.Config`
- `BuildStatus/src/BuildStatus.Tests/App.config`

### SQL files

- No local SQL file was found that references database `CDBurn2` by name.
- In BuildStatus, `CDBurn2` appears as an application integration through config and external components rather than local SQL assets.

### C# / page files

- `BuildStatus/src/BuildStatus/BurnLayouts.aspx.cs`
- `BuildStatus/src/BuildStatus/BurnLayouts.aspx`
- `BuildStatus/src/BuildStatus/BurnLayouts.aspx.designer.cs`
- `BuildStatus/src/BuildStatus/Controls/BuildRow.ascx.cs`
- `BuildStatus/src/BuildStatus/Controls/BuildRow.ascx`
- `BuildStatus/src/BuildStatus/Controls/BuildRow.ascx.designer.cs`
- `BuildStatus/src/BuildStatus/Common.cs`
- `BuildStatus/src/BuildStatus/BuildStatus.csproj`
- `BuildStatus/src/BuildStatus/packages.config`

Note:
- BuildStatus uses `CDBurn.Components` and `Microsoft.CDBurn.Components` rather than a local SQL wrapper class for `CDBurn2`.

---

## 4. devdiv_General

### Connection / config files

- `BuildStatus/src/BuildStatus/Web.Config`

### SQL files

- No local SQL file was found that references database `devdiv_General` by name.

### C# / page files

- `BuildStatus/src/BuildStatus/Default.aspx`
- `BuildStatus/src/BuildStatus/Default.aspx.designer.cs`
- `BuildStatus/src/BuildStatus/BuildStatus.csproj`
- `BuildStatus/src/BuildStatus/packages.config`

Note:
- `devdiv_General` is surfaced through `FlashNewsDBconnectionString` and consumed indirectly by the external `FlashNewsControl2` control.
- No local `BuildStatusDB`-style wrapper was found for this database.

---

## 5. Addax_whby

### Connection / config files

- `BuildStatus/src/DDRelQA.DataLayer/app.config`
- `BuildStatus/src/DDRelQA.DataLayer/Properties/Settings.settings`
- `BuildStatus/src/DDRelQA.DataLayer/Properties/Settings.Designer.cs`

### SQL files

Notes:
- No local `.sql` database project for `Addax_whby` was found in BuildStatus.
- It appears through LINQ-to-SQL `.dbml` models rather than local stored-procedure SQL files.

Representative files:
- `BuildStatus/src/DDRelQA.DataLayer/AllSuitesView.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/ArchLookUp.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/BuildNumbers.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/BuildNumByDateRange.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/DailyExpectedRunsView.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/FeatureLookUp.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/FlavorLookUp.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/GetRunAdminResultsView.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/LabFailure.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/LabResultsSummary.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/LabResultsSummarySxS.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/LabRevisionComment.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/LabSummaryTEST.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/LabSxsScenario.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/OsLookUp.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunTable.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunTitle.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunSteps.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunProgress.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunConfigInfoByRunConfigID.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunConfigIDsByVBLBuildTestType.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunAdminTitle.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/RunTotalsByRunConfigID.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SkuLookUp.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SuiteResultsAll.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SuiteResultsAndBugs.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SxSInstallOrder.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SxSRunAdminResults.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SxSRunConfigInfo.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SxSRunTotals.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SxSScenarioNamesByVBLBuild.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/SxSSuiteRunResults.dbml`
- `BuildStatus/src/DDRelQA.DataLayer/TestTypes.dbml`

### C# files

- `BuildStatus/src/DDRelQA.DataLayer/AddaxManager.cs`
- `BuildStatus/src/DDRelQA.DataLayer/PartialImplDataLayer.cs`
- `BuildStatus/src/DDRelQA.DataLayer/Properties/Settings.Designer.cs`
- All generated `*.designer.cs` files under `BuildStatus/src/DDRelQA.DataLayer` that contain `DatabaseAttribute(Name="Addax_whby")`

---

## 6. LaunchPad

### Connection / config files

- No direct BuildStatus runtime connection string for database `LaunchPad` was found.
- `LaunchPad` appears primarily as a cross-database SQL dependency inside `LabStatus.Database`.

### SQL files

- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_GetBVTMilestone.function.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_GetBVTDecision_BAK.function.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_GetBVTDecision.function.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Functions/up_GetBVTDecision.function.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetUserMachineList.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetLopezInfo.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/xls_BranchData.proc.sql`

### C# files

- `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs`
- `BuildStatus/src/BuildStatus.Components/IBuildStatusDB.cs`
- `BuildStatus/src/BuildStatus.Components/Build.cs`
- `BuildStatus/src/BuildStatus.Components/Issue.cs`

Note:
- The main C# runtime path is `BuildStatusDB.GetRequestDataforBuilds(int[] sessionIds)`, which enriches BuildStatus data with LaunchPad request context.

---

## 7. FeedStore

### Connection / config files

- No direct runtime connection string for `FeedStore` was found in BuildStatus config.
- `FeedStore` appears as a cross-database SQL dependency inside `LabStatus.Database`.

### SQL files

- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_AddNewCostCenter.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetCostCentersWithIssues.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetCostCenter.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_RptNumberOfIssuesPerTeamRolledUpByDate2.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetStatsData.proc.sql`
- `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetStatsData_OLD.proc.sql`

### C# files

- `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs`
- `BuildStatus/src/BuildStatus.Components/IBuildStatusDB.cs`
- `BuildStatus/src/BuildStatus.Components/Issue.cs`

Note:
- The clearest runtime path is:
  - `Issue.cs` -> `BuildStatusDB.CostCenterExists()` -> `sp_GetCostCenter`
  - `Issue.cs` -> `BuildStatusDB.AddNewCostCenter()` -> `sp_AddNewCostCenter`

---

## Summary Notes

- `FeedStore` is included here because, unlike the LaunchPad scan, it appears in real SQL files under `BuildStatus/src/LabStatus.Database/**`, not only in `.dbschema` artifacts.
- `LabStatus` is the primary owned runtime database in BuildStatus.
- `BFD` is a secondary direct runtime database, accessed through `BuildStatusDB.GetBuildBFD()`.
- `CDBurn2` and `devdiv_General` are application integrations exposed through config and external controls/components rather than a local SQL wrapper class.
- `Addax_whby` is a separate LINQ-to-SQL data surface under `DDRelQA.DataLayer`.
- `LaunchPad` and `FeedStore` are cross-database dependencies surfaced through SQL procedures/functions and a smaller set of C# call paths.
