# 🛒 Online Store

A full-stack **Online Store application** built using **.NET (Web API)**, **Angular (Frontend)**, and **SQL Server (Database)**.

---

## 📋 Project Overview

This online store allows users to manage **products**, **orders**, and **customers**. The system includes features for:

* Adding, updating, and deleting products
* Managing customer information
* Processing and tracking orders
* Handling edge cases like placing orders for out-of-stock items
* Asynchronous API operations for fetching product details
* Secure authentication and authorization
* A dashboard with product/order/customer analytics using Streamlit

The app provides:

* A responsive **Angular web interface**
* A **RESTful .NET API** backend
* SQL scripts for database setup
* A **Streamlit dashboard** for insights

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/online-store.git
```

Keep the `.NET API` and `Angular UI` in **separate folders**.

---

### 2. Set Up the Database

* Create a new SQL Server database
* Use the provided **DB scripts** to create tables and insert sample data (see `db-scripts/` folder)

---

### 3. Configure the Backend

* Open `appsettings.json` in the `.NET API` project
* Update the `ConnectionStrings` section with your SQL Server credentials

---

### 4. Run the Angular Frontend

```bash
cd UI
npm install
ng serve
```

---

### 5. Run the .NET Backend

```bash
cd API
dotnet run
```

> Or open the project in Visual Studio and press **Start**

---

### 6. Streamlit Dashboard (Optional)

* Rename `env.sample` to `.env`
* Set your DB connection details in the `.env` file
* Run the dashboard:

```bash
streamlit run app.py
```

---

## 📦 Tech Stack

| Layer       | Technology             |
| ----------- | ---------------------- |
| Frontend    | Angular                |
| Backend API | ASP.NET Core           |
| Database    | SQL Server             |
| Dashboard   | Python + Streamlit     |
| Auth        | JWT-based (or similar) |

---

## 📁 Folder Structure

```
/API    # .NET backend
/UI     # Angular frontend
/DB Scripts           # SQL setup scripts
/Dashboard            # Streamlit dashboard
```

---

