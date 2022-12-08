alter table DriverRepositoryStandardParameterLink ADD lnk_MustBeSaved bit not null default(0)

CREATE TABLE [dbo].[DASAcqTable](
	[ID] [char](36) NOT NULL,
	[IsCurrent] [bit] NOT NULL,
	[ValidToDate] [datetime] NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Version] [int] NOT NULL,
	[ValidFromDate] [datetime] NULL,
	[drv_id] [char](36) NOT NULL
 CONSTRAINT [PK_AcqTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[Name] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[DASAcqTable_ParametersLink](
	[acq_ID] [char](36) NOT NULL,
	[acq_Version] [int] NOT NULL,
	[par_ID] [int] NOT NULL,
	[par_ID_Alias] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_AcqTable_ParametersLink] PRIMARY KEY CLUSTERED 
(
	[acq_ID] ASC,
	[acq_Version] ASC,
	[par_ID] ASC,
	[par_ID_Alias] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[DASAcqTableHistory](
	[pa_id] [int] NOT NULL,
	[acq_id] [char](36) NOT NULL,
	[acq_Version] [int] NOT NULL,
	[db_id] [int] NOT NULL,
 CONSTRAINT [PK_AcqTableHistory] PRIMARY KEY CLUSTERED 
(
	[pa_id] ASC,
	[acq_id] ASC,
	[acq_Version] ASC,
	[db_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[DASParamTextCatalog](
	[ct_id] [int] IDENTITY(1,1) NOT NULL,
	[ct_Text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_DASParamTextCatalog] PRIMARY KEY CLUSTERED 
(
	[ct_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
