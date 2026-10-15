Gym Management System

1. Overview

A web-based application designed to manage gym operations, including member management, trainer scheduling, session booking, and membership plans.

The system is built using ASP.NET Core MVC, Entity Framework Core, and Microsoft SQL Server, following a Three-Layer Architecture to ensure separation of concerns and maintainability.

Project Goals

Centralize member and membership plan management.

Manage trainers and training session schedules.

Organize session bookings and member assignments.

Provide a centralized platform for gym operations.

2. Features

Trainer Management: Full CRUD operations for trainers.

Member Management: Add, update, delete, and view members.

Plan Management: View, update, and deactivate membership plans using Soft Delete.

Membership Management: Assign membership plans to members.

Session Management: Full CRUD operations for training sessions.

Session Scheduling: Schedule training sessions and assign trainers.

Session Booking: Manage member bookings for available training sessions.

Dashboard: Display analytics and reports for gym operations.

Authentication & Authorization: User authentication and access control using ASP.NET Core Identity.

3. Architecture

The project follows a Three-Layer Architecture.

Presentation Layer

ASP.NET Core MVC Controllers.

Razor Views.

Bootstrap and custom CSS.

Handles user interaction and presentation.

Business Logic Layer

Service classes responsible for business rules and application logic.

Handles operations related to trainers, members, plans, sessions, and bookings.

Uses the Result Pattern to represent operation outcomes and handle success and failure cases consistently.

Data Access Layer

Entity Framework Core for database operations.

Microsoft SQL Server as the database.

Repository Pattern for data access abstraction.

Unit of Work Pattern for coordinating repository operations.

4. Technology Stack

Programming Language: C#

Backend Framework: ASP.NET Core MVC

ORM: Entity Framework Core

Database: Microsoft SQL Server

Frontend: Razor Views, HTML, CSS, Bootstrap, JavaScript

Authentication & Authorization: ASP.NET Core Identity

Object Mapping: AutoMapper

Design Patterns: Repository Pattern, Unit of Work, Result Pattern

Dependency Management: Dependency Injection

7. Author

Mohamed Hassan
