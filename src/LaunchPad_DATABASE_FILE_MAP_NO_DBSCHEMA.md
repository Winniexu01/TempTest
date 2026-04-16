# LaunchPad Database File Map (Excluding .dbschema)

Generated:
2026-04-16

Scope:
- Repository scanned: `q:\dd\LaunchPad`
- Excluded completely: all `*.dbschema` files
- Goal: list database names used in LaunchPad, plus the SQL files and C# files where they appear

## Database Inventory

The following database names were found in LaunchPad after excluding `*.dbschema` files:

1. `LaunchPad`
2. `LabStatus`
3. `DAD`
4. `SpaceMan`
5. `SanMan`
6. `SanManJob`
7. `DropManagement`
8. `Techease`
9. `Utils`
10. `LabSchedule`

---

## 1. LaunchPad

### Connection / config files

- `LaunchPad/src/LaunchPad/Web.config`
- `LaunchPad/src/LaunchPad.Service/App.config`
- `LaunchPad/src/LaunchPad.Testing/App.config`
- `LaunchPad/src/LaunchPad.Database/Schema Comparisons/SchemaCompare.scmp`

### SQL files

Notes:
- `LaunchPad/src/LaunchPad.Database/Schema Objects/**` is the owned SQL asset tree for the `LaunchPad` database.
- That folder currently contains 452 `.sql` files.
- The files below are key entry files or explicit `LaunchPad..*` cross-db references.

- `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.dbproj`
- `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Tables/tbl_Request.table.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Tables/tbl_RequestBuild.table.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_Request.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_Definition.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_LopezRequestSessions.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_DefinitionCloneFromDDSQL3.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ReportUniqueUsers.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ReportingQueueBaseData.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ReportDuplicateRequests.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ReportingGetServerUsage.proc.sql`
- `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql`
- `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000007/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000006/up_GetDropMachineSelection.proc.sql`

### C# files

Notes:
- `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` is the central runtime wrapper for the `LaunchPad` database.
- Most application-side `LaunchPad` DB access fans out from files that instantiate `new LaunchPadDB()`.

- `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs`
- `LaunchPad/src/LaunchPad.Components/LaunchInfo.cs`
- `LaunchPad/src/LaunchPad.Components/LabDefinition.cs`
- `LaunchPad/src/LaunchPad.Components/Lab.cs`
- `LaunchPad/src/LaunchPad.Components/HardwareClass.cs`
- `LaunchPad/src/LaunchPad.Components/DropMachine.cs`
- `LaunchPad/src/LaunchPad.Components/Toolset.cs`
- `LaunchPad/src/LaunchPad.Components/Template.cs`
- `LaunchPad/src/LaunchPad.Components/DefinitionStep.cs`
- `LaunchPad/src/LaunchPad.Components/Definition.cs`
- `LaunchPad/src/LaunchPad.Components/San.cs`
- `LaunchPad/src/LaunchPad.Components/ResourceUsage.cs`
- `LaunchPad/src/LaunchPad.Components/ResourceRequirement.cs`
- `LaunchPad/src/LaunchPad.Components/Resource.cs`
- `LaunchPad/src/LaunchPad.Components/BuildNumber.cs`
- `LaunchPad/src/LaunchPad.Components/RequestProperty.cs`
- `LaunchPad/src/LaunchPad.Components/RequestInfoBare.cs`
- `LaunchPad/src/LaunchPad.Components/BuildMachine.cs`
- `LaunchPad/src/LaunchPad.Components/Build.cs`
- `LaunchPad/src/LaunchPad.Components/RequestComment.cs`
- `LaunchPad/src/LaunchPad.Components/RequestBuild.cs`
- `LaunchPad/src/LaunchPad.Components/Request.cs`
- `LaunchPad/src/LaunchPad.Components/Reporting.cs`
- `LaunchPad/src/LaunchPad.Components/PreLaunchAssembly.cs`
- `LaunchPad/src/LaunchPad.Components/Pool.cs`
- `LaunchPad/src/LaunchPad.Components/Org.cs`
- `LaunchPad/src/LaunchPad.Components/Log.cs`
- `LaunchPad/src/LaunchPad.Definitions/Partner/Definitions/Partner/Properties/WhidbeyBuildNumber.cs`
- `LaunchPad/src/LaunchPad.Definitions/Partner/Definitions/Partner/Properties/WhidbeyBranch.cs`
- `LaunchPad/src/LaunchPad.Components.Test/LaunchPadDBTest.cs`
- `LaunchPad/src/LaunchPad.Components.Test/PoolTest.cs`

---

## 2. LabStatus

### Connection / config files

- `LaunchPad/src/LaunchPad.Service/App.config`

### SQL files

Representative and direct references:

- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_LopezRequestSessions.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_LabDefinition.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Synonyms/up_UpdateSessionStatus.synonym.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Synonyms/tlkp_TargetArchitecture.synonym.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Synonyms/tlkp_LocaleSet.synonym.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Synonyms/tlkp_Flavor.synonym.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Synonyms/tbl_Session.synonym.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Synonyms/tbl_Org.synonym.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Synonyms/tbl_Lab.synonym.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetNextRevision.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_LabGenerateRevision.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_LabDefinitionUpdate.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetBuildMachineSelection.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetBuildMachineSelectionForLopez.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetAllocatedResourceOverview.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_DefinitionGet.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_BuildGetByDefinitionStep.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_OrgGet.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_AddResourceToPool.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RemoveResourceFromPool.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_AddLabToPool.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RemoveLabFromPool.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestAbandonBuilds.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestBuildAdd.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestBuildRemove.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestBuildSet.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestMerge.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestProcessFailedRequests.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ReportingQueueBaseData.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_RequestGetByTargetDate.function.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_RequestGetByTargetDate2.function.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_RequestGetByTargetDate_DONT_DELETE.function.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetTodaysSchedules.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.up_GetSchedulesReadyToBeProcessed.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedules.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedule.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedulesWithMatchingSQLServer.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedulesWithMatchingDropServerFromPreviousDay.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleBuilds.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetPoolLabs.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabServers.proc.sql`

### C# files

- `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs`
- `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs`

Note:
- `RequestProcessor.cs` contains the clearest direct `LabStatus` runtime access.
- `LaunchPadDB.cs` is relevant because many LaunchPad stored procedures it invokes read LabStatus internally.

---

## 3. DAD

### Connection / config files

There is no direct SQL connection string for database `DAD` in LaunchPad config.
LaunchPad uses both SQL cross-db references and a DAD SOAP endpoint.

SOAP/service endpoint files:
- `LaunchPad/src/LaunchPad.Components/app.config`
- `LaunchPad/src/LaunchPad.Components/Properties/Settings.settings`
- `LaunchPad/src/LaunchPad.Components/Properties/Settings.Designer.cs`
- `LaunchPad/src/LaunchPad.Components/LaunchPad.Components.csproj`
- `LaunchPad/src/LaunchPad.Components/Web References/ddweb/Reference.cs`
- `LaunchPad/src/LaunchPad.Components/Web References/ddweb/DADService.wsdl`
- `LaunchPad/src/LaunchPad.Components/Web References/ddweb/Reference.map`

### SQL files

- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_ResourceDADServers.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_DropServerPools.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_DADServerPoolInfo.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_HardwareClassGet.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_LabDefinitionUpdate.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetBuildMachineSelection.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetBuildMachineSelectionForLopez.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetAllocatedResourceOverview.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_AddResourceToPool.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RemoveResourceFromPool.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ServerAssignmentsGet.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_Tools_GetServersForPatching.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_Tools_IsReadyForPatching.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UtilizationGetOverallSummary.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateBuildMachineState.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateDropMachineState.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Views/dbo.vw_PoolsAndServers.view.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedulesWithMatchingSQLServer.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedulesWithMatchingDropServerFromPreviousDay.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleSQLServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabSQLServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabDropServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleLayoutServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleDropServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabBuildMachines.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleBuilds.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabBBTServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetPoolServers.proc.sql`
- `LaunchPad/src/deploy/sql/000006/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000007/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql`

### C# files

- `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs`
- `LaunchPad/src/LaunchPad.Components/HardwareClass.cs`
- `LaunchPad/src/LaunchPad.Components/Enumerations.cs`
- `LaunchPad/src/LaunchPad/Admin/DAD/DAD.cs`
- `LaunchPad/src/LaunchPad/Admin/DAD/DADCommunicator.cs`
- `LaunchPad/src/LaunchPad/Admin/ServerProperties.ascx.cs`
- `LaunchPad/src/LaunchPad/Admin/RenameServer.aspx.cs`
- `LaunchPad/src/LaunchPad/Admin/RemoveServer.aspx.cs`
- `LaunchPad/src/LaunchPad/Admin/ManageServerPools.aspx.cs`
- `LaunchPad/src/LaunchPad/Admin/DefinitionEdit.aspx.cs`
- `LaunchPad/src/LaunchPad/Admin/Utilization.aspx.cs`
- `LaunchPad/src/LaunchPad/Services/AJAXCalls.cs`
- `LaunchPad/src/LaunchPad.Components/Web References/ddweb/Reference.cs`

---

## 4. SpaceMan

### SQL files

- `LaunchPad/src/LaunchPad.Database/Scripts/Post-Deployment/Script.PostDeployment.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetSanSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000006/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000007/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql`

### C# files

- No direct local C# file was found that opens or names `SpaceMan` directly.
- In practice, LaunchPad reaches `SpaceMan` through SQL stored procedures.

---

## 5. SanMan

### SQL files

- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000006/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000007/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql`

### C# files

SanMan appears on the service/client side rather than as a direct SQL connection:

- `LaunchPad/src/LaunchPad.Service.Custom.PreLaunchAssemblies.SanMan/SanManPreLaunch.cs`
- `LaunchPad/src/LaunchPad.Service.Custom.PreLaunchAssemblies.SanMan/Service References/SanManWeb/Reference.cs`

---

## 6. SanManJob

### SQL files

- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateDropMachineState.proc.sql`
- `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql`

### C# files

- No direct local C# file was found that names `SanManJob` as a database.
- It appears through SQL only.

---

## 7. DropManagement

### SQL files

- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_CreateRemoteStoreDependenciesInDM.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RemoveRemoteStoreDependenciesInDM.proc.sql`
- `LaunchPad/src/deploy/sql/000006/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000007/up_GetDropMachineSelection.proc.sql`
- `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql`

### C# files

Generated/local service reference files:

- `LaunchPad/src/LaunchPad.Components/Web References/DropManagement/Reference.cs`
- `LaunchPad/src/LaunchPad.Definitions/Orcas/Web References/DropManagement/Reference.cs`
- `LaunchPad/src/LaunchPad.Service.Custom.PreLaunchAssemblies.SanMan/Service References/DropManagement/Reference.cs`

---

## 8. Techease

### SQL files

- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateBuildMachineState.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateDropMachineState.proc.sql`
- `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql`

### C# files

- No direct local C# file was found that names `Techease` as a database.
- It appears through SQL only.

---

## 9. Utils

### SQL files

- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Views/vw_LopezRequestProperty.view.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UtilizationDailyPopulateDataTable.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_TemplateUpdate.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_TemplateSetSchedule.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_Tools_IsReadyForPatching.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_Tools_GetServersForPatching.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestGet.proc.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_RequestGetByTargetDate.function.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_RequestGetByTargetDate2.function.sql`
- `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Functions/fn_RequestGetByTargetDate_DONT_DELETE.function.sql`

### C# files

- No direct local C# file was found that names `Utils` as a runtime database.
- It appears through SQL utility/database-project usage only.

---

## 10. LabSchedule

### Connection / config files

- No runtime application connection string was found for `LabSchedule` in LaunchPad web/service config.
- `LabSchedule` appears as a database-project target rather than a main application connection.

### SQL files

- `LaunchPad/src/LabSchedule.Database/LabSchedule.Database.dbproj`
- `LaunchPad/src/LabSchedule.Database/LabSchedule.Database.sqlproj`
- `LaunchPad/src/LabSchedule.Database/Scripts/Post-Deployment/Permissions.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Views/dbo.vw_PoolsAndServers.view.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetTodaysSchedules.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.up_GetSchedulesReadyToBeProcessed.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedulesWithMatchingSQLServer.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedulesWithMatchingDropServerFromPreviousDay.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleSQLServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleLayoutServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleDropServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetScheduleBuilds.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetPoolServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabSQLServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabDropServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabBuildMachines.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetLabBBTServers.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedules.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetSchedule.proc.sql`
- `LaunchPad/src/LabSchedule.Database/Schema Objects/Stored Procedures/dbo.sp_GetPoolLabs.proc.sql`

### C# files

- No direct local C# file was found that opens `LabSchedule` as an application runtime database.
- It appears through the dedicated SQL project only.

---

## Summary Notes

- `FeedStore` is intentionally absent here because, inside LaunchPad, it was only found in `*.dbschema` artifacts, and those were excluded by request.
- `LaunchPad` and `LabStatus` are the clearest runtime databases from the C# perspective.
- `DAD` is reached both through SQL and through a SOAP service client.
- `SpaceMan`, `SanMan`, `SanManJob`, `DropManagement`, `Techease`, and `Utils` are primarily visible through SQL assets.
- `LabSchedule` is present as its own SQL project target database.
