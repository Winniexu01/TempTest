# LaunchPad 机器自动被指定给 Run 的流程

本文档单独整理 LaunchPad 中“机器自动被指定给某个 run”的完整代码链，重点回答这几个问题：

1. 机器什么时候会被自动挑中
2. 自动挑中的算法入口在哪里
3. 机器是如何保存到 Request 的
4. 后台 service 怎样真正使用这台机器开始 run

---

## 1. 结论

机器在 LaunchPad 里确实可能自动被指定。

自动指定分两种层级：

1. 向导页面里的“自动推荐默认机器”
2. Auto Launch 服务里的“全自动分配并直接保存”

不管是哪一种，最终都会落到同一个数据结果：

- 某个 `Request`
- 下面某个 `Build`
- 保存了一条指向某台机器的 `ResourceUsage`

后台 `RequestProcessor` 最终就是从这条 `ResourceUsage` 里把机器读出来并执行 run。

---

## 2. 自动指定的前提条件

一台机器要能被自动分配，必须先满足这些前置条件：

1. 机器已经在 DAD 中登记为 server record
2. 机器已经被分配到某个 Build Pool
3. 这个 Build Pool 已经绑定到某个 LabDefinition
4. 请求的 `Lab + Definition` 正好对应到这个 LabDefinition
5. 机器当前没有被标记为不可用，也没有被别的 pending launch 占住

相关代码关系：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LabDefinition.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\PoolDefinitionMappings.aspx`

---

## 3. 第一个自动指定层级：Build Resources 页面自动推荐

页面：

- `q:\dd\LaunchPad\src\LaunchPad\RequestWizard_BuildResources.aspx.cs`

### 3.1 页面加载时怎么拿候选机器

页面首次加载时会做两件事：

1. 从当前请求的 Build Pool 读取可选机器：
   - `m_Request.Lab.BuildPool.Servers`
2. 调用：
   - `m_Request.GetBuildMachineSelection()`

对应代码：

- `q:\dd\LaunchPad\src\LaunchPad\RequestWizard_BuildResources.aspx.cs`

### 3.2 GetBuildMachineSelection 的入口

Request 层封装在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\Request.cs`

有两种调用：

1. `Request.GetBuildMachineSelection(DateTime targetDate)`
2. `m_Request.GetBuildMachineSelection()`

底层最终都走：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`
- 存储过程：`up_GetBuildMachineSelection`

SQL 文件：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_GetBuildMachineSelection.proc.sql`

### 3.3 页面如何使用推荐结果

`up_GetBuildMachineSelection` 返回每个 build 推荐的 `ServerID` 和 `Reason`。

页面会：

1. 读取结果表
2. 找出当前 request + build 对应的行
3. 把 `ServerID` 预选到机器下拉框
4. 把 `Reason` 放到隐藏字段里，作为推荐原因说明

所以这一层的自动指定，本质是：

- 系统先替用户选一台默认机器
- 但用户仍然可以在页面上改成别的机器

这一步还不是“最终执行 run”，只是“自动给出默认值”。

---

## 4. 第二个自动指定层级：Auto Launch 全自动分配

服务入口：

- `q:\dd\LaunchPad\src\LaunchPad\Services\AutoLaunchServices.asmx.cs`

### 4.1 Auto Launch 如何找到待处理请求

方法：

- `PrepareAutoLaunchRequestsForLaunching()`

顺序：

1. 计算 `targetDate = DateTime.Today.AddDays(1)`
2. 调 `Request.LoadByReadyToBeProcessedForLaunch(targetDate)`
3. 如果没有请求则返回
4. 调 `Request.GetBuildMachineSelection(targetDate)` 为当天所有请求跑自动选机算法

也就是说：

- 自动选机不是一个请求一个请求单独算
- 它是先针对当天全部请求算出一张统一分配表

### 4.2 BuildMachineSelectionEntry 是什么

自动选机结果被包装成：

- `BuildMachineSelectionEntry`

定义在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\BuildMachineSelectionEntry.cs`

这个对象至少包含：

1. `RequestID`
2. `BuildID`
3. `ServerID`
4. `ServerName`
5. `TargetArch`
6. `TargetFlavor`

所以它表示的是：

- 某个 request 下某个 build
- 被算法选中了哪台 machine

### 4.3 Auto Launch 怎样把机器正式绑到 Request 上

在：

- `AssignDropAndBuildResourcesToRequest(...)`

它会继续调用：

1. `AssignDropServerResource(...)`
2. `AssignBuildMachineResources(...)`

对于 build machine，真正关键逻辑在：

- `AssignBuildMachineResources(...)`

执行顺序：

1. 遍历 `req.Builds`
2. 从 `applicableSvrs` 中找出当前 build 对应的 `ServerID`
3. 构造一个 `ResourceUsage`
4. `buildUsage.Save()`

这里的注释也很明确：

- `Assigned by auto-launch algorithm.`

也就是说，这一步已经不是推荐，而是正式保存。

---

## 5. 真正的绑定关系：ResourceUsage

类定义：

- `q:\dd\LaunchPad\src\LaunchPad.Components\ResourceUsage.cs`

### 5.1 为什么 ResourceUsage 才是关键

机器在 Pool 里，只代表：

- 它“可能被选中”

机器被保存成 `ResourceUsage`，才代表：

- 它“已经被这个 request/build 选中”

### 5.2 Save() 做了什么

`ResourceUsage.Save()` 会把这些信息持久化：

1. `RequestId`
2. `BuildId`
3. `RequirementId`
4. `ResourceId`
5. `ResourceTypeId`
6. `Comments`
7. `Attributes`
8. `ManuallySelected`

最终调用：

- `LaunchPadDB.SaveResourceUsage(...)`

所以，自动分配或手动分配最后都会统一写成一条 resource usage 记录。

---

## 6. 自动选机算法在 SQL 里到底看什么

存储过程：

- `up_GetBuildMachineSelection`

文件：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_GetBuildMachineSelection.proc.sql`

它大致分几阶段：

### 6.1 Phase 1：准备 build 和 server 数据

先准备两张关键临时表：

1. `#Builds`
   - 表示当天需要 launch 的所有 build
   - 一行不是一个 request，而是一个 build
2. `#AllServers`
   - 表示当前可分配的所有 build machines

构造时会过滤掉：

1. 无硬件类的机器
2. 当前状态无效的机器
3. 已经被 LaunchPad 其他 pending builds 占住的机器

### 6.2 先尝试“复用上一轮用过的机器”

如果 build 没有硬件类要求：

- 优先查以前同 lab + definition + build 对应的上一次机器
- 如果找到了，直接复用

理由会写进 `Reason`：

- `No Hardware class specified. Server selected from previous request.`

### 6.3 正常硬件类场景的主算法

对于有硬件类要求的 build：

1. 按 Pool 分开跑
2. 每个 Pool 内，再按 Lab 顺序处理
3. 优先处理 X86，再处理非 X86
4. 对每个 build，围绕目标硬件类做邻近匹配

然后按以下优先级找机器：

1. 最后一次刚好 build 过当前 target lab 的机器
2. 最后 build 的 lab 今天不参与构建的机器
3. 从来没 build 过任何东西的机器
4. 最后 build 过低优先级 lab 的机器

每次一旦选中：

- 会把该 server 从 `#AllServers` 标记删除
- 防止同一台机器被重复分给多个 build

### 6.4 返回结果

最终返回每个 build 对应的：

1. `RequestID`
2. `BuildID`
3. `ServerID`
4. `ServerName`
5. `TargetHardwareClass`
6. `ActualHardwareClass`
7. `Reason`
8. `LastBuiltLabID`
9. `LastBuiltLabName`
10. `LastBuiltRevision`

C# 层会从中取 `ServerID is not null` 的结果，包装成 `BuildMachineSelectionEntry`。

---

## 7. 后台怎样真正把这台机器跑起来

入口：

- `q:\dd\LaunchPad\src\LaunchPad.Service\Components\RequestProcessor.cs`

### 7.1 RequestProcessor 读什么

它不是重新跑一次选机算法，而是读已经保存好的 request 资源：

1. `ProcessRequests()` 取出 `Ready To Launch` 请求
2. `ProcessSingleRequest()` 遍历请求里的 builds
3. 对每个 build 调 `GetBuildMachine(build)`

`GetBuildMachine(build)` 的逻辑是：

- 从 `build.Resources` 里找 requirement 为 Build Machines 的 `ResourceUsage`
- 读出其中的 `Resource`

所以后台 run 阶段真正依赖的是：

- 已经写好的 `ResourceUsage`

### 7.2 真正启动 Run

之后流程是：

1. 对这台机器做 RPC 检查
2. 为 build 生成 LabStatus BuildId / SessionId
3. 遍历 `resourceUsage.UseForLaunch == true` 的资源
4. 调 `LaunchScript(...)`
5. 通过 WMI 在目标 machine 上创建进程

到这一步，这台机器就真正变成了这个 run 的执行机器。

---

## 8. 最短主链

如果只保留最短主链，可以记成：

1. 机器进 Build Pool
2. Build Pool 绑到 LabDefinition
3. 请求进入 Launch 阶段
4. 系统跑 `up_GetBuildMachineSelection`
5. 得到 `BuildMachineSelectionEntry`
6. 保存成 `ResourceUsage`
7. Request 进入 `Ready To Launch`
8. `RequestProcessor` 读出这条 `ResourceUsage`
9. 在这台机器上执行 launch script
10. 这台机器成为该 run 的执行机器

---

## 9. 一句话总结

LaunchPad 中机器“自动被指定给 run”并不是在 run 当下临时现选，而是：

- 先通过 build machine selection algorithm 选出来
- 再保存成 request/build 的 `ResourceUsage`
- 最后由后台 service 按这条 usage 去真正执行 run
