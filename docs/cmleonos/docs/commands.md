# 命令列表

CMLeonOS 提供了丰富的命令行工具，以下是所有可用命令的详细说明。

## 系统命令

### echo
输出文本到控制台。

**用法：**
```bash
echo <text>
```

**示例：**
```bash
echo Hello World
```

### clear / cls
清空控制台屏幕。

**用法：**
```bash
clear
# 或
cls
```

### time
显示当前时间。

**用法：**
```bash
time
```

### date
显示当前日期。

**用法：**
```bash
date
```

### uptime
显示系统运行时间。

**用法：**
```bash
uptime
```

### whoami
显示当前登录的用户名。

**用法：**
```bash
whoami
```

### sleep
暂停执行指定秒数。

**用法：**
```bash
sleep <seconds>
```

**示例：**
```bash
sleep 5
```

## 文件系统命令

### ls
列出当前目录下的文件和文件夹。

**用法：**
```bash
ls [path]
```

**示例：**
```bash
ls
ls /system
```

### cd
切换当前工作目录。

**用法：**
```bash
cd <path>
```

**示例：**
```bash
cd /system
cd ..
```

### pwd
显示当前工作目录的完整路径。

**用法：**
```bash
pwd
```

### mkdir
创建新目录。

**用法：**
```bash
mkdir <directory>
```

**示例：**
```bash
mkdir myfolder
```

### rm
删除文件。

**用法：**
```bash
rm <file>
```

**示例：**
```bash
rm myfile.txt
```

### rmdir
删除空目录。

**用法：**
```bash
rmdir <directory>
```

**示例：**
```bash
rmdir myfolder
```

### cat
显示文件内容。

**用法：**
```bash
cat <file>
```

**示例：**
```bash
cat myfile.txt
```

### touch
创建空文件。

**用法：**
```bash
touch <file>
```

**示例：**
```bash
touch newfile.txt
```

### cp
复制文件。

**用法：**
```bash
cp <source> <destination>
```

**示例：**
```bash
cp file1.txt file2.txt
```

### mv
移动或重命名文件。

**用法：**
```bash
mv <source> <destination>
```

**示例：**
```bash
mv oldname.txt newname.txt
```

### rename
重命名文件。

**用法：**
```bash
rename <oldname> <newname>
```

**示例：**
```bash
rename file1.txt file2.txt
```

### find
在当前目录及其子目录中查找文件。

**用法：**
```bash
find <filename>
```

**示例：**
```bash
find config.txt
```

### tree
以树形结构显示目录内容。

**用法：**
```bash
tree [path]
```

**示例：**
```bash
tree
tree /system
```

### head
显示文件的前几行。

**用法：**
```bash
head <file> [lines]
```

**示例：**
```bash
head myfile.txt 10
```

### tail
显示文件的后几行。

**用法：**
```bash
tail <file> [lines]
```

**示例：**
```bash
tail myfile.txt 10
```

### wc
统计文件的行数、字数和字节数。

**用法：**
```bash
wc <file>
```

**示例：**
```bash
wc myfile.txt
```

### grep
在文件中搜索文本。

**用法：**
```bash
grep <pattern> <file>
```

**示例：**
```bash
grep "hello" myfile.txt
```

### getdisk
显示磁盘信息。

**用法：**
```bash
getdisk
```

## 编辑器命令

### edit
使用内置编辑器编辑文件。

**用法：**
```bash
edit <file>
```

**示例：**
```bash
edit myfile.txt
```

### nano
使用 Nano 编辑器编辑文件。

**用法：**
```bash
nano <file>
```

**示例：**
```bash
nano myfile.txt
```

## 用户管理命令

### user
管理用户账户。

**用法：**
```bash
user add <username> <password>    # 添加普通用户
user add admin <username> <password>  # 添加管理员用户
user remove <username>              # 删除用户
user list                          # 列出所有用户
```

**示例：**
```bash
user add john password123
user add admin admin adminpass
user list
user remove john
```

### cpass
修改当前用户的密码。

**用法：**
```bash
cpass
```

### hostname
显示或设置主机名。

**用法：**
```bash
hostname                    # 显示主机名
hostname <new_hostname>    # 设置主机名
```

**示例：**
```bash
hostname
hostname myserver
```

## 网络命令

### ipconfig
显示网络配置信息。

**用法：**
```bash
ipconfig
```

### setdns
设置 DNS 服务器。

**用法：**
```bash
setdns <dns_server>
```

**示例：**
```bash
setdns 8.8.8.8
```

### setgateway
设置网关地址。

**用法：**
```bash
setgateway <gateway>
```

**示例：**
```bash
setgateway 192.168.1.1
```

### nslookup
查询域名解析。

**用法：**
```bash
nslookup <domain>
```

**示例：**
```bash
nslookup google.com
```

### ping
测试网络连接。

**用法：**
```bash
ping <ip_or_domain>
```

**示例：**
```bash
ping 8.8.8.8
ping google.com
```

### wget
从网络下载文件。

**用法：**
```bash
wget <url>
```

**示例：**
```bash
wget http://example.com/file.txt
```

### ftp
启动 FTP 服务器。

**用法：**
```bash
ftp
```

### tcpserver
启动 TCP 服务器。

**用法：**
```bash
tcpserver <port>
```

**示例：**
```bash
tcpserver 8080
```

### tcpclient
连接到 TCP 服务器。

**用法：**
```bash
tcpclient <ip> <port>
```

**示例：**
```bash
tcpclient 192.168.1.100 8080
```

## 脚本命令

### lua
执行 Lua 脚本。

**用法：**
```bash
lua <file>
```

**示例：**
```bash
lua script.lua
```

### com
执行命令脚本文件。

**用法：**
```bash
com <file>
```

**示例：**
```bash
com script.cm
```

### branswe
执行 Branswe 脚本。

**用法：**
```bash
branswe <file>
```

**示例：**
```bash
branswe script.brs
```

## 实用工具命令

### calc
执行数学计算。

**用法：**
```bash
calc <expression>
```

**示例：**
```bash
calc 2+2
calc 10*5
calc (3+5)*2
```

### history
显示命令历史记录。

**用法：**
```bash
history
```

### prompt
更改命令提示符。

**用法：**
```bash
prompt <new_prompt>
```

**示例：**
```bash
prompt $
```

### background
更改背景颜色。

**用法：**
```bash
background <hex_color>
```

**示例：**
```bash
background 000000
background FF0000
```

### beep
播放系统提示音。

**用法：**
```bash
beep
```

### cal
显示日历。

**用法：**
```bash
cal [month] [year]
```

**示例：**
```bash
cal
cal 2 2026
```

### base64
Base64 编码和解码。

**用法：**
```bash
base64 encode <text>    # 编码
base64 decode <text>    # 解码
```

**示例：**
```bash
base64 encode "Hello World"
base64 decode "SGVsbG8gV29ybGQ="
```

### matrix
显示黑客帝国矩阵效果。

**用法：**
```bash
matrix
```

**说明：**
- 显示《黑客帝国》风格的矩阵雨效果
- 绿色字符在黑色背景上显示
- 随机字符和下落速度
- 按 ESC 或 Q 键退出

### app
应用程序管理器。

**用法：**
```bash
app list              # 列出所有可用应用
app install <name>    # 安装应用
app uninstall <name>  # 卸载应用
app installed         # 列出已安装应用
app help             # 显示帮助信息
```

**示例：**
```bash
app list
app install helloworld.lua
app uninstall helloworld.lua
app installed
```

**说明：**
- 应用存储在嵌入资源中
- 安装的应用保存在 `0:\apps` 目录
- 安装的应用可以使用 `lua` 命令运行

### alias
创建命令别名。

**用法：**
```bash
alias                      # 列出所有别名
alias <name> <command>     # 创建新别名
```

**示例：**
```bash
alias ll 'ls -l'
alias home 'cd /home'
alias cls clear
```

### unalias
删除命令别名。

**用法：**
```bash
unalias <name>
```

**示例：**
```bash
unalias ll
```

## 系统信息命令

### version
显示系统版本信息。

**用法：**
```bash
version
```

### settings
查看或修改系统设置。

**用法：**
```bash
settings                          # 列出所有设置
settings <key>                     # 显示指定设置的值
settings <key> <value>           # 设置指定设置的值
```

**示例：**
```bash
settings
settings LoggerEnabled
settings LoggerEnabled true
settings LoggerEnabled false
```

**说明：**
- 设置存储在 `0:\system\settings.dat` 文件中
- 支持的设置项：LoggerEnabled（控制是否输出 Logger 日志）
- 首次启动时自动创建配置文件并填入默认值
- 如果配置文件存在但缺少某些设置项，系统会自动补充

### about
显示系统关于信息。

**用法：**
```bash
about
```

### help
显示帮助信息。

**用法：**
```bash
help [command]
```

**示例：**
```bash
help
help ls
```

## 系统控制命令

### restart
重启系统。

**用法：**
```bash
restart
```

### shutdown
关闭系统。

**用法：**
```bash
shutdown
```

## 备份与恢复命令

### backup
备份系统。

**用法：**
```bash
backup <backup_file>
```

**示例：**
```bash
backup mybackup.zip
```

### restore
恢复系统备份。

**用法：**
```bash
restore <backup_file>
```

**示例：**
```bash
restore mybackup.zip
```

## 测试命令

### cuitest
测试 CUI 框架。

**用法：**
```bash
cuitest
```

### testgui
测试图形界面。

**用法：**
```bash
testgui
```

### labyrinth
玩迷宫逃脱游戏。

**用法：**
```bash
labyrinth
```

**说明：**
- 使用方向键 (↑ ↓ ← →) 移动玩家
- 按 ESC 键退出游戏
- 目标是找到出口 (E) 并逃脱迷宫
- 玩家位置用绿色 @ 表示
- 出口位置用红色 E 表示
- 迷宫使用递归回溯算法随机生成

### diff
比较两个文件的差异。

**用法：**
```bash
diff <file1> <file2>
```

**示例：**
```bash
diff file1.txt file2.txt
```

## 环境变量命令

### env
管理环境变量。

**用法：**
```bash
env                    # 列出所有环境变量
env <name>             # 显示指定环境变量
env <name> <value>     # 设置环境变量
```

**示例：**
```bash
env
env PATH
env MYVAR hello
```

## 注意事项

1. 所有命令不区分大小写
2. 使用 `help` 命令可以查看所有可用命令
3. 使用 `help <command>` 可以查看特定命令的详细帮助
4. 文件路径使用反斜杠 `\` 或正斜杠 `/` 均可
5. 支持相对路径和绝对路径
