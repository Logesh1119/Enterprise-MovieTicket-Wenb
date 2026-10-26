# Enterprise-MovieTicket-Wenb — Movie Ticket Booking Application

A full-stack enterprise Movie Ticket Booking web application featuring authentication, theater management, screen & seat configuration, movie scheduling, and ticket booking, built with ASP.NET Core Razor Pages, Clean Architecture, and SQL Server.

---

## Security & Configuration Overview

For security best practices, actual credentials (such as database passwords, Gmail SMTP credentials, and JWT signing keys) are intentionally **NOT** committed to this repository. All sensitive settings are configured to be loaded from **ASP.NET Core User Secrets** during local development, keeping your credentials secure and out of source control.

---

## Local Setup & Configuration

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (or target framework installed)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server) (LocalDB, SQL Express, or Developer edition)
- Git & GitHub CLI (optional)

### 1. Database Setup
1. Create a database named `MovieTicketDb` in your local SQL Server instance.
2. Execute the database schema script `SQLDatabase.txt` to initialize tables, relationships, and initial seed data (Roles, Genres, Seat Categories, Amenities).
3. If applicable, execute `db_update.sql` and `db_seed.sql` to include additional modules (Bookings, BookingSeats, ActivityLogs) and sample data.

---

### 2. Configure Local Secrets

Navigate to the presentation project directory:

```bash
cd MovieTicketBookingController
```

#### A. Configure SQL Server Connection
The default connection string in `appsettings.json` points to:
`Server=localhost;Database=MovieTicketDb;Trusted_Connection=True;TrustServerCertificate=True;`

If your SQL Server uses SQL Authentication (username and password) or a named instance, set it using User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER_NAME;Database=MovieTicketDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

#### B. Configure JWT Secret Key
The application uses JWT authentication for session tokens and API authorization. Generate a strong, random 256-bit (or 512-bit) secret key and store it in your local User Secrets:

```bash
dotnet user-secrets set "JwtSettings:SecretKey" "<YOUR_STRONG_RANDOM_SECRET_KEY>"
```

#### C. Configure Gmail SMTP (EmailSettings)
The application requires SMTP to send verification OTPs and email notifications. In `appsettings.json`, non-sensitive SMTP settings (Host: `smtp.gmail.com`, Port: `587`) and sender information are configured.

Configure your Gmail App Password securely via User Secrets:

```bash
dotnet user-secrets set "EmailSettings:Password" "<YOUR_GMAIL_APP_PASSWORD>"
```

> **Note:** If you are using Gmail, generate a 16-character App Password under your Google Account security settings (2-Step Verification required).

---

### 3. Build and Run

To build the entire solution:

```bash
dotnet build
```

To run the application:

```bash
cd MovieTicketBookingController
dotnet run
```

Then navigate to `http://localhost:5176` (or the configured port) in your browser.
