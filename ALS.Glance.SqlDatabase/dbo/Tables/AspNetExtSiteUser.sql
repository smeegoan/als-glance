CREATE TABLE [dbo].[AspNetExtSiteUser] (
    [Id]        NVARCHAR (128) NOT NULL,
    [FirstName] NVARCHAR (256) NULL,
    [LastName]  NVARCHAR (256) NULL,
    CONSTRAINT [PK_dbo.AspNetExtSiteUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetExtSiteUser_dbo.AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Id]
    ON [dbo].[AspNetExtSiteUser]([Id] ASC);

