# Changelog

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