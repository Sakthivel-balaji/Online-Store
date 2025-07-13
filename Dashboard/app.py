import streamlit as st
import pyodbc
import pandas as pd
import plotly.express as px
import os
from dotenv import load_dotenv

load_dotenv()

st.set_page_config(page_title="Store Dashboard", layout="wide")
st.title("Online Store Overview")

st.markdown(
    """
    <style>
        .metric-card {
            background-color: #f5f5f5;
            padding: 15px;
            border: 1px solid black;
            border-radius: 10px;
            text-align: center;
            font-size: 18px;
        }
        .center-text {
            text-align: center;
        }
    </style>
    """,
    unsafe_allow_html=True,
)

try:
    conn = pyodbc.connect(
        f"DRIVER={{{os.getenv("DB_DRIVER")}}};"
        f"SERVER={os.getenv("DB_SERVER")};"
        f"DATABASE={os.getenv("DB_DATABASE")};"
        f"UID={os.getenv("DB_UID")};"
        f"PWD={os.getenv("DB_PWD")}"
    )
    cursor = conn.cursor()
except Exception as e:
    st.error(f"Connection failed: {e}")
    st.stop()

st.divider()

# Get total revenue and orders count
query = """
SELECT SUM(TotalPrice) AS TotalRevenue, COUNT(OrderId) AS TotalOrders 
FROM Orders WHERE IsDeleted = 0;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_metrics = pd.DataFrame.from_records(rows, columns=["TotalRevenue", "TotalOrders"])

# Get monthly revenue grouped by year
query = """
SELECT YEAR(OrderDate) AS Year, MONTH(OrderDate) AS Month, SUM(TotalPrice) AS MonthlyRevenue
FROM Orders WHERE IsDeleted = 0 
GROUP BY YEAR(OrderDate), MONTH(OrderDate) 
ORDER BY Year DESC, Month DESC;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_sales = pd.DataFrame.from_records(rows, columns=["Year", "Month", "MonthlyRevenue"]).sort_values(by=["Year", "Month"], ascending=True)
df_sales["FormattedMonth"] = pd.to_datetime(df_sales[["Year", "Month"]].assign(day=1)).dt.strftime("%B %Y")

# Get top 10 products generating highest revenue
query = """
SELECT TOP 10 p.Name, COUNT(oi.ProductId) AS TotalOrders, SUM(oi.Quantity * oi.UnitPrice) AS TotalRevenue 
FROM OrderItems oi 
JOIN Products p ON oi.ProductId = p.ProductId 
WHERE oi.IsDeleted = 0 
GROUP BY p.Name 
ORDER BY TotalRevenue DESC;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_products = pd.DataFrame.from_records(rows, columns=["Name", "TotalOrders", "TotalRevenue"])
df_products["FormattedRevenue"] = df_products["TotalRevenue"].apply(lambda x: f"₹{x:,.2f}")

# Get new/old customers count
query = """
SELECT 
    COUNT(DISTINCT CASE WHEN CreatedAt >= DATEADD(MONTH, -1, GETDATE()) THEN CustomerId END) AS NewCustomers,
    COUNT(CustomerId) AS TotalCustomers
FROM Customers WHERE IsDeleted = 0;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_customers = pd.DataFrame.from_records(rows, columns=["NewCustomers", "TotalCustomers"])
df_customers["ReturningCustomers"] = df_customers["TotalCustomers"] - df_customers["NewCustomers"]

# Get top 10 highest spending customers
query = """
SELECT TOP 10 c.FullName, SUM(o.TotalPrice) AS TotalSpent
FROM Orders o JOIN Customers c ON o.CustomerId = c.CustomerId
WHERE o.IsDeleted = 0
GROUP BY c.FullName
ORDER BY TotalSpent DESC;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_spending = pd.DataFrame.from_records(rows, columns=["FullName", "TotalSpent"])

cursor.close()
conn.close()

# Metrics
with st.container():
    st.markdown('<div class="center-text"><h3>Key Business Metrics</h3></div>', unsafe_allow_html=True)
    col1, col2, col3 = st.columns(3)

    with col1:
        st.markdown(f'<div class="metric-card"><strong>Total Revenue</strong><br>₹{df_metrics["TotalRevenue"][0]:,.2f}</div>', unsafe_allow_html=True)

    with col2:
        st.markdown(f'<div class="metric-card"><strong>Total Orders</strong><br>{df_metrics["TotalOrders"][0]:,}</div>', unsafe_allow_html=True)

    with col3:
        st.markdown(f'<div class="metric-card"><strong>Total Customers</strong><br>{df_customers["TotalCustomers"][0]:,}</div>', unsafe_allow_html=True)

st.divider()

# Charts
with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Monthly Sales Trend")
        fig_sales = px.line(df_sales, x="FormattedMonth", y="MonthlyRevenue", markers=True)
        fig_sales.update_layout(xaxis_title="Month-Year",yaxis_title="Revenue (₹)")
        st.plotly_chart(fig_sales, use_container_width=True)

    with col2:
        st.markdown("Top 10 Best-Selling Products")
        fig_products = px.bar(df_products, x="TotalRevenue", y="Name", text_auto=True , orientation='h')
        fig_products.update_traces(textposition="outside")
        fig_products.update_layout(xaxis_title="Revenue (₹)",yaxis_title="Product Name")
        st.plotly_chart(fig_products, use_container_width=True)

st.divider()

with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Customer Retention (New customers in the last month vs Existing customers)")
        fig_customers = px.pie(df_customers, names=["NewCustomers", "ReturningCustomers"], values=[df_customers["NewCustomers"][0], df_customers["ReturningCustomers"][0]])
        st.plotly_chart(fig_customers, use_container_width=True)

    with col2:
        st.markdown("Top 10 Highest Spending Customers")
        fig_spending = px.bar(df_spending, x="TotalSpent", y="FullName",  text_auto=True , orientation='h')
        fig_spending.update_traces(textposition="outside")
        fig_spending.update_layout(xaxis_title="Amount Spent (₹)")
        st.plotly_chart(fig_spending, use_container_width=True)

st.divider()
