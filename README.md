# AspNetCoreHangfireDemo

## Introduction

This demo showcases how to use Hangfire for executing recurring jobs and triggering immediate executions while preventing multiple instances of the same job from running simultaneously across multiple server instances.

![image](HangfireDemo.png)

## Prerequisites

- MongoDB Setup (Optional): If using Mongo, ensure it's running locally with settings specified in appsettings.json. Modify the file accordingly or run Mongo using:

```bash
docker run --restart always -d -p 27017:27017 -h $env:COMPUTERNAME --name mongo mongo:4.2.8 --replSet=EtnReplicaSet ; Start-Sleep -Seconds 1 ; docker exec mongo mongo --eval "rs.initiate();"
```

- PostgreSQL Setup (Alternative to Mongo): Alternatively, for PostgreSQL, execute:

```bash
docker run --restart always --name postgresql -p 5432:5432 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_INITDB_ARGS="--lc-collate='en_US.UTF-8' --lc-ctype='en_US.UTF-8'" -d postgres:12.16
```

## How to run

1. Choose Storage Implementation:

Modify the storage implementation in Program.cs by changing:

```csharp
var storageImplementation = StorageImplementation.Mongo;
```

to either:

```csharp
var storageImplementation = StorageImplementation.Mongo; // for MongoDB
```

or

```csharp
var storageImplementation = StorageImplementation.Postgres; // for PostgreSQL
```

2. Running Instances:

Navigate to the /Api folder and open separate consoles for each instance. Execute the following commands in each, ensuring different ports for each instance:

```bash
dotnet run --urls=http://localhost:5001/
```

```bash
dotnet run --urls=http://localhost:5002/
```

3. Testing:

Access the /Jobs endpoint on any active server. The DoSomething job should execute without overlapping with another instance of the same job.
