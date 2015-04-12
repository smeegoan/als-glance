namespace ALS.Glance.UoW.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.D_Date",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Day = c.Byte(nullable: false),
                        DayInMonth = c.String(nullable: false, maxLength: 50),
                        Month = c.Byte(nullable: false),
                        MonthName = c.String(nullable: false, maxLength: 50),
                        Year = c.Short(nullable: false),
                        DayOfWeek = c.String(nullable: false, maxLength: 50),
                        DayOfWeekName = c.String(nullable: false, maxLength: 50),
                        Weekday = c.String(nullable: false, maxLength: 50),
                        MonthInYear = c.String(nullable: false, maxLength: 50),
                        Quarter = c.Byte(nullable: false),
                        QuarterInYear = c.String(nullable: false, maxLength: 30),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Fact",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AUC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateId = c.Long(nullable: false),
                        MuscleId = c.Long(nullable: false),
                        PatientId = c.Long(nullable: false),
                        TimeId = c.Long(nullable: false),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.D_Date", t => t.DateId, cascadeDelete: true)
                .ForeignKey("dbo.D_Muscle", t => t.MuscleId, cascadeDelete: true)
                .ForeignKey("dbo.D_Patient", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.D_Time", t => t.TimeId, cascadeDelete: true)
                .Index(t => t.DateId)
                .Index(t => t.MuscleId)
                .Index(t => t.PatientId)
                .Index(t => t.TimeId);
            
            CreateTable(
                "dbo.D_Muscle",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200, unicode: false),
                        Abbreviation = c.String(nullable: false, maxLength: 30),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.D_Patient",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PatientId = c.String(nullable: false, maxLength: 30),
                        Sex = c.String(nullable: false, maxLength: 1, fixedLength: true, unicode: false),
                        Name = c.String(nullable: false, maxLength: 500, unicode: false),
                        BornOn = c.DateTimeOffset(nullable: false, precision: 7),
                        DiagnosedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.D_Time",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Hour = c.Short(nullable: false),
                        TimeOfDay = c.String(nullable: false, maxLength: 50, unicode: false),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fact", "TimeId", "dbo.D_Time");
            DropForeignKey("dbo.Fact", "PatientId", "dbo.D_Patient");
            DropForeignKey("dbo.Fact", "MuscleId", "dbo.D_Muscle");
            DropForeignKey("dbo.Fact", "DateId", "dbo.D_Date");
            DropIndex("dbo.Fact", new[] { "TimeId" });
            DropIndex("dbo.Fact", new[] { "PatientId" });
            DropIndex("dbo.Fact", new[] { "MuscleId" });
            DropIndex("dbo.Fact", new[] { "DateId" });
            DropTable("dbo.D_Time");
            DropTable("dbo.D_Patient");
            DropTable("dbo.D_Muscle");
            DropTable("dbo.Fact");
            DropTable("dbo.D_Date");
        }
    }
}
