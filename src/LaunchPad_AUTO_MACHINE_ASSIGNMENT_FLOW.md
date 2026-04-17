# LaunchPad 机器自动被指定给 Run 的流程

这份文档整理 LaunchPad 中 Build Machine 自动分配的完整链路，重点回答 5 个问题：

1. 机器什么时候能被自动分配
2. 页面里的推荐值是怎么来的
3. 这条链和 `up_ServerAssignmentsGet` 是什么关系
4. 推荐值什么时候真正落库
5. 后台 service 最后怎样用这台机器启动 run

---

## 1. 先看结论

LaunchPad 里机器确实可能被自动指定给某个 run。

它有两种形态：

1. Request Wizard 里“自动推荐默认机器”
2. Auto Launch 服务里“自动分配并直接保存”

无论是哪一种，最终都会落到同一个结果：

- 某个 `Request`
- 下面某个 `Build`
- 保存了一条指向某台机器的 `ResourceUsage`

后台 `RequestProcessor` 最终并不会重新选机，而是直接读取这条 `ResourceUsage` 来启动 run。

---

## 2. 自动分配的前提条件

一台机器要能进入自动分配链，前提通常是：

1. 机器已经在 DAD 中登记为 server record
2. 机器已经被分配到某个 Build Pool
3. 这个 Build Pool 已经绑定到某个 LabDefinition
4. 当前请求的 `Lab + Definition` 正好落到这个 LabDefinition
5. 机器当前状态可用，没有被其他 pending launch 占住

相关代码位置：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LabDefinition.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\PoolDefinitionMappings.aspx.cs`

---

## 3. 先把四条主链分开

这件事实际不是一条链，而是 4 条相邻的链。

### 3.1 服务器分配管理链

作用：维护“机器属于哪个 Pool”。

调用链：

1. `ManageServerPools.aspx.cs`
2. `BindGrid()`
3. `Resource.LoadByFilters(...)`
4. `LaunchPadDB.GetResourcesByFilters(...)`
5. `up_ServerAssignmentsGet`

结论：

- `up_ServerAssignmentsGet` 管的是 server 和 pool 的管理关系。

### 3.2 Pool 成员读取链

作用：把某个 Build Pool 里已经挂进去的机器读出来，填充页面下拉框。

调用链：

1. `m_Request.Lab.BuildPool.Servers`
2. `Pool.Servers`
3. `Resource.LoadByPool(poolId)`
4. `LaunchPadDB.GetResourcesByPool(poolId)`
5. `up_ResourceGetByPool`

结论：

- `up_ResourceGetByPool` 管的是“这个 Pool 当前有哪些机器”。

### 3.3 自动推荐链

作用：在 Pool 成员里推荐“更适合当前 build 的默认机器”。

调用链：

1. `Request.GetBuildMachineSelection()` 或 `Request.GetBuildMachineSelection(targetDate)`
2. `LaunchPadDB.GetBuildMachineSelection(...)`
3. `up_GetBuildMachineSelection`

结论：

- `up_GetBuildMachineSelection` 管的是“从候选机器里推荐哪一台”。

### 3.4 持久化和执行链

作用：把推荐结果或人工修改结果保存，并在后台真正执行 run。

调用链：

1. `SaveBuildResources()` 或 `AutoLaunchServices.AssignBuildMachineResources()`
2. `ResourceUsage.Save()`
3. `LaunchPadDB.SaveResourceUsage(...)`
4. `up_ResourceUsageSet`
5. `RequestProcessor.GetBuildMachine(build)`
6. `LaunchScript(...)`

结论：

- 真正代表“这台机器已经分给这个 build”的，不是 Pool，也不是推荐表，而是 `ResourceUsage`。

---

## 4. `up_ServerAssignmentsGet` 在整个流程里是什么位置

很多时候容易把 `up_ServerAssignmentsGet` 和自动选机混在一起，但它们职责不同。

### 4.1 它不是推荐算法入口

从 `RequestWizard_BuildResources.aspx.cs` 里的：

- `m_Request.GetBuildMachineSelection()`

并不会走到 `up_ServerAssignmentsGet`。

这行实际走的是：

1. `Request.GetBuildMachineSelection()`
2. `LaunchPadDB.GetBuildMachineSelection(TargetDate)`
3. `up_GetBuildMachineSelection`

也就是说，这条链只负责“推荐哪台机器”。

### 4.2 它是管理页的查询入口

`up_ServerAssignmentsGet` 的直接页面入口是：

- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx.cs`

更完整的顺序是：

1. `Page_Load()`
2. `InitializeControls()`
3. `BindGrid()`
4. `Resource.LoadByFilters(CurrentOrg.Id, poolId, poolTypeId, hardwareClassId, stateId, serverName)`
5. `LaunchPadDB.GetResourcesByFilters(...)`
6. `ExecuteDataSet("up_ServerAssignmentsGet", ...)`

### 4.3 它返回什么

`up_ServerAssignmentsGet` 返回的重点是三类数据：

1. `Resource`
2. `ResourceStatus`
3. `ServerPool`

所以它解决的问题是：

1. 某台机器属于哪些 Pool
2. 机器当前状态是什么
3. 管理页按筛选条件应该看到哪些机器

它不解决的问题是：

1. 当前 build 默认该用哪台机器
2. 当前 request 最终保存了哪台机器

### 4.4 它和自动选机的关系

这三条链的先后关系可以简单记成：

1. `up_ServerAssignmentsGet`
   - 管理 server / pool 关系
2. `up_ResourceGetByPool`
   - 读取某个 Build Pool 的机器成员
3. `up_GetBuildMachineSelection`
   - 在这些成员里推荐默认机器

一句话：

- `up_ServerAssignmentsGet` 管“机器属于哪个池”
- `up_ResourceGetByPool` 管“把池里的机器取出来”
- `up_GetBuildMachineSelection` 管“从这些机器里挑推荐值”

---

## 5. Request Wizard 里的推荐链

页面文件：

- `q:\dd\LaunchPad\src\LaunchPad\RequestWizard_BuildResources.aspx.cs`

### 5.1 页面首次加载时做了两件事

在 `LoadBuildResources()` 里，页面首次加载会并行准备两份数据：

1. 候选机器列表
   - `currentAvailableResources = m_Request.Lab.BuildPool.Servers`
2. 推荐结果表
   - `dtBuilds = m_Request.GetBuildMachineSelection()`

这两份数据不要混起来看：

1. `BuildPool.Servers`
   - 决定下拉框里有哪些机器可选
2. `GetBuildMachineSelection()`
   - 决定默认帮用户选中哪一台

### 5.2 `GetBuildMachineSelection()` 真正做了什么

无参版本：

- `m_Request.GetBuildMachineSelection()`

本质上只是：

- `db.GetBuildMachineSelection(TargetDate)`

它没有把当前 `RequestID` 作为 SQL 参数传下去。

所以它取回来的不是“当前请求的结果”，而是：

- `TargetDate` 当天所有候选 request 的 build-machine 推荐结果

### 5.3 页面为什么还要再按 `RequestID + BuildID` 过滤一次

因为 SQL 返回的是整天的总表，页面还要自己筛出当前请求的那几行：

1. `RequestID = m_Request.Id`
2. `BuildID = build.Id`

然后页面会：

1. 把 `ServerID` 设成下拉框默认选中项
2. 把 `Reason` 存进隐藏字段 `hdnSelectionAlgo`

所以这一步只是：

1. 预选默认机器
2. 保留推荐理由

并没有落库。

---

## 6. `up_GetBuildMachineSelection` 在 SQL 里做什么

SQL 文件：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_GetBuildMachineSelection.proc.sql`

### 6.1 它先准备两类核心数据

1. `#Builds`
   - 当天需要 launch 的所有 build
2. `#AllServers`
   - 当前可分配的 build machines

准备过程中会排除：

1. 硬件类不合法的机器
2. 状态无效的机器
3. 已被其他 pending build 占住的机器

### 6.2 没有硬件类要求时，优先复用历史机器

如果 build 没有 hardware class，要先尝试复用：

- 同 lab + definition + build 上次使用过的机器

理由会写进 `Reason`，例如：

- `No Hardware class specified. Server selected from previous request.`

### 6.3 有硬件类要求时，按优先级选机

大致顺序是：

1. 按 Pool 分开处理
2. Pool 内按 Lab 顺序处理
3. 先处理 X86，再处理非 X86
4. 围绕目标硬件类做邻近匹配

常见优先级是：

1. 最后一次刚好 build 过当前 target lab 的机器
2. 最后 build 的 lab 今天不参与构建的机器
3. 从来没 build 过任何东西的机器
4. 最后 build 过低优先级 lab 的机器

一旦某台机器被挑中，就会从候选集中移除，防止重复分配给多个 build。

### 6.4 它返回什么

返回的关键字段包括：

1. `RequestID`
2. `BuildID`
3. `ServerID`
4. `ServerName`
5. `Reason`
6. `TargetHardwareClass`
7. `ActualHardwareClass`
8. `LastBuiltLabID`
9. `LastBuiltLabName`
10. `LastBuiltRevision`

Auto Launch 会把成功选中的行包装成：

- `BuildMachineSelectionEntry`

---

## 7. 推荐值什么时候变成最终结果

真正落库不是发生在 `GetBuildMachineSelection()`，而是发生在：

- `RequestWizard_BuildResources.aspx.cs` 的 `SaveBuildResources()`

### 7.1 它先把页面最终选择组装成 `ResourceUsage[]`

`SaveBuildResources()` 会遍历 `m_Request.Builds`，从页面读取：

1. `buildresource_<BuildID>`
2. `attribute_<BuildID>_<AttributeID>`
3. `hdnSelectionAlgo_<BuildID>`
4. `hdnChangeLog_<BuildID>`

然后构造：

1. `ResourceUsageAttribute[]`
2. `usageComments`
3. 一条新的 `ResourceUsage`

其中 `usageComments` 会同时保留：

1. 算法推荐理由
2. 用户修改理由

### 7.2 它会先做业务校验

在真正写库前，代码会先执行：

- `requirement.VerifyUsage(usageArray, out validationMessages)`

这不是固定公共规则，而是 definition-specific 的 `ResourceRequirementLogic`。

例如 `Partner`、`Orcas` 的 build-machine requirement 都会检查：

1. 每个 build 都必须有机器
2. 机器必须唯一
3. 只能有一个 `Primary`
4. 必须至少有一个 `Primary`

如果校验失败：

1. 展示错误
2. 重新加载资源页
3. 不落库

### 7.3 校验通过后才真正写库

校验通过后，代码会：

1. 遍历 `usageList`
2. 对每条 `ResourceUsage` 调 `usage.Save()`

`ResourceUsage.Save()` 会：

1. 把 attributes 拼成字符串
2. 调 `LaunchPadDB.SaveResourceUsage(...)`
3. 最终执行 `up_ResourceUsageSet`

所以对向导页来说，真正的持久化终点是：

- `up_ResourceUsageSet`

不是：

- `up_GetBuildMachineSelection`

### 7.4 保存后为什么还要刷新机器状态

页面最后还会调用：

- `Resource.UpdateBuildMachineState()`

继续往下是：

1. `Resource.UpdateBuildMachineState()`
2. `LaunchPadDB.UpdateBuildMachineState()`
3. `up_UpdateBuildMachineState`

目的很明确：

- 刚刚被选中的机器要尽快反映到资源状态里，避免很快又被别的计划再次选中。

---

## 8. Auto Launch 是怎样直接自动分配的

服务入口：

- `q:\dd\LaunchPad\src\LaunchPad\Services\AutoLaunchServices.asmx.cs`

### 8.1 它不是一条请求一条请求地跑算法

`PrepareAutoLaunchRequestsForLaunching()` 的顺序是：

1. `targetDate = DateTime.Today.AddDays(1)`
2. `Request.LoadByReadyToBeProcessedForLaunch(targetDate)`
3. `Request.GetBuildMachineSelection(targetDate)`
4. 对当天所有待处理请求共用这一张选机结果表

也就是说：

- Auto Launch 先对整天所有请求统一算一次选机结果

### 8.2 它怎样把结果正式写进 Request

之后会走到：

1. `PrepareSingleRequestForAutoLaunching(...)`
2. `AssignDropAndBuildResourcesToRequest(...)`
3. `AssignBuildMachineResources(...)`

关键动作是：

1. 遍历 `req.Builds`
2. 从 `BuildMachineSelectionEntry` 找出当前 build 的 `ServerID`
3. 构造 `ResourceUsage`
4. `buildUsage.Save()`

这里的 comments 就是：

- `Assigned by auto-launch algorithm.`

所以 Auto Launch 和 Wizard 的共同点是：

- 最终都写成 `ResourceUsage`

区别是：

1. Wizard
   - 算法给默认值
   - 人工最终确认
2. Auto Launch
   - 算法直接成为最终结果

### 8.3 Auto Launch 还会把请求推进到可执行状态

资源保存成功后，它还会执行：

- `req.Save(RequestSequence.Launch)`

底层会走到 `up_RequestSet`，把请求推进到：

- `ReadyToLaunch`

这一步是 Wizard 保存 build machine 时没有直接完成的，因为 Wizard 只是资源选择步骤的一部分。

---

## 9. 后台 service 最后怎么真正用这台机器

执行端在：

- `q:\dd\LaunchPad\src\LaunchPad.Service\Components\RequestProcessor.cs`

### 9.1 它读取的是已保存的 `ResourceUsage`

`RequestProcessor` 不会重新跑 `up_GetBuildMachineSelection`。

它做的是：

1. `ProcessRequests()` 取出 `ReadyToLaunch` 请求
2. `ProcessSingleRequest()` 遍历 builds
3. `GetBuildMachine(build)` 从 `build.Resources` 里找到 Build Machines 对应的 `ResourceUsage`
4. 取出其中的 `Resource`

所以后台真正依赖的是：

- 已经落库的 `ResourceUsage`

### 9.2 真正启动 run 的顺序

后续顺序大致是：

1. 对目标机器做 RPC 检查
2. 为 build 生成 LabStatus BuildId / SessionId
3. 遍历 `resourceUsage.UseForLaunch == true` 的资源
4. 调 `LaunchScript(...)`
5. 通过 WMI 在目标机器上创建进程

到这里，这台机器才真正变成这个 run 的执行机器。

---

## 10. 最短主链

如果只保留最短主链，可以记成：

1. 机器先在后台被挂进 Build Pool
2. Request Wizard 通过 `BuildPool.Servers` 读到候选机器
3. 系统通过 `up_GetBuildMachineSelection` 算出默认推荐值
4. 用户确认或 Auto Launch 直接采用该结果
5. 最终保存成 `ResourceUsage`
6. `RequestProcessor` 读取这条 `ResourceUsage`
7. 在该机器上执行 launch script

---

## 11. 一句话总结

LaunchPad 中机器“自动被指定给 run”并不是在 run 当下临时现选，而是分成 4 个层次：

1. `up_ServerAssignmentsGet` 管 server 和 pool 的管理关系
2. `up_ResourceGetByPool` 管 pool 里的候选机器列表
3. `up_GetBuildMachineSelection` 管默认推荐哪台机器
4. `up_ResourceUsageSet` 把最终选择固化成 `ResourceUsage`

后台 run 阶段真正消费的，始终是最后这条 `ResourceUsage`。
