USE [sac]
GO

/****** Object:  Table [dbo].[SACQueryState]    Script Date: 2016/4/30 10:15:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SACQueryState](
	[Uri] [nvarchar](450) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[ProcessedPage] [int] NULL
CONSTRAINT [PK_SACQueryState] PRIMARY KEY CLUSTERED 
(
	[Uri] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
