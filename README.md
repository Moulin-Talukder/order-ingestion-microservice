# Order Ingestion Microservice  
*A Technical Assessment Submission by Talukder Makid Uddin Moulin*

A production-ready Order Ingestion Microservice designed for a cloud-based ERP system.  
This service provides robust APIs with idempotency, validation, async processing, and optimized SQL operations.


## Repository Structure

order-ingestion-microservice/
│
├── sql/
│   ├── tables.sql
│   └── sp_InsertOrder.sql
│
├── src/
│   └── OrderIngest.Api/
│       ├── Program.cs
│       ├── Controllers/
│       │   └── OrdersController.cs
│       ├── Models/
│       │   └── OrderDto.cs
│       ├── Data/
│       │   └── SqlConnectionFactory.cs
│       └── Services/
│           └── MockLogisticsService.cs
│
├── Dockerfile
├── .github/workflows/ci-cd.yml
└── README.md


# run SQL scripts (using sqlcmd)
sqlcmd -S localhost,1433 -U sa -P 'YourStrong!Passw0rd' -i ./sql/tables.sql
sqlcmd -S localhost,1433 -U sa -P 'YourStrong!Passw0rd' -i ./sql/sp_InsertOrder.sql


docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest


