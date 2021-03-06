USE [master]
GO
/****** Object:  Database [Repositorify]    Script Date: 1/11/2021 11:10:12 PM ******/
CREATE DATABASE [Repositorify]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Repositorify', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Repositorify.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Repositorify_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Repositorify_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Repositorify] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Repositorify].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Repositorify] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Repositorify] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Repositorify] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Repositorify] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Repositorify] SET ARITHABORT OFF 
GO
ALTER DATABASE [Repositorify] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Repositorify] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Repositorify] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Repositorify] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Repositorify] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Repositorify] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Repositorify] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Repositorify] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Repositorify] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Repositorify] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Repositorify] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Repositorify] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Repositorify] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Repositorify] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Repositorify] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Repositorify] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Repositorify] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Repositorify] SET RECOVERY FULL 
GO
ALTER DATABASE [Repositorify] SET  MULTI_USER 
GO
ALTER DATABASE [Repositorify] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Repositorify] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Repositorify] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Repositorify] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Repositorify] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Repositorify', N'ON'
GO
ALTER DATABASE [Repositorify] SET QUERY_STORE = OFF
GO
USE [Repositorify]
GO
/****** Object:  Schema [utscrepc]    Script Date: 1/11/2021 11:10:13 PM ******/
CREATE SCHEMA [utscrepc]
GO
/****** Object:  Table [utscrepc].[ImageTags]    Script Date: 1/11/2021 11:10:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [utscrepc].[ImageTags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageId] [varchar](100) NOT NULL,
	[TagId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ImageTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [utscrepc].[Images]    Script Date: 1/11/2021 11:10:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [utscrepc].[Images](
	[Id] [varchar](100) NOT NULL,
	[Uploader] [varchar](50) NOT NULL,
	[DateUploaded] [datetime] NOT NULL,
	[Size] [int] NULL,
	[Extension] [varchar](10) NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [utscrepc].[vw_Images]    Script Date: 1/11/2021 11:10:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [utscrepc].[vw_Images]
AS

SELECT
	T.ImageId,
	CONCAT('/Uploads/Images/', I.Id, I.Extension) AS ImageLink,
	CONCAT('/Uploads/Thumbnails/', I.Id, I.Extension) AS ThumbnailLink,
	I.DateUploaded,
	T.TagId,
	T2.TagId as Tag
FROM 
	utscrepc.ImageTags T
LEFT JOIN
	utscrepc.Images I
ON
	T.ImageId = I.Id
LEFT JOIN
	utscrepc.ImageTags T2
ON
	T2.ImageId = I.Id
WHERE I.Enabled = 1

GO
/****** Object:  Table [utscrepc].[Tags]    Script Date: 1/11/2021 11:10:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [utscrepc].[Tags](
	[Id] [varchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [utscrepc].[Images] ADD  CONSTRAINT [DF_Images_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [utscrepc].[Images] ADD  CONSTRAINT [DF_Images_DateUploaded]  DEFAULT (getdate()) FOR [DateUploaded]
GO
ALTER TABLE [utscrepc].[Images] ADD  CONSTRAINT [DF_Images_Enabled]  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [utscrepc].[Tags] ADD  CONSTRAINT [DF_Tags_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [utscrepc].[ImageTags]  WITH CHECK ADD  CONSTRAINT [FK_ImageTags_Images] FOREIGN KEY([ImageId])
REFERENCES [utscrepc].[Images] ([Id])
GO
ALTER TABLE [utscrepc].[ImageTags] CHECK CONSTRAINT [FK_ImageTags_Images]
GO
ALTER TABLE [utscrepc].[ImageTags]  WITH CHECK ADD  CONSTRAINT [FK_ImageTags_Tags] FOREIGN KEY([TagId])
REFERENCES [utscrepc].[Tags] ([Id])
GO
ALTER TABLE [utscrepc].[ImageTags] CHECK CONSTRAINT [FK_ImageTags_Tags]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T"
            Begin Extent = 
               Top = 15
               Left = 96
               Bottom = 281
               Right = 424
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "I"
            Begin Extent = 
               Top = 15
               Left = 520
               Bottom = 324
               Right = 848
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'utscrepc', @level1type=N'VIEW',@level1name=N'vw_Images'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'utscrepc', @level1type=N'VIEW',@level1name=N'vw_Images'
GO
USE [master]
GO
ALTER DATABASE [Repositorify] SET  READ_WRITE 
GO
