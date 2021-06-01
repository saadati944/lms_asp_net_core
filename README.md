# LearningManagementSystem with asp.net core mvc







## useful commands :

### database management commands
1. creating migrations
	```bash
	dotnet ef migrations add <migname>
	```
1. applying migrations
	```bash
	dotnet ef database update
	```

### sql server manangement commands

1. create volume
	```bash
	docker volume create mvclms
	```
1. remove volume
	```bash
	docker volume rm mvclms
	```
1. running container
	```bash
	docker run -e 'ACCEPT_EULA=Y' -v mvclms:/var/opt/mssql --name mssql -e 'SA_PASSWORD=!@qwe234' -p 1433:1433 -d mcr.microsoft.com/mssql/server
	```
1. stop the running container
	```bash
	docker container stop mssql
	```
1. start stopped container
	```bash
	docker container start mssql
	```


