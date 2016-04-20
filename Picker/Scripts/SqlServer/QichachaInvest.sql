USE [qichacha]
GO

/****** Object:  Table [dbo].[QichachaInvest]    Script Date: 2016/4/19 21:58:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QichachaInvest](
	[InvestorId] [varchar](100) NOT NULL,
	[InvestorName] [nvarchar](500) NULL,
	[TargetId] [varchar](100) NOT NULL,
	[TargetName] [nvarchar](500) NULL,
	[Content] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
 CONSTRAINT [PK_QichachaInvest] PRIMARY KEY CLUSTERED 
(
	[InvestorId] ASC,
	[TargetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


