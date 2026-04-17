# LaunchPad 中 tbl_ResourceState / tbl_ResourceStateHistory 的作用

这份文档专门整理 LaunchPad 里这两张资源状态表的职责、数据结构、状态位含义、典型 XML 结构，以及 `up_UpdateBuildMachineState` 的完整中文流程。

---

## 1. 两张表分别干什么

### 1.1 `tbl_ResourceState`

这是“当前状态表”。

含义是：

- 每台资源当前处于什么状态
- 状态的详细上下文是什么

表定义：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Tables\tbl_ResourceState.table.sql`

字段：

1. `ResourceID`
2. `RsDateTime`
3. `RsStatusID`
4. `RsData`

主键：

- `ResourceID`

对应文件：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Tables\Keys\PK_tbl_ResourceState_1.pkey.sql`

这说明设计上：

- 一台资源在当前表里最多只有一行
- 这一行就是“当前快照”

### 1.2 `tbl_ResourceStateHistory`

这是“历史表”。

含义是：

- 某台资源在什么时间进入过什么状态
- 每次状态变化时，记录一笔历史快照

表定义：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Tables\tbl_ResourceStateHistory.table.sql`

字段和当前表相同：

1. `ResourceID`
2. `RsDateTime`
3. `RsStatusID`
4. `RsData`

主键：

- `RsDateTime + ResourceID`

对应文件：

- `q:\dd\LaunchPad\src\LaunchPad.Database\Schema Objects\Schemas\dbo\Tables\Keys\PK_tbl_ResourceStateHistory.pkey.sql`

这说明设计上：

- 历史表不是覆盖更新
- 而是状态变化时不断追加

---

## 2. `RsStatusID` 是什么

`RsStatusID` 是一个位标志枚举。

定义在：

- `q:\dd\LaunchPad\src\LaunchPad.Components\Enumerations.cs`

源码定义：

- `None = 0`
- `Imaging = 1`
- `Maintenance = 2`
- `Offline = 4`
- `BuildInProgress = 8`
- `OpenIssues = 16`
- `OpenTickets = 32`
- `OnICE = 64`
- `Inactive = 128`
- `DropInProgress = 256`
- `RequestInProgress = 512`
- `NoAvailableDriveLetters = 1024`

因为这是位枚举，所以一台机器可能同时拥有多个状态。

例如：

- `128 + 16 = 144`，表示 `Inactive + OpenIssues`
- `8 + 16 + 32 = 56`，表示 `BuildInProgress + OpenIssues + OpenTickets`

---

## 3. `RsStatusID` 各个位的含义与典型 XML

| 位值 | 名称 | 含义 | 典型 XML 节点 | 主要来源 |
| --- | --- | --- | --- | --- |
| `0` | `None` | 空闲，无任何当前状态 | `<State></State>` | 自动状态刷新在清空状态时写入 history |
| `1` | `Imaging` | 机器处于成像/监控暂停状态 | `<Imaging>...</Imaging>` | DAD `tbl_ServerMonitoringSuspend` |
| `2` | `Maintenance` | 机器处于维护暂停状态 | `<Maintenance>...</Maintenance>` | DAD `tbl_ServerMonitoringSuspend` |
| `4` | `Offline` | 机器离线、监控关闭或 ping alert 异常 | `<Alerts>...</Alerts>` | DAD `vw_Alert` / `tbl_Server` |
| `8` | `BuildInProgress` | build machine 正在参与构建 | `<Builds>...</Builds>` | LabStatus `tbl_Session` |
| `16` | `OpenIssues` | 机器或相关 session 有未关闭 Issue | `<Issues>...</Issues>` | LabStatus `tbl_Issue` |
| `32` | `OpenTickets` | 机器有关联的未关闭 Ticket | `<Tickets>...</Tickets>` | TechEase |
| `64` | `OnICE` | 机器处于不可回收/ICE 状态 | `<Ice>...</Ice>` | LabStatus `tbl_Session.SeCanRecycle = 0` |
| `128` | `Inactive` | 管理员手工停用 | `<Inactive>...</Inactive>` | `up_ResourceDeactivate` |
| `256` | `DropInProgress` | drop server 正在被使用 | `<Drops>...</Drops>` | LaunchPad + LabStatus |
| `512` | `RequestInProgress` | 机器已被 request 绑定且 request 正在进行 | `<RequestInProgress>...</RequestInProgress>` | LaunchPad request/resource usage |
| `1024` | `NoAvailableDriveLetters` | drop server 没有可用 drive letter | `<NoAvailableDrives></NoAvailableDrives>` | SanManJob `vw_ServerDriveLetters` |

注意两点：

1. 名称 `Offline` 对应的 XML 容器在代码里是 `<Alerts>`，因为它把 ping alert、monitoring disabled 等离线类问题一起装进这个节点。
2. `Inactive` 是人工停用位，它会被自动刷新逻辑保留，而不会被自动状态覆盖掉。

---

## 4. 典型 `RsData` XML 结构

### 4.1 手工停用

典型结构：

```xml
<State>
  <Inactive>
    <Date>2026-04-17T10:20:30</Date>
    <User>alias</User>
    <Reason>maintenance window</Reason>
  </Inactive>
</State>
```

来源：

- `up_ResourceDeactivate`

### 4.2 Build in progress

典型结构：

```xml
<State>
  <Builds>
    <Build>
      <SessionID>12345</SessionID>
      <LabID>10</LabID>
      <Lab>Main</Lab>
      <Revision>25000.1</Revision>
    </Build>
  </Builds>
</State>
```

来源：

- `up_UpdateBuildMachineState`

### 4.3 Open issue

典型结构：

```xml
<State>
  <Issues>
    <IssueID>98765</IssueID>
  </Issues>
</State>
```

来源：

- `up_UpdateBuildMachineState`

### 4.4 Ticket

典型结构：

```xml
<State>
  <Tickets>
    <TicketID>54321</TicketID>
  </Tickets>
</State>
```

来源：

- `up_UpdateBuildMachineState`
- `up_UpdateDropMachineState`

### 4.5 OnICE

典型结构：

```xml
<State>
  <Ice>
    <SessionID>112233</SessionID>
  </Ice>
</State>
```

来源：

- `up_UpdateBuildMachineState`

### 4.6 RequestInProgress

典型结构：

```xml
<State>
  <RequestInProgress>
    <RequestID>1001</RequestID>
    <Revision>25000.1</Revision>
  </RequestInProgress>
</State>
```

来源：

- `up_UpdateBuildMachineState`

### 4.7 Drop in progress

典型结构：

```xml
<State>
  <Drops>
    <Drop>
      <LabID>10</LabID>
      <Lab>Main</Lab>
      <Revision>25000.1</Revision>
    </Drop>
  </Drops>
</State>
```

来源：

- `up_UpdateDropMachineState`

### 4.8 No available drive letters

典型结构：

```xml
<State>
  <NoAvailableDrives></NoAvailableDrives>
</State>
```

来源：

- `up_UpdateDropMachineState`

---

## 5. 这两张表由谁来写

### 5.1 手工状态操作

相关过程：

1. `up_ResourceDeactivate`
2. `up_ResourceActivate`

#### `up_ResourceDeactivate`

作用：

- 给资源加上 `Inactive` 位
- 在 `RsData` 中写入 `<Inactive>` 节点
- 同时写一笔 `tbl_ResourceStateHistory`

#### `up_ResourceActivate`

作用：

- 去掉 `Inactive` 位
- 删除 `<Inactive>` 节点
- 如果去掉后没有剩余状态，则删除 `tbl_ResourceState` 当前行
- 同时写一笔 `tbl_ResourceStateHistory`

### 5.2 自动状态刷新

相关过程：

1. `up_UpdateBuildMachineState`
2. `up_UpdateDropMachineState`

它们会周期性重算机器当前状态，然后：

1. 更新 `tbl_ResourceState`
2. 如果状态发生变化，追加写入 `tbl_ResourceStateHistory`
3. 如果机器恢复为空闲，会删除当前表状态行，并在 history 里写一条 `0` 状态的 idle 记录

---

## 6. `up_UpdateBuildMachineState` 的整段状态合成逻辑

下面按中文步骤拆开。

### Step 1. 先找出所有 build machines

它先从 `vw_ResourceDADServers` 里取：

- `ResourceTypeID = 1`
- `ResourceCategoryID = 1`

也就是所有 build machine。

结果放入：

- `#Servers`

这一步相当于建立一个“全量基线集合”。

### Step 2. 找最近 14 天相关 session

它从 LabStatus `tbl_Session` 取最近 14 天、`SeBuildMachine` 非空的 session，构造：

- `RecentSessions`

然后再逐步整理出：

1. 当前正在构建的机器
2. 受这些构建影响的机器

### Step 3. 识别正在构建中的机器

它通过 `AllBuildMachines`、`BuildingMachines`、`AllAffectedBuildMachines` 这些 CTE，找出：

1. 当前 build machine 正在跑的 build
2. 同一个 lab/revision/locale set 下受影响的关联机器

这些机器会被赋状态：

- `8 = BuildInProgress`

并生成类似：

```xml
<Build><SessionID>...</SessionID><LabID>...</LabID><Lab>...</Lab><Revision>...</Revision></Build>
```

的数据片段。

### Step 4. 处理“可提前释放”的机器

它不会简单地认为“只要 session 没结束就一直占着机器”。

中间会构造：

- `BuildSummary`
- `#MachinesToBeReleased`

用来判断某些 lab 是否允许 partial release。

例如：

1. CHK 都完成了，可以释放对应机器
2. RET/COV 都完成了，可以释放对应机器

被判定可释放的机器会从：

- `#AllAffectedBuildMachines`

里删掉。

这意味着：

- 即使某些 build 集合整体还没完全结束，部分机器也可能提前从 `BuildInProgress` 状态中释放出来。

### Step 5. 找出 Open Issues

它会取最近 30 天内、未关闭、未删除、未重复、非 test 的 issue，形成：

- `OpenIssues`

然后把两类情况都转成机器状态：

1. 机器名直接命中的 issue
2. session 关联 issue 反推到 build machine

这些机器会被赋状态：

- `16 = OpenIssues`

### Step 6. 找出 Offline / Maintenance / Imaging / Tickets

它从 DAD 和 TechEase 收集：

1. `tbl_ServerMonitoringSuspend`
   - `1` 或 `2`，即 Imaging / Maintenance
2. `vw_Alert`
   - `4`，即 Offline 类 alert
3. `tbl_Server` 中 monitoring disabled
   - `4`
4. TechEase 未关闭 ticket
   - `32 = OpenTickets`

这些形成：

- `OfflineMachines`

### Step 7. 找出 OnICE 和 RequestInProgress

它还会补充两类 build machine 特有状态：

1. `OnICE = 64`
   - 来自 `tbl_Session.SeCanRecycle = 0`
2. `RequestInProgress = 512`
   - 来自 LaunchPad request/resource usage
   - 典型是 validation/Lopez request 已经分配了机器但 request 还在进行中

### Step 8. 合并成一张“状态片段表”

前面所有来源最终合并成：

- `#ServerStates`

每一行都长这样：

1. `ServerID`
2. `SeName`
3. `StatusID`
4. `Data`

这一步还不是最终 XML，只是按状态种类先拆散的片段表。

### Step 9. 按机器逐台合成最终状态

然后过程会 cursor 遍历 `#Servers`，对每台机器做：

1. 读取旧的 `RsData`
2. 保留旧 XML 里的 `<Inactive>` 节点
3. 从 `#ServerStates` 里按状态位逐类收集片段
4. 把片段包成：
   - `<Imaging>`
   - `<Maintenance>`
   - `<Alerts>`
   - `<Builds>`
   - `<Issues>`
   - `<Tickets>`
5. 再额外处理：
   - `64 = OnICE`，片段本身已经是 `<Ice>...</Ice>`
   - `512 = RequestInProgress`，片段本身已经是 `<RequestInProgress>...</RequestInProgress>`
   - `128 = Inactive`，从旧状态保留下来
6. 把所有片段拼成一个完整：

```xml
<State> ... </State>
```

同时把所有状态位按位或，得到最终 `@ServerStatusID`。

### Step 10. 写回当前状态表

如果最终 `@ServerData` 为空：

1. 删除 `tbl_ResourceState` 当前行
2. 如果确实删掉了旧行，就向 history 写一条：
   - `RsStatusID = 0`
   - `RsData = '<State></State>'`

如果最终 `@ServerData` 不为空：

1. 更新现有 `tbl_ResourceState`
2. 如果原来没有当前行，则插入一行新记录

### Step 11. 状态变化才写 History

最后还有一个关键判断：

- 只有当新状态不为 0，且新 XML 与旧 XML 不同时，才写一笔 `tbl_ResourceStateHistory`

所以 `tbl_ResourceStateHistory` 记录的是：

- 状态变化事件

而不是每次任务调度都盲目刷一遍。

---

## 7. `up_UpdateDropMachineState` 和 build machine 的区别

`up_UpdateDropMachineState` 结构很像，但关注点不一样。

它更关心：

1. `DropInProgress = 256`
2. `NoAvailableDriveLetters = 1024`
3. Imaging / Maintenance / Offline / Tickets
4. 手工 `Inactive = 128`

而 build machine 那条更关心：

1. `BuildInProgress = 8`
2. `OpenIssues = 16`
3. `OnICE = 64`
4. `RequestInProgress = 512`
5. partial release 逻辑

---

## 8. 这两张表在 LaunchPad 里被哪些功能依赖

### 8.1 管理页显示状态

`up_ServerAssignmentsGet` 会左联 `tbl_ResourceState`，所以 ManageServerPools 页面能看到机器当前状态。

### 8.2 资源候选过滤

以下选择逻辑都会用当前状态表过滤机器：

1. `up_GetBuildMachineSelection`
2. `up_GetDropMachineSelection`
3. `up_ResourceGetByPool`
4. `up_ResourceGet`
5. `up_ResourceGetAll`

### 8.3 资源对象状态展示

C# 里 `ResourceStatus` 会读取：

1. `RsStatusID`
2. `RsData`

然后挂到 `Resource.Status` 上。

对应文件：

- `q:\dd\LaunchPad\src\LaunchPad.Components\ResourceStatus.cs`

---

## 9. 最短理解

如果只记最短版本，可以记成：

1. `tbl_ResourceState`
   - 当前状态快照
   - 一台资源最多一行
2. `tbl_ResourceStateHistory`
   - 状态变化历史
   - 每次变化追加一行
3. `up_ResourceActivate / up_ResourceDeactivate`
   - 手工修改 `Inactive` 状态
4. `up_UpdateBuildMachineState / up_UpdateDropMachineState`
   - 自动重算当前状态
5. LaunchPad 后续所有“这台机器还能不能被选”的逻辑，都会先看 `tbl_ResourceState`

---

## 10. 一句话总结

在 LaunchPad 里，`tbl_ResourceState` 是资源当前可用性和占用情况的统一事实表，`tbl_ResourceStateHistory` 是这份事实表的变更审计日志；自动选机、管理页展示、资源过滤、运行保护，最终都建立在这两张表之上。