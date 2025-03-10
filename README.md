# 📌 Sample API
A Sample .NET 9 Web API project template with basic CRUD operations.  

---

## 📜 Features
✅ **RESTful API** with standard HTTP methods  
✅ **Structured API responses** using `ApiResponse`  
✅ **Model validation** with `DataAnnotations`  
✅ **Dependency injection** for better modularity  
✅ **Cancellation tokens** for graceful API termination  
✅ **Swagger integration** for API documentation  
✅ **In-Memory Database** for lightweight data storage  

---

## **🚀 Getting Started**  

### Prerequisites  
- Download and Install [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download)  
- **Visual Studio 2022** / **JetBrains Rider** / **VS Code**  

### 📥 Setup
1. Clone the repository:  
    ```bash
    git clone https://github.com/slalithprasad/ConsoleFocus
    cd ConsoleFocus
    ```
2. Install the required NuGet packages:  
    ```bash
    dotnet restore
    ```

### ▶️ Run the API 
```sh
dotnet run
```

---

## 🗄️ Database  
This project uses an **In-Memory Database** for simplicity. No external database setup is required. Data will be lost when the application restarts.  

---

## 📡 API Endpoints  

### 📝 Orders

| Method | Endpoint           | Description                     |
|--------|--------------------|---------------------------------|
| `GET`  | `/api/order/{id}`  | Get an order by ID             |
| `GET`  | `/api/order`       | Get a list of orders (filter)  |
| `POST` | `/api/order`       | Create a new order             |
| `PUT`  | `/api/order`       | Update an order                |
| `DELETE` | `/api/order/{id}` | Delete an order by ID          |

---