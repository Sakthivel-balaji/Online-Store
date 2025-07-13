import streamlit as st
import pyodbc
import pandas as pd
import plotly.express as px
import os
from dotenv import load_dotenv

load_dotenv()


st.set_page_config(page_title="Customer Insights Dashboard", layout="wide")
st.title("Customer Insights Overview")

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

# Get key metrics
query = """
SELECT 
    COUNT(CustomerId) AS TotalCustomers,
    COUNT(DISTINCT CASE WHEN CreatedAt >= DATEADD(MONTH, -1, GETDATE()) THEN CustomerId END) AS NewCustomers,
    COUNT(DISTINCT CASE WHEN LatestOrderDate >= DATEADD(MONTH, -3, GETDATE()) THEN CustomerId END) AS ActiveCustomers
FROM (
    SELECT c.CustomerId, c.CreatedAt, MAX(o.OrderDate) AS LatestOrderDate 
    FROM Customers c
    LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
    WHERE c.IsDeleted = 0
    GROUP BY c.CustomerId, c.CreatedAt
) AS CustomerStats;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_metrics = pd.DataFrame.from_records(rows, columns=["TotalCustomers", "NewCustomers", "ActiveCustomers"])

# Get customer growth data
query = """
SELECT YEAR(CreatedAt) AS Year, MONTH(CreatedAt) AS Month, COUNT(CustomerId) AS NewCustomers
FROM Customers WHERE IsDeleted = 0 
GROUP BY YEAR(CreatedAt), MONTH(CreatedAt) ORDER BY Year DESC, Month DESC;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_growth = pd.DataFrame.from_records(rows, columns=["Year", "Month", "NewCustomers"]).sort_values(by=["Year", "Month"], ascending=True)
df_growth["YearMonth"] = df_growth["Year"].astype(str) + "-" + df_growth["Month"].astype(str)
df_growth["FormattedMonth"] = pd.to_datetime(df_growth[["Year", "Month"]].assign(day=1)).dt.strftime("%B %Y")

# Get active vs inactive customer data
query = """
WITH CustomerOrders AS (
    SELECT c.CustomerId, MAX(o.OrderDate) AS LatestOrderDate
    FROM Customers c
    LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
    WHERE c.IsDeleted = 0
    GROUP BY c.CustomerId
)
SELECT 
    COUNT(DISTINCT CASE WHEN LatestOrderDate >= DATEADD(MONTH, -3, GETDATE()) THEN CustomerId END) AS ActiveCustomers,
    COUNT(DISTINCT CASE WHEN LatestOrderDate IS NULL OR LatestOrderDate < DATEADD(MONTH, -3, GETDATE()) THEN CustomerId END) AS InactiveCustomers
FROM CustomerOrders;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_active_inactive = pd.DataFrame.from_records(rows, columns=["ActiveCustomers", "InactiveCustomers"])

# Get country-wise customer data
query = """
SELECT da.Country, COUNT(c.CustomerId) AS TotalCustomers
FROM Customers c JOIN DeliveryAddresses da ON c.CustomerId = da.CustomerId
WHERE c.IsDeleted = 0 AND da.IsPrimary = 1
GROUP BY da.Country ORDER BY TotalCustomers DESC;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_country = pd.DataFrame.from_records(rows, columns=["Country", "TotalCustomers"])

# Get customer shopping frequency
query = """
SELECT c.CustomerId, c.FullName, COUNT(o.OrderId) AS PurchaseCount
FROM Customers c JOIN Orders o ON c.CustomerId = o.CustomerId
WHERE c.IsDeleted = 0 AND o.IsDeleted = 0
GROUP BY c.CustomerId, c.FullName ORDER BY PurchaseCount DESC;
"""
cursor.execute(query)
rows = cursor.fetchall()
df_shopping_frequency = pd.DataFrame.from_records(rows, columns=["CustomerId", "FullName", "PurchaseCount"])

cursor.close()
conn.close()

# Display Key Metrics
with st.container():
    st.markdown('<div class="center-text"><h3>Customer Metrics</h3></div>', unsafe_allow_html=True)
    col1, col2, col3 = st.columns(3)

    with col1:
        st.markdown(f'<div class="metric-card"><strong>Total Customers</strong><br>{df_metrics["TotalCustomers"][0]:,}</div>', unsafe_allow_html=True)

    with col2:
        st.markdown(f'<div class="metric-card"><strong>New Customers(In last month)</strong><br>{df_metrics["NewCustomers"][0]:,}</div>', unsafe_allow_html=True)

    with col3:
        st.markdown(f'<div class="metric-card"><strong>Active Customers(In last 3 months)</strong><br>{df_metrics["ActiveCustomers"][0]:,}</div>', unsafe_allow_html=True)

st.divider()

# Display Charts
with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Customer Growth Over Time")
        fig_growth = px.line(df_growth, x="FormattedMonth", y="NewCustomers", markers=True)
        fig_growth.update_layout(xaxis_title="Month-Year", yaxis_title="New Customers")
        st.plotly_chart(fig_growth, use_container_width=True)

    with col2:
        st.markdown("Active vs Inactive Customers")
        fig_active_inactive = px.pie(df_active_inactive, names=["ActiveCustomers", "InactiveCustomers"], values=[df_active_inactive["ActiveCustomers"][0], df_active_inactive["InactiveCustomers"][0]])
        st.plotly_chart(fig_active_inactive, use_container_width=True)

st.divider()

with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Customer Distribution")
        fig_country = px.choropleth(df_country, locations="Country", locationmode="country names", color="TotalCustomers")
        st.plotly_chart(fig_country, use_container_width=True)

    with col2:
        st.markdown("Customer Shopping Frequency")
        fig_shopping = px.histogram(df_shopping_frequency, x="PurchaseCount", nbins=15)
        st.plotly_chart(fig_shopping, use_container_width=True)

st.divider()
