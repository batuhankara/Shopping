
# Running Project
In shopping folder you need to Running
```bash 
  docker-compose up
```
this script will run 3 container
mssqlDb,ShoppingAPI,EventStore read db

you can see endpoints on swagger.

# Project Flow
in this project , DDD,CQRS,Event sourcing applied with .net core 6.
some of basic features may be missing like command validation , seperated projectors , Service Bus , Mediator Pattern etc.
when you make a post request ,

Command>CommandService>Aggregate>SaveEvents>response

Eventstore>Projections>ReadStore

when you make a get request
Controller>ReadServiceService>Mssql
