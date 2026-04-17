# LaunchPad 和 tbl_Session 的关系

这份文档专门整理 `tbl_Session` 在 LaunchPad 里的位置，重点回答这几个问题：

1. `tbl_Session` 是什么
2. 它和 LaunchPad 的 `Request` / `RequestBuild` / `ResourceUsage` 分别是什么关系
3. LaunchPad 什么时候会拿到 `SessionID`
4. 为什么 LaunchPad 的资源状态和自动选机会依赖 `tbl_Session`

---

## 1. 先看结论

`tbl_Session` 不是 LaunchPad 自己的主业务表，它属于 BuildStatus / LabStatus 体系。

但对 LaunchPad 来说，它非常关键，因为它代表：

- 一次真实 build/run 在下游运行系统里的落地记录

可以把几层关系简单记成：

1. `Request`
   - 用户在 LaunchPad 里提出“我要跑什么”
2. `RequestBuild`
   - 这个 request 里具体有哪些 build 组合
3. `ResourceUsage`
   - 这些 build 准备用哪些机器、哪些资源来跑
4. `tbl_Session`
   - 真正运行起来以后，在 BuildStatus / LabStatus 里生成的运行实例记录

所以 `tbl_Session` 不是“候选计划”，而是“真实运行实例”。

---

## 2. `tbl_Session` 表里最重要的字段

表定义在：

- `q:\dd\BuildStatus\src\LabStatus.Database\Schema Objects\Schemas\dbo\Tables\tbl_Session.table.sql`

从 LaunchPad 视角，最重要的字段是这些：

1. `SessionID`
   - 一次 build session 的主键
2. `LabID`
   - 这次构建跑在哪个 lab
3. `FlavorID`
   - 这次构建是什么 flavor，例如 CHK / RET / COV
4. `TargetArchitectureID`
   - 这次构建的架构，例如 X86 / AMD64 / IA64
5. `LocaleSetID`
   - locale 维度
6. `SeRevision`
   - 当前 revision / build number
7. `StatusID`
   - 这次 session 当前状态
8. `SeStarted`
   - 什么时候开始
9. `SeFinished`
   - 什么时候结束
10. `SeBuildMachine`
   - 实际执行这次 build 的机器名
11. `SeCanRecycle`
   - 是否可回收，LaunchPad 会用它判断机器是否 `OnICE`
12. `SeValidationBuild`
   - 是否 validation build

如果只保留一句话：

- `tbl_Session` 记录的是“某个 lab、某个 revision、某个架构/风味的真实一次运行”。

---

## 3. 它和 LaunchPad 里的几个对象分别是什么关系

### 3.1 `Request`

`Request` 是 LaunchPad 里的用户请求实体。

它关心的是：

1. 想跑哪个 `Lab`
2. 想跑哪个 `Definition`
3. 目标日期是什么
4. revision、属性、资源、步骤是什么

但 `Request` 本身还不是下游 BuildStatus 的运行实例。

### 3.2 `RequestBuild`

`RequestBuild` 是 `Request` 下面的 build 粒度对象。

例如一个 request 里可能会有：

1. X86 CHK
2. X86 RET
3. AMD64 CHK
4. AMD64 RET

这些每一条就是一个 `RequestBuild`。

在 LaunchPad 代码里，`RequestBuild` 有一个关键字段：

- `SessionId`

定义在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\RequestBuild.cs`

这就是 LaunchPad 和 `tbl_Session` 之间最直接的桥。

### 3.3 `ResourceUsage`

`ResourceUsage` 记录的是：

- 这个 request / build 实际绑定了哪些资源

对于 build machine 来说，`ResourceUsage` 说明：

- 这次 `RequestBuild` 将在哪台机器上跑

所以真正的顺序是：

1. 先有 `RequestBuild`
2. 再有 `ResourceUsage` 决定跑在哪台机器
3. 最后 launch 时才会创建对应的 `tbl_Session`

### 3.4 `tbl_Session`

当 LaunchPad 真正开始 launch 时，下游系统里会出现一条 `tbl_Session`。

这条记录代表：

- “这条 `RequestBuild` 现在已经不是计划，而是一个真实运行中的 build session”

---

## 4. LaunchPad 什么时候拿到 `SessionID`

这一步发生在 LaunchPad Service 真正 launch 的时候。

关键代码在：

- `q:\dd\LaunchPad\src\LaunchPad.Service\Components\RequestProcessor.cs`

在 `ProcessSingleRequest()` 里，LaunchPad 会遍历 `m_Request.Builds`，对每个 build 做：

1. 如果 `build.SessionId` 还没有值
2. 调 `CreateBuildId(...)`
3. 让下游系统生成一条真实的 build session
4. 把返回的 `SessionID` 赋给 `build.SessionId`

这一步是 LaunchPad 从“计划阶段”进入“真实运行阶段”的关键转折点。

---

## 5. `build.SessionId = ...` 写回到哪里

`RequestBuild.SessionId` 的 setter 在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\RequestBuild.cs`

逻辑是：

1. 调 `LaunchPadDB.SetRequestBuildSessionId(...)`
2. 再把内存里的 `m_SessionId` 更新掉

而 `LaunchPadDB.SetRequestBuildSessionId(...)` 在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\LaunchPadDB.cs`

它最终执行：

- `up_RequestBuildSetSessionID`

也就是说：

- `tbl_Session.SessionID` 本身在 BuildStatus / LabStatus 那边生成
- LaunchPad 这边保存的是“我的 `RequestBuild` 对应到了那个 `SessionID`”

所以是“关联”关系，不是 LaunchPad 自己存了一份完整的 session 主记录。

---

## 6. 一个完整例子：从 RequestBuild 到 tbl_Session

如果按时间顺序看，一条 build 大致这样流转：

### Step 1. 用户创建 Request

LaunchPad 里先有一个 `Request`。

### Step 2. Request 下生成多个 RequestBuild

比如会生成：

1. X86 CHK
2. X86 RET

这时这些 `RequestBuild` 还是“待运行计划”。

### Step 3. 机器被选出来

通过向导或 Auto Launch，把某台 machine 保存成 `ResourceUsage`。

这时含义是：

- 这个 `RequestBuild` 计划在这台机器上跑

但这时还没有真实 session。

### Step 4. RequestProcessor 开始 launch

LaunchPad Service 在：

- `ProcessSingleRequest()`

里开始处理这个 request。

### Step 5. 创建 BuildStatus session

对每个 build 调：

- `CreateBuildId(...)`

下游系统创建真实运行实例，于是 `tbl_Session` 里出现一条新记录。

### Step 6. LaunchPad 保存 SessionID 关联

返回的 `SessionID` 会写回 `RequestBuild.SessionId`。

从这一步开始，LaunchPad 就能说：

- “我这条请求里的这个 build，对应 BuildStatus 上的哪条 session”

### Step 7. 后续查看状态、链接详情、更新资源状态

之后 LaunchPad 就能用这个 `SessionID`：

1. 打开 BuildStatus 详情页
2. 判断机器是不是还在构建中
3. 根据 session 状态刷新 `tbl_ResourceState`

---

## 7. 为什么 `tbl_Session` 会影响自动选机和机器状态

这是它在 LaunchPad 里最重要的第二层作用。

### 7.1 它决定机器是不是正在被占用

`up_UpdateBuildMachineState` 会读 `tbl_Session`，判断：

1. 哪些机器最近在 build
2. 哪些机器仍然应该算 `BuildInProgress`
3. 哪些机器处于 `OnICE`
4. 哪些机器虽然 build 相关 session 还在，但已经可以 partial release

这些结果最后会写进：

- `tbl_ResourceState`
- `tbl_ResourceStateHistory`

### 7.2 它会反过来影响自动选机

而自动选机过程又会参考：

- `tbl_ResourceState`

来排除：

1. 正在构建中的机器
2. 状态异常的机器
3. 不可回收或 request in progress 的机器

所以形成了一条闭环：

1. `tbl_Session` 描述真实运行情况
2. 运行情况被汇总进 `tbl_ResourceState`
3. 自动选机根据 `tbl_ResourceState` 过滤候选机器
4. 新请求再生成新的 session

这就是为什么 `tbl_Session` 虽然不在 LaunchPad 主库里，却对 LaunchPad 的选机逻辑影响很大。

---

## 8. LaunchPad 里常见的 `tbl_Session` 使用方式

### 8.1 用 `SessionID` 关联详情页

LaunchPad 的一些 UI 会根据 `RequestBuild.SessionId` 生成跳转到 BuildStatus 的链接。

这意味着：

- 一旦 session 建立，用户就能从 LaunchPad 跳到下游构建详情页

### 8.2 用 `SeBuildMachine` 反推机器状态

状态刷新 sproc 会根据：

- `SeBuildMachine`
- `SeStarted`
- `StatusID`
- `SeCanRecycle`

判断一台机器当前是不是仍然占用中。

### 8.3 用 `SeRevision + LabID + FlavorID + ArchitectureID` 匹配构建集合

在 build machine 状态刷新和 partial release 逻辑里，经常会按：

1. `LabID`
2. `SeRevision`
3. `LocaleSetID`
4. `FlavorID`

等字段把 session 组合起来看，而不是只看单条 session。

因为 LaunchPad 真正关心的通常不是“单条 session”，而是：

- 这个 lab/revision 的整个 build 集合是不是还占着机器

---

## 9. 它和 LaunchPad 几张核心表的定位对照

| 对象 | 所在系统 | 含义 |
| --- | --- | --- |
| `Request` | LaunchPad | 用户提出的一次请求 |
| `RequestBuild` | LaunchPad | 请求里的某个具体 build 维度 |
| `ResourceUsage` | LaunchPad | 这条 request/build 实际使用哪些资源 |
| `tbl_Session` | BuildStatus / LabStatus | 真实跑起来的一次 build session |
| `tbl_ResourceState` | LaunchPad | 从真实运行和外部状态汇总出来的当前机器状态 |

最关键的一句是：

- `RequestBuild` 是 LaunchPad 的计划单元，`tbl_Session` 是 BuildStatus 的运行单元。

---

## 10. 最短主链

如果只记最短链路，可以记成：

1. LaunchPad 先有 `Request`
2. `Request` 下面有多个 `RequestBuild`
3. `ResourceUsage` 决定这些 build 用哪台机器
4. `RequestProcessor` launch 时创建真实 `tbl_Session`
5. 返回的 `SessionID` 写回 `RequestBuild.SessionId`
6. 以后 LaunchPad 就靠这个关联去查看 build 详情、刷新机器状态、判断资源占用

---

## 11. 一句话总结

`tbl_Session` 在 LaunchPad 里不是“请求配置表”，而是“真实运行实例表”；LaunchPad 通过 `RequestBuild.SessionId` 把自己的请求世界和 BuildStatus 的运行世界连起来，而机器状态刷新、资源保护和后续跳转展示，都会依赖这层关联。