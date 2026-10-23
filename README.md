# Gift of the Givers — Relief Management System

An ASP.NET Core Web Application (MVC) designed to streamline relief project tracking, volunteer allocations, and secure donation processing for the **Gift of the Givers** foundation.

---

## 🚀 Key Features

* **Relief Project Management:** Browse active crisis response initiatives and direct support where it's needed most.
* **Donation Flow:**
  * Support for one-time and recurring (monthly) donations.
  * Multi-currency support (ZAR, USD, EUR) with custom amount presets.
  * Section 18A Tax Certificate requests and ID/Tax Reference capture.
  * Guest and authenticated user donation paths.
* **Volunteer Portal:** Register for specific relief projects and manage volunteer availability.
* **Employee Portal:** Review donations and volunteer applications, and create relief projects with optional photos and campaign links.
* **Authentication & Role-Based Access (Identity):**
  * Secure User Registration and Login via ASP.NET Core Identity.
  * Role-Based Access Control (`Admin`, `Employee`, `Volunteer`, `Donor`).

---

## 🛠️ Tech Stack

* **Framework:** ASP.NET Core 8.0 / 9.0 / 10.0(MVC)
* **Language:** C#
* **Database & ORM:** SQL Server / LocalDB with Entity Framework Core
* **Authentication:** ASP.NET Core Identity (EF Core Stores)
* **Frontend:** Razor Views (`.cshtml`), Tailwind CSS, JavaScript (ES6+)

---

## 📋 Prerequisites

Ensure you have the following installed on your machine:

* [.NET SDK (8.0 or higher)](https://dotnet.microsoft.com/download)
* [Visual Studio 2022](https://visualstudio.microsoft.com/) (with *ASP.NET and web development* workload) or [Visual Studio Code](https://code.visualstudio.com/)
* [SQL Server Express / LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

---

## 🏁 Getting Started

### 1. Clone the Repository
```bash
git clone [https://github.com/YOUR_USERNAME/GiftOfTheGivers.git](https://github.com/YOUR_USERNAME/GiftOfTheGivers.git)
cd GiftOfTheGivers
