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

API Usage Examples
Authentication


//Register

POST /api/auth/register
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePassword123!"
}

//login
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}

//Create Team
POST /api/teams
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "name": "Development Team",
  "description": "Main development team"
}


//Add User to Team

POST /api/teams/{teamId}/users
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "userId": "user-guid-here"
}


//Create Task
POST /api/teams/{teamId}/tasks
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "title": "Implement user authentication",
  "description": "Add JWT-based authentication to the API",
  "dueDate": "2024-01-15T10:00:00Z",
  "assignedToUserId": "user-guid-here"
}


//Update Task Status
PATCH /api/tasks/{taskId}/status
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "status": 1
}

// Database Schema
The database uses the following main entities:

Users: User accounts with authentication
Teams: Team containers for tasks and users
TeamUsers: Many-to-many relationship with roles
Tasks: Task items with assignments and status

//Assumptions

Users must be authenticated to access any API endpoints except registration/login
Only team admins can add users to teams
All team members can view and manage team tasks
Tasks can only be assigned to team members


