# ABC Pharmacy — Web API (Backend)

> .NET 8 Web API · In-Memory Data · Swagger UI · CORS-ready for frontend

---

## Project Structure

```
AbcPharmacy/
├── Controllers/
│   ├── MedicinesController.cs   ← CRUD + search for medicines
│   └── SalesController.cs       ← Record & view sales, summary stats
├── DTOs/
│   ├── MedicineDtos.cs          ← Request/Response models for medicine
│   ├── SaleDtos.cs              ← Request/Response models for sales
│   └── ApiResponse.cs           ← Standard response envelope + PagedResult
├── Middleware/
│   └── GlobalExceptionMiddleware.cs  ← Catches all unhandled exceptions
├── Models/
│   ├── Medicine.cs              ← Medicine entity
│   └── Sale.cs                  ← Sale entity
├── Services/
│   ├── DataStore.cs             ← In-memory DB with seed data (replace with EF Core later)
│   ├── IMedicineService.cs
│   ├── MedicineService.cs
│   ├── ISaleService.cs
│   └── SaleService.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs                   ← DI setup, middleware pipeline
├── appsettings.json
└── AbcPharmacy.csproj
```

---

## Quick Start

```bash
# 1. Go to project folder
cd AbcPharmacy

# 2. Restore packages
dotnet restore

# 3. Run the API
dotnet run

# 4. Open Swagger UI in browser
http://localhost:5000
```

---

## API Endpoints

### Base URL
```
http://localhost:5000/api
```

### Standard Response Shape
Every endpoint returns:
```json
{
  "success": true,
  "message": "...",
  "data": { ... },
  "errors": []
}
```

---

## Medicines

### GET /api/medicines
List all medicines (grid view — Notes excluded).

**Query Parameters:**

| Param      | Type    | Default | Description                          |
|------------|---------|---------|--------------------------------------|
| search     | string  | null    | Filter by name or brand              |
| page       | int     | 1       | Page number                          |
| pageSize   | int     | 10      | Items per page (max 100)             |
| sortBy     | string  | name    | name / expiry / quantity / price / brand |
| ascending  | bool    | true    | Sort direction                       |

**Example:**
```
GET /api/medicines?search=paracetamol&page=1&pageSize=10
GET /api/medicines?sortBy=expiry&ascending=true
GET /api/medicines?search=cipla&sortBy=price&ascending=false
```

**Response:**
```json
{
  "success": true,
  "message": "15 medicine(s) found.",
  "data": {
    "items": [
      {
        "id": 1,
        "fullName": "Paracetamol 500mg",
        "expiryDate": "2026-01-15T00:00:00Z",
        "quantity": 250,
        "price": 12.50,
        "brand": "GSK Pharma",
        "daysUntilExpiry": 245,
        "isExpiringSoon": false,
        "isLowStock": false,
        "isExpired": false
      }
    ],
    "totalCount": 15,
    "page": 1,
    "pageSize": 10,
    "totalPages": 2,
    "hasNext": true,
    "hasPrev": false
  },
  "errors": []
}
```

**UI Colour Logic:**
- `isExpired: true`       → Red background (expired)
- `isExpiringSoon: true`  → Red background (< 30 days)
- `isLowStock: true`      → Yellow background (quantity < 10)

---

### GET /api/medicines/{id}
Get full medicine details including Notes.

```
GET /api/medicines/1
```

---

### POST /api/medicines
Add a new medicine.

**Request Body:**
```json
{
  "fullName": "Aspirin 100mg",
  "notes": "Blood thinner. Do not give to children under 16.",
  "expiryDate": "2026-06-30T00:00:00Z",
  "quantity": 100,
  "price": 8.50,
  "brand": "Bayer"
}
```

**Validation Rules:**
- `fullName`   — required, max 200 chars
- `notes`      — optional, max 1000 chars
- `expiryDate` — required
- `quantity`   — required, >= 0
- `price`      — required, > 0
- `brand`      — required, max 150 chars

---

### PUT /api/medicines/{id}
Partial update — send only the fields you want to change.

```json
{
  "quantity": 150,
  "price": 9.00
}
```

---

### DELETE /api/medicines/{id}
Delete a medicine by ID.

```
DELETE /api/medicines/3
```

---

## Sales

### GET /api/sales
List all sale records (newest first).

**Query Parameters:**

| Param      | Type | Default | Description                          |
|------------|------|---------|--------------------------------------|
| medicineId | int  | null    | Filter by medicine                   |
| page       | int  | 1       | Page number                          |
| pageSize   | int  | 10      | Items per page                       |

---

### GET /api/sales/{id}
Get a single sale record.

---

### POST /api/sales
Record a new sale. Automatically:
- Validates the medicine exists
- Validates the medicine is not expired
- Validates sufficient stock is available
- Deducts sold quantity from medicine stock
- Captures unit price at time of sale (snapshot)

**Request Body:**
```json
{
  "medicineId": 1,
  "quantitySold": 10,
  "customerName": "Rahul Sharma",
  "notes": "Monthly prescription refill"
}
```

**Error cases (400):**
- Medicine not found
- Medicine is expired
- Insufficient stock available

**Success Response (201):**
```json
{
  "success": true,
  "message": "Sale recorded successfully. Stock updated.",
  "data": {
    "id": 7,
    "medicineId": 1,
    "medicineName": "Paracetamol 500mg",
    "medicineBrand": "GSK Pharma",
    "quantitySold": 10,
    "unitPrice": 12.50,
    "totalAmount": 125.00,
    "customerName": "Rahul Sharma",
    "notes": "Monthly prescription refill",
    "soldAt": "2025-05-14T10:30:00Z"
  }
}
```

---

### GET /api/sales/summary
Aggregated stats for sales dashboard.

**Response:**
```json
{
  "success": true,
  "data": {
    "totalTransactions": 6,
    "totalRevenue": 3302.50,
    "totalUnitsSold": 55,
    "latestSale": { ... }
  }
}
```

---

## Seed Data

The in-memory store contains **15 medicines** covering all UI states:

| State                         | Examples                                     |
|-------------------------------|----------------------------------------------|
| Normal (green)                | Paracetamol, Ibuprofen, Metformin            |
| Expiring soon — RED (< 30d)   | Azithromycin (20d), Pantoprazole (12d), Diclofenac (5d) |
| Low stock — YELLOW (< 10 qty) | Insulin Glargine (7), Levothyroxine (5), Clonazepam (3) |
| Both conditions               | Dexamethasone (18d, qty 8)                   |
| Expired                       | Ranitidine (expired 10 days ago)             |

---

## Migrating to Database (Next Steps)

When ready to connect to SQL Server with EF Core:

1. Install packages:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

2. Create `AppDbContext` with `DbSet<Medicine>` and `DbSet<Sale>`

3. Replace `DataStore` injection with `AppDbContext` in services

4. Change service methods from in-memory LINQ to async EF Core queries

5. Add connection string to `appsettings.json`

6. Run migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

## CORS

CORS is configured to `AllowAll` for development.
Before going to production, restrict to your frontend domain in `Program.cs`:

```csharp
options.AddPolicy("Production", policy =>
    policy.WithOrigins("https://abc-pharmacy.com")
          .AllowAnyMethod()
          .AllowAnyHeader());
```
