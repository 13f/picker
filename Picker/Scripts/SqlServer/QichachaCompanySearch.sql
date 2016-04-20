USE [qichacha]
GO

/****** Object:  Table [dbo].[QichachaCompanySearch]    Script Date: 2016/4/20 22:43:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QichachaCompanySearch](
	[Keyword] [nvarchar](400) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[ProcessedPage] [int] NOT NULL,
 CONSTRAINT [PK_QichachaCompanySearch] PRIMARY KEY CLUSTERED 
(
	[Keyword] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


