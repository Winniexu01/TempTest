﻿# DB / Code / Procedure Call Map

Generated:
2026-04-16

## Quick Inventory

### LaunchPad: databases mentioned

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

### BuildStatus: databases mentioned

| Database | Where it appears | Current classification |
| --- | --- | --- |
| LabStatus | Web/REST/test/component config, `BuildStatusDB.cs`, DB project, deploy manifest | Main runtime database |
| BFD | Web/test config, `BuildStatusDB.GetBuildBFD()` | Runtime direct read |
| CDBurn2 | Web/test config, CDBurn-related pages/components | Configured external integration |
| devdiv_General | `FlashNewsDBconnectionString`, `FlashNewsControl2` | Configured external integration |
| Addax_whby | `DDRelQA.DataLayer` config and data layer | Separate runtime data layer |
| LaunchPad | `up_GetLaunchPadRequestsForSessions` result path in `BuildStatusDB.cs` | Cross-database dependency via LabStatus procedures |

## Read / Call Chains

- Boundary used here: database/config or cross-db proc -> DB wrapper/procedure -> component/service/model -> top-level entry point (page, service, API, control).
- For external controls/assemblies, the chain stops at the local code boundary if the actual implementation is outside the repo.

### LaunchPad: call-chain by database

| Database | Where it is read or referenced | Called by | Higher-level entry points |
| --- | --- | --- | --- |
| LaunchPad | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` via `LaunchPad` connection string; deploy procedure `up_UpdateDropMachineState` also reads `LaunchPad..*` | `Request.cs`, `Lab.cs`, `Pool.cs`, `Build.cs`, `BuildMachine.cs`, `Org.cs`, `Resource.cs`, `Template.cs`, other `LaunchPad.Components/*` using `new LaunchPadDB()` | `RequestView.aspx.cs`, `Default.aspx.cs`, `RequestWizard_*.aspx.cs`, `Templates.aspx.cs`, `BuildResourceChangeLog.aspx.cs` |
| LabStatus | `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs` via `LabStatus` connection string and `up_CreateBuildIDs`; deploy procedure `up_UpdateDropMachineState` reads `LabStatus..tbl_Session` / `LabStatus..tbl_Lab` | `RequestProcessor` direct service workflow; `LaunchPadDB` state-update methods depend on procedures that read LabStatus | `LaunchPad.Service` background processing and request state transitions |
| DAD | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` calls `DAD..up_ServerGet`; deploy procedures `up_UpdateDropMachineState` and `up_GetDropMachineSelection` read `DAD..*`; `LabSchedule.Database.sqlproj` references `DAD.dacpac` | `LaunchPadDB` wrapper methods, drop/build machine selection procedures, DB project build | Components around build machines, drop machines, resources, and state updates |
| SpaceMan | Deploy procedure `up_GetDropMachineSelection` reads `SpaceMan..tbl_Allocation`, `SpaceMan..tbl_SAN`; post-deploy scripts also reference `SpaceMan..*` | SQL procedures invoked through `LaunchPadDB` drop/SAN selection methods | Resource pool, drop machine, and SAN-related component flows |
| SanMan | Deploy procedure `up_GetDropMachineSelection` reads `SanMan..tbl_VDisk`, `SanMan..vw_CreatedVDisk`, `SanMan..tbl_San` | SQL procedures invoked through `LaunchPadDB` SAN/drop selection methods | Drop machine and SAN workflows |
| DropManagement | Deploy procedure `up_GetDropMachineSelection` joins `DropManagement..tbl_Drop` | SQL procedures invoked through `LaunchPadDB` drop selection methods | Drop-management related request/resource flows |
| Techease | Deploy procedure `up_UpdateDropMachineState` reads `Techease..vw_Ticket`, `Techease..tbl_TicketMachine` | SQL procedure executed through LaunchPad state-update path | Drop/build machine state update workflows |
| Utils | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj` SQLCMD variable `UtilsDB` | DB project build/deployment | Database project only |
| LabSchedule | `LaunchPad/src/LabSchedule.Database/LabSchedule.Database.sqlproj` / `.dbproj` target DB `LabSchedule` | DB project build/deployment | Database project only |

### BuildStatus: call-chain by database

| Database | Where it is read or referenced | Called by | Higher-level entry points |
| --- | --- | --- | --- |
| LabStatus | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` via `BuildStatusConnectionString`; REST API/Web/test config; `LabStatus.Database.dbproj`; deploy manifest | `Build.cs`, `Issue.cs`, `Fix.cs`, `Lab.cs`, `Org.cs`, `SessionGroup.cs`, `ProcessGroup.cs`, `PreservationRequest.cs`, `BuildStatusService.cs`, `BuildService.cs`, `AjaxService.cs` via `BuildStatusDBFactory.Create()` | `BuildChangeLog.aspx.cs`, `BuildDetails.aspx.cs`, `BuildLog.aspx.cs`, `FixLaunch.aspx.cs`, `BuildService.asmx`, `BuildFixService.asmx`, `AjaxService.asmx`, `BuildStatusRestAPI` controllers/services |
| BFD | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` method `GetBuildBFD()` opens `BFDConnectionString` and executes `sp_GetBuildBFD` | `BuildStatusDB` direct helper method; downstream consumers go through component/service layer | Build detail / build metadata flows that need BFD enrichment |
| CDBurn2 | `BuildStatus/src/BuildStatus/Web.Config` and test config define `CDBurnConnectionString`; local code references CDBurn integration and package | `BurnLayouts.aspx.cs`, `Common.cs`, `Controls/BuildRow.ascx.cs`; external `Microsoft.CDBurn.Components` package likely consumes the connection | Burn layouts pages and controls |
| devdiv_General | `BuildStatus/src/BuildStatus/Web.Config` defines `FlashNewsDBconnectionString` | `Default.aspx` hosts `FlashNewsControl2`; actual DB consumption appears to occur inside external `FlashNewsControl2.dll` | `Default.aspx` home/status page |
| Addax_whby | `BuildStatus/src/DDRelQA.DataLayer/app.config` defines `Addax_whbyConnectionString1`; `AddaxManager.cs` and generated `.designer.cs/.dbml` files read it | `DDRelQA.DataLayer` consumers | DDRelQA-related tools/pages/components that consume the separate data layer |
| LaunchPad | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` method `GetRequestDataforBuilds()` executes `up_GetLaunchPadRequestsForSessions` | `Build.cs` request-loading helpers use this method to enrich builds with LaunchPad request data | Build detail, request display, and related service flows |

## Recommended Reading Order

1. Read the inventory tables above to see every database that is mentioned in each repo.
2. For runtime DBs, start from the wrapper file: `LaunchPadDB.cs` or `BuildStatusDB.cs`.
3. Then move to the component/model layer listed in `Called by`.
4. Finally move to the page/service/API entry points listed in `Higher-level entry points`.
5. For external databases without local wrapper code, start from the procedure or config line and follow the local caller boundary only.

## Representative Trace Chains

### LaunchPad examples

| Database | Example chain |
| --- | --- |
| LaunchPad | `RequestView.aspx.cs` `Page_Load()` -> `Request.Load(requestId)` in `LaunchPad.Components/Request.cs` -> `LaunchPadDB.GetRequest*` in `LaunchPad.Components/LaunchPadDB.cs` -> procedure `up_RequestGet` |
| LaunchPad | `Default.aspx.cs` request-loading flow -> `Request.LoadByTargetDate()` -> `LaunchPadDB.GetRequestByTargetDate()` -> procedure `up_RequestGetByTargetDate` |
| LaunchPad | `Request.cs` launch-date update path -> `LaunchPadDB.SaveRequestLaunchTime()` -> procedure `up_RequestSetLaunchDate` |
| LaunchPad | resource/pool selection flow in `Resource.cs` -> `LaunchPadDB.GetResourcesByPool()` -> procedure `up_ResourceGetByPool` |
| LabStatus | `LaunchPad.Service/Components/RequestProcessor.cs` `CreateBuildId()` -> direct `Database(CollectionHandler["LabStatus"])` -> procedure `up_CreateBuildIDs` |
| DAD / LabStatus / SpaceMan / Techease | drop/build-machine state update flow -> `Resource` / `LaunchPadDB.UpdateDropMachineState()` -> procedure `up_UpdateDropMachineState` -> cross-db reads from `DAD..*`, `LabStatus..*`, `SpaceMan..*`, `Techease..*` |

### BuildStatus examples

| Database | Example chain |
| --- | --- |
| LabStatus | `BuildStatusRestAPI/Controllers/BuildStatusController.cs` issue lookup -> service layer -> `Issue.GetIssuesByBuildIDs()` -> `BuildStatusDB.GetIssuesByBuildIDs()` -> procedure `up_GetIssuesBySessions` |
| LabStatus | REST/API create-issue flow -> `Issue.Add()` -> `BuildStatusDB.AddIssue()` -> procedure `up_AddIssue` |
| LabStatus | AJAX abandon/update flow -> `AjaxService.cs` -> `Build.UpdateStatus()` / `BuildStatusDB.UpdateSessionsStatus()` -> procedure `up_UpdateSessionStatus` |
| LabStatus | build search / status-page flow -> `BuildService.cs` / `Build.cs` -> `BuildStatusDB.GetLabsAndMilestonesForStatusPage()` and session query methods -> procedures like `sp_GetLabsAndMilestonesForStatusPageByOrg`, `up_GetSessions*` |
| BFD | build detail enrichment flow -> `BuildStatusDB.GetBuildBFD()` -> dedicated `Database(connectionSettings["BFDConnectionString"])` -> procedure `sp_GetBuildBFD` |
| devdiv_General | `BuildStatus/Default.aspx` hosts `FlashNewsControl2` -> control consumes `FlashNewsDBconnectionString` from `BuildStatus/Web.Config` -> database `devdiv_General` |
| LaunchPad | build/request enrichment flow -> `Build.cs` request-loading helpers -> `BuildStatusDB.GetRequestDataforBuilds()` -> procedure `up_GetLaunchPadRequestsForSessions` |

## Wrapper Method Mappings

### LaunchPadDB: method -> procedure -> caller

| Wrapper method | Procedure / external call | Immediate callers | Purpose |
| --- | --- | --- | --- |
| `GetRequest(int requestId)` | `up_RequestGet` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.Load()` | Load one request by ID |
| `GetRequests(string requestIds)` | `up_RequestGet` | `LaunchPad/src/LaunchPad.Components/Request.cs` | Batch load requests |
| `GetRequestsToLaunch(string serviceInstanceName)` | `up_RequestGetForLaunch` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.GetRequestsToLaunch()` | Fetch requests ready for launch |
| `GetRequestByRevisionAndLab(...)` | `up_RequestGetByRevisionAndLab` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.GetByRevisionAndLab()` | Find request by revision and lab |
| `GetRequestForTargetDate(...)` | `up_RequestGetByTargetDate` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.GetByTargetDate()` | Query requests by target date |
| `GetRequestByValidation(...)` | `up_RequestGetByValidation` | `LaunchPad/src/LaunchPad.Components/Request.cs` | Query validation requests |
| `GetRequestTemplates(int orgId)` | `up_RequestGetTemplates` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.GetTemplates()` | Load request templates |
| `CreateRequestDraft(...)` | `up_RequestCreateDraft` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.Create()` | Create draft request |
| `SaveRequest(...)` | `up_RequestSet` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.Save()` | Persist request changes |
| `SetRequestStatus(...)` | `up_RequestSetStatus` | `LaunchPad/src/LaunchPad.Components/Request.cs` status setter | Update request workflow status |
| `CancelRequest(...)` | `up_RequestCancel` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.Cancel()` | Cancel request |
| `SaveRequestLaunchTime(...)` | `up_RequestSetLaunchDate` | `LaunchPad/src/LaunchPad.Components/Request.cs` `LaunchTime` setter | Save launch datetime |
| `SaveRequestRevision(...)` | `up_RequestSetRevision` | `LaunchPad/src/LaunchPad.Components/Request.cs` revision setter | Save request revision |
| `GetLab(int labId)` | `up_LabGet` | `LaunchPad/src/LaunchPad.Components/Lab.cs` `Lab.Load()` | Load lab metadata |
| `GetLabsByPool(int poolId)` | `up_LabGetByPool` | LaunchPad lab/pool queries | Find labs for resource pool |
| `GetLabsByOrg(int orgId)` | `up_LabGetAllByOrg` | LaunchPad lab queries | Find labs by org |
| `GetLabDefinition(...)` | `up_LabDefinitionGet` | definition/lab binding code | Load lab-definition mappings |
| `UpdateLabDefinitionPoolAssignments(...)` | `up_PoolAssignmentsUpdate` | lab definition property flows | Update build/drop pool assignments |
| `GetDefinition(int definitionId)` | `up_DefinitionGet` | `LaunchPad/src/LaunchPad.Components/Definition.cs` `Definition.Load()` | Load definition |
| `GetDefinitionByBuildStatusWorkflow(...)` | `up_DefinitionGetBuildStatusWorkflow` | BuildStatus workflow integration | Map BuildStatus workflow to LaunchPad definition |
| `GetResourcesByPool(int poolId)` | `up_ResourceGetByPool` | resource allocation code in `Resource.cs` | Query resources in pool |
| `SaveResourceUsage(...)` | `up_ResourceUsageSet` | `LaunchPad/src/LaunchPad.Components/RequestProperty.cs` and request/resource flows | Persist resource usage |
| `ActivateResource(...)` | `up_ResourceActivate` | resource management code | Enable resource |
| `UpdateBuildMachineState()` | `up_UpdateBuildMachineState` | maintenance/state sync flows | Sync build machine state |
| `UpdateDropMachineState()` | `up_UpdateDropMachineState` | maintenance/state sync flows | Sync drop machine state across DBs |
| `AddRequestBuild(...)` | `up_RequestBuildAdd` | `LaunchPad/src/LaunchPad.Components/RequestBuild.cs` `Add()` | Attach build to request |
| `SetRequestBuildStatus(...)` | `up_RequestBuildSetStatus` | request/build lifecycle updates | Update build status within request |
| `GetServer(...)` | `DAD..up_ServerGet` | machine/resource lookup flows | External DAD server lookup |

### BuildStatusDB: method -> procedure -> caller

| Wrapper method | Procedure / external call | Immediate callers | Purpose |
| --- | --- | --- | --- |
| `GetSession(int sessionId)` | `up_GetSessions` | `BuildStatus/src/BuildStatus.Components/Build.cs` load helpers | Load one build session |
| `GetSessions(List<int> sessionIds)` | `up_GetSessions` | `BuildStatus/src/BuildStatus.Components/Build.cs` `GetSessions()` | Batch load sessions |
| `GetSessionsByDateRange(...)` | `up_GetSessionsByDateRange` | `BuildStatus/src/BuildStatus.Components/Build.cs` search flows | Query sessions by date |
| `GetSessionsByRevisionRange(...)` | `up_GetSessionsByRevisionRange` | `BuildStatus/src/BuildStatus.Components/Build.cs` search flows | Query sessions by revision |
| `GetSessionsByLabAndDate(...)` | `up_GetSessionsByLabAndDate` | `BuildStatus/src/BuildStatus.Components/Build.cs`, `BuildStatus/src/BuildStatus/Services/BuildService.cs` | Dashboard/time-range lab query |
| `GetSessionsByStartedDate(...)` | `up_GetSessionsByStartedDate` | `BuildStatus/src/BuildStatus.Components/Build.cs` | Query sessions by started date |
| `GetRecentSessions(string labIds)` | `up_GetRecentSessions` | `BuildStatus/src/BuildStatus.Components/Build.cs` | Load recent sessions |
| `GetSessionLog(int sessionId)` | `up_GetSessionLog` | build log retrieval code | Load build execution logs |
| `GetSessionProperties(int sessionId)` | `up_GetSessionProperties` | `BuildStatus/src/BuildStatus.Components/Build.cs` property getter | Load session properties |
| `GetSessionGroups(...)` | `up_GetSessionGroups` | `BuildStatus/src/BuildStatus.Components/ProcessGroup.cs` | Load process groups |
| `GetSessionGroupProcess(int processId)` | `up_GetSessionGroupProcess` | process-loading code | Load one process in group |
| `GetProcessLogs(...)` | `up_GetProcessLogs` | troubleshooting/log queries | Load detailed process logs |
| `UpdateSession(...)` | `up_UpdateSession` | `BuildStatus/src/BuildStatus.Components/Build.cs` update flows | Update session state/details |
| `UpdateSessionsStatus(...)` | `up_UpdateSessionStatus` | `BuildStatus/src/BuildStatus.Components/Build.cs`, `BuildStatus/src/BuildStatus/Services/AjaxService.cs` | Bulk/session status update |
| `MarkNewSessionErrorsAsProcessed(...)` | `up_UpdateSessionErrorsAsProcessed` | error management flows | Clear new-error flag |
| `MarkNewSessionStallWarningsAsProcessed(...)` | `up_UpdateSessionStallWarningsAsProcessed` | stall management flows | Clear new-stall flag |
| `GetIssue(int issueId)` | `up_GetIssues` | `BuildStatus/src/BuildStatus.Components/Issue.cs` `Issue.Load()` | Load one issue |
| `GetIssues(IEnumerable<int> issueIds)` | `up_GetIssues` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Batch load issues |
| `GetIssuesByBuildID(int buildId)` | `up_GetIssuesBySession` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Get issues for one build |
| `GetIssuesByBuildIDs(...)` | `up_GetIssuesBySessions` | `BuildStatus/src/BuildStatus.Components/Issue.cs`, REST/API issue lookup flows | Get issues for many builds |
| `SearchIssues(...)` | `up_SearchIssues` | issue search UI/report code | Search issues by many filters |
| `UpdateIssue(...)` | `up_UpdateIssue` | `BuildStatus/src/BuildStatus.Components/Issue.cs` update flows | Persist issue edits |
| `UpdateIssueCausedBy(...)` | `up_UpdateIssueCausedBy` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Save caused-by / cost center |
| `DeleteIssue(int issueId)` | `sp_DeleteIssue` | `BuildStatus/src/BuildStatus.Components/Issue.cs` delete flows | Soft-delete issue |
| `AssignIssueToSession(...)` | `sp_AddIssueToSession` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Link issue to build |
| `DeleteIssueFromSession(...)` | `sp_DeleteIssueFromSession` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Unlink issue from build |
| `AssignBugToIssue(...)` | `sp_AddIssueBug` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Link bug to issue |
| `AssignTicketToIssue(...)` | `sp_AddIssueTicket` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Link ticket to issue |
| `CloseIssue(int issueId)` | `sp_CloseIssue` | `BuildStatus/src/BuildStatus.Components/Issue.cs`, service/API close flows | Close issue |
| `UpdateIssueTriage(...)` | `up_UpdateIssueTriage` | `BuildStatus/src/BuildStatus.Components/Issue.cs` | Update issue triage |
| `GetRequestDataforBuilds(int[] sessionIds)` | `up_GetLaunchPadRequestsForSessions` | `BuildStatus/src/BuildStatus.Components/Build.cs` request enrichment | Pull LaunchPad request context |
| `MakeBuildMachineCanRecycle(...)` | `up_UpdateSessionCanRecycle` | build machine lifecycle code | Mark machine recyclable |
| `DeleteSession(...)` | `up_DeleteSession` | build cleanup flows | Delete/archive session |
| `GetOtherBuildsByBuildId(int buildId)` | `up_GetOtherBuildsByLabBySession` | comparison/history queries | Load related builds |
| `GetBuildBFD(int sessionId)` | `sp_GetBuildBFD` via `BFDConnectionString` | build detail enrichment flows | Load BFD info from BFD DB |

## Procedure Definition Mappings

### LaunchPad: procedure -> SQL file -> caller chain

| Procedure | Definition file | Wrapper / direct DB method | Representative caller chain |
| --- | --- | --- | --- |
| `up_RequestGet` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestGet.proc.sql` | `LaunchPadDB.GetRequest()` / `GetRequests()` | `RequestView.aspx.cs` `Page_Load()` -> `Request.Load(requestId)` -> `LaunchPadDB.GetRequests()` -> `up_RequestGet` |
| `up_RequestGetByTargetDate` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestGetByTargetDate.proc.sql` | `LaunchPadDB.GetRequestForTargetDate()` | request filtering flow -> `Request.GetByTargetDate()` -> `LaunchPadDB.GetRequestForTargetDate()` -> `up_RequestGetByTargetDate` |
| `up_RequestSetLaunchDate` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestSetLaunchDate.proc.sql` | `LaunchPadDB.SaveRequestLaunchTime()` | `Request.LaunchTime` setter -> `LaunchPadDB.SaveRequestLaunchTime()` -> `up_RequestSetLaunchDate` |
| `up_ResourceGetByPool` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ResourceGetByPool.proc.sql` | `LaunchPadDB.GetResourcesByPool()` | `Resource.cs` allocation flow -> `LaunchPadDB.GetResourcesByPool()` -> `up_ResourceGetByPool` |
| `up_UpdateDropMachineState` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateDropMachineState.proc.sql` | `LaunchPadDB.UpdateDropMachineState()` | maintenance/state sync -> `LaunchPadDB.UpdateDropMachineState()` -> `up_UpdateDropMachineState` -> cross-db reads from `DAD`, `LabStatus`, `SpaceMan`, `SanMan`, `DropManagement`, `Techease` |
| `up_CreateBuildIDs` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_CreateBuildIDs.proc.sql` | `LaunchPad.Service/Components/RequestProcessor.cs` `CreateBuildId()` direct `Database.ExecuteScalar()` | `LaunchPad.Service` workflow -> `RequestProcessor.CreateBuildId()` -> `LabStatus` DB -> `up_CreateBuildIDs` |
| `DAD..up_ServerGet` | not found locally | `LaunchPadDB.GetServer()` | machine/resource lookup -> `LaunchPadDB.GetServer()` -> external `DAD` database procedure |

### BuildStatus: procedure -> SQL file -> caller chain

| Procedure | Definition file | Wrapper / direct DB method | Representative caller chain |
| --- | --- | --- | --- |
| `up_GetSessions` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetSessions.proc.sql` | `BuildStatusDB.GetSession()` / `GetSessions()` | `BuildDetails.aspx.cs` -> `Build.Load(sessionId)` -> `BuildStatusDB.GetSession()` -> `up_GetSessions` |
| `up_GetIssuesBySessions` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetIssuesBySessions.proc.sql` | `BuildStatusDB.GetIssuesByBuildIDs()` | REST/API issue lookup -> `Issue.GetIssuesByBuildIds()` -> `BuildStatusDB.GetIssuesByBuildIDs()` -> `up_GetIssuesBySessions` |
| `up_AddIssue` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_AddIssue.proc.sql` | `BuildStatusDB.AddIssue()` | create-issue flow -> `Issue.Add()` -> `BuildStatusDB.AddIssue()` -> `up_AddIssue` |
| `up_UpdateSessionStatus` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateSessionStatus.proc.sql` | `BuildStatusDB.UpdateSessionsStatus()` | `AjaxService.cs` update/abandon flow -> `Build.UpdateStatus()` -> `BuildStatusDB.UpdateSessionsStatus()` -> `up_UpdateSessionStatus` |
| `sp_GetLabsAndMilestonesForStatusPageByOrg` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetLabsAndMilestonesForStatusPageByOrg.proc.sql` | `BuildStatusDB.GetLabsAndMilestonesForStatusPage()` | dashboard/status page flow -> `BuildService.cs` -> `BuildStatusDB.GetLabsAndMilestonesForStatusPage()` -> `sp_GetLabsAndMilestonesForStatusPageByOrg` |
| `sp_GetBuildBFD` | not found locally | `BuildStatusDB.GetBuildBFD()` via `BFDConnectionString` | build detail enrichment -> `BuildStatusDB.GetBuildBFD()` -> external `BFD` database -> `sp_GetBuildBFD` |
| `up_GetLaunchPadRequestsForSessions` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetLaunchPadRequestsForSessions.proc.sql` | `BuildStatusDB.GetRequestDataforBuilds()` | build/request enrichment -> `Build.LoadRequestData()` -> `BuildStatusDB.GetRequestDataforBuilds()` -> `up_GetLaunchPadRequestsForSessions` |

### Notes on procedure ownership

- `up_CreateBuildIDs` belongs to the shared `LabStatus` database, so LaunchPad calls it through a `LabStatus` connection instead of its own `LaunchPad` DB project.
- `DAD..up_ServerGet` and `sp_GetBuildBFD` are external database procedures and are therefore not defined in the local repo SQL projects.
- `up_GetLaunchPadRequestsForSessions` is defined in `LabStatus.Database`, but its business meaning is cross-system because it enriches BuildStatus data with LaunchPad request context.

## End-to-End Trace Matrix

### LaunchPad traces

| Scenario | Entry point | Component / model method | Wrapper / direct DB call | Procedure | SQL definition |
| --- | --- | --- | --- | --- | --- |
| Request details page | `LaunchPad/src/LaunchPad/RequestView.aspx.cs` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.Load()` | `LaunchPadDB.GetRequest()` / `GetRequests()` | `up_RequestGet` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestGet.proc.sql` |
| Request list by target date | `LaunchPad/src/LaunchPad/Default.aspx.cs` | `LaunchPad/src/LaunchPad.Components/Request.cs` `Request.GetByTargetDate()` | `LaunchPadDB.GetRequestForTargetDate()` | `up_RequestGetByTargetDate` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestGetByTargetDate.proc.sql` |
| Save launch time | request property/update flow in `LaunchPad/src/LaunchPad.Components/Request.cs` | `LaunchTime` setter | `LaunchPadDB.SaveRequestLaunchTime()` | `up_RequestSetLaunchDate` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_RequestSetLaunchDate.proc.sql` |
| Resource pool lookup | resource-related pages / components | `LaunchPad/src/LaunchPad.Components/Resource.cs` pool/resource lookup | `LaunchPadDB.GetResourcesByPool()` | `up_ResourceGetByPool` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_ResourceGetByPool.proc.sql` |
| Build ID creation via service | `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs` | `CreateBuildId()` | direct `Database.ExecuteScalar()` on `LabStatus` connection | `up_CreateBuildIDs` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_CreateBuildIDs.proc.sql` |
| Drop machine state sync | maintenance / scheduled-state flow | resource/machine state sync code | `LaunchPadDB.UpdateDropMachineState()` | `up_UpdateDropMachineState` | `LaunchPad/src/LaunchPad.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateDropMachineState.proc.sql` |
| External DAD server lookup | machine/resource lookup flow | LaunchPad machine/resource query code | `LaunchPadDB.GetServer()` | `DAD..up_ServerGet` | not found locally |

### BuildStatus traces

| Scenario | Entry point | Component / model method | Wrapper / direct DB call | Procedure | SQL definition |
| --- | --- | --- | --- | --- | --- |
| Build details page | `BuildStatus/src/BuildStatus/BuildDetails.aspx.cs` | `BuildStatus/src/BuildStatus.Components/Build.cs` `Build.Load()` | `BuildStatusDB.GetSession()` | `up_GetSessions` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetSessions.proc.sql` |
| REST issue lookup | `BuildStatus/src/BuildStatusRestAPI/Controllers/BuildStatusController.cs` | `BuildStatus/src/BuildStatus.Components/Issue.cs` `GetIssuesByBuildIDs()` | `BuildStatusDB.GetIssuesByBuildIDs()` | `up_GetIssuesBySessions` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetIssuesBySessions.proc.sql` |
| Create issue | issue creation UI / REST / service flow | `BuildStatus/src/BuildStatus.Components/Issue.cs` `Add()` | `BuildStatusDB.AddIssue()` | `up_AddIssue` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_AddIssue.proc.sql` |
| Update/abandon session | `BuildStatus/src/BuildStatus/Services/AjaxService.cs` | `BuildStatus/src/BuildStatus.Components/Build.cs` `UpdateStatus()` | `BuildStatusDB.UpdateSessionsStatus()` | `up_UpdateSessionStatus` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_UpdateSessionStatus.proc.sql` |
| Status page lab/milestone summary | `BuildStatus/src/BuildStatus/Services/BuildService.cs` | service/dashboard query flow | `BuildStatusDB.GetLabsAndMilestonesForStatusPage()` | `sp_GetLabsAndMilestonesForStatusPageByOrg` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/sp_GetLabsAndMilestonesForStatusPageByOrg.proc.sql` |
| BFD enrichment | build detail enrichment flow | build/session detail helper code | `BuildStatusDB.GetBuildBFD()` via `BFDConnectionString` | `sp_GetBuildBFD` | not found locally |
| LaunchPad request enrichment | build/request detail flow in `BuildStatus/src/BuildStatus.Components/Build.cs` | `Build.LoadRequestData()` | `BuildStatusDB.GetRequestDataforBuilds()` | `up_GetLaunchPadRequestsForSessions` | `BuildStatus/src/LabStatus.Database/Schema Objects/Schemas/dbo/Programmability/Stored Procedures/up_GetLaunchPadRequestsForSessions.proc.sql` |
| Flash news integration | `BuildStatus/src/BuildStatus/Default.aspx` | external `FlashNewsControl2` control | consumes `FlashNewsDBconnectionString` | n/a | n/a, external control path |

### How to trace further

- If a flow starts from an ASPX/ASMX/API file, first locate the entry method there, then jump to the component/model method in the same row.
- From the component/model method, find the wrapper method listed in the next column.
- From the wrapper method, look up the stored procedure in the method-mapping tables above.
- If the procedure has a local SQL definition file, open that file next; if it says `not found locally`, the logic lives in an external database outside the repo.

## LaunchPad database map

### C# direct access

| Database | Connection/config evidence | Direct C# entry points | Notes |
| --- | --- | --- | --- |
| LaunchPad | `LaunchPad/src/LaunchPad/Web.config:59-63`<br>`LaunchPad/src/LaunchPad.Service/App.config:40-45` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:12-14` | Main application database. `LaunchPadDB` is the central data access wrapper and uses the `LaunchPad` connection string. |
| LabStatus | `LaunchPad/src/LaunchPad.Service/App.config:40-45` | `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs:302-313` | Service-side direct access. `RequestProcessor.CreateBuildId` opens `LabStatus` and executes `up_CreateBuildIDs`. |

### LaunchPad-related C# files using `LaunchPadDB`

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

### LaunchPad SQL/external database dependencies

| Database | Evidence file | How it is used |
| --- | --- | --- |
| LaunchPad | `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql:36-42` | Cross-database joins against `LaunchPad..tbl_RequestBuild`, `LaunchPad..tbl_ResourceUsage`, `LaunchPad..vw_ResourceDADServers`. |
| LabStatus | `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql:36-39` | Cross-database joins against `LabStatus..tbl_Session`, `LabStatus..tbl_Lab`. |
| DAD | `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql:50-67` | Reads `DAD..tbl_ServerMonitoringSuspend`, `DAD..vw_Alert`, `DAD..tbl_Server`. |
| SpaceMan | `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql:228-233` | Reads `SpaceMan..tbl_Allocation`, `SpaceMan..tbl_SAN` for SANMan drop capacity logic. |
| SanMan | `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql:237-245` | Reads `SanMan..tbl_VDisk`, `SanMan..vw_CreatedVDisk`, `SanMan..tbl_San`. |
| DropManagement | `LaunchPad/src/deploy/sql/000008/up_GetDropMachineSelection.proc.sql:242-243` | Joins `DropManagement..tbl_Drop` while calculating SANMan usage. |
| Techease | `LaunchPad/src/deploy/sql/000009/up_UpdateDropMachineState.sql:62-64` | Reads `Techease..vw_Ticket`, `Techease..tbl_TicketMachine`. |
| Utils | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj:2090-2100` | Declared as SQLCMD variable `UtilsDB` in DB project. |
| DAD | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj:2090-2094` | Declared as SQLCMD variable `DADDB` in DB project. |
| LabStatus | `LaunchPad/src/LaunchPad.Database/LaunchPad.Database.sqlproj:2095-2099` | Declared as SQLCMD variable `LabStatusDB` in DB project. |
| DAD | `LaunchPad/src/LabSchedule.Database/LabSchedule.Database.sqlproj:538-541` | External artifact reference `DAD.dacpac`. |

### Core direct-access implementation files

| File | DB | What it does |
| --- | --- | --- |
| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` | LaunchPad | Central stored procedure wrapper. Uses `ExecuteDataSet`, `ExecuteStoredProcedure`, `ExecuteScalar` across LaunchPad operations. |
| `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs` | LabStatus | Calls LabStatus stored procedure `up_CreateBuildIDs` to allocate build IDs. |

### Notes

- From C# perspective, the only clearly direct databases are `LaunchPad` and `LabStatus`.
- `DAD`, `SpaceMan`, `SanMan`, `DropManagement`, `Techease`, and `Utils` are visible mainly through SQL scripts, SQL projects, and cross-database stored procedure logic.
- If needed, next pass can expand `LaunchPadDB.cs` into a per-stored-procedure table grouped by functional area.

### LaunchPadDB stored procedure map by functional area

| Functional area | Representative stored procedures / external calls | Primary file |
| --- | --- | --- |
| Org and Lab lookup | `up_OrgGet`, `up_LabGet`, `up_LabGetByName`, `up_LabGetAll`, `up_LabGetLPEnabled`, `up_LabGetByPool`, `up_LabGetAllByOrg`, `up_LabGetRequests`, `up_LabGetRecentRevision`, `up_LabGenerateRevision` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Lab definition management | `up_LabDefinitionGet`, `up_LabDefinitionGetByFilters`, `up_LabDefinitionUpdate`, `up_LabDefinitionRemove`, `up_LabDefinitionGetLog` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Definitions and steps | `up_DefinitionGet`, `up_DefinitionGetByName`, `up_DefinitionGetBuildStatusWorkflow`, `up_DefinitionGetByLab`, `up_DefinitionGetByOrg`, `up_DefinitionStepGet`, `up_DefinitionStepGetByDefinition`, `up_DefinitionStepGetNext`, `up_DefinitionStepGetPrevious` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Properties and extended properties | `up_PropertyGetByDefinition`, `up_PropertyGetByDefinitionStep`, `up_PropertyGetCustomByDefinition`, `up_RequestPropertySet`, `up_ExtendedPropertySet` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Build and request-build flow | `up_BuildGetByDefinitionStep`, `up_RequestBuildClear`, `up_RequestBuildAdd`, `up_RequestBuildSetStatus`, `up_RequestBuildSetSessionID`, `up_RequestBuildRemove`, `up_RelaunchBuild` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Request lifecycle | `up_RequestGetSchema`, `up_RequestGet`, `up_RequestGetByRevisionAndLab`, `up_RequestGetByPool`, `up_RequestGetForLaunch`, `up_RequestGetTemplates`, `up_RequestSet`, `up_RequestCancel`, `up_RequestAbort`, `up_RequestSetStatus`, `up_RequestSetOwners`, `up_RequestSetIsTestBuild`, `up_RequestSetLaunchDate`, `up_RequestSetRevision`, `up_RequestClone`, `up_RequestCreateDraft`, `up_RequestGetByTargetDate`, `up_RequestGetByValidation`, `up_RequestGetPrevious`, `up_RequestGetLaunchInfo`, `up_RequestGetValidationRequestQueue` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Comments, logs, auditing | `up_RequestCommentAdd`, `up_TemplateCommentAdd`, `up_AddLog`, `up_RequestAddLaunchLog`, `up_GetLog` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Pre-launch configuration | `up_PreLaunchGetAll`, `up_PreLaunchGetByLabDefinition`, `up_PreLaunchAdd`, `up_PreLaunchRemove` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Resources and assignments | `up_ResourceRequirementGetByName`, `up_ResourceRequirementGetByBuild`, `up_ResourceActivate`, `up_ResourceDeactivate`, `up_ResourceUsageSet`, `up_ResourceUsageClear`, `up_ResourceGetAll`, `up_ResourceGetWithSessionId`, `up_ResourceGetByName`, `up_ResourceGetRankedByPool`, `up_ResourceGetByPool`, `up_ResourceGetByCategory`, `up_ServerAssignmentsGet`, `up_ServerAssignmentsAdd`, `up_ServerAssignmentsRemove`, `up_PoolAssignmentsUpdate` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Pools and SAN selection | `up_PoolAdd`, `up_PoolUpdate`, `up_PoolRemove`, `up_PoolGet`, `up_PoolGetByType`, `up_GetSanSelection`, `up_GetSansByPool`, `up_SetSansForPool` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Machines and scheduling support | `up_GetPartialMachineReleaseInfo`, `up_UpdateBuildMachineState`, `up_UpdateDropMachineState`, `up_GetBuildMachineSelection`, `up_GetDropMachineSelection`, `up_GetNextRevision` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Reporting and utilization | `up_ReportingGetServerUsage`, `up_UtilizationGetOverallSummary`, `up_UtilizationGetBuildMachineHistory`, `up_UtilizationGetPoolHistory`, `up_UtilizationGetClassHistory` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Whidbey / legacy support | `up_GetRunBVTFlag`, `up_GetWhidbeyLabs`, `up_GetCurrentWhidbeyBuilds` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| External database call from LaunchPadDB | `DAD..up_ServerGet` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Template management | `up_TemplateLoadById`, `up_TemplateLoadByRequestId`, `up_TemplateLoadAll`, `up_TemplateLoadByDefinition`, `up_TemplateAdd`, `up_TemplateDelete`, `up_TemplateAddDefinition`, `up_TemplateUpdate`, `up_TemplateSetSchedule` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |
| Toolset lookup | `up_ToolsetGet` | `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs` |

### Key direct call sites worth starting from

| Start point | Why it matters |
| --- | --- |
| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:12-14` | Root database wrapper constructor for the `LaunchPad` connection string. |
| `LaunchPad/src/LaunchPad.Service/Components/RequestProcessor.cs:302-313` | Only clearly direct LabStatus call in C# found so far. |
| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:1753` | Explicit cross-database stored procedure call into `DAD..up_ServerGet`. |
| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:1774` | Drop machine selection entry point, often useful when tracing LaunchPad-SAN/DAD dependencies. |
| `LaunchPad/src/LaunchPad.Components/LaunchPadDB.cs:1356-1361` | State update entry points that connect LaunchPad logic with cross-database SQL in deploy scripts. |

## BuildStatus database map

### C# direct access

| Database | Connection/config evidence | Direct C# entry points | Notes |
| --- | --- | --- | --- |
| LabStatus | `BuildStatus/src/BuildStatus/Web.Config:124-134`<br>`BuildStatus/src/BuildStatusRestAPI/Web.config:45-49`<br>`BuildStatus/src/BuildStatus.Tests/App.config:3-7`<br>`BuildStatus/src/BuildStatus.Components/app.config:5-7`<br>`BuildStatus/src/LabStatus.DataBase.Test/App.config:3-5` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:14-23` | Main application database. `BuildStatusDB` initializes `m_Db` with `BuildStatusConnectionString`. |
| BFD | `BuildStatus/src/BuildStatus/Web.Config:124-134`<br>`BuildStatus/src/BuildStatus.Tests/App.config:3-7` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:1913-1925` | Secondary direct DB access. `GetBuildBFD` opens a dedicated `Database` using `BFDConnectionString`. |
| devdiv_General | `BuildStatus/src/BuildStatus/Web.Config:32` | `BuildStatus/src/BuildStatus/Default.aspx:6`<br>`BuildStatus/src/BuildStatus/Default.aspx:92-94`<br>`BuildStatus/src/BuildStatus/Default.aspx.designer.cs:31` | Configured through `FlashNewsDBconnectionString`. Current evidence shows it is consumed indirectly by the external `FlashNewsControl2` control rather than by a local `BuildStatusDB`/`SqlConnection` wrapper. |
| Addax_whby | `BuildStatus/src/DDRelQA.DataLayer/app.config:5-8` | `BuildStatus/src/DDRelQA.DataLayer/AddaxManager.cs`<br>`BuildStatus/src/DDRelQA.DataLayer/*.designer.cs` | Separate LINQ-to-SQL style data layer used by DDRelQA components. |

### BuildStatus-related C# files using `BuildStatusDB`

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

### BuildStatus SQL/external database dependencies

| Database | Evidence file | How it is used |
| --- | --- | --- |
| LabStatus | `BuildStatus/src/BuildStatus/Web.Config:124-134` | Main application connection strings for dev/staging/prod. |
| LabStatus | `BuildStatus/src/BuildStatusRestAPI/Web.config:45-49` | REST API connection strings for dev/staging/prod. |
| LabStatus | `BuildStatus/src/LabStatus.Database/LabStatus.Database.dbproj:33-47` | Database project for the main `LabStatus` schema/build output. |
| LabStatus | `BuildStatus/Deploy/sql/Dev_ContentDeploy_Dev.xml:1-4` | SQL patch manifest deploys to database `LabStatus`. |
| BFD | `BuildStatus/src/BuildStatus/Web.Config:125-132` | Configured as `BFDConnectionString`; used by `GetBuildBFD`. |
| CDBurn2 | `BuildStatus/src/BuildStatus/Web.Config:14`<br>`BuildStatus/src/BuildStatus/Web.Config:127-133` | Configured as production/dev/staging CDBurn DB integration. |
| devdiv_General | `BuildStatus/src/BuildStatus/Web.Config:32`<br>`BuildStatus/src/BuildStatus/Default.aspx:6`<br>`BuildStatus/src/BuildStatus/Default.aspx:92-94` | `FlashNewsDBconnectionString` points to `devdiv_General`, and the page hosts the external `FlashNewsControl2` control that appears to consume it. |
| Addax_whby | `BuildStatus/src/DDRelQA.DataLayer/app.config:5-8` | Dedicated DDRelQA database used by the separate data layer. |
| LaunchPad | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:332-338` | `GetRequestDataforBuilds` calls `up_GetLaunchPadRequestsForSessions`, indicating LaunchPad-related request linkage exposed through LabStatus DB stored procedures. |

### Core direct-access implementation files

| File | DB | What it does |
| --- | --- | --- |
| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` | LabStatus | Central stored procedure wrapper. Uses `ExecuteDataSet`, `ExecuteStoredProcedure`, and `ExecuteScalar` for core BuildStatus operations. |
| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:1913-1925` | BFD | Dedicated helper method `GetBuildBFD` that opens the BFD database directly. |
| `BuildStatus/src/DDRelQA.DataLayer/AddaxManager.cs` | Addax_whby | DDRelQA data access surface backed by the `Addax_whby` connection. |

### BuildStatusDB stored procedure map by functional area

| Functional area | Representative stored procedures / external calls | Primary file |
| --- | --- | --- |
| Lab, org, milestone lookup | `sp_GetLabsAndMilestonesForStatusPageByOrg`, `up_GetLabsAll`, `up_GetLabsByOrg`, `up_GetOrg`, `up_GetOrgs`, `up_GetMilestonesByOrg` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Sessions and search | `up_GetSessions`, `up_GetSessionLog`, `up_GetSimilarBuilds`, `up_GetSessionsByDateRange`, `up_GetSessionsByRevisionRange`, `up_GetRecentSessions`, `up_GetSessionsByLabAndDate`, `up_GetSessionsByStartedDate`, `up_GetSessionsByFinishedDate`, `up_GetSessionsByBuildUrl` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Session update and lifecycle | `up_UpdateSession`, `up_UpdateSessionStatus`, `up_UpdateSessionTestBuild`, `up_UpdateSessionCanRecycle`, `up_DeleteSession`, `up_GetSessionChangeLog` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Session logs and process data | `up_GetSessionUnprocessedLogCount`, `up_UpdateSessionErrorsAsProcessed`, `up_UpdateSessionStallWarningsAsProcessed`, `up_GetSessionSetups`, `up_GetSessionGroups`, `up_GetSessionGroup`, `up_GetSessionProcessNames`, `up_GetProcessLogs` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Build history and related data | `up_GetRealSignedBuilds`, `up_GetOtherBuildsByLabBySession`, `up_GetBuildHistory`, `up_GetLaunchPadRequestsForSessions`, `sp_GetBuildBFD` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Issues | `up_GetIssue`, `up_GetIssues`, `up_GetIssuesByBuildID`, `up_AddIssue`, `up_UpdateIssue`, `up_UpdateIssueSourceAndSubSource`, `up_CloseIssue`, `up_DeleteIssue`, `up_SearchIssues`, `up_AssignIssueToSession`, `up_DeleteIssueFromSession` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Issue logs and ownership | `up_AddIssueLog`, `up_UpdateIssueCausedBy`, `up_UpdateIssueTriage`, `up_UpdateIssuePendingOwnerAsAssignedTo`, `up_ReAssignIssue`, `up_ReAssignIssueToPending`, `up_AddNewCostCenter`, `up_CostCenterExists` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Fix workflow | `up_GetFix`, `up_GetFixLogs`, `up_AddFixForDefault`, `up_AddFixForSourceCode`, `up_AddFixForSetup`, `up_StartFix`, `up_CompleteFix`, `up_AddFixLog`, `up_UpdateManualFixStatus` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Fix synchronization | `up_AddFixSynchronization`, `up_UpdateFixSynchronization`, `up_UpdateFixSynchronizationMachineStatus`, `up_GetFixSynchronization`, `up_GetLastFixIdByIssueId` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Preservation requests | `up_AddPreservationRequest`, `up_DeletePreservationRequest`, `up_GetPreservationRequestsByBuild`, `up_GetAllActivePreservationRequests` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |
| Session groups | `up_EnsureSessionGroup`, `up_DeleteSessionGroup`, `up_AddSessionGroupDependency`, `up_UpdateSessionGroup`, `up_UpdateSessionGroupProcess`, `up_UpdateSessionGroupExpirationInfo` | `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs` |

### Key direct call sites worth starting from

| Start point | Why it matters |
| --- | --- |
| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:14-23` | Root database wrapper constructor for the `BuildStatusConnectionString` / `LabStatus` database. |
| `BuildStatus/src/BuildStatus.Components/BuildStatusDB.cs:1913-1925` | Only clearly direct BFD database call found in the main wrapper. |
| `BuildStatus/src/BuildStatus.Components/Build.cs` | Central model for build/session reads, request linkage, and session update flows. |
| `BuildStatus/src/BuildStatus.Components/Issue.cs` | Main issue lifecycle entry point: update, assign, close, logs, notifications. |
| `BuildStatus/src/BuildStatus.Components/Fix.cs` | Main fix scheduling/completion/synchronization entry point. |

### Notes

- From C# perspective, the primary direct databases are `LabStatus`, `BFD`, and `Addax_whby`.
- `CDBurn2` and `devdiv_General` are currently visible as application integrations through config and external components rather than through a local `BuildStatusDB`-style wrapper class.
- `BuildStatusDB` is the central access layer for most of the application, while `DDRelQA.DataLayer` is a separate data-access surface backed by `Addax_whby`.
