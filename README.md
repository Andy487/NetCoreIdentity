# NetCoreIdentity – User Authentication & Management

An ASP.NET Core 8 Razor Pages application demonstrating secure user authentication, role-based authorization, and user account management using ASP.NET Core Identity.

## Tech Stack

- **Backend:** C#, ASP.NET Core 8, Razor Pages
- **Authentication:** ASP.NET Core Identity, Cookie Authentication
- **Authorization:** Role-Based Authorization
- **ORM:** Entity Framework Core 8
- **Database:** SQL Server
- **Frontend:** HTML, CSS, Bootstrap
- **Development Tools:** Visual Studio, Git, GitHub

## Features Implemented

### 1. User Authentication
- User registration and login using ASP.NET Core Identity.
- Secure password hashing managed by Identity.
- Cookie-based authentication and logout.
- Unique email validation.
- Login and logout functionality.

### 2. Role-Based Authorization
- Admin and Employee roles.
- Role-based page access using `[Authorize(Roles = "Admin")]`.
- Protected Dashboard pages using `[Authorize]`.
- Access Denied page for unauthorized users.
- Separate administrative user-management functionality.

### 3. User Management (Admin)
- Admin can create new users.
- Admin can edit user details.
- Admin can assign or change user roles.
- Admin can activate and deactivate accounts.
- Admin can delete user accounts.
- User status is managed through a custom `IsActive` property.

### 4. Account Activation & Deactivation
- Inactive users cannot log in.
- Existing authenticated sessions are rejected when an account is deactivated.
- Administrators can reactivate accounts.
- Deleted users cannot continue accessing protected pages with an existing authentication cookie.

### 5. Account Security
- Configurable password policy:
  - Minimum 6 characters.
  - At least one uppercase letter.
  - At least one lowercase letter.
  - At least one digit.
- Account lockout configuration:
  - Maximum 5 failed login attempts.
  - 5-minute lockout duration.
- Authentication and authorization middleware.
- Secure POST-based logout with antiforgery protection.

### 6. Administrator Protection
- Prevent deletion of the last active Admin.
- Prevent deactivation of the last active Admin.
- Prevent removal of the Admin role from the last active Admin.
- Prevent an administrator from deleting their own account.
- Startup seeding automatically restores the configured System Administrator if the account is missing.

### 7. Database & Entity Framework Core
- Code-First database approach.
- Entity Framework Core migrations.
- SQL Server integration.
- ASP.NET Core Identity database tables.
- Custom `ApplicationUser` model with additional properties:
  - FullName
  - CreatedDate
  - IsActive

## Project Structure

```text
NetCoreIdentity/
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Models/
│   ├── ApplicationUser.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── EditUserViewModel.cs
│
├── Seed/
│   ├── RoleSeeder.cs
│   └── IdentitySeeder.cs
│
├── Pages/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   ├── Logout.cshtml
│   │   └── AccessDenied.cshtml
│   │
│   ├── Admin/
│   │   ├── CreateUser.cshtml
│   │   ├── EditUser.cshtml
│   │   ├── DeleteUser.cshtml
│   │   └── Users.cshtml
│   │
│   ├── Dashboard/
│   │   └── Dashboard.cshtml
│   │
│   └── Shared/
│       └── _Layout.cshtml
│
├── Migrations/
├── appsettings.json
└── Program.cs
```

*Note: The structure above represents the main project areas; actual files may differ.*

## Database Setup

1. Clone the repository:

```bash
git clone https://github.com/Andy487/NetCoreIdentity.git
```

2. Open the solution in Visual Studio.

3. Configure the SQL Server connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=NetCoreIdentityDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

4. Open Package Manager Console and execute:

```powershell
Update-Database
```

5. Run the application:

```bash
dotnet run
```

The application seeds the required roles and configured System Administrator account during startup.

## Security Testing

The following scenarios have been implemented and tested:

- Successful registration and login.
- Inactive user login rejection.
- Existing session rejection after deactivation.
- Existing session rejection after user deletion.
- Unauthorized access redirection.
- Employee restriction from Admin pages.
- POST-based logout.
- Last active Admin deletion protection.
- Last active Admin deactivation and role-removal protection.
- Administrator recovery through startup seeding.

## Learning Objectives

This personal project was developed to understand:

- ASP.NET Core Identity architecture.
- Authentication versus authorization.
- Role-based access control.
- Cookie authentication and session management.
- Entity Framework Core Code-First migrations.
- UserManager and SignInManager.
- Secure user lifecycle management.
- Practical application security.

**AI-assisted development:** ChatGPT was used as a learning, implementation, and debugging assistant while developing the project.

## GitHub

Repository: https://github.com/Andy487/NetCoreIdentity

---

**Project Status:** Authentication, authorization, user management, and core account-security features implemented. Further security hardening and testing are ongoing.
