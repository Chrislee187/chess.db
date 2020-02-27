# Chess DB

This is the EFCore code-first project containing the entities and migrations for the Chess database. It also contains reusable configuration/startup code to initialise the DbContext and perform any outstanding migrations.

Both SQLite and SQLServer are supported by using the `DbServerType` and `ChessDB` entries in the `appSettings.json`.

Although this is a component that is typically only referenced by other components requiring direct DB access, it is also a stand-alone executable which will create/migrate a DB as required.

If no settings are available we default to a SQLite database placed in `...\My Documents\ChessDB`

By default a new SQLite database (SQL Server also supported) will be created at `...\My Documents\Chess\ChessDB.sqlite`.

It will also report some simple metrics about the database upon successful creation/migration.

## Example Usage
Build with `dotnet build` and run the compiled `chess.games.db.exe`, or use `dotnet run`, in this folder to setup a default SQLite Database.

You should see output similar to;
```
Chess DB Creator
Creating new SQLite database: D:\Documents\ChessDB\ChessDB.sqlite
Connecting to SQLite chess database...
  Checking for pending migrations...
  Applying 1 pending migration(s)...
  ... database successfully updated.
Chess DB Status
  Valid games: 0
  Pending validation: 0
  Failed validations: 0
```

Change the `appSettings.json` configuration file for other DB options

### SQLlite appSettings.json config example
SQLite connection strings tend to be a simple file path and maybe a few options.
i.e.
```
DbServerType=SQLite
ChessDB="Data Source=c:\mydb.db;"
```

### SQLServer appSettings.json config example
SQLServer connection strings are typically a little more verbose.
i.e.
```
DbServerType=SQLServer
ChessDB="Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;"
```
Please note that currently, only `SQLite` & `SQLServer` database types are supported by the Chess DB EF code.

For more information on connection string syntax see [https://www.connectionstrings.com/](https://www.connectionstrings.com/). 

