SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[Events]( 
[Id] [int] IDENTITY(1,1) NOT NULL,
[CreateDate] [datetime] NOT NULL,
[UpdateDate] [datetime] NOT NULL,
[CreateUser] [nvarchar](20) NOT NULL,
[UpdateUser] [nvarchar](20) NOT NULL,
[UpdateComment] [nvarchar](150) NULL,
[Version] [int] NOT NULL,
[IsActive] [bit] NOT NULL,
[Title] [nvarchar](150) NOT NULL,
[Summary] [nvarchar](200) NOT NULL,
[Description] [ntext] NOT NULL,
[StartDate] [datetime] NOT NULL,
[EndDate] [datetime] NOT NULL,
[StartTime] [int] NULL,
[EndTime] [int] NULL,
[Email] [nvarchar](30) NULL,
[Phone] [nvarchar](20) NULL,
[Url] [nvarchar](150) NULL,
[Keywords] [nvarchar](100) NULL,
[Street] [nvarchar](40) NULL,
[City] [nvarchar](40) NULL,
[State] [nvarchar](20) NULL,
[Country] [nvarchar](20) NULL,
[Zip] [nvarchar](10) NULL,
[CityId] [int] NULL,
[StateId] [int] NULL,
[CountryId] [int] NULL,
[IsOnline] [bit] NULL,
[AverageRating] [decimal] NULL,
[TotalLiked] [int] NULL,
[TotalDisLiked] [int] NULL,
[TotalBookMarked] [int] NULL,
[TotalAbuseReports] [int] NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
( [Id] ASC
 )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
 ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
