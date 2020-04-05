# 停车场管理系统

浙江农林大学地理信息科学171班 HMc、FZ、HHb、XHc、ZLl、CC小组

# 模块

模块名|命名空间|介绍|完成度
-|-|-|-
核心模块|Park.Core|提供数据库模型、业务处理方法|60%
管理模块|Park.Admin|提供管理页面供管理员对各种信息进行管理、查看和汇总|30%
用户模块|Park.Mobile|提供手机网站，供用户进行查看账户信息和停车场信息|0%
外部接口模块|Park.API|为停车场硬件设施提供API|0%
车位设计模块|Park.Designer|用于设计停车位地图。由于技术不够，故使用WPF作为设计器框架。|80%
测试模块|Park.Test|用于测试，非单元测试|-

# 日志

## 20200329

### 设计器

- 基本完成车位设计器的主要功能：
	- 画板网格显示
	- 鼠标绘制停车位、通道
	- 停车区的选择
	- 停车区的新增、删除、重命名
	- 使用鼠标浏览画板
	- 使用鼠标选取对象
	- 配置文件的导入导出
	- 配置文件的自动保存和恢复

## 20200330

### 设计器

- 将通道从矩形改为线
- 通道支持了非横纵方向
- 通道支持了朝左上方拉伸
- 通道支持按Shift进行约束
- 停车位新增支持旋转
- 鼠标悬浮样式改为图形显示阴影
- 支持了删除功能

## 20200331

### 管理端

- 搭建了对车主管理表格的基本页面

### 核心

- 增加了停车场业务处理相关方法

### 测试

- 增加了测试类

## 20200401

### 核心

- 增加了交易充值业务处理相关方法

### 测试

- 增加了非会员进出、会员进出的测试方法

## 20200402

### 核心

- 为数据库添加了显式外键声明

### 管理端

- 基本完成车主管理表格
- 基本完成车位管理表格

## 20200403

### 核心

- 支持了从Json文件（设计器导出）导入停车位的功能
- 基本完成停车区地图的显示（To Bitmap）

## 20200404

### 管理端

- 显示了车主拥有的车辆数量、交易订单数量，点击车辆数量可以跳转到车辆表
- 提升了”表格模型“基类和js方法，方便之后的编写
- 基本完成了停车记录表格
