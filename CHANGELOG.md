# Changelog

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