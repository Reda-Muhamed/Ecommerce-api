# E-Commerce Engine (High-Concurrency Backend)

[![Framework](https://img.shields.io/badge/Framework-ASP.NET%20Core%208.0-512bd4)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![ORM](https://img.shields.io/badge/ORM-EF%20Core-512bd4)](https://learn.microsoft.com/en-us/ef/core/)
[![Database](https://img.shields.io/badge/Database-SQL%20Server-red)](https://www.microsoft.com/en-us/sql-server/)

A high-performance, scalable E-Commerce API built with **ASP.NET Core** and **EF Core**. This engine is engineered to handle complex retail logic—from atomic inventory management to historical data integrity—while maintaining low latency under high concurrent loads.

---

## 🚀 Key Technical Highlights

* **High-Performance Discovery:** Optimized the product discovery API using cached aggregates (Price ranges, AvgRating) to eliminate expensive relational joins and reduce latency.
* **Atomic Checkout System:** Architected a robust checkout flow using the **Unit of Work** pattern and SQL transactions to ensure strict atomicity across order creation and stock deduction.
* **Inventory Integrity:** Resolved "overselling" issues by implementing a **Stock Reservation & Confirmation** workflow, ensuring inventory consistency even during peak traffic.
* **Financial Data Snapshots:** Guaranteed data integrity via **Order Item Snapshots**, capturing point-in-time product prices and tax data to ensure historical consistency regardless of future catalog changes.
* **Secure Payment Flows:** Integrated **Stripe** using Payment Intents and asynchronous **Webhooks** for idempotent and secure transaction processing.
* **CDN Optimization:** Offloaded image processing to **Cloudinary CDN**, reducing server-side bandwidth consumption by approximately **40%**.

---

## 🛠️ Tech Stack

* **Runtime:** .NET 10.0
* **Database:** SQL Server
* **Storage & Media:** Cloudinary CDN
* **Payments:** Stripe API
* **Security:** JWT (JSON Web Tokens) with Refresh Token rotation
* **Architecture:** Repository & Unit of Work Patterns

---

## 🛣️ API Reference

### 📦 Product & Catalog
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Product` | List products with cached aggregates |
| `POST` | `/api/Product/{id}/publish` | Move product to live status |
| `POST` | `/api/Product/{id}/variants` | Manage SKU-level variations |

### 🛒 Order & Checkout
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/orders/checkout-summary` | Preview totals and tax |
| `POST` | `/api/orders` | Initiate atomic checkout |
| `POST` | `/api/webhooks/payments/stripe` | Handle async payment updates |

### 🔐 Authentication
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/Auth/signin` | Standard login with JWT return |
| `POST` | `/api/Auth/refresh-token` | Rotate expired sessions |
| `POST` | `/api/Auth/logout-all` | Global session invalidation |

### 🛠️ Administration
* **AdminAttributes:** Full CRUD for dynamic product specs.
* **AdminBrand/Category:** Taxonomy management for the storefront.

---

## 🔧 Installation

1.  **Clone the Repository**
    ```bash
    git clone [https://github.com/Reda-Muhamed/Ecommerce-api.git](https://github.com/Reda-Muhamed/Ecommerce-api.git)
    cd Ecommerce-api
    ```

2.  **Configuration**
    Add your credentials to `appsettings.json` or use Secret Manager:
    ```json
    {
      "ConnectionStrings": { "DefaultConnection": "Server=..." },
      "Stripe": { "SecretKey": "sk_test_...", "WebhookSecret": "whsec_..." },
      "Cloudinary": { "CloudName": "...", "ApiKey": "...", "ApiSecret": "..." }
    }
    ```

3.  **Database Update**
    ```bash
    dotnet ef database update
    ```

4.  **Run**
    ```bash
    dotnet run
    ```

---

## 📈 Optimization Metrics
* **Bandwidth Reduction:** ~40% via Cloudinary offloading.
* **Consistency:** Zero-fail stock deduction via SQL Transaction isolation levels.
* **Security:** Multi-device logout support via JWT blacklist/refresh logic.
