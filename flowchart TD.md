# Tag 清理流程

> [!TIP]
> 每批最多清理 5 个；达到 maxCleanupCount 后立即停止。

## 流程图
```mermaid
flowchart TD
    A[开始] --> B[遍历每个 str]
    B --> C[按 / 拆分: strs]
    C --> D{strs.Count >= 3?}

    D -- 否 --> D1[记录日志: Ignore str]
    D -- 是 --> E[取 first second third]
    E --> F{first=drop 且 second不在exempts 且 third以o或i开头?}

    F -- 是 --> G{third 以 o 开头?}
    G -- 是 --> G1[cutoffDate = 当前时间 - 官方保留月数]
    G -- 否 --> G2[cutoffDate = 当前时间 - 集成保留月数]
    G1 --> H[Get-TagDate 获取 tagDate]
    G2 --> H
    H --> I{tagDate < cutoffDate?}
    I -- 是 --> I1[tagsArr 加入 str.Trim]
    I -- 否 --> P

    F -- 否 --> J{str 以 o/v/s 开头?}
    J -- 否 --> P
    J -- 是 --> K[dropTag = dropTags 中匹配 Name like str]
    K --> L{dropTag 为空?}
    L -- 是 --> L1[tagsArr 加入 str.Trim]
    L -- 否 --> M[取 dropTagDate 与当前UTC时间]
    M --> N{非 RetainedForever 且 dropTagDate < 当前UTC?}
    N -- 是 --> N1[tagsArr 加入 str.Trim]
    N -- 否 --> P

    D1 --> P
    I1 --> P
    L1 --> P
    N1 --> P

    P --> Q{tagsArr.Count >= 5?}
    Q -- 是 --> R[curCleanupCount += tagsArr.Count]
    R --> S[Remove-Tags tagsArr]
    S --> T[tagsArr 重置为空]
    Q -- 否 --> U{curCleanupCount >= maxCleanupCount?}
    T --> U

    U -- 是 --> V[记录Warning并 break]
    U -- 否 --> B

    B -->|遍历结束| W[循环结束后]
    V --> W
    W --> X{tagsArr 还有剩余?}
    X -- 是 --> Y[Remove-Tags tagsArr ]
    X -- 否 --> Z[结束]
    Y --> Z
```


## 规则速览
| 场景 | 条件 | 动作 |
|---|---|---|
| drop 路径 | 符合 drop + 非豁免 + o/i 前缀 + 过期 | 删除 |
| o/v/s 标签 | 不存在对应 drop 或保留已过期 | 删除 |
| 其他 | 不满足规则 | 忽略 |

<details>
<summary>说明</summary>

- o 前缀按官方保留月数计算 cutoffDate
- i 前缀按集成保留月数计算 cutoffDate
- 循环结束后若队列有剩余，做最后一次 Remove-Tags

</details>