
# 💸 UPI Transaction System – Backend (.NET Core)

A lightweight and modular UPI backend system built using ASP.NET Core 8 Web API and Entity Framework Core, following Clean Architecture principles.

---

## 🚀 Features

- ✅ Enable/Disable UPI for a 10 digit number (UPI Number / Mobile Number)
- 💰 Add money to a UPI Account
- 🔄 Transfer money between UPI Numbers
- 📊 Check Account balance
- 
- 🛡️ Enforces strict transaction rules:
  - Max Account balance: ₹1,00,000
  - Max Amount per transaction: ₹20,000
  - Max Amount daily transfer: ₹50,000
  - Max 3 transfers per day

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8 Web API  
- **ORM:** Entity Framework Core  
- **Database:** SQL Server  
- **Architecture:** Clean Architecture (Layered)

---

## 🏗️ Project Structure & Architecture

This solution follows **Clean Architecture (Layered)**:

```

UPITransactionSystem.sln
├── UPITransaction.API              → Presentation Layer (Controllers, Program.cs, CORS)
├── UPITransaction.Application      → Application Layer (Services, Interfaces, DTOs, BaseResponse)
├── UPITransaction.DataAccessLayer  → Data Layer (DbContext, Repositories, Entities)

````

Each layer has a clear responsibility for separation of concerns.

---

## ⚙️ Core Business Logic

All main business logic is implemented in the `Services` folder:

- 🔗 [UpiService.cs](https://github.com/ManikandanPanneer/UPITransactionSystem-Backend/blob/main/UPITransaction.Application/Services/UpiService.cs)
- 🔗 [UserService.cs](https://github.com/ManikandanPanneer/UPITransactionSystem-Backend/blob/main/UPITransaction.Application/Services/UserService.cs)

### business logic

1. ✅ **Enable/Disable UPI**
   - Verifies user existence
   - Toggles `IsUpiEnabled` flag

2. 💰 **Add Money**
   - Ensures balance doesn’t exceed ₹1,00,000

3. 🔄 **Transfer Money**
   - Validates:
     - UPI enabled for both users
     - Max ₹20,000/transaction
     - Max 3 transfers/day
     - Max ₹50,000/day total
   - Updates balances and stores transactions

4. 📊 **Check Balance**
   - Reads current balance from the DB

---

## 📂 Layer Highlights

| Layer                     | Responsibilities                             |
|---------------------------|----------------------------------------------|
| **API**                   | REST API Controllers, DI, Middleware setup   |
| **Application**           | Services, DTOs, Interfaces, Business Logic   |
| **DataAccessLayer**       | EF Core setup, Entities, Repositories        |

---

## 🔁 API Endpoints

### 👤 User Controller

| Endpoint                                                    | Method | Description                               |
|-------------------------------------------------------------|--------|-------------------------------------------|
| `/api/users`                                                | POST   | Register a new user                       |
| `/api/users/{phoneNumber}`                                  | GET    | Get user info by phone number             |
| `/api/users/validate/{phoneNumber}`                         | GET    | Validate if user exists                   |
| `/api/users/validate-receiver?senderPhone=&receiverPhone=`  | GET    | Validate sender and receiver for transfer |

🔗 [UserController.cs](https://github.com/ManikandanPanneer/UPITransactionSystem-Backend/blob/main/UPITransaction.API/Controllers/UserController.cs)

---

### 💳 UPI Controller

| Endpoint                              | Method | Description                    |
|---------------------------------------|--------|--------------------------------|
| `/api/upi/upi-status/{phoneNumber}`   | PATCH  | Enable or disable UPI          |
| `/api/upi/balance/{phoneNumber}`      | GET    | Get current account balance    |
| `/api/upi/add-money/{phoneNumber}`    | PUT    | Add money to account           |
| `/api/upi/transfer`                   | POST   | Transfer money to another user |

🔗 [UpiController.cs](https://github.com/ManikandanPanneer/UPITransactionSystem-Backend/blob/main/UPITransaction.API/Controllers/UpiController.cs)

---

## 🧱 Database Design (Simplified)

### User Table

| Column        | Type      |
|---------------|-----------|
| Id            | int       |
| Name          | string    |
| PhoneNumber   | string    |
| Balance       | decimal   |
| IsUpiEnabled  | boolean   |

### Transaction Table

| Column        | Type      |
|---------------|-----------|
| Id            | int       |
| FromUserId    | int       |
| ToUserId      | int       |
| Amount        | decimal   |
| Timestamp     | DateTime  |

---

## 🔐 CORS & Security

- ✅ CORS enabled for `http://localhost:5173` (frontend dev)
- 🧵 Asynchronous programming for scalability
- 🔍 Basic input validation included

---

## 📦 Setup Instructions

```bash
# 1. Clone the repository
git clone https://github.com/ManikandanPanneer/UPITransactionSystem-Backend.git
cd UPITransactionSystem-Backend

# 2. Update your SQL connection string in appsettings.json

# 3. Apply EF Core migrations
dotnet ef database update

# 4. Run the application
dotnet run
````

🔗 [appsetting.json](https://github.com/ManikandanPanneer/UPITransactionSystem-Backend/blob/main/UPITransaction.API/appsettings.json)

```

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=UpiDB;Trusted_Connection=True;"
}

````

