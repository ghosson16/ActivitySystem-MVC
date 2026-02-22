# 🚀 Activity Management System

### 📌 Project Overview
A scalable web application built with **ASP.NET Core 10 MVC**. This system bridges the gap between event organizers and attendees by providing dedicated, role-based dashboards to manage activities, subscriptions, and attendance tracking seamlessly.

---

### 🏗️ Architectural Decisions & Logic
* **ViewModel Strategy:** Implemented to decouple domain models from the presentation layer. This approach ensures better security, maintains data integrity, and prevents sensitive information from being exposed in the UI.
* **Smart Soft Delete Policy:** To ensure a consistent user experience (UX), activities are never abruptly removed from the database. Instead, a soft delete mechanism is applied so that attendees do not experience sudden data disappearance.

---

### 🛠️ Tech Stack
* **Framework:** .NET 10.0 (Latest Release)
* **ORM:** Entity Framework Core 10.0.3 (SQL Server)
* **Identity:** Microsoft ASP.NET Core Identity for secure Authentication & Role-Based Authorization.
* **Frontend:** HTML5, CSS3, JavaScript, and Bootstrap 5 for a responsive, modern UI.

---

### ⚙️ Setup & Installation
1.  **Prerequisites:** Ensure you have the **.NET 10 SDK** installed.
2.  **Configuration:** Update the `DefaultConnection` string in `appsettings.json` to point to your SQL Server instance.
3.  **Database Migration:** Open the Package Manager Console and run:
    ```powershell
    Update-Database
    ```
4.  **Run:** Press `F5` in Visual Studio or use `dotnet run`.
