# Professional Full-Stack Auth System

A modern, secure, and robust authentication system built with **Angular 21** and **.NET 10 Web API**. This project demonstrates best practices for JWT-based identity management, including refresh tokens, social login integration, and secure account management.

---

## 🛠️ Our Implementation Journey

We built this project from the ground up, focusing on security, user experience, and modern architecture. Here is a breakdown of the key milestones:

### 1. Foundation & Core Auth
- **Backend Architecture**: Set up a .NET 10 Web API using Repository Pattern and Dependency Injection.
- **Identity Storage**: Implemented EF Core with SQLite for lightweight, portable data storage.
- **JWT & Refresh Tokens**: Implemented a dual-token system for secure, persistent sessions.
- **Angular Scaffolding**: Created a standalone Angular 21 application with a modular service-based architecture.

### 2. Social Integration & User Data
- **Google OAuth2**: Integrated Google One-Tap/Button sign-in, verifying backend tokens against Google's API.
- **Enhanced Profiles**: Added timestamp tracking for user registration (`CreatedAt`).
- **Standardized Messaging**: standardizing backend responses to provide clear "User already exists" feedback for signup conflicts.

### 3. Account Management & Security
- **Secure Account Deletion**: Built a `DELETE /api/users/delete-account` endpoint that uses JWT claims (NameIdentifier) to ensure users can only delete their own data.
- **Frontend Cleanup**: Implemented automatic token clearing and redirection to the sign-in page upon account deletion.
- **Git Security**: Audited the codebase for secrets and set up a multi-layer configuration system (`appsettings.json` + `appsettings.Development.json`) to keep API keys off GitHub.

### 4. UI/UX Refinement
- **Timezone Intelligence**: Solved the UTC vs. Local Time conflict by ensuring the backend emits ISO-8601 UTC strings and the frontend converts them to the user's local system time.
- **Material Design**: Used Angular Material for sleek, responsive cards, forms, and buttons.

---

## 🚀 Features

-   **JWT Authentication**: Secure token-based authentication.
-   **Refresh Token Pattern**: Silent token renewal to keep users logged in securely.
-   **Google OAuth2 Integration**: "Sign in with Google" implementation.
-   **Secure Account Deletion**: Self-service account removal with JWT-claim verification.
-   **Dynamic Local Timezone**: Automatic conversion of UTC timestamps to user's local system time.

## 🛠️ Technology Stack

### Backend
-   **.NET 10 Web API**
-   **Entity Framework Core**
-   **SQLite**
-   **JWT Bearer Authentication**

### Frontend
-   **Angular 21** (Standalone)
-   **Angular Material**
-   **RxJS**

## 📋 Prerequisites

-   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
-   [Node.js](https://nodejs.org/) (v18+)

## ⚙️ Setup Instructions

### 1. Backend Setup
```bash
cd JwtDemo.API
dotnet restore
dotnet ef database update
dotnet run
```

### 2. Frontend Setup
```bash
cd jwt-demo-client
npm install
ng serve
```

## 🔒 Security Practices
-   **Secret Masking**: Secrets are stored in `appsettings.Development.json` (gitignored).
-   **Claim Verification**: The backend verifies token claims before performing sensitive actions like account deletion.
-   **ISO-8601 UTC**: All dates are handled in UTC on the server and converted locally to prevent time discrepancies.
