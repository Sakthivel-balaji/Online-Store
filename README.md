# 🛒 Online Store

A full-stack **Online Store application** built using **.NET (Web API)**, **Angular/React (Frontend)**, and **SQL Server (Database)**, with a **Streamlit dashboard** for analytics.

---

## 📋 Project Overview

This online store allows users to manage **products**, **orders**, and **customers**. The system includes features for:

- **Add, update, delete products**
- **Manage customer information**
- **Process and track orders**
- **Handle edge cases** (e.g., out-of-stock orders)
- **Asynchronous API operations** for product details
- **Secure authentication & authorization**
- **Dashboard analytics** for products, orders, and customers

The app provides:

- A responsive **Angular web interface** (complete)  
- A **React web interface** (work-in-progress, only two pages)  
- A **RESTful .NET API** backend  
- SQL scripts for database setup  
- A **Streamlit dashboard** for insights  

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Sakthivel-balaji/Online-Store.git
```

Keep the `.NET API`, `Angular UI`, `React UI`, and `Dashboard` in **separate folders**.

---

### 2. Set Up the Database

- Create a new **SQL Server database**  
- Use the provided **DB scripts** to create tables and insert sample data (see `DB Scripts/` folder)  

---

### 3. Configure the Backend

- Open `appsettings.json` in the `.NET API` project  
- Update the **ConnectionStrings** section with your SQL Server credentials  and JWT configurations

---

### 4. Run the Angular Frontend (Recommended)

```bash
cd UI/Angular UI
npm install --force
ng serve
```

---

### 5. Run the React Frontend (Experimental)

> ⚠️ **Note:** React version is incomplete (only two pages implemented).

```bash
cd UI/React UI
npm install --force
npm start
```

---

### 6. Run the .NET Backend

```bash
cd API
dotnet run
```

> Or open the project in **Visual Studio** and press **Start**

---

### 7. Streamlit Dashboard

- Install dependencies:

```bash
pip install -r requirements.txt
```

- Rename `env.sample` to `.env` and set your DB connection details  
- Run the dashboard:

```bash
python -m streamlit run app.py --server.runOnSave true --server.port 3000
```

> ⚠️ The dashboard is compatible **only in light theme**

---

## 📦 Tech Stack

| Layer       | Technology             |
| ----------- | ---------------------- |
| Frontend    | Angular (complete), React (in-progress) |
| Backend API | ASP.NET Core           |
| Database    | SQL Server             |
| Dashboard   | Python + Streamlit     |
| Auth        | JWT-based (or similar) |

---

## 📁 Folder Structure

```
/API          # .NET backend
/UI/Angular UI   # Angular frontend
/UI/React UI     # React frontend (incomplete)
/DB Scripts   # SQL setup scripts
/Dashboard    # Streamlit dashboard
```

---

