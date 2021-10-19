# Changelog

## [0.1.14] - 2021-10-19
###Added
- 增加框架通过类型获取管理器

###Fixed
- 打包编译错误

## [0.1.13] - 2021-10-18
###Modify
- 修改TimerManager实现方式

###Fixed
- 定时器回收，状态未重置

## [0.1.12] - 2021-10-15
###Added
- 增加编辑器标识、IOS平台标识
- 增加定时器管理器

###Fixed
- 修复编译报错

## [0.1.11] - 2021-10-13
###Added
- 生命周期，增加GUI句柄
- 管理器增加注册完成的生命周期
- 框架GameObject根节点
- 管理器基类增加框架引用mSnakeFramework

###Modify
- 生命周期修改为对象引用(SnakeFramework.mLifeCycle)

## [0.1.10] - 2021-10-12
###Modify
- 修改脚本名字
- 实现内置单例模式

###Removed 
- 单例工具，由业务层自行实现

## [0.1.9] - 2021-10-11
###Added
- 下载管理器(DownloadManager)
- 获取自定义门户实例API接口(GetAppFacadeCostom<T>())

###Modify
- 优化启动流程的代码实现
- 优化Utility.Web代码结构
- 优化引用池工具的命名空间

###Fixed
- 增加已存在的状态，导致报错

## [0.1.8] - 2021-09-27
###Added
- 增加BootDriverSetting.cs对框架进行配置

###Modify
- 重构Utility.Web
- 增加BootDriverSetting.BootUpTagName来标记启动场景

## [0.1.7] - 2021-09-24
###Modify
- 使用UnityEngine.RuntimeInitializeOnLoadMethod启动游戏
- 重构管理器的初始化和预加载框架

## [0.1.6] - 2021-09-23
###Fixed
- 修复没有初始化完成，就直接进入下一个流程

## [0.1.5] - 2021-09-22
###Added
- 异步初始化管理器流程
- 异步预加载管理器流程

## [0.1.3] - 2021-09-22
###Added
- 启动流程，闪屏流程，预加载流程修改为框架内置流程
- 状态机增加Remove方法

###Removed
- 移除运行流程(PlayingProcedure.cs)
- 移除状态机接口(IFiniteStateMachine)

###Modify
- IAppFacadeCostom接口中的游戏运行方法(GameLaunch)修改为进入游戏内容(EnterGameContent)
- 规范代码命名空间(namespace)

## [0.1.0] - 2021-09-18
###Added
- CSV解析工具

###Removed
- 清理冗余代码

###Updated
- 流程管理器为默认管理器

## [0.0.3] - 2021-09-08
###Added
- Fixed README.md