CREATE TABLE [dbo].[AspNetExtApiUsers] (
    [Id] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetExtApiUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetExtApiUsers_dbo.AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Id]
    ON [dbo].[AspNetExtApiUsers]([Id] ASC);

