USE [qichacha]
GO

/****** Object:  Table [dbo].[QichachaAlbum]    Script Date: 2016/4/20 13:53:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QichachaAlbum](
	[Uri] [varchar](500) NOT NULL,
	[Id] [int] NOT NULL,
	[Title] [nvarchar](500) NULL,
	[ModifiedAt] [datetime] NULL,
	[CreatedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
 CONSTRAINT [PK_QichachaAlbum] PRIMARY KEY CLUSTERED 
(
	[Uri] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


