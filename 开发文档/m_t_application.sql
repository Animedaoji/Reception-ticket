/*
 Navicat Premium Data Transfer

 Source Server         : SQLServer 2014
 Source Server Type    : SQL Server
 Source Server Version : 12004100
 Source Host           : localhost:1962
 Source Catalog        : Daoji
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 12004100
 File Encoding         : 65001

 Date: 30/12/2018 02:08:33
*/


-- ----------------------------
-- Table structure for m_t_application
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[m_t_application]') AND type IN ('U'))
	DROP TABLE [dbo].[m_t_application]
GO

CREATE TABLE [dbo].[m_t_application] (
  [id] int  IDENTITY(1001,1) NOT NULL,
  [meal_date] date  NOT NULL,
  [meal_location] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [meal_type] varchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [meal] varchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [ticketStatus] varchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [identification] varchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [ticketCreate] datetime  NOT NULL,
  [UsedTime] datetime DEFAULT '' NULL,
  [amount] decimal(18,2) DEFAULT ((0.00)) NULL,
  [operator_man] varchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[m_t_application] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'唯一ID',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'就餐日期',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'meal_date'
GO

EXEC sp_addextendedproperty
'MS_Description', N'就餐地点',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'meal_location'
GO

EXEC sp_addextendedproperty
'MS_Description', N'餐票类型',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'meal_type'
GO

EXEC sp_addextendedproperty
'MS_Description', N'餐次',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'meal'
GO

EXEC sp_addextendedproperty
'MS_Description', N'餐票状态',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'ticketStatus'
GO

EXEC sp_addextendedproperty
'MS_Description', N'餐票标识、随机字符串、用于生成二维码',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'identification'
GO

EXEC sp_addextendedproperty
'MS_Description', N'餐票生成时间',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'ticketCreate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'消费时间/退款时间',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'UsedTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'金额',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'amount'
GO

EXEC sp_addextendedproperty
'MS_Description', N'操作者',
'SCHEMA', N'dbo',
'TABLE', N'm_t_application',
'COLUMN', N'operator_man'
GO


-- ----------------------------
-- Indexes structure for table m_t_application
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [id]
ON [dbo].[m_t_application] (
  [id] ASC
)
GO

CREATE NONCLUSTERED INDEX [ticketCreate]
ON [dbo].[m_t_application] (
  [ticketCreate] ASC
)
GO

CREATE NONCLUSTERED INDEX [meal_date]
ON [dbo].[m_t_application] (
  [meal_date] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table m_t_application
-- ----------------------------
ALTER TABLE [dbo].[m_t_application] ADD CONSTRAINT [PK__m_t_appl__3213E83F0B2D1CF5] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

