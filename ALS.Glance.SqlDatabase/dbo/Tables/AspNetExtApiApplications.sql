CREATE TABLE [dbo].[AspNetExtApiApplications] (
    [Id] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetExtApiApplications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetExtApiApplications_dbo.AspNetExtApplications_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AspNetExtApplications] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Id]
    ON [dbo].[AspNetExtApiApplications]([Id] ASC);

