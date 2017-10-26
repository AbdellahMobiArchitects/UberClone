CREATE DATABASE uberclonedb

GO

USE uberclonedb


CREATE TABLE [dbo].[Users] (
    [user_id]        INT           IDENTITY (1, 1) NOT NULL,
    [username]       AS            ('user'+CONVERT([varchar](max),[user_id])),
    [usertype]       VARCHAR (MAX) NOT NULL,
    [user_longitude] FLOAT (53)    NULL,
    [user_latitude]  FLOAT (53)    NULL,
    PRIMARY KEY CLUSTERED ([user_id] ASC)
);

go


CREATE TABLE [dbo].[Requests] (
    [request_id]          INT           IDENTITY (1, 1) NOT NULL,
    [requester_username]  VARCHAR (MAX) NOT NULL,
    [requester_longitude] FLOAT (53)    NOT NULL,
    [requester_latitude]  FLOAT (53)    NOT NULL,
    [driver_usename]      VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([request_id] ASC)
);

GO

SET IDENTITY_INSERT [dbo].[Users] ON
INSERT INTO [dbo].[Users] ([user_id], [usertype], [user_longitude], [user_latitude]) VALUES (1, N'rider', -7.670762, 33.570823)
INSERT INTO [dbo].[Users] ([user_id], [usertype], [user_longitude], [user_latitude]) VALUES (2, N'rider', -7.665895, 33.563008)
INSERT INTO [dbo].[Users] ([user_id], [usertype], [user_longitude], [user_latitude]) VALUES (3, N'rider', -7.639283, 33.543044)
INSERT INTO [dbo].[Users] ([user_id], [usertype], [user_longitude], [user_latitude]) VALUES (4, N'driver', -7.60965, 33.578598)
SET IDENTITY_INSERT [dbo].[Users] OFF

GO

SET IDENTITY_INSERT [dbo].[Requests] ON
INSERT INTO [dbo].[Requests] ([request_id], [requester_username], [requester_longitude], [requester_latitude], [driver_usename]) VALUES (3, N'user6', -7.670762, 33.570823, NULL)
INSERT INTO [dbo].[Requests] ([request_id], [requester_username], [requester_longitude], [requester_latitude], [driver_usename]) VALUES (4, N'user7', -7.665895, 33.563008, NULL)
INSERT INTO [dbo].[Requests] ([request_id], [requester_username], [requester_longitude], [requester_latitude], [driver_usename]) VALUES (5, N'user8', -7.639283, 33.543044, N'user9')
SET IDENTITY_INSERT [dbo].[Requests] OFF

go