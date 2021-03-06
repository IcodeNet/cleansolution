USE [master]
GO

/****** Object:  Database [HRApplicationData]    Script Date: 10/19/2009 16:14:48 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'HRApplicationData')
DROP DATABASE [HRApplicationData]
GO

USE [master]
GO

/****** Object:  Database [HRApplicationData]    Script Date: 10/19/2009 16:14:48 ******/
CREATE DATABASE [HRApplicationData]
GO

ALTER DATABASE [HRApplicationData] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HRApplicationData].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [HRApplicationData] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [HRApplicationData] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [HRApplicationData] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [HRApplicationData] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [HRApplicationData] SET ARITHABORT OFF 
GO

ALTER DATABASE [HRApplicationData] SET AUTO_CLOSE ON 
GO

ALTER DATABASE [HRApplicationData] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [HRApplicationData] SET AUTO_SHRINK ON 
GO

ALTER DATABASE [HRApplicationData] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [HRApplicationData] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [HRApplicationData] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [HRApplicationData] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [HRApplicationData] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [HRApplicationData] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [HRApplicationData] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [HRApplicationData] SET  DISABLE_BROKER 
GO

ALTER DATABASE [HRApplicationData] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [HRApplicationData] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [HRApplicationData] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [HRApplicationData] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [HRApplicationData] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [HRApplicationData] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [HRApplicationData] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [HRApplicationData] SET  READ_WRITE 
GO

ALTER DATABASE [HRApplicationData] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [HRApplicationData] SET  MULTI_USER 
GO

ALTER DATABASE [HRApplicationData] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [HRApplicationData] SET DB_CHAINING OFF 
GO


USE [HRApplicationData]
GO
/****** Object:  Table [dbo].[Applicants]    Script Date: 10/19/2009 16:13:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Applicants](
	[ApplicationID] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [uniqueidentifier] NOT NULL,
	[ApplicantName] [nvarchar](50) NOT NULL,
	[Education] [nvarchar](50) NOT NULL,
	[NumberOfReferences] [int] NOT NULL,
	[HireApproved] [bit] NULL,
 CONSTRAINT [PK_Applicants] PRIMARY KEY CLUSTERED 
(
	[ApplicationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [HRApplicationData]
GO
CREATE USER [IIS Users] FOR LOGIN [BUILTIN\IIS_IUSRS]
GO
USE [HRApplicationData]
GO
EXEC sp_addrolemember N'db_datawriter', N'IIS Users'
GO
USE [HRApplicationData]
GO
EXEC sp_addrolemember N'db_datareader', N'IIS Users'
GO
