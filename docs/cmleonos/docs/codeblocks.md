# CodeBlocks 积木文档

`CodeBlocks` 是 CMLeonOS GUI 桌面的可视化编程应用。  
本文档列出当前版本所有积木的参数和执行行为。

## 参数说明

- `A`：整数参数（来自 `argA`）
- `B`：整数参数（来自 `argB`）
- `text`：文本参数（来自输入框，可作为字符串或变量名）

## 基础输出

### Say (text)
- 功能：输出文本到输出区
- 行为：`[Say] <text>`

### Say Counter
- 功能：输出计数器变量 `counter`
- 行为：`[Counter] <value>`

### Say Var(text)
- 功能：输出变量 `text` 的值（`text` 作为变量名）
- 行为：`[Var] <name> = <value>`

### Clear Output
- 功能：清空输出区
- 行为：重置为初始提示行

## Counter 运算

### Set Counter (A)
- 功能：设置 `counter = A`

### Add Counter (A)
- 功能：`counter += A`

### Sub Counter (A)
- 功能：`counter -= A`

### Mul Counter (A)
- 功能：`counter *= A`

### Div Counter by A
- 功能：`counter /= A`（整数除法）
- 说明：当 `A = 0` 时内部自动按 `1` 处理，避免错误

### Counter %= A
- 功能：`counter %= A`
- 说明：当 `A = 0` 时内部自动按 `1` 处理

### Counter += A*B
- 功能：`counter += A * B`

### Counter = abs(Counter)
- 功能：`counter = |counter|`

### Counter = random(A..B)
- 功能：将 `counter` 设为区间 `[A, B]` 内随机值
- 说明：若 `A > B`，会自动交换区间边界

## 变量运算

### Set Var(text)=A
- 功能：设置变量 `text = A`
- 说明：变量名不区分大小写；空变量名会回退为 `x`

### Add Var(text)+=A
- 功能：`text += A`

### Mul Var(text)*=A
- 功能：`text *= A`

### Swap Counter <-> Var(text)
- 功能：交换 `counter` 与变量 `text` 的值

## 流程与条件

### Wait (ms=A)
- 功能：等待 `A` 毫秒
- 说明：范围会限制在 `0..2000ms`

### Repeat Say (A times)
- 功能：重复输出 `text` 共 `A` 次
- 说明：执行次数会限制在 `0..100`

### If Counter > A -> Say(text)
- 功能：当 `counter > A` 时输出 `text`

### If Counter == A -> Say(text)
- 功能：当 `counter == A` 时输出 `text`

## 系统动作

### Beep
- 功能：执行一次系统蜂鸣器提示音

## 工程文件

`CodeBlocks` 工程文件扩展名为 `.cblk`，支持：

- New
- Load
- Save
- Save As

当前格式头为 `CBLK1`，每行保存一个积木记录（类型与参数）。
