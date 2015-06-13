CREATE TABLE [dbo].[EMG] (
    [Id]        BIGINT             NOT NULL,
    [PatientId] BIGINT             NOT NULL,
    [Data]      VARCHAR (MAX)      NOT NULL,
    [Date]      DATETIMEOFFSET (7) NOT NULL,
    [CreatedOn] DATETIMEOFFSET (7) DEFAULT (getdate()) NOT NULL,
    [UpdatedOn] DATETIMEOFFSET (7) DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_EMG] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.EMG_dbo.Patient_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[D_Patient] ([Id])
);



