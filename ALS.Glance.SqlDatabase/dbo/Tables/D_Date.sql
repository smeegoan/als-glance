CREATE TABLE [dbo].[D_Date] (
    [Id]            BIGINT             IDENTITY (1, 1) NOT NULL,
    [date]          DATE               NOT NULL,
    [Day]           TINYINT            NOT NULL,
    [DayInMonth]    NVARCHAR (50)      NOT NULL,
    [Month]         TINYINT            NOT NULL,
    [MonthName]     NVARCHAR (50)      NOT NULL,
    [Year]          SMALLINT           NOT NULL,
    [DayOfWeek]     NVARCHAR (50)      NOT NULL,
    [DayOfWeekName] NVARCHAR (50)      NOT NULL,
    [Weekday]       NVARCHAR (50)      NOT NULL,
    [MonthInYear]   NVARCHAR (50)      NOT NULL,
    [Quarter]       TINYINT            NOT NULL,
    [QuarterInYear] NVARCHAR (30)      NOT NULL,
    [CreatedOn]     DATETIMEOFFSET (7) DEFAULT (getdate()) NOT NULL,
    [UpdatedOn]     DATETIMEOFFSET (7) DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_dbo.Date] PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([date] ASC)
);

