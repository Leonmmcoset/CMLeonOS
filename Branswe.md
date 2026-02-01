# Branswe编程语言使用指南

## 简介

Branswe是一种简单的编程语言，用于CMLeonOS系统中进行脚本编写和自动化操作。它提供了基本的控制台操作、变量管理、条件判断等功能。

## 基本语法

### 命令格式
```
命令名 [参数]
```

### 注释
```
# 这是注释
```

## 变量操作

### 定义变量
```
var() 变量名 = 值
```

### 定义文本变量
```
var(text) 变量名 = 文本内容
```

### 显示变量
```
conshow 变量名
```

### 显示变量列表
```
conshowl
```

## 控制台操作

### 清屏
```
concls
```

### 输入
```
coninput 提示文本
```

### 显示文本
```
conshow 文本内容
```

## 条件判断

### if语句
```
if 条件
then
    条件为真时执行的代码
else
    条件为假时执行的代码
end
```

### 示例
```
if hello == world
then
    conshow match
else
    conshow no match
end
```

## 循环

### loop语句
```
loop 次数
    循环体代码
end
```

### 示例
```
loop 5
    conshow count: i
    var(text) count = i
    conshow count: count
end
```

## 字符串操作

### 读取字符串
```
rstr 变量名
```

### 示例
```
var() myString = Hello, World!
rstr myString
conshow myString
```

## 数学运算

### 加法
```
var(text) result = num1
var(text) num1 = 10
var(text) num2 = 20
rstr result + num1 + num2
conshow result
```

### 减法
```
var(text) result = num1
var(text) num1 = 10
var(text) num2 = 20
rstr result + num1 - num2
conshow result
```

### 乘法
```
var(text) result = num1
var(text) num1 = 10
var(text) num2 = 20
rstr result + num1 * num2
conshow result
```

### 除法
```
var(text) result = num1
var(text) num1 = 10
var(text) num2 = 20
rstr result + num1 / num2
conshow result
```

## 系统功能

### 扬声器
```
conbeep
```

### 睡眠
```
sleep 毫秒数
```

### 获取磁盘信息
```
getalldisks
```

## 方法定义

### 定义方法
```
method 方法名 << 参数名 >> 代码
```

### 示例
```
method print << name >> conshow name
```

## 完整示例

### 示例1：Hello World
```
# 简单的Hello World程序
conshow hello
var(text) hello = Hello, Branswe!
conshow hello
```

### 示例2：变量操作
```
# 变量定义和显示
var() name = LeonOS
var(text) greeting = Hello, name!
conshow greeting
```

### 示例3：条件判断
```
# 条件判断示例
if greeting == Hello, name!
then
    conshow match
else
    conshow no match
end
```

### 示例4：循环
```
# 循环示例
loop 5
    conshow count: i
    var(text) count = i
    conshow count: count
end
```

### 示例5：数学运算
```
# 数学运算示例
var(text) result = 10
var(text) num1 = 5
var(text) num2 = 3
rstr result + num1 * num2
conshow result
```

### 示例6：系统功能
```
# 系统功能示例
conbeep
sleep 1000
getalldisks
```

## 与CMLeonOS的集成

### 在CMLeonOS中使用
```
branswe example.bran
```

### 支持的命令
- `cat` - 显示文件内容
- `echo` - 写入文件
- `ls` - 列出目录
- `pwd` - 显示当前目录
- `mkdir` - 创建目录
- `rm` - 删除文件
- `rmdir` - 删除目录

## 注意事项

1. **大小写敏感**：命令和变量名区分大小写
2. **空格处理**：参数之间用空格分隔
3. **错误处理**：不支持的命令会显示错误消息
4. **注释支持**：使用#号添加注释
5. **变量作用域**：变量在整个脚本中有效
6. **条件嵌套**：支持多层条件判断
7. **循环嵌套**：支持多层循环

## 最佳实践

1. **添加注释**：在复杂逻辑前添加注释说明
2. **使用变量**：避免硬编码，使用变量提高灵活性
3. **错误检查**：在使用变量前检查其值
4. **代码格式**：保持代码缩进和格式一致
5. **测试验证**：逐步测试每个功能确保正确性

## 参考资料

- CMLeonOS Shell命令
- Branswe编程语言规范
- Cosmos系统文档

## 更新日志

### 版本1.0
- 初始版本
- 添加了CMLeonOS兼容性支持
- 完善了错误处理
- 添加了文件操作支持
