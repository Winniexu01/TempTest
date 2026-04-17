# LaunchPad 从零开始到 Create Request，并包含 ManageServerPools 的完整步骤

## 结论先说

`ManageServerPools.aspx` 不是“创建请求之后”的步骤。

它在整条业务链里的位置是：

1. 先有机器记录
2. 再把机器分配进 Build Pool / Drop Pool
3. 再把 Pool 绑定到 LabDefinition
4. 然后用户才能创建请求
5. 后续才是 Launch 和真正 Run

也就是说，`ManageServerPools.aspx` 属于“创建请求之前的资源准备页面”。

---

## 一、从什么都没有开始的完整业务链

完整顺序如下：

1. 新增服务器记录
2. 创建资源池
3. 进入 `ManageServerPools.aspx`，把服务器分配到 Pool
4. 进入 `PoolDefinitionMappings.aspx`，把 Pool 绑定到 Lab + Definition
5. 用户从首页发起 `Create Request`
6. 创建 Draft Request
7. 进入 Request Wizard，补全请求数据
8. 保存为正式 Request
9. 用户发起 Launch，或者 Auto Launch 服务预处理
10. 后台服务选机器、生成 BuildId、真正执行 Run

下面按代码顺序展开。

---

## 二、创建机器记录

### 2.1 入口页面

入口是 `AddServer.aspx` 或 `EditServer.aspx`。

相关文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\AddServer.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\EditServer.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\ServerProperties.ascx.cs`

### 2.2 真正保存逻辑

真正保存不在 `AddServer.aspx.cs` / `EditServer.aspx.cs` 里，而在：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ServerProperties.ascx.cs`

这里分两种情况：

1. 新增机器：`DADCommunicator.BulkAddServers(...)`
2. 批量更新机器：`DADCommunicator.BulkUpdateServer(...)`

对应外部调用封装在：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\DAD\DADCommunicator.cs`

这一步的结果是：

- 机器被登记到 DAD
- 但它还没有属于任何 LaunchPad Pool
- 所以还不能被请求流程使用

---

## 三、创建 Pool

### 3.1 入口页面

Pool 管理入口在：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\AddPool.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\EditPool.aspx.cs`

### 3.2 创建 / 编辑 / 删除 Pool

代码入口：

- `Pool.Add(...)`
- `Pool.Update(...)`
- `Pool.Delete(...)`

封装位置：

- `q:\dd\LaunchPad\src\LaunchPad.Components\Pool.cs`

数据库调用位置：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`

对应存储过程：

- `up_PoolAdd`
- `up_PoolUpdate`
- `up_PoolRemove`

做到这里，LaunchPad 有了逻辑上的 Build Pool / Drop Pool，但 pool 里还没有机器。

---

## 四、进入 ManageServerPools.aspx

### 4.1 它是从哪里进去的

`ManageServerPools.aspx` 是资源管理页，不属于 Request Wizard。

常见入口：

1. Admin 首页
2. Definitions 页
3. PoolDefinitionMappings 页
4. PoolDetails / ServerDetails 里的链接

页面文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx.cs`

### 4.2 页面首次加载顺序

`Page_Load()` 执行顺序：

1. 设置 subtitle
2. 读取排序参数 `sortexp` / `sortdir`
3. 如果首次加载，调用 `InitializeControls()`
4. `InitializeControls()` 最后调用 `BindGrid()`

### 4.3 BindGrid 的核心作用

`BindGrid()` 做的事：

1. 读取并应用筛选条件
2. 调 `Resource.LoadByFilters(...)`
3. 底层执行 `up_ServerAssignmentsGet`
4. 取回服务器、状态、pool 关系
5. 把一台 server 对多个 pool 的关系“拍平”成 grid 行
6. 绑定到 `grdServerAssignments`

相关文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\Resource.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_ServerAssignmentsGet.proc.sql`

这一步只是“看当前分配情况”，还没有修改数据。

---

## 五、在 ManageServerPools.aspx 里把机器分配进 Pool

### 5.1 前端入口

页面左侧 `Server Tasks` 里有：

- `Assign Servers To Pool`
- `Remove Servers From Pool`
- `Make Active/Inactive`
- `Toggle Reserve/Unreserve`

这些按钮都定义在：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx`

### 5.2 Assign Servers To Pool

前端顺序：

1. 用户勾选一批 server
2. JS `assignServers()` 执行
3. 打开 `ChangeServerAssignments.aspx?servers=...`

后台顺序：

1. `ChangeServerAssignments.aspx.cs` 读取选中的 server
2. 用户选择目标 Build Pool 或 Drop Pool
3. 点击 Update
4. 调用 `Pool.AddServersToPool(...)`
5. 再到 `LaunchPadDB.AddResourceToPool(...)`
6. 最终执行 `up_ServerAssignmentsAdd`

相关文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ChangeServerAssignments.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\Pool.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_ServerAssignmentsAdd.proc.sql`

这一步完成后，机器正式属于某个 Build Pool 或 Drop Pool。

### 5.3 Remove Servers From Pool

顺序：

1. JS `unassignServers()`
2. 打开 `UnassignServer.aspx?servers=...`
3. 提交后调用 `Pool.RemoveServersFromPools(...)`
4. 再到 `LaunchPadDB.RemoveResourcesFromPools(...)`
5. 最终执行 `up_ServerAssignmentsRemove`

相关文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\UnassignServer.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\Pool.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_ServerAssignmentsRemove.proc.sql`

### 5.4 Make Active / Inactive

顺序：

1. JS `changeState()`
2. 打开 `ChangeServerState.aspx?servers=...`
3. 提交后调用 `Resource.Activate(...)` 或 `Resource.Deactivate(...)`
4. 再到 `LaunchPadDB.ActivateResource(...)` / `DeactivateResource(...)`
5. 最终执行 `up_ResourceActivate` / `up_ResourceDeactivate`

相关文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ChangeServerState.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\Resource.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`

### 5.5 Toggle Reserve / Unreserve

顺序：

1. JS `changeReserve()`
2. 打开 `ChangeReserve.aspx?servers=...`
3. 提交后调用 BuildManager Web Service
4. 执行 `SetBuildAgentsReservedStatus`

相关文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ChangeReserve.aspx.cs`

注意：

- 这一步不是改 LaunchPad 数据库里的 pool 归属
- 它是改 BuildManager 里的保留状态

### 5.6 Edit / Rename / Delete Server Record

这几个也都挂在 `ManageServerPools.aspx` 左侧：

1. Edit Server Records
2. Delete Server Records
3. Rename Server Record

实际走向：

- Edit -> `EditServer.aspx` -> `ServerProperties.ascx.cs` -> `DADCommunicator.BulkUpdateServer(...)`
- Rename -> `RenameServer.aspx.cs` -> `DADCommunicator.RenameServer(...)`
- Delete -> `RemoveServer.aspx.cs` -> `DADCommunicator.RemoveServer(...)`

这些动作是在维护 DAD 里的机器记录。

---

## 六、Pool 还必须绑定到 LabDefinition

即使机器已经在 pool 里了，也还不能直接创建可运行请求。

还必须做这一步：

- 把 Build Pool / Drop Pool 绑定到具体的 Lab + Definition 组合

### 6.1 入口页面

页面：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\PoolDefinitionMappings.aspx`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\PoolDefinitionMappings.aspx.cs`

### 6.2 修改绑定关系

前端顺序：

1. 勾选一个或多个 LabDefinition
2. 点击 `Change Pool Assignment`
3. 打开 `ChangePoolAssignments.aspx?labdefinitions=...`

后台顺序：

1. `ChangePoolAssignments.aspx.cs` 读取 LabDefinition 列表
2. 用户选择新的 Build Pool / Drop Pool
3. 点击 Update
4. 调 `LabDefinition.UpdatePoolAssignments(...)`
5. 再到 `LaunchPadDB.UpdateLabDefinitionPoolAssignments(...)`
6. 最终执行 `up_PoolAssignmentsUpdate`

相关文件：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ChangePoolAssignments.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\LabDefinition.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`

### 6.3 这一步为什么重要

Request 在 launch 阶段会直接读取：

- `request.Lab.BuildPool`
- `request.Lab.DropPool`

代码在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LabDefinition.cs`

如果这里没有配好，后面的 launch 预检查会直接失败。

---

## 七、现在才轮到 Create Request

### 7.1 首页入口

前端入口函数：

- `openCreateRequestDialog(...)`

调用点：

- `q:\dd\LaunchPad\src\LaunchPad\Default.aspx`
- `q:\dd\LaunchPad\src\LaunchPad\Scripts\LaunchPad.js`

它会打开：

- `CreateRequestStep1.aspx`

### 7.2 CreateRequestStep1

后台文件：

- `q:\dd\LaunchPad\src\LaunchPad\CreateRequestStep1.aspx.cs`

首次加载顺序：

1. `LoadLabs()`
2. `LoadSettingsFromQueryString()`
3. `LoadLabDefinitions()`

这里会：

1. 读取 LP-enabled labs
2. 按 lab 加载 definitions
3. 生成 revision
4. 校验日期和定义是否可请求

用户点击 Next 后：

1. 调 `Request.CreateDraft(definitionId, labId, targetDate, revision)`
2. 返回新的 draft request
3. 跳到 `RequestWizard_Start.aspx?rid=...&Seq=Request`

---

## 八、进入 Request Wizard

### 8.1 起点

页面：

- `q:\dd\LaunchPad\src\LaunchPad\RequestWizard_Start.aspx.cs`

它本身不显示内容，只负责根据 request 和 sequence 跳到正确的第一页。

### 8.2 Request Wizard 作用

核心类：

- `q:\dd\LaunchPad\src\LaunchPad\RequestWizard.cs`

它会按 DefinitionStep 动态组织页面：

- LaunchTime
- Properties
- Builds
- BuildResources
- Resources
- Comments
- Summary
- 其他定义驱动的步骤

用户一路填写，最后 Finish 时保存请求。

### 8.3 最终保存

保存路径是：

1. `Request.Save(RequestSequence.Request)`
2. 底层调用 `up_RequestSet`

保存后，请求进入正式 Request 状态，而不是仅仅 Draft。

---

## 九、为什么 Create Request 之后还不能立刻 Run

因为 Create Request 只完成了“请求对象存在”。

真正 Run 还需要：

1. 发起 Launch
2. 进入 Launch 的预处理页
3. 检查 BuildPool / DropPool 是否存在
4. 选择或验证资源
5. 把 request 置成 `Ready To Launch`
6. 后台服务去真正执行

如果前面没有做过 `ManageServerPools.aspx` 和 `PoolDefinitionMappings.aspx`，这一段通常会失败。

---

## 十、Launch 和 Run 的后半段

### 10.1 手动 Launch

入口：

- `LaunchRequestStep1.aspx`
- `LaunchPad\KJax.aspx.cs`

这里会做：

1. Template materialize
2. 属性校验
3. 检查 machine pool
4. 检查 drop space
5. 进入 Launch Wizard
6. 保存为 `RequestSequence.Launch`
7. Request 状态变成 `Ready To Launch`

### 10.2 Auto Launch

服务：

- `q:\dd\LaunchPad\src\LaunchPad\Services\AutoLaunchServices.asmx.cs`

它会：

1. 找到 ready for auto-launch 的请求
2. 自动选 drop server
3. 自动选 build machines
4. 把资源写回 request
5. 调 `req.Save(RequestSequence.Launch)`

### 10.3 真正 Run

后台服务：

- `q:\dd\LaunchPad\src\LaunchPad.Service\Components\RequestProcessor.cs`

顺序：

1. `LoadReadyToLaunch(...)`
2. 把 request 状态设为 `Launching`
3. 检查机器 RPC
4. 调 `up_CreateBuildIDs` 生成 LabStatus BuildId
5. 执行 pre-launch assemblies
6. 对目标 build machine 通过 WMI 发 launch script
7. 成功后把 build / request 状态设为 `Launched`

---

## 十一、把整条链压成一句话

从零开始的最短主链是：

1. Add Server -> 把机器登记到 DAD
2. Add Pool -> 创建 Build Pool / Drop Pool
3. ManageServerPools.aspx -> 把机器分配到 Pool
4. PoolDefinitionMappings.aspx -> 把 Pool 绑定到 LabDefinition
5. openCreateRequestDialog -> CreateRequestStep1 -> RequestWizard
6. 保存为正式 Request
7. LaunchRequestStep1 / AutoLaunchServices -> 置为 Ready To Launch
8. RequestProcessor -> 生成 BuildId -> 在目标机器上真正启动 Run

---

## 十二、ManageServerPools.aspx 在全流程中的准确位置

它的位置是：

`Add Server / Edit Server`
-> `ManageServerPools.aspx`
-> `PoolDefinitionMappings.aspx`
-> `Create Request`
-> `Launch`
-> `Run`

不是：

`Create Request`
-> `ManageServerPools.aspx`

所以如果问题是“创建请求到 ManageServerPools.aspx 的步骤”，准确说法应该是：

- `ManageServerPools.aspx` 是创建请求前的资源准备步骤
- 没有它，后续 Request 虽然能创建，但大概率不能顺利 Launch / Run

---

## 十三、一个机器被指给某个 Run 的过程

这一段回答的是：

- 一台机器怎么从“只是存在于 LaunchPad / DAD”
- 变成“被某一个具体请求 / 具体 build 使用”
- 最后真正执行 run

### 13.1 机器先进入可选范围

机器不会直接被某个 run 使用，它必须先满足以下前置条件：

1. 机器已经作为 server record 存在于 DAD
2. 机器被分配到某个 Build Pool
3. 该 Build Pool 被绑定到某个 LabDefinition
4. 请求选择的 Lab + Definition 正好对应这个 LabDefinition

这样，请求代码才能通过：

- `m_Request.Lab.BuildPool`

拿到候选机器集合。

相关代码：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LabDefinition.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\PoolDefinitionMappings.aspx`

### 13.2 手动分配机器给请求

手动 Launch 时，用户会在 Build Resources 页面看到可选机器。

页面：

- `q:\dd\LaunchPad\src\LaunchPad\RequestWizard_BuildResources.aspx.cs`

执行顺序：

1. 页面加载时读取当前请求的 Build Pool 服务器列表：
	- `m_Request.Lab.BuildPool.Servers`
2. 然后调用：
	- `m_Request.GetBuildMachineSelection()`
3. 这一步会取 build machine selection algorithm 的推荐结果
4. 页面将推荐的 `ServerID` 预选到每个 build 的下拉框
5. 用户可以保留推荐结果，也可以手动改成别的机器
6. 点击 Next / Save 后，页面会为每个 build 创建一条 `ResourceUsage`
7. 然后对每条 `ResourceUsage` 调用 `Save()`

关键代码位置：

- `q:\dd\LaunchPad\src\LaunchPad\RequestWizard_BuildResources.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\ResourceUsage.cs`
- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`

### 13.3 自动分配机器给请求

Auto Launch 不需要用户手动选机器。

服务入口：

- `q:\dd\LaunchPad\src\LaunchPad\Services\AutoLaunchServices.asmx.cs`

执行顺序：

1. `PrepareAutoLaunchRequestsForLaunching()` 找到待自动 launch 的请求
2. 调用：
	- `Request.GetBuildMachineSelection(targetDate)`
3. 从 selection 结果里筛出属于当前 request 的机器
4. 在 `AssignDropAndBuildResourcesToRequest(...)` 中，为每个 build 创建 `ResourceUsage`
5. 调用 `buildUsage.Save()` 写库

这说明：

- 自动分配和手动分配，最终都落成同一种数据结构
- 都是把机器保存成 request/build 下的一条 `ResourceUsage`

### 13.4 ResourceUsage 是真正的“绑定关系”

机器被指给某个 run，真正的关键不是 pool 本身，而是：

- request 下某个 build requirement
- 保存了一条指向某台机器的 `ResourceUsage`

`ResourceUsage.Save()` 会把下面这些信息持久化：

1. RequestId
2. BuildId
3. RequirementId
4. ResourceId
5. ResourceTypeId
6. Comments
7. Attributes
8. 是否手工选择

相关代码：

- `q:\dd\LaunchPad\src\LaunchPad.Components\ResourceUsage.cs`

所以，“一台机器被指给某个 run”在数据层的真实含义是：

- 该 request 的某个 build 已经保存了一条指向这台 machine 的 resource usage

### 13.5 后台服务如何真正拿这台机器去 Run

真正执行 Run 的代码在：

- `q:\dd\LaunchPad\src\LaunchPad.Service\Components\RequestProcessor.cs`

顺序如下：

1. `ProcessRequests()` 取出所有 `Ready To Launch` 的请求
2. 每个 request 交给 `SingleRequestProcessor.ProcessSingleRequest()`
3. 对每个 build，调用 `GetBuildMachine(build)`
4. `GetBuildMachine(build)` 会从 `build.Resources` 中找到 requirement 为 Build Machines 的 `ResourceUsage`
5. 取出其中关联的 `Resource`
6. 对该机器先做 RPC 检查
7. 调 `CreateBuildId(...)` 给这个 build 生成 LabStatus 的 BuildId / SessionId
8. 之后遍历该 build 的资源，只对 `UseForLaunch == true` 的资源执行 launch
9. 最终通过 `LaunchScript(...)` 在目标机器上创建进程，真正开始 run

这一步说明：

- pool 决定“候选范围”
- `ResourceUsage` 决定“最终选中谁”
- `RequestProcessor` 决定“真正在哪台机器上执行”

### 13.6 最短主链

把“一个机器被指给某个 run”的过程压成最短链就是：

1. 机器被登记到 DAD
2. 机器在 `ManageServerPools.aspx` 里被放进 Build Pool
3. Build Pool 在 `PoolDefinitionMappings.aspx` 里被绑定到 LabDefinition
4. 请求在 Launch 阶段进入 `RequestWizard_BuildResources.aspx`
5. 用户手选或系统自动推荐某台 build machine
6. 该机器被保存成 `ResourceUsage`
7. 后台 `RequestProcessor` 读出这条 usage
8. 在这台机器上执行 launch script
9. 这台机器就成为这个 run 的执行机器
