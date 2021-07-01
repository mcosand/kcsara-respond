@echo off
pushd %~dp0

set USE_SQLITE=true
dotnet ef migrations add %1 --context SqliteRespondDbContext --output-dir Data/Migrations/SqliteMigrations

set USE_SQLITE=
dotnet ef migrations add %1 --context SqlServerRespondDbContext --output-dir Data/Migrations/SqlServerMigrations

popd