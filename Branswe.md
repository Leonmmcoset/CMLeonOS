# Branswe编程语言使用指南

## 简介

Branswe是一种简单的编程语言，用于CMLeonOS系统中进行脚本编写和自动化操作。它提供了基本的控制台操作、变量管理、条件判断、循环、文件系统操作等功能。

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

### 定义文本变量
```
var(text) 变量名 = 值
```

### 变量赋值
```
var() 变量名 = 变量2
```

### 变量删除
```
var() 变量名 rm
```

### 变量运算
```
var() 变量名 =+ 变量2
var() 变量名 =- 变量2
var() 变量名 =* 变量2
var() 变量名 =/ 变量2
```

### 获取变量值
```
ref getvar 变量名
```

## 控制台操作

### 显示文本（不换行）
```
conshow 变量名
```

### 显示文本（换行）
```
conshowl 变量名
```

### 清屏
```
concls
```

### 输入
```
coninput 变量名
```

### 蜂鸣
```
conbeep
```

## 系统引用

### 鼠标位置
```
ref mousex
ref mousey
```

### 屏幕尺寸
```
ref screenx
ref screeny
```

### 控制台颜色
```
ref concolour-b
ref concolour-f
```

### 磁盘信息
```
ref getalldisks
```

## 条件判断

### decide语句
```
decide 变量1 操作符 变量2
```

支持的操作符：
- `==` 等于
- `!=` 不等于
- `>` 大于
- `<` 小于
- `>=` 大于等于
- `<=` 小于等于

### if语句
```
if 条件 then 真代码 else 假代码
```

### 示例
```
var(text) num1 = 10
var(text) num2 = 20
decide num1 == num2
if [] then conshow match else conshow no match
```

## 循环

### loop语句
```
loop << 变量名
```

执行变量中的代码，无限循环。

### 示例
```
var(text) code = conshow hello
loop << code
```

## 字符串操作

### 读取字符串并执行
```
rstr 变量名
```

执行变量中存储的代码（支持\n换行）。

### 示例
```
var(text) mycode = conshow hello\nconshow world
rstr mycode
```

## 文件系统操作

### 注册VFS
```
diskfile reg
```

### 创建文件
```
diskfile create file 路径
```

### 创建目录
```
diskfile create dir 路径
```

### 写入文件
```
diskfile write 内容 to 路径
```

### 示例
```
diskfile reg
diskfile create file /test.txt
var(text) content = Hello, World!
diskfile write content to /test.txt
```

## 系统功能

### 睡眠
```
sleep 变量名
```

### 电源管理
```
power off
power reboot
```

### 结束程序
```
end
```

## 方法定义

### 定义方法
```
method 变量部分 << 代码部分
```

### 调用方法
```
方法名 参数
```

### 示例
```
var(text) printname = print
var(text) myname = Leon
method printname [] << conshow []
printname myname
```

## 完整示例

### 示例1：Hello World
```
# 简单的Hello World程序
var(text) hello = Hello, Branswe!
conshow hello
```

### 示例2：变量操作
```
# 变量定义和运算
var(text) num1 = 10
var(text) num2 = 20
var() num1 =+ num2
conshowl num1
```

### 示例3：条件判断
```
# 条件判断示例
var(text) a = 10
var(text) b = 20
decide a < b
if [] then conshow a is smaller else conshow a is larger
```

### 示例4：循环
```
# 循环示例
var(text) code = conshow loop\nsleep 1000
loop << code
```

### 示例5：文件操作
```
# 文件操作示例
diskfile reg
diskfile create file /test.txt
var(text) content = Hello, File System!
diskfile write content to /test.txt
```

### 示例6：系统功能
```
# 系统功能示例
conbeep
var(text) wait = 1000
sleep wait
ref mousex
conshowl []
```

### 示例7：自定义方法
```
# 自定义方法示例
var(text) greet = greet
var(text) name = Leon
method greet [] << conshow Hello, []!
greet name
```

## 注意事项

1. **大小写敏感**：命令和变量名区分大小写
2. **空格处理**：参数之间用空格分隔
3. **变量存储**：所有变量都作为文本存储，运算时会自动转换
4. **注释支持**：使用#号添加注释
5. **变量作用域**：变量在整个脚本中有效
6. **无限循环**：loop命令会无限循环，需要使用end或其他方式退出
7. **文件系统**：使用diskfile前需要先执行diskfile reg注册VFS

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

### 版本2.0
- 更新了所有命令的实际语法
- 添加了文件系统操作支持
- 添加了系统引用命令
- 添加了电源管理功能
- 添加了自定义方法功能
- 修正了条件判断和循环的语法
- 完善了变量操作命令

### 版本1.0
- 初始版本
- 添加了CMLeonOS兼容性支持
- 完善了错误处理
