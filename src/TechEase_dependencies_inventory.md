# TechEase Dependencies Inventory

## Scope

This file summarizes the stored procedures, related SQL dependencies, and application-layer callers in LaunchPad and BuildStatus that still depend on the TechEase database.

## Direct TechEase DB Access

### LaunchPad

1. `dbo.up_UpdateBuildMachineState`
	- File: `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_UpdateBuildMachineState.proc.sql`
	- Direct references:
	  - `Techease..vw_Ticket`
	  - `Techease..tbl_TicketMachine`
	- Purpose:
	  - Builds machine invalid-state fragments for open tickets and folds them into `tbl_ResourceState` / `tbl_ResourceStateHistory`.

2. `dbo.up_UpdateDropMachineState`
	- File: `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_UpdateDropMachineState.proc.sql`
	- Direct references:
	  - `Techease..vw_Ticket`
	  - `Techease..tbl_TicketMachine`
	- Purpose:
	  - Marks drop servers as unusable when they have TechEase tickets or other invalid conditions.

3. `dbo.up_ResourceGetRankedByPool`
	- File: `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_ResourceGetRankedByPool.proc.sql`
	- Direct references:
	  - `Techease..tbl_Ticket`
	  - `Techease..tbl_TicketMachine`
	- Purpose:
	  - Computes `HasTecheaseTicket` for machines in a pool and feeds the ranking function.

### BuildStatus

1. `dbo.up_GetIssues`
	- File: `q:\dd\BuildStatus\src\LabStatus.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_GetIssues.proc.sql`
	- Direct references:
	  - `[Techease].[dbo].[tbl_Ticket]`
	  - `[Techease].[dbo].[tbl_Org]`
	- Purpose:
	  - Returns ticket rows associated with issues and enriches them with TechEase org name.

2. `dbo.sp_AddIssueTicket`
	- File: `q:\dd\BuildStatus\src\LabStatus.Database\dbo\Stored Procedures\sp_AddIssueTicket.sql`
	- Direct references:
	  - `[Techease].[dbo].[tbl_Ticket]`
	- Purpose:
	  - Validates TechEase ticket existence when `@TicketSystemId = 1` before inserting into `tbl_IssueTicket`.

## LaunchPad Dependency Chain

### Build machine state path

1. `RequestWizard_BuildResources.aspx.cs`
	- Calls `Resource.UpdateBuildMachineState()`
2. `LaunchPad.Components.Resource`
	- `UpdateBuildMachineState()` calls `LaunchPadDB.UpdateBuildMachineState()`
3. `LaunchPad.Components.LaunchPadDB`
	- `UpdateBuildMachineState()` executes `up_UpdateBuildMachineState`
4. `up_UpdateBuildMachineState`
	- Reads TechEase ticket data
	- Writes merged machine state into `tbl_ResourceState` and `tbl_ResourceStateHistory`

### Drop machine state path

1. `RequestWizard_Resources.aspx.cs`
	- Calls `Resource.UpdateDropMachineState()`
2. `LaunchPad.Components.Resource`
	- `UpdateDropMachineState()` calls `LaunchPadDB.UpdateDropMachineState()`
3. `LaunchPad.Components.LaunchPadDB`
	- `UpdateDropMachineState()` executes `up_UpdateDropMachineState`
4. `up_UpdateDropMachineState`
	- Reads TechEase ticket data
	- Produces drop-server invalid state

### Resource ranking path

1. `LaunchPad.Components.Resource`
	- `GetRankedResourcesByPool()` requests ranked resources from DB
2. `LaunchPad.Components.LaunchPadDB`
	- `GetRankedResourcesByPool()` executes `up_ResourceGetRankedByPool`
3. `up_ResourceGetRankedByPool`
	- Reads TechEase ticket data into `#TechaseTickets`
	- Passes `HasTecheaseTicket` into `dbo.fn_GetResourceRank_TEST`
4. `dbo.fn_GetResourceRank_TEST`
	- Does not query TechEase directly
	- Depends on the `HasHighPriTecheaseTicket` input flag produced upstream

## BuildStatus Dependency Chain

### Issue query path

1. `BuildStatus.Components.BuildStatusDB`
	- `GetIssue()` executes `up_GetIssues`
	- `GetIssuesByBuildID()` executes `up_GetIssuesBySession`
	- `GetIssuesByBuildIDs()` executes `up_GetIssuesBySessions`
	- `GetIssuesBySource()` executes `up_GetIssuesBySource`
	- `GetIssuesByUser()` executes `up_GetIssuesByUser`
	- `GetIssuesByLabAndTeam()` executes `up_GetIssuesByLabAndTeam`
2. Wrapper stored procedures
	- `up_GetIssuesBySession`
	- `up_GetIssuesBySessions`
	- `up_GetIssuesBySource`
	- `up_GetIssuesByUser`
	- `up_GetIssuesByLabAndTeam`
	- `up_SearchIssues`
	- `up_SearchIssuesForOfficialBuilds`
	- All eventually `EXEC up_GetIssues ...`
3. `up_GetIssues`
	- Reads TechEase ticket and org data
	- Returns one TechEase-related result set
4. `BuildStatus.Components.BuildStatusDB.SetIssueTablesRelations()`
	- Names that result table `Techease`
	- Creates relation `IssueTechease`
5. Consumers
	- `BuildStatus.Components.Issue`
	- `BuildStatus.Issues.Issues.aspx.cs`
	- `BuildStatusRestAPI`

### Ticket assignment path

1. `BuildStatus\Issues\IssueTicket.aspx.cs`
	- Calls `Issue.AssignTicketToIssue(...)`
2. `BuildStatus.Components.Issue`
	- Calls `BuildStatusDB.AssignTicketToIssue(...)`
3. `BuildStatus.Components.BuildStatusDB`
	- Executes `sp_AddIssueTicket`
4. `sp_AddIssueTicket`
	- If `@TicketSystemId = 1`, validates ticket existence in TechEase DB
	- Then inserts local association row into `tbl_IssueTicket`

## Related Non-DB TechEase Dependencies

These do not directly query the TechEase database, but they are still tightly coupled to TechEase behavior or naming.

### LaunchPad

1. Page header template XML files under multiple projects still contain a TechEase entry and URL.
2. `LaunchPad\XSL\MachineState.xsl` and `LaunchPad\XSL\MachineStateMinimal.xslt` still generate `http://techease/...` links.
3. `LaunchPad.Components\Web References\ddweb\Reference.cs` and `DADService.wsdl` contain TechEase ticket service types and `AssociateTecheaseTicket` operations.

### BuildStatus

1. `BuildStatus\Common.cs` still auto-links `techease #123` patterns to `http://techease/...`.
2. `BuildStatus\IssueOverview.cs` still treats `TicketSystemId == 1` as TechEase and uses `TecheaseOrg`.
3. `BuildStatus.Components\Ticket.cs` still exposes `TecheaseOrg`.
4. `BuildStatus.Components\Constants.cs` and `BuildStatus.Components\Issue.cs` still use the relation name `IssueTechease`.
5. `BuildStatus\Issues\IssueTicket.aspx*` still uses class/form names containing `IssueTechease` and still shows a TechEase-specific error string.

## Historical / Deployment Script References

There are also historical deployment scripts that still include TechEase references. These are not necessarily active at runtime, but they matter if the goal is full repository cleanup.

### LaunchPad

1. `q:\dd\LaunchPad\src\deploy\sql\000009\up_UpdateDropMachineState.sql`

### BuildStatus

1. Older `up_GetIssues` deploy scripts under:
	- `Deploy\sql\000019`
	- `Deploy\sql\000057`
	- `Deploy\sql\000071`
	- `Deploy\sql\000086`
	- `Deploy\sql\000106`
2. Legacy rename / migration scripts for:
	- `tbl_IssueTechease`
	- `sp_AddIssueTechease`
	- `sp_DeleteIssueTechease`
3. Redirect configuration:
	- `Deploy\sql\000107\001_ChangeRedirectionURL_Techease.sql`

## Summary

### Direct TechEase DB callers

1. LaunchPad: `up_UpdateBuildMachineState`
2. LaunchPad: `up_UpdateDropMachineState`
3. LaunchPad: `up_ResourceGetRankedByPool`
4. BuildStatus: `up_GetIssues`
5. BuildStatus: `sp_AddIssueTicket`

### Important note

No local SQL view definition was found in the two database projects that directly references TechEase. The current direct DB dependencies are concentrated in stored procedures.
