CREATE TABLE [dbo].[AspNetExtApiRoles] (
    [Id]          NVARCHAR (128) NOT NULL,
    [Description] NVARCHAR (512) NULL,
    CONSTRAINT [PK_dbo.AspNetExtApiRoles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetExtApiRoles_dbo.AspNetRoles_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AspNetRoles] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Id]
    ON [dbo].[AspNetExtApiRoles]([Id] ASC);

