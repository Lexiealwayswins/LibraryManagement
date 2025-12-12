# Library Management System

This is a .NET Core web application for managing a library's customers, books, and borrowing records. The application uses Entity Framework Core for database operations and provides CRUD functionality for customers.

## Prerequisites

- .NET 9.0.200 SDK
- SQLite3 database
- Visual Studio Code or another IDE with .NET support (optional)

## Setup Instructions

### 1. Download the project and open a terminal in the project directory
```bash
cd LibraryManagement
```

### 2. Configure the Database
- Open the `appsettings.json` file in the project root.
- Update the `ConnectionStrings:DefaultConnection` with your SQLite connection string. Example:
  ```json
  {
      "DefaultConnection": "Data Source=library.db"
  }
  ```
- Ensure the database user has permissions to create and modify databases.

### 3. Apply Database Migrations
- Run the following commands to create and apply the database schema:
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```
- This will create a database named `Library` (or the name specified in your connection string) with the necessary tables (`Customers`, `Books`, `LibraryBranches`, `Authors`).

### 4. Social Media Authentication Setup - Configure Google and Microsoft Authentication
- Register Applications
  - Google
    - Go to Google Cloud Console
    - Create a new project and navigate to APIs & Services → Credentials
    - Choose OAuth Client ID → Web Application
    - Set Authorized redirect URI to: https://localhost:5171/signin-google
    - Save and copy your Client ID and Client Secret

  - Microsoft
    - Go to Azure Portal
    - Navigate to Microsoft Entra ID → App registrations
    - Register a new application
    - Set redirect URI to: https://localhost:5171/signin-microsoft
    - In the manifest, set:
      ```Json
      "signInAudience": "AzureADandPersonalMicrosoftAccount",
      "accessTokenAcceptedVersion": 2
      ```
    - Generate Client Secret under Certificates & secrets
  - Add Secrets Locally
    - Run these commands to securely store secrets using dotnet user-secrets:
      ```bash
      dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
      dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"

      dotnet user-secrets set "Authentication:Microsoft:ClientId" "your-microsoft-client-id"
      dotnet user-secrets set "Authentication:Microsoft:ClientSecret" "your-microsoft-client-secret"

      ```
    - These secrets will be injected into your app at runtime via configuration.


### 5. Run the Application
- Restore dependencies and build the project:
  ```bash
  dotnet restore
  dotnet build
  ```
- Start the application:
  ```bash
  dotnet run
  ```
