# Changelog

## [0.2.18] - 2021-11-18
###Modify
- 完善资源录制编辑器窗口

## [0.2.17] - 2021-11-17
###Added
- 资源录制编辑器窗口(preview)

## [0.2.16] - 2021-11-17
###Added
- 校验文件MD5接口
- 拓展资源分离功能（使用资源调用路径）

## [0.2.15] - 2021-11-12
###Fixed
- BundleManager报错(暂时先屏蔽)

## [0.2.14] - 2021-11-12
###Fixed
- 修改打包规则目录AssetRule数据丢失

###Added
- 通用定义路径脚本
- 资源包管理器（BundleManager.cs）
- 打包规则中，类型和过滤字段默认或许目录下所有文件或者目录

###Modify
- 框架编辑器程序集命名修改
- 选取打包目录时，不处理限制Packages下面的资源目录

## [0.2.13] - 2021-11-09
###Fixed
- 获取Packages路径
- 构建编辑器“独立打包”选项不生效

## [0.2.12] - 2021-11-09
###Fixed
- 获取Packages路径

## [0.2.11] - 2021-11-09
###Added
- 获取Package路径的静态方法

###Modify
- 修改编辑器uxml的加装路径
- Web.Post结构避免重载改名

## [0.2.10] - 2021-11-08
###Added
- 资源构建编辑器打开窗口方法

###Modify
- 修改构建配置Header
- Header增加编辑器前缀

## [0.2.9] - 2021-11-08
###Added
- 增加环境变量配置(EnvironmentSetting.cs)
- 构建配置(BuilderSetting.cs)
- 资源打包编辑器
- UPM依赖：unity打包工具

###Modify
- Setting结构，优化启动流程
- 框架拓展的程序集名抽至启动设置中填写(BootDriverSetting.cs)

###Fixed
- 打包回调为null，在出现打包异常时报错

## [0.2.8] - 2021-11-03
###Fixed 
- 计时管理器调用Cancel()后，又执行了一次Tick

## [0.2.7] - 2021-11-02
###Fixed 
- 计时管理器调用Cancel()后，又执行了一次Tick

###Modify
- WebAPI增加Post请求时，传入HeaderDict

## [0.2.6] - 2021-10-28
###Removed
- 生命周期中移除OnGUIHandle

## [0.2.5] - 2021-10-26
###Added
- 下载管理器，增加后台下载接口
- 框架环境变量的设计

###Fixed
- 修复闪屏接口，热更接口未初始化

## [0.2.4] - 2021-10-21
###Modify
- 拓展类用internal内部标记不对外开放使用

## [0.2.3] - 2021-10-21
###Modify
- 拓展类使用partial标记为分布类

## [0.2.2] - 2021-10-21
###Modify
- 拓展类移除命名空间

## [0.2.1] - 2021-10-21
###Modify
- 优化闪屏界面接口，热更控制器接口的结构

## [0.1.16] - 2021-10-21
###Added
- 增加闪屏界面接口
- 增加热更控制器接口
- 增加热更流程实现
- 流程管理器通过流程类型进行切换、注册的操作接口

###Fixed
- 计时管理器Tick不执行

###Modify
- Common合并到Runtime下
- Debuger修改为SnakeDebuger
- 规整命名空间
- 调整闪频逻辑，热更逻辑，预加载逻辑

###Removed
- 移除冗余目录

## [0.1.15] - 2021-10-20
###Added
- 下载器增加状态获取的变量和完成时的回调
- 下载器获取未开始下载的任务数量API接口

###Fixed
- 下载器未清理下载列表
- 下载器在下载完成的临界状态添加任务，无法成功

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