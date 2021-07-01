@echo off
pushd %~dp0

set USE_SQLITE=true
dotnet ef migrations remove --context SqliteRespondDbContext

set USE_SQLITE=
dotnet ef migrations remove --context SqlServerRespondDbContext

popd