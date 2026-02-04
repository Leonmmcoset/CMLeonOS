# CMLeonOS Lua API 文档

本文档记录了 CMLeonOS 中 Lua 支持的所有函数和库。

## 基础库 (base)

### assert(v [, message])
如果 v 为 false 或 nil，则抛出错误。可选参数 message 为错误信息。

```lua
assert(true, "This should be true")
assert(false, "This is false")
```

### collectgarbage([opt])
垃圾回收控制。支持以下选项：
- `"collect"` - 执行垃圾回收
- `"count"` - 返回当前内存使用量（千字节）
- `"step"` - 执行一步垃圾回收
- `"isrunning"` - 返回垃圾回收器是否在运行

```lua
collectgarbage("collect")
collectgarbage("count")
```

### dofile([filename])
加载并执行指定文件。

```lua
dofile("script.lua")
```

### input([prompt])
从控制台读取一行输入。

```lua
local name = input("Enter your name: ")
print("Hello, " .. name)
```

### error(message [, level])
抛出错误并终止当前受保护的调用。

```lua
error("Something went wrong")
```

### ipairs(t)
遍历数组，返回迭代器函数。

```lua
for i, v in ipairs({1, 2, 3}) do
    print(i, v)
end
```

### loadfile([filename])
加载文件但不执行，返回函数或 nil, error。

```lua
local func, err = loadfile("script.lua")
if func then
    func()
end
```

### load(string [, chunkname])
加载字符串但不执行，返回函数或 nil, error。

```lua
local func = load("print('Hello')")
if func then
    func()
end
```

### loadstring(string [, chunkname])
加载字符串但不执行，返回函数或 nil, error。

```lua
local func = loadstring("print('Hello')")
if func then
    func()
end
```

### next(table [, index])
返回表的下一个键值对。

```lua
t = {a = 1, b = 2}
local k, v = next(t)
print(k, v)  -- a, 1
```

### pairs(t)
遍历表，返回迭代器函数。

```lua
t = {x = 1, y = 2, z = 3}
for k, v in pairs(t) do
    print(k, v)
end
```

### pcall(f [, arg1, ...])
安全调用函数，捕获错误。

```lua
local status, err = pcall(function()
    error("Something went wrong")
end)
if not status then
    print(err)
end
```

### print(...)
打印参数到控制台。

```lua
print("Hello, World!")
print("Value:", 42)
```

### rawequal(v1, v2)
比较两个值是否相等（不使用元方法）。

```lua
print(rawequal(1, 1))  -- true
print(rawequal(1, 2))  -- false
```

### rawlen(v)
返回值的长度（不使用元方法）。

```lua
print(rawlen("hello"))  -- 5
```

### rawget(table, index)
获取表的值（不使用元方法）。

```lua
t = setmetatable({}, {__index = function() return "default" end})
print(rawget(t, "key"))  -- default
```

### rawset(table, index, value)
设置表的值（不使用元方法）。

```lua
t = {}
rawset(t, "key", "value")
```

### select(index, ...)
返回第 index 个参数。

```lua
print(select(2, "a", "b", "c"))  -- c
```

### getmetatable(object)
返回对象的元表。

```lua
t = {}
mt = {__add = function(a, b) return a + b end}
setmetatable(t, mt)
print(getmetatable(t).__add)  -- function
```

### setmetatable(table, metatable)
设置对象的元表。

```lua
t = {}
mt = {__add = function(a, b) return a + b end}
setmetatable(t, mt)
```

### tonumber(e [, base])
将值转换为数字。

```lua
print(tonumber("123"))  -- 123
print(tonumber("FF", 16))  -- 255
```

### tostring(v)
将值转换为字符串。

```lua
print(tostring(123))  -- "123"
print(tostring({}))  -- "table: 0x..."
```

### type(v)
返回值的类型。

```lua
print(type(123))  -- number
print(type("hello"))  -- string
print(type({}))  -- table
print(type(nil))  -- nil
```

### xpcall(f [, arg1, ...])
扩展的 pcall，返回状态和错误。

```lua
local ok, err = xpcall(function()
    error("Something went wrong")
end)
if not ok then
    print(err)
end
```

---

## 数学库 (math)

### math.abs(x)
返回 x 的绝对值。

```lua
print(math.abs(-5))  -- 5
print(math.abs(5))   -- 5
```

### math.acos(x)
返回 x 的反余弦值（弧度）。

```lua
print(math.acos(0))  -- 1.570796
```

### math.asin(x)
返回 x 的反正弦值（弧度）。

```lua
print(math.asin(0))  -- 0
```

### math.atan2(y, x)
返回 y/x 的反正切值（弧度）。

```lua
print(math.atan2(0, 1))  -- 0
```

### math.atan(x)
返回 x 的反正切值（弧度）。

```lua
print(math.atan(1))  -- 0.785398
```

### math.ceil(x)
返回不小于 x 的最小整数。

```lua
print(math.ceil(3.5))  -- 4
print(math.ceil(-3.5))  -- -3
```

### math.cosh(x)
返回 x 的双曲余弦值。

```lua
print(math.cosh(0))  -- 1
```

### math.cos(x)
返回 x 的余弦值（弧度）。

```lua
print(math.cos(0))  -- 1
print(math.cos(math.pi))  -- -1
```

### math.deg(x)
将弧度转换为角度。

```lua
print(math.deg(math.pi))  -- 180
```

### math.exp(x)
返回 e 的 x 次方。

```lua
print(math.exp(1))  -- 2.718282
```

### math.floor(x)
返回不大于 x 的最大整数。

```lua
print(math.floor(3.5))  -- 3
print(math.floor(-3.5))  -- -4
```

### math.fmod(x, y)
返回 x 除以 y 的余数。

```lua
print(math.fmod(7, 3))  -- 1
```

### math.frexp(x)
将 x 分解为尾数和指数。

```lua
local m, e = math.frexp(1.5)
print(m, e)
```

### math.ldexp(m, e)
返回 m 乘以 2 的 e 次方。

```lua
print(math.ldexp(1, 2))  -- 4
```

### math.log10(x)
返回 x 的以 10 为底的对数。

```lua
print(math.log10(100))  -- 2
```

### math.log(x [, base])
返回 x 的对数。默认以 e 为底。

```lua
print(math.log(10))  -- 2.302585
print(math.log(100, 10))  -- 2
```

### math.max(x1, ...)
返回参数中的最大值。

```lua
print(math.max(1, 2, 3))  -- 3
print(math.max(-5, -2, -1))  -- -1
```

### math.min(x1, ...)
返回参数中的最小值。

```lua
print(math.min(1, 2, 3))  -- 1
print(math.min(-5, -2, -1))  -- -5
```

### math.modf(x)
将 x 分解为整数和小数部分。

```lua
local i, f = math.modf(3.5)
print(i, f)
```

### math.pow(x, y)
返回 x 的 y 次方。

```lua
print(math.pow(2, 3))  -- 8
print(math.pow(10, 2))  -- 100
```

### math.rad(x)
将角度转换为弧度。

```lua
print(math.rad(180))  -- 3.141593
```

### math.random([m [, n]])
返回随机数。无参数返回 [0,1)，一个参数返回 [1,m]，两个参数返回 [m,n]。

```lua
print(math.random())      -- 0 或 1
print(math.random(10))   -- 1-10
print(math.random(5, 10))  -- 5-10
```

### math.randomseed(x)
设置随机数生成器的种子。

```lua
math.randomseed(os.time())
print(math.random())
```

### math.sinh(x)
返回 x 的双曲正弦值。

```lua
print(math.sinh(0))  -- 0
```

### math.sin(x)
返回 x 的正弦值（弧度）。

```lua
print(math.sin(0))  -- 0
print(math.sin(math.pi/2))  -- 1
```

### math.sqrt(x)
返回 x 的平方根。

```lua
print(math.sqrt(16))  -- 4
print(math.sqrt(2))   -- 1.414214
```

### math.tanh(x)
返回 x 的双曲正切值。

```lua
print(math.tanh(0))  -- 0
```

### math.tan(x)
返回 x 的正切值（弧度）。

```lua
print(math.tan(0))  -- 0
print(math.tan(math.pi/4))  -- 1
```

### math.pi
圆周率常量。

```lua
print(math.pi)  -- 3.141592653589793
```

### math.huge
表示最大数值的常量。

```lua
print(math.huge)
```

---

## 字符串库 (string)

### string.byte(s [, i [, j]])
返回字符的内部数值表示。

```lua
print(string.byte("ABC"))  -- 65
print(string.byte("ABC", 2))  -- 66
print(string.byte("ABC", -1))  -- 67
```

### string.char(...)
将数值转换为字符。

```lua
print(string.char(65, 66, 67))  -- ABC
```

### string.dump(function)
返回函数的二进制表示。

```lua
print(string.dump(print))
```

### string.find(s, pattern [, init [, plain]])
在字符串中查找模式。

```lua
print(string.find("hello world", "world"))  -- 7
print(string.find("hello world", "l"))  -- 3
```

### string.format(formatstring, ...)
格式化字符串。

```lua
print(string.format("Value: %d", 42))
print(string.format("Name: %s, Age: %d", "Alice", 30))
```

### string.gmatch(s, pattern)
返回字符串中所有匹配模式的迭代器。

```lua
for word in string.gmatch("hello world", "%a+") do
    print(word)
end
```

### string.gsub(s, pattern, repl [, n])
替换字符串中的模式。

```lua
print(string.gsub("hello world", "world", "Lua"))  -- hello Lua
print(string.gsub("hello world", "l", "L"))  -- HeLLo Lua
```

### string.len(s)
返回字符串长度。

```lua
print(string.len("hello"))  -- 5
```

### string.lower(s)
将字符串转换为小写。

```lua
print(string.lower("HELLO"))  -- hello
```

### string.match(s, pattern [, init])
返回字符串中第一个匹配模式的子串。

```lua
print(string.match("hello world", "%w+"))  -- hello
```

### string.rep(s, n)
重复字符串 n 次。

```lua
print(string.rep("ab", 3))  -- ababab
```

### string.reverse(s)
反转字符串。

```lua
print(string.reverse("hello"))  -- olleh
```

### string.sub(s, i [, j])
返回字符串的子串。

```lua
print(string.sub("hello world", 1, 5))  -- hello
print(string.sub("hello world", 7))  -- world
```

### string.upper(s)
将字符串转换为大写。

```lua
print(string.upper("hello"))  -- HELLO
```

---

## 操作系统库 (os)

### os.clock()
返回程序使用的 CPU 时间（秒）。

```lua
print(os.clock())
```

### os.gethostname()
返回系统主机名。

```lua
print(os.gethostname())
```

### os.getenv(varname)
返回环境变量的值。

```lua
print(os.getenv("PATH"))
```

### os.setenv(varname, value)
设置环境变量的值。

```lua
os.setenv("MYVAR", "value")
```

### os.delenv(varname)
删除环境变量。

```lua
os.delenv("MYVAR")
```

### os.addenv(varname, value)
添加环境变量（同 setenv）。

```lua
os.addenv("MYVAR", "value")
```

### os.execute(command)
执行 Shell 命令。

```lua
os.execute("ls")
os.execute("echo Hello")
```

### os.executefile(path)
执行命令文件（.cm）。

```lua
os.executefile("myscript.cm")
```

### os.reboot()
重启系统。

```lua
os.reboot()
```

### os.shutdown()
关闭系统。

```lua
os.shutdown()
```

### os.sleep(seconds)
休眠指定秒数。

```lua
os.sleep(5)
```

### os.beep()
发出蜂鸣声。

```lua
os.beep()
```

### os.clear()
清屏。

```lua
os.clear()
```

### os.getusername()
返回当前登录用户名。

```lua
print(os.getusername())
```

### os.isadmin()
检查当前用户是否是管理员。

```lua
if os.isadmin() then
    print("User is admin")
else
    print("User is not admin")
end
```

### os.sha256(input)
计算字符串的 SHA256 哈希值。

```lua
local hash = os.sha256("Hello World")
print(hash)
```

### os.base64encrypt(input)
将字符串编码为 Base64。

```lua
local encoded = os.base64encrypt("Hello World")
print(encoded)
```

### os.base64decrypt(input)
将 Base64 字符串解码。

```lua
local decoded = os.base64decrypt("SGVsbG8gV29ybGxcmVvcmxvYg==")
print(decoded)
```

---

## 输入输出库 (io)

### io.close(file)
关闭文件。

```lua
local file = io.open("test.txt", "w")
file:write("Hello")
io.close(file)
```

### io.flush()
刷新所有打开文件的输出缓冲区。

```lua
io.flush()
```

### io.input([file])
设置默认输入文件。

```lua
io.input("input.txt")
```

### io.lines([filename])
返回文件行的迭代器。

```lua
for line in io.lines("test.txt") do
    print(line)
end
```

### io.open(filename [, mode])
打开文件，返回文件句柄。

```lua
local file = io.open("test.txt", "r")
local content = file:read("*a")
file:close()
```

### io.output([file])
设置默认输出文件。

```lua
io.output("output.txt")
```

### io.popen(prog [, mode])
启动程序并返回文件句柄。

```lua
local file = io.popen("ls", "r")
local output = file:read("*a")
file:close()
```

### io.read(...)
从默认输入文件读取数据。

```lua
local input = io.read()
print(input)
```

### io.tmpfile()
返回临时文件的句柄。

```lua
local file = io.tmpfile()
file:write("temp data")
file:close()
```

### io.type(file)
检查文件是否是打开的文件句柄。

```lua
local file = io.open("test.txt", "r")
print(io.type(file))  -- file
```

### io.write(...)
写入数据到默认输出文件。

```lua
io.write("Hello, World!\n")
```

---

## 协程库 (coroutine)

### coroutine.create(f)
创建新的协程。

```lua
co = coroutine.create(function()
    print("Coroutine started")
end)
```

### coroutine.resume(co [, val1, ...])
继续或启动协程。

```lua
coroutine.resume(co)
```

### coroutine.running()
返回当前运行的协程。

```lua
print(coroutine.running())
```

### coroutine.status(co)
返回协程的状态。

```lua
print(coroutine.status(co))  -- running, suspended, normal, dead
```

### coroutine.wrap(f)
创建包装函数的函数。

```lua
co = coroutine.create(function()
    print("Coroutine")
end)
local wrapped = coroutine.wrap(co)
wrapped()
```

### coroutine.yield(...)
挂起当前协程。

```lua
coroutine.yield("value")
```

---

## 表库 (table)

### table.concat(list [, sep [, i [, j]])
连接表中的元素。

```lua
t = {"a", "b", "c"}
print(table.concat(t, ", "))  -- a, b, c
```

### table.insert(list, [pos,] value)
在表中插入元素。

```lua
t = {1, 2, 3}
table.insert(t, 4)
```

### table.maxn(t)
返回表的最大数字索引（已弃用，使用 # 运算符）。

```lua
t = {1, 2, nil, 4}
print(table.maxn(t))  -- 4
```

### table.remove(list [, pos])
从表中删除元素。

```lua
t = {1, 2, 3}
table.remove(t, 2)
```

### table.sort(list [, comp])
对表进行排序。

```lua
t = {3, 1, 4, 2}
table.sort(t)
```

---

## 调试库 (debug)

### debug.debug()
进入调试模式。

```lua
debug.debug()
```

### debug.getfenv(object)
返回对象的环境。

```lua
print(debug.getfenv(function() return x end))
```

### debug.gethook()
返回当前的钩子函数。

```lua
print(debug.gethook())
```

### debug.getinfo([thread [, f [, what]])
返回函数的信息。

```lua
print(debug.getinfo(1))
```

### debug.getlocal([thread [, f [, loc]])
返回函数的局部变量。

```lua
print(debug.getlocal(1, 1))
```

### debug.getmetatable(object)
返回对象的元表。

```lua
t = {}
mt = {__add = function(a, b) return a + b end}
setmetatable(t, mt)
print(debug.getmetatable(t).__add)
```

### debug.getregistry()
返回注册表。

```lua
print(debug.getregistry())
```

### debug.getupvalue(f [, i])
返回函数的上值。

```lua
print(debug.getupvalue(1))
```

### debug.setfenv(object, table)
设置对象的环境。

```lua
debug.setfenv(function() return x end, {})
```

### debug.sethook(hook, mask [, count])
设置钩子函数。

```lua
debug.sethook(function() print("Hook called") end, "c")
```

### debug.setlocal([thread [, f,] level, value)
设置函数的局部变量。

```lua
debug.setlocal(1, 1, "value")
```

### debug.setmetatable(object, table)
设置对象的元表。

```lua
debug.setmetatable(t, mt)
```

### debug.setupvalue(f [, i,] value)
设置函数的上值。

```lua
debug.setupvalue(1, 1, "newvalue")
```

### debug.traceback([thread [, message [, level]])
返回调用栈的跟踪信息。

```lua
print(debug.traceback())
```

---

## 位运算库 (bit)

### bit.band(x1, x2 [, ...])
按位与操作。

```lua
print(bit.band(0x0F, 0x0F))  -- 0x0F
```

### bit.bnot(x)
按位非操作。

```lua
print(bit.bnot(0x0F))  -- 0xFFFFFFF0
```

### bit.bor(x1, x2 [, ...])
按位或操作。

```lua
print(bit.bor(0x0F, 0x0F))  -- 0x0F
```

### bit.bxor(x1, x2 [, ...])
按位异或操作。

```lua
print(bit.bxor(0x0F, 0x0F))  -- 0x00
```

### bit.lshift(x, disp)
按位左移。

```lua
print(bit.lshift(1, 2))  -- 4
```

### bit.rshift(x, disp)
按位右移。

```lua
print(bit.rshift(4, 2))  -- 1
```

### bit.arshift(x, disp)
按位算术右移。

```lua
print(bit.arshift(4, 2))  -- 1
```

### bit.rol(x, disp)
循环左移。

```lua
print(bit.rol(0x0F, 4))  -- 0xF0
```

### bit.ror(x, disp)
循环右移。

```lua
print(bit.ror(0x0F, 4))  -- 0x0F
```

### bit.tobit(x)
转换为位字符串。

```lua
print(bit.tobit(0x0F))  -- 00001111
```

---

## 编码库 (enc)

### enc.base64encode(input)
将字符串编码为 Base64。

```lua
local encoded = enc.base64encode("Hello World")
print(encoded)
```

### enc.base64decode(input)
将 Base64 字符串解码。

```lua
local decoded = enc.base64decode("SGVsbG8gV29ybGxcmVvcmxvYg==")
print(decoded)
```

---
## JSON 库

### json.encode(table)
将 Lua 表编码为 JSON 字符串。

```lua
local data = {name = "John", age = 30}
local jsonStr = json.encode(data)
print(jsonStr)
```

### json.decode(string)
将 JSON 字符串解码为 Lua 表。

```lua
local jsonStr = '{"name": "John", "age": 30}'
local data = json.decode(jsonStr)
print(data.name)
print(data.age)
```

### json.null()
返回 null 值。

```lua
local nullValue = json.null()
print(nullValue)
```

### json.parse(string)
解析 JSON 字符串（同 decode）。

```lua
local jsonStr = '{"name": "John", "age": 30}'
local data = json.parse(jsonStr)
print(data.name)
```

### json.stringify(table)
将 Lua 表转换为 JSON 字符串（同 encode）。

```lua
local data = {name = "John", age = 30}
local jsonStr = json.stringify(data)
print(jsonStr)
```

---
## 包库 (package)

### package.loaded
已加载的包表。

```lua
print(package.loaded)
```

### package.loaders
包加载器表。

```lua
print(package.loaders)
```

### package.loadlib(libname [, initfunc])
加载 C 库。

```lua
package.loadlib("mylib")
```

### package.path
包搜索路径。

```lua
print(package.path)
```

### package.searchpath(name [, path [, sep [, rep]])
搜索包路径。

```lua
print(package.searchpath("mylib"))
```

### package.seeall([name])
列出所有已加载的包。

```lua
print(package.seeall())
```

---

## 使用示例

### 交互式 Lua Shell

使用 `lua --shell` 进入交互模式：

```
lua --shell
====================================
        Lua Interactive Shell
====================================
Type 'exit' or 'quit' to exit

lua> print("Hello, World!")
Hello, World!

lua> os.gethostname()
myserver

lua> os.sha256("Hello")
a591a6d40bf420404a011733cfb7b1d12e

lua> exit
Exiting Lua shell...
```

### 执行 Lua 脚本文件

```lua
lua script.lua
```

### 系统信息查询

```lua
print("Hostname:", os.gethostname())
print("Username:", os.getusername())
print("Is Admin:", os.isadmin())
print("SHA256:", os.sha256("Hello"))
```

### 环境变量操作

```lua
os.setenv("MYVAR", "value")
print(os.getenv("MYVAR"))
os.delenv("MYVAR")
```

### 加密和编码

```lua
local hash = os.sha256("password")
local encoded = os.base64encrypt("secret")
local decoded = os.base64decrypt(encoded)
print("Hash:", hash)
print("Encoded:", encoded)
print("Decoded:", decoded)
```

## 注意事项

1. **交互模式**：使用 `lua --shell` 进入交互式 Lua Shell
2. **错误处理**：所有 Lua 执行错误都会被捕获并显示
3. **系统函数**：`os` 库包含 CMLeonOS 特定的系统函数
4. **加密支持**：支持 SHA256 和 Base64 编码/解码
5. **标准 Lua**：完全兼容 Lua 5.1 标准

## 版本信息

- **Lua 版本**：5.2
- **CMLeonOS 版本**：最新
