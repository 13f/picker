USE [sac]
GO

/****** Object:  Table [dbo].[SACChinaStandard]    Script Date: 2016/5/5 05:04:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SACChinaStandard](
	[StandardCode] [nvarchar](300) NOT NULL,
	[ChineseTitle] [nvarchar](500) NULL,
	[EnglishTitle] [nvarchar](500) NULL,
	[ICS] [nvarchar](100) NULL,
	[CCS] [nvarchar](100) NULL,
	[Content] [nvarchar](max) NULL,
	[Remark] [nvarchar](max) NULL,
	[IssuanceDate] [date] NOT NULL,
	[ExecuteDate] [date] NULL,
	[RevocatoryDate] [date] NOT NULL,
	[Revocative] [bit] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
 CONSTRAINT [PK_SACChinaStandard] PRIMARY KEY CLUSTERED 
(
	[StandardCode] ASC,
	[RevocatoryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


