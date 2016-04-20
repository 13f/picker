USE [qichacha]
GO

/****** Object:  Table [dbo].[QichachaCompany]    Script Date: 2016/4/19 21:58:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QichachaCompany](
	[Id] [varchar](100) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[RegNum] [nvarchar](100) NULL,
	[OrgCode] [nvarchar](100) NULL,
	[Content] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
	[CertificateUpdated] [datetime] NULL,
	[CopyrightUpdated] [datetime] NULL,
	[SoftwareCopyrightUpdated] [datetime] NULL,
	[InvestUpdated] [datetime] NULL,
	[PatentUpdated] [datetime] NULL,
	[TrademarkUpdated] [datetime] NULL,
	[WebsiteUpdated] [datetime] NULL,
 CONSTRAINT [PK_QichachaCompany] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


