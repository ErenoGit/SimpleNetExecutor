![bagde](https://img.shields.io/badge/GUI_type-WPF-blue)
![bagde2](https://img.shields.io/badge/.NET-8.0-purple)
![bagde3](https://img.shields.io/badge/REST%20API-green)
![bagde4](https://img.shields.io/badge/Server_application_type-ASP.NET_Core_Web_API-orange)
![bagde5](https://img.shields.io/badge/SQL-MSSQL-yellow)
![bagde6](https://img.shields.io/badge/Projekt_structure-client/server/example_module-pink)
![bagde7](https://img.shields.io/badge/OpenAPI-Swagger-lightblue)
![bagde8](https://img.shields.io/badge/Target_platform-Windows-darkgreen)
# Simple Net Executor
Simple .net DLL executor, separated to client (endpoint, simple .NET 8.0 program) / server (ASP.NET Core Web API application) / example module (simple NET 8.0 class library).

## Short description of how it works
The client (endpoint) downloads the DLL module via REST API from the server and runs it locally.

## Server
The server connects to the SQL (connection string defined in appsettings.json) database and handles three API requests:
![Swagger](https://i.imgur.com/BA7QRac.jpeg "Swagger")\
GET moduleMd5 - retrieves md5 of latest dll module from the sql database\
GET moduleDll - retrieves dll of selected by md5 module from the sql database\
POST moduleDll - adds a DLL module to the SQL database, also calculating its MD5 hash

POST moduleDll example code:\
![moduleDll](https://i.imgur.com/oVrnGO7.jpeg "moduleDll")

## Client
After launching, the client (endpoint) checks whether %appdata%/SimpleNetExecutor contains a clientId.txt with the client ID. If it is missing, we generate a new one based on a new GUID.\
The next step is to download current MD5 of latest version of the DLL module downloaded from the server via a GET request to the api/moduleMd5 API (with the client ID parameter). This request updates ‘lastEndpointHearthbeat’ in the SQL database for this client ID and downloads the MD5 of the latest DLL module.\
If %appdata%/SimpleNetExecutor contains module.dll with the same MD5 as the one just downloaded, we simply run it (we load this DLL into Assembly and run the Main method with output parameter Action\<string>). If the MD5s differ, we will download and update module.dll.

After completing above steps, the endpoint launches a WPF window that serves as a place to display the output from the loaded DLL module.
![client](https://i.imgur.com/Y1liCpx.jpeg "client")

Client UpdateService:
![updateService](https://i.imgur.com/NQWfekQ.jpeg "updateService")

## Example module
An example of a supported module is a regular .dll file containing Main(Action\<string> output).
![example](https://i.imgur.com/pOyi4Lp.jpeg "example")

## SQL server
SQL server structure (database name: "SimpleNetExecutor"):\
Table "modules":\
[id] [int] IDENTITY(1,1) NOT NULL,\
[endpointId] \[nvarchar](max) NOT NULL,\
[lastEndpointHeartbeat] \[datetime2](7) NOT NULL

Table "endpoints":\
[id] [int] IDENTITY(1,1) NOT NULL,\
[moduleMd5] \[nvarchar](max) NOT NULL,\
[moduleDll] \[varbinary](max) NOT NULL