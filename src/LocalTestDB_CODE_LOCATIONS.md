# DB / Code / Procedure Call Map



Generated:

2026-04-16



\## Quick Inventory



\### LaunchPad: databases mentioned



| Database | Where it appears | Current classification |

| --- | --- | --- |

| LaunchPad | App/Web config, `LaunchPadDB.cs`, deploy SQL cross-db reads | Main runtime database |

| LabStatus | Service app config, deploy SQL cross-db reads, db project variable | Runtime direct read + cross-db dependency |

| DAD | `LaunchPadDB.cs` external proc call, deploy SQL, db project reference | External dependency |

| SpaceMan | Deploy SQL, external dependency schema/project | External dependency |

| SanMan | Deploy SQL, external dependency schema/project | External dependency |

| DropManagement | Deploy SQL and external dependency schema/project | External dependency |

| Techease | Deploy SQL cross-db reads | External dependency |

| Utils | `LaunchPad.Database.sqlproj` SQLCMD variable | Database-project dependency |

| LabSchedule | `LabSchedule.Database` project target DB | Database-project dependency |



\### BuildStatus: databases mentioned



| Database | Where it appears | Current classification |

| --- | --- | --- |

| LabStatus | Web/REST/test/component config, `BuildStatusDB.cs`, DB project, deploy manifest | Main runtime database |

| BFD | Web/test config, `BuildStatusDB.GetBuildBFD()` | Runtime direct read |

| CDBurn2 | Web/test config, CDBurn-related pages/components | Configured external integration |

| devdiv\_General | `FlashNewsDBconnectionString`, `FlashNewsControl2` | Configured external integration |

| Addax\_whby | `DDRelQA.DataLayer` config and data layer | Separate runtime data layer |

| LaunchPad | `up\_GetLaunchPadRequestsForSessions` result path in `BuildStatusDB.cs` | Cross-database dependency via LabStatus procedures |



\## Read / Call Chains



\- Boundary used here: database/config or cross-db proc -> DB wrapper/procedure -> component/service/model -> top-level entry point (page, service, API, control).

\- For external controls/assemblies, the chain stops at the local code boundary if the actual implementation is outside the repo.



\### LaunchPad: call-chain by database



| Database | Where it is read or referenced | Called by | Higher-level entry points |

| --- | --- | --- | --- |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` via `LaunchPad` connection string; deploy procedure `up\_UpdateDropMachineState` also reads `LaunchPad..\*` | `Request.cs`, `Lab.cs`, `Pool.cs`, `Build.cs`, `BuildMachine.cs`, `Org.cs`, `Resource.cs`, `Template.cs`, other `LaunchPad.Components/\*` using `new LaunchPadDB()` | `RequestView.aspx.cs`, `Default.aspx.cs`, `RequestWizard\_\*.aspx.cs`, `Templates.aspx.cs`, `BuildResourceChangeLog.aspx.cs` |

| LabStatus | `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs` via `LabStatus` connection string and `up\_CreateBuildIDs`; deploy procedure `up\_UpdateDropMachineState` reads `LabStatus..tbl\_Session` / `LabStatus..tbl\_Lab` | `RequestProcessor` direct service workflow; `LaunchPadDB` state-update methods depend on procedures that read LabStatus | `LaunchPad.Service` background processing and request state transitions |

| DAD | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` calls `DAD..up\_ServerGet`; deploy procedures `up\_UpdateDropMachineState` and `up\_GetDropMachineSelection` read `DAD..\*`; `LabSchedule.Database.sqlproj` references `DAD.dacpac` | `LaunchPadDB` wrapper methods, drop/build machine selection procedures, DB project build | Components around build machines, drop machines, resources, and state updates |

| SpaceMan | Deploy procedure `up\_GetDropMachineSelection` reads `SpaceMan..tbl\_Allocation`, `SpaceMan..tbl\_SAN`; post-deploy scripts also reference `SpaceMan..\*` | SQL procedures invoked through `LaunchPadDB` drop/SAN selection methods | Resource pool, drop machine, and SAN-related component flows |

| SanMan | Deploy procedure `up\_GetDropMachineSelection` reads `SanMan..tbl\_VDisk`, `SanMan..vw\_CreatedVDisk`, `SanMan..tbl\_San` | SQL procedures invoked through `LaunchPadDB` SAN/drop selection methods | Drop machine and SAN workflows |

| DropManagement | Deploy procedure `up\_GetDropMachineSelection` joins `DropManagement..tbl\_Drop` | SQL procedures invoked through `LaunchPadDB` drop selection methods | Drop-management related request/resource flows |

| Techease | Deploy procedure `up\_UpdateDropMachineState` reads `Techease..vw\_Ticket`, `Techease..tbl\_TicketMachine` | SQL procedure executed through LaunchPad state-update path | Drop/build machine state update workflows |

| Utils | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj` SQLCMD variable `UtilsDB` | DB project build/deployment | Database project only |

| LabSchedule | `LaunchPad/src/LabSchedule.Database/LabSchedule.Database.sqlproj` / `.dbproj` target DB `LabSchedule` | DB project build/deployment | Database project only |



\### BuildStatus: call-chain by database



| Database | Where it is read or referenced | Called by | Higher-level entry points |

| --- | --- | --- | --- |

| LabStatus | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` via `BuildStatusConnectionString`; REST API/Web/test config; `LabStatus.Database.dbproj`; deploy manifest | `Build.cs`, `Issue.cs`, `Fix.cs`, `Lab.cs`, `Org.cs`, `SessionGroup.cs`, `ProcessGroup.cs`, `PreservationRequest.cs`, `BuildStatusService.cs`, `BuildService.cs`, `AjaxService.cs` via `BuildStatusDBFactory.Create()` | `BuildChangeLog.aspx.cs`, `BuildDetails.aspx.cs`, `BuildLog.aspx.cs`, `FixLaunch.aspx.cs`, `BuildService.asmx`, `BuildFixService.asmx`, `AjaxService.asmx`, `BuildStatusRestAPI` controllers/services |

| BFD | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` method `GetBuildBFD()` opens `BFDConnectionString` and executes `sp\_GetBuildBFD` | `BuildStatusDB` direct helper method; downstream consumers go through component/service layer | Build detail / build metadata flows that need BFD enrichment |

| CDBurn2 | `BuildStatus/src/BuildStatus/Web.Config` and test config define `CDBurnConnectionString`; local code references CDBurn integration and package | `BurnLayouts.aspx.cs`, `Common.cs`, `Controls/BuildRow.ascx.cs`; external `Microsoft.CDBurn.Components` package likely consumes the connection | Burn layouts pages and controls |

| devdiv\_General | `BuildStatus/src/BuildStatus/Web.Config` defines `FlashNewsDBconnectionString` | `Default.aspx` hosts `FlashNewsControl2`; actual DB consumption appears to occur inside external `FlashNewsControl2.dll` | `Default.aspx` home/status page |

| Addax\_whby | `BuildStatus/src/DDRelQA.DataLayer/app.config` defines `Addax\_whbyConnectionString1`; `AddaxManager.cs` and generated `.designer.cs/.dbml` files read it | `DDRelQA.DataLayer` consumers | DDRelQA-related tools/pages/components that consume the separate data layer |

| LaunchPad | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` method `GetRequestDataforBuilds()` executes `up\_GetLaunchPadRequestsForSessions` | `Build.cs` request-loading helpers use this method to enrich builds with LaunchPad request data | Build detail, request display, and related service flows |



\## Recommended Reading Order



1\. Read the inventory tables above to see every database that is mentioned in each repo.

2\. For runtime DBs, start from the wrapper file: `LaunchPadDB.cs` or `BuildStatusDB.cs`.

3\. Then move to the component/model layer listed in `Called by`.

4\. Finally move to the page/service/API entry points listed in `Higher-level entry points`.

5\. For external databases without local wrapper code, start from the procedure or config line and follow the local caller boundary only.



\## LaunchPad database map



\### C# direct access



| Database | Connection/config evidence | Direct C# entry points | Notes |

| --- | --- | --- | --- |

| LaunchPad | `LaunchPad/src/LaunchPad/Web.config:59-63`<br>`LaunchPad/src/LaunchPad.Service/App.config:40-45` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:12-14` | Main application database. `LaunchPadDB` is the central data access wrapper and uses the `LaunchPad` connection string. |

| LabStatus | `LaunchPad/src/LaunchPad.Service/App.config:40-45` | `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs:302-313` | Service-side direct access. `RequestProcessor.CreateBuildId` opens `LabStatus` and executes `up\_CreateBuildIDs`. |



\### LaunchPad-related C# files using `LaunchPadDB`



| Database | C# file | Evidence |

| --- | --- | --- |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/LaunchInfo.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/LabDefinition.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Lab.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/HardwareClass.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/DropMachine.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Toolset.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Template.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/DefinitionStep.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Definition.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/San.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/ResourceUsage.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/ResourceRequirement.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Resource.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/BuildNumber.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/RequestProperty.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/RequestInfoBare.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/BuildMachine.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Build.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/RequestComment.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/RequestBuild.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Request.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Reporting.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/PreLaunchAssembly.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Pool.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Org.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Components/Log.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Definitions/Partner/Definitions/Partner/Properties/WhidbeyBuildNumber.cs` | `new LaunchPadDB()` |

| LaunchPad | `LaunchPad/src/LaunchPad.Definitions/Partner/Definitions/Partner/Properties/WhidbeyBranch.cs` | `new LaunchPadDB()` |



\### LaunchPad SQL/external database dependencies



| Database | Evidence file | How it is used |

| --- | --- | --- |

| LaunchPad | `LaunchPad/src/deploy/sql/000009/up\_UpdateDropMachineState.sql:36-42` | Cross-database joins against `LaunchPad..tbl\_RequestBuild`, `LaunchPad..tbl\_ResourceUsage`, `LaunchPad..vw\_ResourceDADServers`. |

| LabStatus | `LaunchPad/src/deploy/sql/000009/up\_UpdateDropMachineState.sql:36-39` | Cross-database joins against `LabStatus..tbl\_Session`, `LabStatus..tbl\_Lab`. |

| DAD | `LaunchPad/src/deploy/sql/000009/up\_UpdateDropMachineState.sql:50-67` | Reads `DAD..tbl\_ServerMonitoringSuspend`, `DAD..vw\_Alert`, `DAD..tbl\_Server`. |

| SpaceMan | `LaunchPad/src/deploy/sql/000008/up\_GetDropMachineSelection.proc.sql:228-233` | Reads `SpaceMan..tbl\_Allocation`, `SpaceMan..tbl\_SAN` for SANMan drop capacity logic. |

| SanMan | `LaunchPad/src/deploy/sql/000008/up\_GetDropMachineSelection.proc.sql:237-245` | Reads `SanMan..tbl\_VDisk`, `SanMan..vw\_CreatedVDisk`, `SanMan..tbl\_San`. |

| DropManagement | `LaunchPad/src/deploy/sql/000008/up\_GetDropMachineSelection.proc.sql:242-243` | Joins `DropManagement..tbl\_Drop` while calculating SANMan usage. |

| Techease | `LaunchPad/src/deploy/sql/000009/up\_UpdateDropMachineState.sql:62-64` | Reads `Techease..vw\_Ticket`, `Techease..tbl\_TicketMachine`. |

| Utils | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj:2090-2100` | Declared as SQLCMD variable `UtilsDB` in DB project. |

| DAD | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj:2090-2094` | Declared as SQLCMD variable `DADDB` in DB project. |

| LabStatus | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj:2095-2099` | Declared as SQLCMD variable `LabStatusDB` in DB project. |

| DAD | `LaunchPad/src/LabSchedule.Database/LabSchedule.Database.sqlproj:538-541` | External artifact reference `DAD.dacpac`. |



\### Core direct-access implementation files



| File | DB | What it does |

| --- | --- | --- |

| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` | LaunchPad | Central stored procedure wrapper. Uses `ExecuteDataSet`, `ExecuteStoredProcedure`, `ExecuteScalar` across LaunchPad operations. |

| `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs` | LabStatus | Calls LabStatus stored procedure `up\_CreateBuildIDs` to allocate build IDs. |



\### Notes



\- From C# perspective, the only clearly direct databases are `LaunchPad` and `LabStatus`.

\- `DAD`, `SpaceMan`, `SanMan`, `DropManagement`, `Techease`, and `Utils` are visible mainly through SQL scripts, SQL projects, and cross-database stored procedure logic.

\- If needed, next pass can expand `LaunchPadDB.cs` into a per-stored-procedure table grouped by functional area.



\### LaunchPadDB stored procedure map by functional area



| Functional area | Representative stored procedures / external calls | Primary file |

| --- | --- | --- |

| Org and Lab lookup | `up\_OrgGet`, `up\_LabGet`, `up\_LabGetByName`, `up\_LabGetAll`, `up\_LabGetLPEnabled`, `up\_LabGetByPool`, `up\_LabGetAllByOrg`, `up\_LabGetRequests`, `up\_LabGetRecentRevision`, `up\_LabGenerateRevision` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Lab definition management | `up\_LabDefinitionGet`, `up\_LabDefinitionGetByFilters`, `up\_LabDefinitionUpdate`, `up\_LabDefinitionRemove`, `up\_LabDefinitionGetLog` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Definitions and steps | `up\_DefinitionGet`, `up\_DefinitionGetByName`, `up\_DefinitionGetBuildStatusWorkflow`, `up\_DefinitionGetByLab`, `up\_DefinitionGetByOrg`, `up\_DefinitionStepGet`, `up\_DefinitionStepGetByDefinition`, `up\_DefinitionStepGetNext`, `up\_DefinitionStepGetPrevious` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Properties and extended properties | `up\_PropertyGetByDefinition`, `up\_PropertyGetByDefinitionStep`, `up\_PropertyGetCustomByDefinition`, `up\_RequestPropertySet`, `up\_ExtendedPropertySet` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Build and request-build flow | `up\_BuildGetByDefinitionStep`, `up\_RequestBuildClear`, `up\_RequestBuildAdd`, `up\_RequestBuildSetStatus`, `up\_RequestBuildSetSessionID`, `up\_RequestBuildRemove`, `up\_RelaunchBuild` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Request lifecycle | `up\_RequestGetSchema`, `up\_RequestGet`, `up\_RequestGetByRevisionAndLab`, `up\_RequestGetByPool`, `up\_RequestGetForLaunch`, `up\_RequestGetTemplates`, `up\_RequestSet`, `up\_RequestCancel`, `up\_RequestAbort`, `up\_RequestSetStatus`, `up\_RequestSetOwners`, `up\_RequestSetIsTestBuild`, `up\_RequestSetLaunchDate`, `up\_RequestSetRevision`, `up\_RequestClone`, `up\_RequestCreateDraft`, `up\_RequestGetByTargetDate`, `up\_RequestGetByValidation`, `up\_RequestGetPrevious`, `up\_RequestGetLaunchInfo`, `up\_RequestGetValidationRequestQueue` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Comments, logs, auditing | `up\_RequestCommentAdd`, `up\_TemplateCommentAdd`, `up\_AddLog`, `up\_RequestAddLaunchLog`, `up\_GetLog` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Pre-launch configuration | `up\_PreLaunchGetAll`, `up\_PreLaunchGetByLabDefinition`, `up\_PreLaunchAdd`, `up\_PreLaunchRemove` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Resources and assignments | `up\_ResourceRequirementGetByName`, `up\_ResourceRequirementGetByBuild`, `up\_ResourceActivate`, `up\_ResourceDeactivate`, `up\_ResourceUsageSet`, `up\_ResourceUsageClear`, `up\_ResourceGetAll`, `up\_ResourceGetWithSessionId`, `up\_ResourceGetByName`, `up\_ResourceGetRankedByPool`, `up\_ResourceGetByPool`, `up\_ResourceGetByCategory`, `up\_ServerAssignmentsGet`, `up\_ServerAssignmentsAdd`, `up\_ServerAssignmentsRemove`, `up\_PoolAssignmentsUpdate` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Pools and SAN selection | `up\_PoolAdd`, `up\_PoolUpdate`, `up\_PoolRemove`, `up\_PoolGet`, `up\_PoolGetByType`, `up\_GetSanSelection`, `up\_GetSansByPool`, `up\_SetSansForPool` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Machines and scheduling support | `up\_GetPartialMachineReleaseInfo`, `up\_UpdateBuildMachineState`, `up\_UpdateDropMachineState`, `up\_GetBuildMachineSelection`, `up\_GetDropMachineSelection`, `up\_GetNextRevision` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Reporting and utilization | `up\_ReportingGetServerUsage`, `up\_UtilizationGetOverallSummary`, `up\_UtilizationGetBuildMachineHistory`, `up\_UtilizationGetPoolHistory`, `up\_UtilizationGetClassHistory` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Whidbey / legacy support | `up\_GetRunBVTFlag`, `up\_GetWhidbeyLabs`, `up\_GetCurrentWhidbeyBuilds` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| External database call from LaunchPadDB | `DAD..up\_ServerGet` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Template management | `up\_TemplateLoadById`, `up\_TemplateLoadByRequestId`, `up\_TemplateLoadAll`, `up\_TemplateLoadByDefinition`, `up\_TemplateAdd`, `up\_TemplateDelete`, `up\_TemplateAddDefinition`, `up\_TemplateUpdate`, `up\_TemplateSetSchedule` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

| Toolset lookup | `up\_ToolsetGet` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |



\### Key direct call sites worth starting from



| Start point | Why it matters |

| --- | --- |

| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:12-14` | Root database wrapper constructor for the `LaunchPad` connection string. |

| `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs:302-313` | Only clearly direct LabStatus call in C# found so far. |

| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:1753` | Explicit cross-database stored procedure call into `DAD..up\_ServerGet`. |

| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:1774` | Drop machine selection entry point, often useful when tracing LaunchPad-SAN/DAD dependencies. |

| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:1356-1361` | State update entry points that connect LaunchPad logic with cross-database SQL in deploy scripts. |



\## BuildStatus database map



\### C# direct access



| Database | Connection/config evidence | Direct C# entry points | Notes |

| --- | --- | --- | --- |

| LabStatus | `BuildStatus/src/BuildStatus/Web.Config:124-134`<br>`BuildStatus/src/BuildStatusRestAPI/Web.config:45-49`<br>`BuildStatus/src/BuildStatus.Tests/App.config:3-7`<br>`BuildStatus/src/BuildStatus.Components/app.config:5-7`<br>`BuildStatus/src/LabStatus.DataBase.Test/App.config:3-5` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:14-23` | Main application database. `BuildStatusDB` initializes `m\_Db` with `BuildStatusConnectionString`. |

| BFD | `BuildStatus/src/BuildStatus/Web.Config:124-134`<br>`BuildStatus/src/BuildStatus.Tests/App.config:3-7` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:1913-1925` | Secondary direct DB access. `GetBuildBFD` opens a dedicated `Database` using `BFDConnectionString`. |

| devdiv\_General | `BuildStatus/src/BuildStatus/Web.Config:32` | `BuildStatus/src/BuildStatus/Default.aspx:6`<br>`BuildStatus/src/BuildStatus/Default.aspx:92-94`<br>`BuildStatus/src/BuildStatus/Default.aspx.designer.cs:31` | Configured through `FlashNewsDBconnectionString`. Current evidence shows it is consumed indirectly by the external `FlashNewsControl2` control rather than by a local `BuildStatusDB`/`SqlConnection` wrapper. |

| Addax\_whby | `BuildStatus/src/DDRelQA.DataLayer/app.config:5-8` | `BuildStatus/src/DDRelQA.DataLayer/AddaxManager.cs`<br>`BuildStatus/src/DDRelQA.DataLayer/\*.designer.cs` | Separate LINQ-to-SQL style data layer used by DDRelQA components. |



\### BuildStatus-related C# files using `BuildStatusDB`



| Database | C# file | Evidence |

| --- | --- | --- |

| LabStatus | `BuildStatus/src/BuildStatus.Components/Build.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Components/Issue.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Components/Fix.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Components/Lab.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Components/Org.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Components/SessionGroup.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Components/ProcessGroup.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Components/PreservationRequest.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus/ExternalServices/BuildStatusService.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus/Services/BuildService.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus/Services/AjaxService.cs` | `BuildStatusDBFactory.Create()` |

| LabStatus | `BuildStatus/src/BuildStatus.Tests/BuildTests.cs` | `BuildStatusDBFactory.Override` |



\### BuildStatus SQL/external database dependencies



| Database | Evidence file | How it is used |

| --- | --- | --- |

| LabStatus | `BuildStatus/src/BuildStatus/Web.Config:124-134` | Main application connection strings for dev/staging/prod. |

| LabStatus | `BuildStatus/src/BuildStatusRestAPI/Web.config:45-49` | REST API connection strings for dev/staging/prod. |

| LabStatus | `BuildStatus/src/LabStatus.Database/LabStatus.Database.dbproj:33-47` | Database project for the main `LabStatus` schema/build output. |

| LabStatus | `BuildStatus/Deploy/sql/Dev\_ContentDeploy\_Dev.xml:1-4` | SQL patch manifest deploys to database `LabStatus`. |

| BFD | `BuildStatus/src/BuildStatus/Web.Config:125-132` | Configured as `BFDConnectionString`; used by `GetBuildBFD`. |

| CDBurn2 | `BuildStatus/src/BuildStatus/Web.Config:14`<br>`BuildStatus/src/BuildStatus/Web.Config:127-133` | Configured as production/dev/staging CDBurn DB integration. |

| devdiv\_General | `BuildStatus/src/BuildStatus/Web.Config:32`<br>`BuildStatus/src/BuildStatus/Default.aspx:6`<br>`BuildStatus/src/BuildStatus/Default.aspx:92-94` | `FlashNewsDBconnectionString` points to `devdiv\_General`, and the page hosts the external `FlashNewsControl2` control that appears to consume it. |

| Addax\_whby | `BuildStatus/src/DDRelQA.DataLayer/app.config:5-8` | Dedicated DDRelQA database used by the separate data layer. |

| LaunchPad | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:332-338` | `GetRequestDataforBuilds` calls `up\_GetLaunchPadRequestsForSessions`, indicating LaunchPad-related request linkage exposed through LabStatus DB stored procedures. |



\### Core direct-access implementation files



| File | DB | What it does |

| --- | --- | --- |

| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` | LabStatus | Central stored procedure wrapper. Uses `ExecuteDataSet`, `ExecuteStoredProcedure`, and `ExecuteScalar` for core BuildStatus operations. |

| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:1913-1925` | BFD | Dedicated helper method `GetBuildBFD` that opens the BFD database directly. |

| `BuildStatus/src/DDRelQA.DataLayer/AddaxManager.cs` | Addax\_whby | DDRelQA data access surface backed by the `Addax\_whby` connection. |



\### BuildStatusDB stored procedure map by functional area



| Functional area | Representative stored procedures / external calls | Primary file |

| --- | --- | --- |

| Lab, org, milestone lookup | `sp\_GetLabsAndMilestonesForStatusPageByOrg`, `up\_GetLabsAll`, `up\_GetLabsByOrg`, `up\_GetOrg`, `up\_GetOrgs`, `up\_GetMilestonesByOrg` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Sessions and search | `up\_GetSessions`, `up\_GetSessionLog`, `up\_GetSimilarBuilds`, `up\_GetSessionsByDateRange`, `up\_GetSessionsByRevisionRange`, `up\_GetRecentSessions`, `up\_GetSessionsByLabAndDate`, `up\_GetSessionsByStartedDate`, `up\_GetSessionsByFinishedDate`, `up\_GetSessionsByBuildUrl` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Session update and lifecycle | `up\_UpdateSession`, `up\_UpdateSessionStatus`, `up\_UpdateSessionTestBuild`, `up\_UpdateSessionCanRecycle`, `up\_DeleteSession`, `up\_GetSessionChangeLog` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Session logs and process data | `up\_GetSessionUnprocessedLogCount`, `up\_UpdateSessionErrorsAsProcessed`, `up\_UpdateSessionStallWarningsAsProcessed`, `up\_GetSessionSetups`, `up\_GetSessionGroups`, `up\_GetSessionGroup`, `up\_GetSessionProcessNames`, `up\_GetProcessLogs` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Build history and related data | `up\_GetRealSignedBuilds`, `up\_GetOtherBuildsByLabBySession`, `up\_GetBuildHistory`, `up\_GetLaunchPadRequestsForSessions`, `sp\_GetBuildBFD` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Issues | `up\_GetIssue`, `up\_GetIssues`, `up\_GetIssuesByBuildID`, `up\_AddIssue`, `up\_UpdateIssue`, `up\_UpdateIssueSourceAndSubSource`, `up\_CloseIssue`, `up\_DeleteIssue`, `up\_SearchIssues`, `up\_AssignIssueToSession`, `up\_DeleteIssueFromSession` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Issue logs and ownership | `up\_AddIssueLog`, `up\_UpdateIssueCausedBy`, `up\_UpdateIssueTriage`, `up\_UpdateIssuePendingOwnerAsAssignedTo`, `up\_ReAssignIssue`, `up\_ReAssignIssueToPending`, `up\_AddNewCostCenter`, `up\_CostCenterExists` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Fix workflow | `up\_GetFix`, `up\_GetFixLogs`, `up\_AddFixForDefault`, `up\_AddFixForSourceCode`, `up\_AddFixForSetup`, `up\_StartFix`, `up\_CompleteFix`, `up\_AddFixLog`, `up\_UpdateManualFixStatus` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Fix synchronization | `up\_AddFixSynchronization`, `up\_UpdateFixSynchronization`, `up\_UpdateFixSynchronizationMachineStatus`, `up\_GetFixSynchronization`, `up\_GetLastFixIdByIssueId` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Preservation requests | `up\_AddPreservationRequest`, `up\_DeletePreservationRequest`, `up\_GetPreservationRequestsByBuild`, `up\_GetAllActivePreservationRequests` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

| Session groups | `up\_EnsureSessionGroup`, `up\_DeleteSessionGroup`, `up\_AddSessionGroupDependency`, `up\_UpdateSessionGroup`, `up\_UpdateSessionGroupProcess`, `up\_UpdateSessionGroupExpirationInfo` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |



\### Key direct call sites worth starting from



| Start point | Why it matters |

| --- | --- |

| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:14-23` | Root database wrapper constructor for the `BuildStatusConnectionString` / `LabStatus` database. |

| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:1913-1925` | Only clearly direct BFD database call found in the main wrapper. |

| `BuildStatus/src/BuildStatus.Components/Build.cs` | Central model for build/session reads, request linkage, and session update flows. |

| `BuildStatus/src/BuildStatus.Components/Issue.cs` | Main issue lifecycle entry point: update, assign, close, logs, notifications. |

| `BuildStatus/src/BuildStatus.Components/Fix.cs` | Main fix scheduling/completion/synchronization entry point. |



\### Notes



\- From C# perspective, the primary direct databases are `LabStatus`, `BFD`, and `Addax\_whby`.

\- `CDBurn2` and `devdiv\_General` are currently visible as application integrations through config and external components rather than through a local `BuildStatusDB`-style wrapper class.

\- `BuildStatusDB` is the central access layer for most of the application, while `DDRelQA.DataLayer` is a separate data-access surface backed by `Addax\_whby`.



