# TeamTaskManagement
A secure RESTful API for team-based task management built with Clean Architecture principles using C#/.NET 8, Entity Framework Core, and SQL Server.

## Architecture

This project follows Clean Architecture with clear separation of concerns:

- **Domain Layer**: Core business entities, enums, and domain logic
- **Application Layer**: Use cases, business rules, DTOs, and service interfaces
- **Infrastructure Layer**: Data access, external services, and cross-cutting concerns
- **API Layer**: Controllers, middleware, and API configuration

## Tech Stack

- **.NET 8**: Latest LTS version
- **Entity Framework Core 8**: ORM for data access
- **SQL Server**: Database
- **JWT Authentication**: Secure token-based authentication
- **BCrypt**: Password hashing
- **Serilog**: Structured logging
- **Swagger**: API documentation
- **xUnit**: Unit testing framework

## Features

- **User Management**: Registration, login, profile access
- **Team Management**: Create teams, invite users, role-based access
- **Task Management**: CRUD operations with team-scoped access
- **Security**: JWT authentication, password hashing, authorization
- **Logging**: Comprehensive logging with Serilog
- **Exception Handling**: Global exception middleware
- **API Documentation**: Interactive Swagger UI

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Setup Steps

1. **Clone the repository**
```bash
git clone <repository-url>
cd TeamTaskManagement
