# TODO List App

A minimal, professional full-stack TODO app showcasing modern Angular + .NET Web API with in-memory persistence, clean architecture, and comprehensive testing.

## ✨ Features

- **View** all TODO items (ordered by creation date, newest first)
- **Add** new TODO items with validation
- **Delete** TODO items
- **Toggle completion** status for tasks
- **Show/Hide completed** tasks
- **Responsive UI** with Notion-like design
- **Loading and error states**
- **In-memory storage** (data resets on server restart)
- **CORS enabled** for development
- **Swagger/OpenAPI** documentation in development mode
- **Comprehensive error handling** with global exception middleware
- **Structured logging** with Serilog
- **Input validation** with FluentValidation

## 🏗️ Architecture

```
todo-app/
├─ README.md
├─ .gitignore
├─ client/                    # Angular 18 (standalone components)
│ ├─ src/app/
│ │ ├─ app.component.*        # Main UI component
│ │ ├─ todo.service.ts        # HTTP service for API calls
│ │ ├─ models.ts              # TypeScript interfaces
│ │ ├─ todo.service.spec.ts   # Unit tests
│ │ └─ app.component.spec.ts  # Component tests
│ ├─ package.json
│ └─ angular.json
└─ server/                    # .NET 9 minimal API
├─ Program.cs                 # API endpoints, DI, CORS, Swagger
├─ Models/
│ ├─ TodoItem.cs              # Todo record type
│ └─ CreateTodoRequest.cs     # Request DTO
├─ Services/
│ ├─ ITodoRepository.cs       # Repository abstraction
│ └─ InMemoryTodoRepository.cs # Thread-safe in-memory store
├─ Middleware/
│ └─ GlobalExceptionMiddleware.cs # Global error handling
├─ Validators/
│ └─ CreateTodoRequestValidator.cs # FluentValidation rules
└─ tests/                     # xUnit integration tests
├─ TodoApiTests.cs
├─ TodoRepositoryTests.cs
└─ TodoApiIntegrationTests.cs
```

## 🔌 API Contract

**Base URL**: `http://localhost:5000`

### GET `/api/todo`
- **200 OK** → `TodoItem[]` (ordered by `createdAt` desc)

### POST `/api/todo`
- **Request**: `{ "title": string }` (non-empty, max 500 chars)
- **201 Created** → `TodoItem`
- **400 Bad Request** if invalid

### DELETE `/api/todo/{id}`
- **204 No Content** on success
- **404 Not Found** if item doesn't exist

### PATCH `/api/todo/{id}/toggle`
- **200 OK** → Updated `TodoItem` with toggled completion status
- **404 Not Found** if item doesn't exist

## 🚀 Quick Start

### Prerequisites
- **Node.js 20+** and npm
- **.NET 9 SDK**

### 1. Run the API Server

```bash
cd server
dotnet restore
dotnet run
```

The API will be available at: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

### 2. Run the Angular App

```bash
cd client
npm install
npm start
```

The Angular app will be available at: `http://localhost:4200`

## 🧪 Testing

### Backend Tests
```bash
cd server
dotnet test
```

### Frontend Tests
```bash
cd client
npm test
```

## 📦 Technologies Used

### Frontend (Angular 18)
- **Angular 18** with standalone components
- **TypeScript** with strict mode
- **Reactive Forms** for input handling
- **HttpClient** for API communication
- **Jasmine/Karma** for unit testing
- **CSS Grid** for responsive layout
- **Notion-like UI** design

### Backend (.NET 9)
- **.NET 9** minimal Web API
- **ConcurrentDictionary** for thread-safe in-memory storage
- **Repository pattern** for data abstraction
- **Swagger/OpenAPI** for API documentation
- **xUnit** with FluentAssertions for integration testing
- **Serilog** for structured logging
- **FluentValidation** for input validation
- **Global exception handling** middleware

## 🔒 Error Handling

- **Global exception middleware** for consistent error responses
- **Structured logging** with Serilog for better debugging
- **Input validation** with FluentValidation rules
- **Validation**: Rejects blank/whitespace titles (400 Bad Request)
- **Idempotent operations**: Unknown ID returns 404 Not Found
- **Thread-safe**: Concurrent access handled with ConcurrentDictionary
- **Graceful degradation**: Loading and error states in UI

## 📝 Development Notes

- Data is stored **in memory only** and resets on server restart
- CORS is configured for `http://localhost:4200` (Angular dev server)
- All API endpoints are documented via Swagger in development mode
- Tests cover happy paths, edge cases, and error scenarios
- Comprehensive test coverage with unit, integration, and component tests

## 🛠️ Build Commands

### Backend
```bash
cd server
dotnet build          # Build the project
dotnet run            # Run the API server
dotnet test           # Run tests
```

### Frontend
```bash
cd client
npm install           # Install dependencies
npm start            # Start development server
npm run build        # Build for production
npm test             # Run tests
```

## 📊 Project Structure

The application follows clean architecture principles:
- **Separation of concerns** between frontend and backend
- **Repository pattern** for data access abstraction
- **DTOs** for request/response models
- **Dependency injection** for loose coupling
- **Global exception handling** for consistent error management
- **Comprehensive testing** at unit, integration, and component levels

## 🎯 Features Implemented

✅ **Core Functionality**
- View TODO items (ordered by creation date, newest first)
- Add new TODO items with validation
- Delete TODO items
- Toggle completion status
- Show/Hide completed tasks

✅ **Technical Requirements**
- Angular 18 with standalone components
- .NET 9 minimal Web API
- In-memory persistence
- CORS enabled for development
- Swagger/OpenAPI documentation

✅ **Quality & Testing**
- Comprehensive unit tests
- Integration tests
- Component tests
- Error handling and validation
- Structured logging

✅ **UI/UX**
- Notion-like modern interface
- Responsive design with CSS Grid
- Loading and error states
- Clean, professional appearance

## 🔗 Repository

This project is available on GitHub: [https://github.com/Pradhan200/Todo-app](https://github.com/Pradhan200/Todo-app)