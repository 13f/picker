USE [picker]
GO

/****** Object:  Table [dbo].[FellowPlusNews]    Script Date: 2016/4/6 19:01:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[FellowPlusNews](
	[ProjectId] [varchar](50) NOT NULL,
	[Uri] [varchar](300) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
	[Content] [nvarchar](max) NULL,
 CONSTRAINT [PK_FellowPlusNews] PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC,
	[Uri] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


