namespace Chronos.Server.Data

open FluentMigrator

[<Migration(20230927L, "Initial migration with base tables")>]
type InitialMigration() =
    inherit Migration()

    override this.Up() =
        this.Create
            .Table("Apps")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Name")
            .AsString()
            .NotNullable()
            .WithColumn("ProcessId")
            .AsGuid()
        |> ignore

        this.Create
            .Table("Processes")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Name")
            .AsString()
            .NotNullable()
        |> ignore

        this.Create
            .Table("Activities")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Name")
            .AsString()
            .NotNullable()
            .WithColumn("Start")
            .AsDateTime()
            .NotNullable()
            .WithColumn("End")
            .AsDateTime()
            .NotNullable()
            .WithColumn("AppId")
            .AsGuid()
            .ForeignKey("Apps", "Id")
            .WithColumn("ProcessId")
            .AsGuid()
            .ForeignKey("Processes", "Id")
        |> ignore

    override this.Down() =
        this.Delete.Table("Activities") |> ignore
        this.Delete.Table("Apps") |> ignore
        this.Delete.Table("Processes") |> ignore
