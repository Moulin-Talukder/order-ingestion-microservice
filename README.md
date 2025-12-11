```markdown
# Order Ingestion Microservice  
*A Technical Assessment Submission by Talukder Makid Uddin Moulin*

A production-ready Order Ingestion Microservice designed for a cloud-based ERP system.  
This service provides robust APIs with idempotency, validation, async processing, and optimized SQL operations.

---

## Repository Structure

```

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

````

---

## Prerequisites

- **.NET 8 SDK**  
- **SQL Server** (local or Docker)  
- **sqlcmd** (optional, for running SQL scripts)  
- **Docker** (optional, for containerized runs)

---

## Quick Start — Run Locally

### 1. Start SQL Server (via Docker)
> Replace `"YourStrong!Passw0rd"` with your own password before running.

```bash
docker run -e "ACCEPT_EULA=Y" \
    -e "SA_PASSWORD=YourStrong!Passw0rd" \
    -p 1433:1433 \
    -d mcr.microsoft.com/mssql/server:2022-latest
````

---

### 2. Create database & run SQL scripts

```bash
# Create database
sqlcmd -S localhost,1433 -U sa -P 'YourStrong!Passw0rd' \
  -Q "CREATE DATABASE OrderIngest"
```

```bash
# Run DDL and stored procedure
sqlcmd -S localhost,1433 -U sa -P 'YourStrong!Passw0rd' \
  -d OrderIngest -i ./sql/tables.sql

sqlcmd -S localhost,1433 -U sa -P 'YourStrong!Passw0rd' \
  -d OrderIngest -i ./sql/sp_InsertOrder.sql
```

If you prefer, you can execute the `.sql` files manually using SQL Server Management Studio or Azure Data Studio.

---

### 3. Configure Development Connection String

Create the file:

**`src/OrderIngest.Api/appsettings.Development.json`**

```json
{
  "ConnectionStrings": {
    "OrdersDb": "Server=localhost,1433;Database=OrderIngest;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
  }
}
```

> ⚠️ **Do NOT commit real passwords or production secrets.**
> This is for local testing only.

---

### 4. Run the API

```bash
cd src/OrderIngest.Api
dotnet run
```

The API will start at:

* `http://localhost:5000`
* `https://localhost:5001`

Swagger UI is enabled in Development (`/swagger`).

---

## API Usage

### **POST /api/v1/orders**

Headers:

```
Idempotency-Key: <unique-key>
Content-Type: application/json
```

Example request:

```bash
curl -X POST http://localhost:5000/api/v1/orders \
  -H "Content-Type: application/json" \
  -H "Idempotency-Key: my-request-123" \
  -d '{
    "orderId":"00000000-0000-0000-0000-000000000001",
    "customer":{"email":"moulin@example.com","name":"Moulin"},
    "currency":"USD",
    "totalAmount":20.00,
    "items":[{"sku":"ABC","productName":"Widget","quantity":2,"unitPrice":10.00}]
  }'
```

---

## Validation Errors Returned

| Code         | Meaning                  |
| ------------ | ------------------------ |
| **ERR_1001** | Missing Idempotency-Key  |
| **ERR_1002** | Invalid or missing email |
| **ERR_1003** | Quantity must be > 0     |
| **ERR_1004** | UnitPrice must be > 0    |
| **ERR_1005** | TotalAmount mismatch     |
| **ERR_1006** | No items in order        |
| **ERR_1500** | Internal server error    |

---

## Key Implementation Details

### ✔ Idempotency

Implemented using the `IdempotencyRequests` table + stored procedure logic.
Duplicate requests return the existing `orderId` without re-inserting data.

### ✔ Performance

* SQL Server **table-valued parameter (TVP)** used for batch insert of order items.
* Single stored procedure ensures atomic, efficient transactions.

### ✔ Async Processing

A simulated third-party logistics call (2-second delay) runs **asynchronously**, keeping the API responsive.

### ✔ CI/CD

GitHub Actions workflow included (`.github/workflows/ci-cd.yml`) for full CI and example CD pipeline.

---

## Docker Support

Build container image:

```bash
docker build -t order-ingest-api .
```

Run:

```bash
docker run -p 5000:80 order-ingest-api
```

---

## Contact

**Talukder Makid Uddin Moulin**
Email: [moulin@example.com](mailto:moulin@example.com)
Linkedin: https://www.linkedin.com/in/moulin-talukder/

---

## License

This repository is provided for technical assessment and demonstration purposes.
Add a `LICENSE` file (MIT or other) if required.
