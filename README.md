# CMLeonOS 源代码仓库
需要 .NET 6.0 或更高版本，Cosmos版本为最新的修改版DevKit。

## 如何编译CMLeonOS  
### 1.下载开发环境  
#### Visual Studio  
下载Visual Studio 2026正式版（[链接](https://visualstudio.microsoft.com/zh-hans/thank-you-downloading-visual-studio/?sku=Community&channel=Stable&version=VS18&source=VSLandingPage&cid=2500&passive=false)），在Visual Studio Installer里勾选.Net开发、C++桌面开发、VSIX包开发、.Net 6.0支持、MSBuild。  
#### VS C++  
下载Microsoft Visual C++ Redistributable 2010（[链接](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2010-vc-100-sp1-no-longer-supported)）（x86、x64均下载并安装）  
### 2.下载并安装编译环境  
使用  
```bash
git clone https://github.com/leonmmcoset/cosmos

```
下载编译环境源代码，运行里面的bat文件，然后直接下一步，直到开始编译。  
如果弹出下载程序窗口并下载完成后即可关闭整个编译窗口。  
进入Visual Studio -> Nuget包管理器，配置包镜像源，选择编译好的nupkg文件夹位置（自己搜索）  
## 3.编译操作系统  
使用  
```bash
git clone https://github.com/leonmmcoset/cmleonos

```
下载CMLeonOS源代码，使用Visual Studio打开cmleonos文件夹里的sln文件，右键解决方案资源管理器里面的CMLeonOS项，选择Nuget包配置，将前文提到的配置出来的Cosmos镜像源里的**所有库**全部安装。  
然后将运行配置的Release复制一个新的名字为Fixed_Release，架构选项从Any CPU改为x64。  
配置完成后点击顶栏的运行按钮，等待编译完成（如果电脑里有VMWare将会自动启动VMWare Player运行），编译出来的iso文件位于`/bin/x64/Fixed_Release/.net6.0/CMLeonOS.iso`。  