﻿CREATE TABLE [dbo].[Announcements] (
    [Id]            INT          IDENTITY (1, 1) NOT NULL,
    [Title]         VARCHAR (50) NOT NULL,
    [Body]          TEXT         NOT NULL,
    [Importance]    INT          NOT NULL,
    [StartDate]     DATE         NOT NULL,
    [EndDate]       DATE         NOT NULL,
    [CreateTime]    DATETIME     NOT NULL,
    [CreateUser]    INT          NOT NULL,
    [EditTime]      DATETIME     NOT NULL,
    [EditUser]      INT          NULL,
    [StatusTime]    DATETIME     NOT NULL,
    [StatusUser]    INT          NULL,
    [StatusMessage] VARCHAR (50) NULL,
    [Status]        INT          NOT NULL,
    [Scope]         INT          NULL,
    CONSTRAINT [PK_Announcements] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Announcements_Users_Create] FOREIGN KEY ([CreateUser]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Announcements_Users_Edit] FOREIGN KEY ([EditUser]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Announcements_Users_Status] FOREIGN KEY ([StatusUser]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Announcements_Scopes] FOREIGN KEY ([Scope]) REFERENCES [dbo].[Scopes] ([Id])
);