CREATE TABLE [Sample](
	[SampleId] [uniqueidentifier] NOT NULL,
	[SampleCode] [varchar](100) NOT NULL,
	[SampleName] [nvarchar](500) NOT NULL,
	[IsDel] [bit] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [uniqueidentifier] NOT NULL,
	[UpdateTime] [datetime] NULL,
	[UpdateUserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Sample] PRIMARY KEY CLUSTERED 
(
	[SampleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Sample] ADD  CONSTRAINT [DF_Sample_SampleId]  DEFAULT (newid()) FOR [SampleId]
GO

ALTER TABLE [Sample] ADD  CONSTRAINT [DF_Sample_IsDel]  DEFAULT ((0)) FOR [IsDel]
GO

ALTER TABLE [Sample] ADD  CONSTRAINT [DF_Sample_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO


