# BRaynov

## Init UI submodules

git submodule init  
git submodule update

## Adjust sqlite DB by CLI

1. install ef tools:
	dotnet tool install -g dotnet-ef

	1.1 update ef tools:
		dotnet tool update -g dotnet-ef --version <versionValue>

2. create migration:
	dotnet ef --startup-project .\src\WebApi migrations add InitialCreate -p .\src\Infrastructure -o Database\Migrations

3. apply migration:
	3.1 in startup project folder:
		dotnet ef --startup-project .\src\WebApi database update  -p .\src\Infrastructure 

	3.1 in custom folder:
		dotnet ef --startup-project .\src\WebApi database update -p .\src\Infrastructure --connection "Data Source=D:\\BRaynov.db"