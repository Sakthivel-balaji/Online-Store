import streamlit as st
import pyodbc
import pandas as pd
import plotly.express as px
import os
from dotenv import load_dotenv

load_dotenv()


st.set_page_config(page_title="Products Dashboard", layout="wide")
st.title("Products Insights Overview")

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
    SUM(CASE WHEN p.IsPopular = 1 THEN 1 ELSE 0 END) AS TotalPopularProducts,
    SUM(CASE WHEN p.IsFeatured = 1 THEN 1 ELSE 0 END) AS TotalFeaturedProducts,
    COUNT(p.ProductId) AS TotalProducts
FROM Products p 
WHERE p.IsDeleted = 0;
"""
cursor.execute(query_metrics)
rows = cursor.fetchall()
df_metrics = pd.DataFrame.from_records(rows, columns=["TotalPopularProducts", "TotalFeaturedProducts", "TotalProducts"])

# Get products by category
query_categories = """
SELECT TOP 10 p.Category, COUNT(p.ProductId) AS TotalProducts
FROM Products p
WHERE p.IsDeleted = 0
GROUP BY p.Category
ORDER BY TotalProducts DESC;
"""
cursor.execute(query_categories)
rows = cursor.fetchall()
df_categories = pd.DataFrame.from_records(rows, columns=["Category", "TotalProducts"])

# Get best-selling products
query_best_sellers = """
SELECT TOP 10 p.Name, COUNT(oi.OrderId) AS TotalOrders, SUM(oi.Quantity * oi.UnitPrice) AS TotalRevenue
FROM OrderItems oi
JOIN Products p ON oi.ProductId = p.ProductId
WHERE oi.IsDeleted = 0 AND p.IsDeleted = 0
GROUP BY p.Name
ORDER BY TotalRevenue DESC;
"""
cursor.execute(query_best_sellers)
rows = cursor.fetchall()
df_best_sellers = pd.DataFrame.from_records(rows, columns=["Name", "TotalOrders", "TotalRevenue"])

# Get stock alerts (low-stock items)
query_stock_alerts = """
SELECT Name, StockQuantity
FROM Products
WHERE StockQuantity < 10 AND IsDeleted = 0
ORDER BY StockQuantity ASC;
"""
cursor.execute(query_stock_alerts)
rows = cursor.fetchall()
df_stock_alerts = pd.DataFrame.from_records(rows, columns=["Name", "StockQuantity"])

# Get product ratings categorization
query_ratings = """
SELECT p.Name, COALESCE(AVG(r.Rating), 0) AS AvgRating,
       CASE 
           WHEN COALESCE(AVG(r.Rating), 0) > 3 THEN 'Better'
           WHEN COALESCE(AVG(r.Rating), 0) = 3 THEN 'Average'
           WHEN COALESCE(AVG(r.Rating), 0) < 3 THEN 'Poor'
           ELSE 'No Ratings'
       END AS RatingCategory
FROM Products p
LEFT JOIN Reviews r ON p.ProductId = r.ProductId
JOIN OrderItems oi ON p.ProductId = oi.ProductId
JOIN Orders o ON oi.OrderId = o.OrderId
WHERE o.Status = 'Delivered' AND p.IsDeleted = 0 AND o.IsDeleted = 0 AND oi.IsDeleted = 0
GROUP BY p.Name
ORDER BY AvgRating DESC;
"""
cursor.execute(query_ratings)
rows = cursor.fetchall()
df_ratings = pd.DataFrame.from_records(rows, columns=["Name", "AvgRating", "RatingCategory"])

# Get monthly sales count per product
query_monthly_sales = """
SELECT YEAR(o.OrderDate) AS Year, MONTH(o.OrderDate) AS Month, p.Name, SUM(oi.Quantity) AS TotalSold
FROM Orders o
JOIN OrderItems oi ON o.OrderId = oi.OrderId
JOIN Products p ON oi.ProductId = p.ProductId
WHERE o.IsDeleted = 0 AND p.IsDeleted = 0 AND oi.IsDeleted = 0
GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate), p.Name
ORDER BY Year DESC, Month DESC, TotalSold DESC;
"""
cursor.execute(query_monthly_sales)
rows = cursor.fetchall()
df_monthly_sales = pd.DataFrame.from_records(rows, columns=["Year", "Month", "Name", "TotalSold"]).sort_values(by=["Year", "Month"], ascending=True)
df_monthly_sales["YearMonth"] = df_monthly_sales["Year"].astype(str) + "-" + df_monthly_sales["Month"].astype(str)
df_monthly_sales["FormattedMonth"] = pd.to_datetime(df_monthly_sales[["Year", "Month"]].assign(day=1)).dt.strftime("%B %Y")

# Get brand-wise product distribution
query_brand_distribution = """
SELECT TOP 10 p.Brand, COUNT(p.ProductId) AS TotalProducts
FROM Products p
WHERE p.IsDeleted = 0
GROUP BY p.Brand
ORDER BY TotalProducts DESC;
"""
cursor.execute(query_brand_distribution)
rows = cursor.fetchall()
df_brand_distribution = pd.DataFrame.from_records(rows, columns=["Brand", "TotalProducts"])

cursor.close()
conn.close()

# Display Key Metrics
with st.container():
    st.markdown('<div class="center-text"><h3>Product Metrics</h3></div>', unsafe_allow_html=True)
    col1, col2, col3 = st.columns(3)

    with col1:
        st.markdown(f'<div class="metric-card"><strong>Total Products</strong><br>{df_metrics["TotalProducts"][0]:,}</div>', unsafe_allow_html=True)

    with col2:
        st.markdown(f'<div class="metric-card"><strong>Featured Products</strong><br>{df_metrics["TotalFeaturedProducts"][0]:,}</div>', unsafe_allow_html=True)

    with col3:
        st.markdown(f'<div class="metric-card"><strong>Popular Products</strong><br>{df_metrics["TotalPopularProducts"][0]:,}</div>', unsafe_allow_html=True)

st.divider()

# Display Charts
with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Top 10 Category-Wise Product Distribution")  
        fig_categories = px.bar(df_categories, y="Category", x="TotalProducts", orientation='h')
        fig_categories.update_layout(xaxis_title="Total Products", yaxis_title="Category")
        st.plotly_chart(fig_categories, use_container_width=True)

    with col2:
        st.markdown("Top 10 Brand-Wise Product Distribution")  
        fig_brand = px.bar(df_brand_distribution, y="Brand", x="TotalProducts", orientation='h')
        fig_brand.update_layout(xaxis_title="Total Products", yaxis_title="Brand")
        st.plotly_chart(fig_brand, use_container_width=True)

st.divider()

with st.container():
    col1, col2 = st.columns(2)

    with col1:
        st.markdown("Low Stock Products (Stock Alerts)")
        fig_stock_alerts = px.bar(df_stock_alerts, y="Name", x="StockQuantity", orientation='h')
        fig_stock_alerts.update_layout(xaxis_title="Stock Quantity", yaxis_title="Product Name")
        st.plotly_chart(fig_stock_alerts, use_container_width=True)

    with col2:
        st.markdown("Product Rating Categories")  
        fig_ratings = px.bar(df_ratings, y="Name", x="AvgRating", orientation='h', color="RatingCategory")
        fig_ratings.update_layout(xaxis_title="Average Rating", yaxis_title="Product Name")
        st.plotly_chart(fig_ratings, use_container_width=True)

st.divider()

with st.container():
    col1, col2 = st.columns(2)
    
    with col1:
        st.markdown("Monthly sales count per product")
        fig_growth = px.line(df_monthly_sales, x="FormattedMonth", y="TotalSold", color="Name", markers=True)
        fig_growth.update_layout(xaxis_title="Month-Year", yaxis_title="Total Orders")
        st.plotly_chart(fig_growth, use_container_width=True)

    with col2:
        st.markdown("Top 10 Best-Selling Products")  
        fig_best_sellers = px.bar(df_best_sellers, y="Name", x="TotalRevenue", orientation='h')
        fig_best_sellers.update_layout(xaxis_title="Total Revenue", yaxis_title="Product Name")
        st.plotly_chart(fig_best_sellers, use_container_width=True)

st.divider()
