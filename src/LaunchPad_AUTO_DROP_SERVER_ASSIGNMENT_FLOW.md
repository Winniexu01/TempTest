# LaunchPad Drop Server 自动分配流程

这份文档对应前一份 build machine 文档，单独说明 LaunchPad 里 Drop Server 是怎样被自动指定给一个 request / run 的。

核心问题是：

1. Drop Server 从哪里选
2. 选机算法入口在哪里
3. 什么时候只是默认推荐，什么时候已经正式保存
4. 后台 service 最终怎么使用这台 Drop Server

---

## 1. 结论

Drop Server 也会被系统自动指定。

和 Build Machine 类似，Drop Server 也分两层：

1. ResourceRequirement 里的默认推荐
2. Auto Launch 里的正式自动分配

最终落地形式也一样：

- 写成一条 `ResourceUsage`
- 绑定到当前 `Request`
- 后台 launch 时按 `UseForLaunch` 去执行对应脚本

---

## 2. 前置条件

要让一个 request 能自动拿到 Drop Server，前提是：

1. LabDefinition 已经绑定了 `DropPool`
2. `DropPool` 里已经分配了可用的 drop server
3. 该 definition 对应的 drop hardware class、drop drive letter 等配置是完整的
4. 机器当前状态可用
5. 目标日期上这台机器没有被别的 request 占用

相关代码位置：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LabDefinition.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\PoolDefinitionMappings.aspx.cs`
- `q:\dd\LaunchPad\src\LaunchPad\Admin\ManageServerPools.aspx.cs`

---

## 3. 第一层：默认推荐 Drop Server

一个较早的自动分配入口在 definition resource logic 中。

示例文件：

- `q:\dd\LaunchPad\src\LaunchPad.Definitions\Partner\Definitions\Partner\ResourceRequirements\DropMachine.cs`

### 3.1 GetDefault 的做法

在 `GetDefault()` 里会：

1. 检查 `m_Request.Lab.DropPool != null`
2. 调用 `LaunchPad.Components.DropMachine.GetDropMachineSelection(...)`
3. 不管有没有选到合适机器，都会返回一条 `ResourceUsage`
4. 这条 usage 里会带上 `selectionLog`

这一层的重点是：

- UI 或请求准备阶段可以拿到系统推荐的 drop server
- 同时也能拿到“为什么选它”的说明日志

### 3.2 为什么即使没选到机器也要返回 usage

代码注释写得很直接：

- 即使没有 suitable machines，也要返回 `ResourceUsage`
- 因为 selection log 仍然有价值

所以这条逻辑不只是“分配资源”，也是“解释为什么没分配到资源”。

---

## 4. 核心入口：DropMachine.GetDropMachineSelection

文件：

- `q:\dd\LaunchPad\src\LaunchPad.Components\DropMachine.cs`

这个方法的作用非常直接：

1. 调 `LaunchPadDB.GetDropMachineSelection(...)`
2. 读取返回的 `DataSet`
3. 第一张表如果有记录，就把第一台机器包装成 `Resource`
4. 再扫描所有表，把 `DropMachineSelectionLog` 读出来
5. 返回：
   - 选中的 `Resource`
   - 一份文字化 `selectionLog`

这说明 Drop Server 的自动选择结果天然包含两部分：

1. 机器本身
2. 选择过程日志

---

## 5. LaunchPadDB 到 SQL 的入口

DB 封装在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`

方法：

- `GetDropMachineSelection(int poolId, int labId, int definitionId, DateTime targetDate)`

它最终执行：

- `up_GetDropMachineSelection`

SQL 文件：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_GetDropMachineSelection.proc.sql`

LaunchPadDB 注释里已经说明返回结构：

1. 第 1 张表：选中的 drop server resource
2. 第 2 张表：该 resource 的 state information
3. 第 3 张表：selection log

---

## 6. up_GetDropMachineSelection 的主要逻辑

这个存储过程的目标不是“找所有机器”，而是“从 Drop Pool 中浮出最合适的一台”。

### 6.1 先确定目标硬件要求

它先做几件初始化：

1. 计算 Drop Server 硬件类的相对 rank
2. 找出当前 `LabID + DefinitionID` 要求的最小 drop hardware class rank
3. 如果没配置硬件类，则退化成最弱要求，也就是允许更广泛的候选集
4. 判断当前是不是 SANMan lab：
   - 有 `DropDriveLetter` -> 不是 SANMan lab
   - 没有 `DropDriveLetter` -> 认为是 SANMan lab

### 6.2 先做一轮排除

候选集先从指定 `PoolID` 开始，也就是：

- 只在这个 Drop Pool 里选

之后逐轮删除不合格机器：

1. 删除硬件等级不满足要求的机器
2. 删除状态无效的机器
3. 对 SANMan lab，删除没有可用 drive letter 的机器
4. 删除在 `TargetDate` 已经被其他 request 占用的机器

这一轮结束后，剩下的才是“还能继续比较”的候选集。

### 6.3 再计算空间信息

接下来算法会给每台候选机补充 `SpaceAvailable`：

1. SANMan lab：
   - 看这台 server 关联 SAN 的总分配空间
   - 减去已使用空间
2. 静态 drop lab：
   - 看对应 `DropDriveLetter` 的剩余空间

所以 Drop Server 的主排序维度，不是“最近谁用过”，而是“这台机器还能承载多少 drop 空间”。

### 6.4 最终排序规则

最后会把候选机按这个顺序排序：

1. `SpaceAvailable DESC`
2. `TargetDateRequestCount ASC`
3. `ResourceID ASC`

也就是：

1. 优先空间最大的
2. 空间差不多时，优先目标日期请求更少的
3. 再相同就按资源 ID 稳定选一台

### 6.5 返回 selection log

存储过程会把整个筛选过程拼成 `@FullLog`，包括：

1. 池里找到多少台机器
2. 符合硬件类的还有多少
3. 状态有效的还有多少
4. 有空余 drive letter / 空间的还有多少
5. 最终候选清单
6. 最后选中的资源名

如果失败，也会把失败原因放进 `DropMachineSelectionLog` 返回。

---

## 7. Auto Launch 里的正式自动分配

入口文件：

- `q:\dd\LaunchPad\src\LaunchPad\Services\AutoLaunchServices.asmx.cs`

### 7.1 PrepareSingleRequestForAutoLaunching 的顺序

自动 launch 时，对单个 request 的关键顺序是：

1. `req.AttemptToAssignOptimalSAN()`
2. `DropMachine.GetDropMachineSelection(...)`
3. 检查是否成功拿到 drop server
4. 检查是否成功拿到所有 build machines
5. `AssignDropAndBuildResourcesToRequest(...)`
6. `req.Save(RequestSequence.Launch)`

也就是说，Drop Server 和 Build Machines 是一起准备的；只要缺一个，请求就不会进入 `ReadyToLaunch`。

### 7.2 AssignDropServerResource 如何保存

真正保存 Drop Server 的方法是：

- `AssignDropServerResource(...)`

它会：

1. 用 `Resource.LoadByName(dropSvr.Name, ResourceType.DADServer)` 重新拿资源对象
2. 构造一条 `ResourceUsage`
3. `Build` 参数传 `null`，因为 Drop Server 绑定在 request 级别，不是某个具体 build 级别
4. `Comments` 写成：
   - `Assigned by auto-launch algorithm: ` + `selectionLog`
5. `ManuallySelected = false`
6. 调 `dropUsage.Save()`

所以 Drop Server 和 Build Machine 的一个关键区别是：

- Build Machine 通常绑定到某个 `Build`
- Drop Server 绑定到整个 `Request`

---

## 8. ReadyToLaunch 是怎么推进的

Auto Launch 里分配完资源之后，会执行：

- `req.Save(RequestSequence.Launch)`

对应 SQL：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Programmability\Stored Procedures\up_RequestSet.proc.sql`

其中：

- `@Sequence = 3` 表示 Launch
- 会写日志：`Request set to Ready to Launch`
- 会把 `RequestStatusID` 更新为 `7`，也就是 `ReadyToLaunch`

所以真正把请求推进到后台 service 可消费状态的，不是“选到机器”这一刻，而是：

- 资源保存成功后
- `RequestSequence.Launch` 保存成功

---

## 9. 后台 service 怎样消费 Drop Server

最终执行端在：

- `q:\dd\LaunchPad\src\LaunchPad.Service\Components\RequestProcessor.cs`

这里不会重新跑 `up_GetDropMachineSelection`。

它只会读取当前 request/build 上已经保存好的 `ResourceUsage`，然后对 `UseForLaunch == true` 的资源逐个执行 launch script。

所以对 Drop Server 来说，后台阶段依赖的也是：

- 已经持久化好的 `ResourceUsage`

这和 Build Machine 是同一模式，只是绑定粒度不同。

---

## 10. 和 Build Machine 自动分配的关键差异

两者都属于“先选资源，再保存 usage，再由 service 消费”，但侧重点不同：

1. Build Machine：
   - 一般按 `Build` 粒度分配
   - 更关注 architecture、flavor、hardware class、历史 build 亲和性
2. Drop Server：
   - 按 `Request` 粒度分配
   - 更关注 drop 空间、SAN / drive letter、目标日期占用情况

简化理解就是：

- Build Machine 解决“谁来跑 build”
- Drop Server 解决“drop 放到哪台机器上”

---

## 11. 最短主链

如果只记最短主链，可以记成：

1. LabDefinition 绑定 DropPool
2. DropPool 里有若干 Drop Server
3. 系统调用 `DropMachine.GetDropMachineSelection`
4. 底层执行 `up_GetDropMachineSelection`
5. 按硬件类、状态、空间、目标日期占用情况选出一台
6. Auto Launch 调 `AssignDropServerResource`
7. 保存成 request 级别的 `ResourceUsage`
8. `req.Save(RequestSequence.Launch)` 把请求推进到 `ReadyToLaunch`
9. `RequestProcessor` 使用这条 resource usage 执行 launch

---

## 12. 一句话总结

LaunchPad 里的 Drop Server 自动分配，本质上是一个“以 Pool 为范围、以空间和可用性为核心排序”的选机过程；选中后不会在运行时临时重算，而是先保存成 `ResourceUsage`，再由后台 service 按这条记录去执行 run。