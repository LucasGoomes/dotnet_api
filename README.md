# DotnetAPI Project README

This repository contains a C# ASP.NET Core API project that provides CRUD operations for managing user data, along with authentication functionality. Below is a detailed explanation of the project's structure and code.

## Project Structure

The project is organized into several files and folders:

1. `Program.cs`: This file contains the main entry point of the application.
2. `Startup.cs`: Here, the application configuration and service registrations are defined.
3. `Models` folder: Contains data models representing the `User`, `UserJobInfo`, and `UserSalary`.
4. `Data` folder: Contains the `DataContextDapper` class responsible for handling data access using Dapper.
5. `Controllers` folder: Contains two controllers, `UserController` and `AuthController`, responsible for handling API endpoints.
6. `Dtos` folder: Contains Data Transfer Objects (DTOs) used for data exchange between the client and server.
7. `appsettings.json`: Configuration file containing app settings, including the connection string, password key, and token key.

## Setting up the Project

To set up the project, follow these steps:

1. Ensure you have .NET SDK installed on your machine.
2. Clone the repository to your local machine.
3. Open the solution in Visual Studio or your preferred IDE.
4. Restore NuGet packages and build the solution.

## Running the Project

To run the API, follow these steps:

1. Configure the connection string and other settings in the `appsettings.json` file.
2. Set up a local SQL Server database and execute the necessary scripts to create the required tables (not included here).
3. Run the application.

## Endpoints

The API provides the following endpoints:

### User Controller

- `GET /User/Test`: Test endpoint to check database connection by returning the current date.
- `GET /User/GetUsers`: Retrieve a list of all users.
- `GET /User/GetUsers/{userId}`: Retrieve a single user by their ID.
- `PUT /User/EditUser`: Update an existing user's details.
- `POST /User/AddUser`: Add a new user.
- `DELETE /User/DeleteUser/{userId}`: Delete a user by their ID.
- `GET /User/UserSalary/{userId}`: Retrieve a user's salary details.
- `POST /User/UserSalary`: Add a new user salary entry.
- `PUT /User/UserSalary`: Update a user's salary details.
- `DELETE /User/UserSalary/{userId}`: Delete a user's salary entry by their ID.
- `GET /User/UserJobInfo/{userId}`: Retrieve a user's job information.
- `POST /User/UserJobInfo`: Add a new user job information entry.
- `PUT /User/UserJobInfo`: Update a user's job information.
- `DELETE /User/UserJobInfo/{userId}`: Delete a user's job information entry by their ID.

### Auth Controller

- `POST /Auth/Register`: Register a new user.
- `POST /Auth/Login`: Log in an existing user and receive a JWT token for authentication.

## Authentication

The API uses JWT-based authentication. When a user logs in, the server generates a token containing the user's ID. This token should be included in the `Authorization` header of subsequent requests to secure endpoints.

## CORS Configuration

The API allows cross-origin resource sharing (CORS) for different environments (development and production). For development, it permits requests from `http://localhost:4200`, `http://localhost:3000`, and `http://localhost:8000`. In production, it allows requests from `https://myProductionSite.com`.

## Database Access

The project uses Dapper to interact with the SQL Server database. The `DataContextDapper` class encapsulates data access methods like `LoadData`, `LoadDataSingle`, `ExecuteSql`, and `ExecuteSqlWithParameters`.

Please make sure to set up the database and connection string correctly for the application to work as expected.

## Dependencies

The project uses ASP.NET Core, Dapper, and Microsoft SQL Server libraries. Ensure that the required NuGet packages are properly restored.


