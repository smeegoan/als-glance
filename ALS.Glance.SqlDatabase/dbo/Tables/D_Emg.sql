CREATE TABLE [dbo].[D_Emg] (
    [Id]        BIGINT             IDENTITY (1, 1) NOT NULL,
    [Data]      VARCHAR (MAX)      NULL,
    [CreatedOn] DATETIMEOFFSET (7) CONSTRAINT [DF__D_Emg__CreatedOn__5DCAEF64] DEFAULT (getdate()) NOT NULL,
    [UpdatedOn] DATETIMEOFFSET (7) CONSTRAINT [DF__D_Emg__UpdatedOn__5EBF139D] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_dbo.Emg] PRIMARY KEY CLUSTERED ([Id] ASC)
);



