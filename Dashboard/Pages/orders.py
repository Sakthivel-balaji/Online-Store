import streamlit as st
import pyodbc
import pandas as pd
import plotly.express as px
import os
from dotenv import load_dotenv

load_dotenv()


st.set_page_config(page_title="Orders Dashboard", layout="wide")
st.title("Orders Insights Overview")

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
query_metrics = """
SELECT 
    COUNT(OrderId) AS TotalOrders,
    SUM(TotalPrice) AS TotalRevenue,
    AVG(TotalPrice) AS AverageOrderValue
FROM Orders
WHERE IsDeleted = 0;
"""
cursor.execute(query_metrics)
rows = cursor.fetchall()
df_metrics = pd.DataFrame.from_records(rows, columns=["TotalOrders", "TotalRevenue", "AverageOrderValue"])

# Get order volume trends (Yearly & Monthly)
query_order_volume = """
SELECT YEAR(OrderDate) AS Year, MONTH(OrderDate) AS Month, COUNT(OrderId) AS TotalOrders
FROM Orders
WHERE IsDeleted = 0 AND OrderDate IS NOT NULL
GROUP BY YEAR(OrderDate), MONTH(OrderDate)
ORDER BY Year DESC, Month DESC;
"""
cursor.execute(query_order_volume)
rows = cursor.fetchall()
df_order_volume = pd.DataFrame.from_records(rows, columns=["Year", "Month", "TotalOrders"]).sort_values(by=["Year", "Month"], ascending=True)
df_order_volume["YearMonth"] = df_order_volume["Year"].astype(str) + "-" + df_order_volume["Month"].astype(str)
df_order_volume["FormattedMonth"] = pd.to_datetime(df_order_volume[["Year", "Month"]].assign(day=1)).dt.strftime("%B %Y")

# Get revenue breakdown (Yearly & Monthly)
query_revenue_breakdown = """
SELECT YEAR(OrderDate) AS Year, MONTH(OrderDate) AS Month, SUM(TotalPrice) AS MonthlyRevenue
FROM Orders
WHERE IsDeleted = 0
GROUP BY YEAR(OrderDate), MONTH(OrderDate)
ORDER BY Year DESC, Month DESC;
"""
cursor.execute(query_revenue_breakdown)
rows = cursor.fetchall()
df_revenue_breakdown = pd.DataFrame.from_records(rows, columns=["Year", "Month", "MonthlyRevenue"]).sort_values(by=["Year", "Month"], ascending=True)
df_revenue_breakdown["YearMonth"] = df_revenue_breakdown["Year"].astype(str) + "-" + df_revenue_breakdown["Month"].astype(str)
df_revenue_breakdown["FormattedMonth"] = pd.to_datetime(df_revenue_breakdown[["Year", "Month"]].assign(day=1)).dt.strftime("%B %Y")

# Get order status analysis
query_order_status = """
SELECT Status, COUNT(OrderId) AS TotalOrders
FROM Orders
WHERE IsDeleted = 0
GROUP BY Status;
"""
cursor.execute(query_order_status)
rows = cursor.fetchall()
df_order_status = pd.DataFrame.from_records(rows, columns=["Status", "TotalOrders"])

# Get repeat vs first-time buyers
query_repeat_buyers = """
SELECT 
    COUNT(DISTINCT CASE WHEN OrderCount = 1 THEN CustomerId END) AS FirstTimeBuyers,
    COUNT(DISTINCT CASE WHEN OrderCount > 1 THEN CustomerId END) AS RepeatBuyers
FROM (
    SELECT CustomerId, COUNT(OrderId) AS OrderCount
    FROM Orders
    WHERE IsDeleted = 0
    GROUP BY CustomerId
) AS CustomerOrderCounts;
"""
cursor.execute(query_repeat_buyers)
rows = cursor.fetchall()
df_repeat_buyers = pd.DataFrame.from_records(rows, columns=["FirstTimeBuyers", "RepeatBuyers"])

# Get average delivery time (Regional)
query_delivery_time = """
SELECT da.Country, AVG(DATEDIFF(DAY, OrderDate, DeliveryDate)) AS AvgDeliveryTime
FROM Orders o
JOIN DeliveryAddresses da ON o.AddressId = da.AddressId 
WHERE Status = 'Delivered' AND o.IsDeleted = 0 AND DeliveryDate IS NOT NULL
GROUP BY da.Country
ORDER BY AvgDeliveryTime ASC;
"""
cursor.execute(query_delivery_time)
rows = cursor.fetchall()
df_delivery_time = pd.DataFrame.from_records(rows, columns=["Country", "AvgDeliveryTime"])

# Get order value distribution
query_order_value = """
SELECT OrderCategory, COUNT(OrderId) AS TotalOrders
FROM (
    SELECT 
        OrderId,
        CASE 
            WHEN TotalPrice < 50 THEN 'Low-Value'
            WHEN TotalPrice BETWEEN 50 AND 200 THEN 'Mid-Value'
            ELSE 'High-Value'
        END AS OrderCategory
    FROM Orders
    WHERE IsDeleted = 0
) AS CategorizedOrders
GROUP BY OrderCategory
ORDER BY TotalOrders DESC;
"""
cursor.execute(query_order_value)
rows = cursor.fetchall()
df_order_value = pd.DataFrame.from_records(rows, columns=["OrderCategory", "TotalOrders"])

df_order_value["Label"] = df_order_value["OrderCategory"].replace({
    "Low-Value": "₹0 - ₹49",
    "Mid-Value": "₹50 - ₹200",
    "High-Value": "₹201+"
})

cursor.close()
conn.close()

# Display Key Metrics
with st.container():
    st.markdown('<div class="center-text"><h3>Order Metrics Overview</h3></div>', unsafe_allow_html=True)
    col1, col2, col3 = st.columns(3)

    with col1:
        st.markdown(f'<div class="metric-card"><strong>Total Orders</strong><br>{df_metrics["TotalOrders"][0]:,}</div>', unsafe_allow_html=True)

    with col2:
        st.markdown(f'<div class="metric-card"><strong>Total Revenue</strong><br>₹{df_metrics["TotalRevenue"][0]:,.2f}</div>', unsafe_allow_html=True)

    with col3:
        st.markdown(f'<div class="metric-card"><strong>Avg Order Value (₹)</strong><br>₹{df_metrics["AverageOrderValue"][0]:,.2f}</div>', unsafe_allow_html=True)

st.divider()

# Display Charts
with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Order Volume Over Time")  
        fig_order_volume = px.line(df_order_volume, x="FormattedMonth", y="TotalOrders", markers=True)
        fig_order_volume.update_layout(xaxis_title="Month-Year",yaxis_title="TotalOrders")
        st.plotly_chart(fig_order_volume, use_container_width=True)

    with col2:
        st.markdown("Revenue Breakdown (Monthly)")  
        fig_revenue_breakdown = px.line(df_revenue_breakdown, x="FormattedMonth", y="MonthlyRevenue", markers=True)
        fig_revenue_breakdown.update_layout(xaxis_title="Month-Year",yaxis_title="Revenue (₹)")
        st.plotly_chart(fig_revenue_breakdown, use_container_width=True)

st.divider()

with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Order Status Overview")  
        fig_order_status_treemap = px.treemap(df_order_status, path=["Status"], values="TotalOrders", title="Order Status Distribution")
        st.plotly_chart(fig_order_status_treemap, use_container_width=True)

    with col2:
        st.markdown("Customer Retention (Repeat vs First-Time Buyers)")
        fig_repeat_buyers = px.pie(df_repeat_buyers, names=["FirstTimeBuyers", "RepeatBuyers"], values=[df_repeat_buyers["FirstTimeBuyers"][0], df_repeat_buyers["RepeatBuyers"][0]])
        st.plotly_chart(fig_repeat_buyers, use_container_width=True)

st.divider()

with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Average Delivery Time by Region")  
        fig_delivery_time = px.scatter(df_delivery_time, x="Country", y="AvgDeliveryTime", size="AvgDeliveryTime", color="Country")
        fig_delivery_time.update_layout(xaxis_title="Country",yaxis_title="Average Delivery Time in Days")
        st.plotly_chart(fig_delivery_time, use_container_width=True)


    with col2:
        st.markdown("Order Value Distribution")  
        fig_order_value = px.bar(df_order_value, x="OrderCategory", y="TotalOrders", color="OrderCategory", text="Label")
        fig_order_value.update_traces(textposition="outside")
        fig_order_value.update_layout(xaxis_title="OrderCategory (Value Range)",yaxis_title="TotalOrders")
        st.plotly_chart(fig_order_value, use_container_width=True)

st.divider()
