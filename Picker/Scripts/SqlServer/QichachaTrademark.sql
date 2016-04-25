USE [qichacha]
GO

/****** Object:  Table [dbo].[QichachaTrademark]    Script Date: 2016/4/23 08:35:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QichachaTrademark](
	[Id] [varchar](100) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[RegNo] [varchar](50) NULL,
	[Applicant] [nvarchar](500) NULL,
	[Content] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
 CONSTRAINT [PK_QichachaTrademark] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


