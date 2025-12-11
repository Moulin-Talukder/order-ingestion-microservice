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

