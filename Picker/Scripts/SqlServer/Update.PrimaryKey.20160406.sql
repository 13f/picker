
-- 1 删除主键
-- 2 
--alter table [picker].[dbo].[FellowPlusProjectPreview]
--alter column [Id] nvarchar(450) NOT NULL
---- 3 设置主键
--alter table [picker].[dbo].[FellowPlusProjectPreview]
--add constraint PK_FellowPlusProjectPreview primary key(Id)

-- Warning: 警告! 最大键长度为 900 个字节。索引 'PK_FellowPlusProjectPreview' 的最大长度为 1000 个字节。对于某些大值组合，插入/更新操作将失败。



-- Project
--alter table [picker].[dbo].[FellowPlusProject]
--alter column [Id] nvarchar(450) NOT NULL

--alter table [picker].[dbo].[FellowPlusProject]
--add constraint PK_FellowPlusProject primary key(Id)



-- Company
--alter table [picker].[dbo].[FellowPlusCompany]
--alter column [ProjectId] nvarchar(450) NOT NULL

--alter table [picker].[dbo].[FellowPlusCompany]
--add constraint PK_FellowPlusCompany primary key(ProjectId, Name)



-- Invest
--alter table [picker].[dbo].[FellowPlusInvest]
--alter column [ProjectId] nvarchar(450) NOT NULL

--alter table [picker].[dbo].[FellowPlusInvest]
--add constraint PK_FellowPlusInvest primary key(ProjectId)



-- News
--alter table [picker].[dbo].[FellowPlusNews]
--alter column [ProjectId] nvarchar(450) NOT NULL

--alter table [picker].[dbo].[FellowPlusNews]
--add constraint PK_FellowPlusNews primary key(ProjectId, Uri)



-- Website
--alter table [picker].[dbo].[FellowPlusWebsite]
--alter column [ProjectId] nvarchar(450) NOT NULL

--alter table [picker].[dbo].[FellowPlusWebsite]
--add constraint PK_FellowPlusWebsite primary key(ProjectId, Id)



-- Weibo
--alter table [picker].[dbo].[FellowPlusWeibo]
--alter column [ProjectId] nvarchar(450) NOT NULL

--alter table [picker].[dbo].[FellowPlusWeibo]
--add constraint PK_FellowPlusWeibo primary key(ProjectId)



-- Weixin
--alter table [picker].[dbo].[FellowPlusWeixin]
--alter column [ProjectId] nvarchar(450) NOT NULL

--alter table [picker].[dbo].[FellowPlusWeixin]
--add constraint PK_FellowPlusWeixin primary key(ProjectId)