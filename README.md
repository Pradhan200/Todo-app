# TODO List App

A minimal, professional full-stack TODO app showcasing modern Angular + .NET Web API with in-memory persistence, clean architecture, and basic tests.

## ✨ Features

- **View** all TODO items (ordered by creation date, newest first)
- **Add** new TODO items with validation
- **Delete** TODO items
- **Responsive UI** with loading and error states
- **In-memory storage** (data resets on server restart)
- **CORS enabled** for development
- **Swagger/OpenAPI** documentation in development mode

## 🏗️ Architecture

```
todo-app/
├─ README.md
├─ client/                    # Angular 18 (standalone components)
│ ├─ src/app/
│ │ ├─ app.component.*        # Main UI component
│ │ ├─ todo.service.ts        # HTTP service for API calls
│ │ ├─ models.ts              # TypeScript interfaces
│ │ └─ todo.service.spec.ts   # Unit tests
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
└─ tests/                     # xUnit integration tests
├─ TodoApiTests.cs
└─ Server.Tests.csproj
```

## 🔌 API Contract

**Base URL**: `http://localhost:5000`

### GET `/api/todo`
- **200 OK** → `TodoItem[]` (ordered by `createdAt` desc)

### POST `/api/todo`
- **Request**: `{ "title": string }` (non-empty)
- **201 Created** → `TodoItem`
- **400 Bad Request** if invalid

### DELETE `/api/todo/{id}`
- **204 No Content** on success
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

### Backend (.NET 9)
- **.NET 9** minimal Web API
- **ConcurrentDictionary** for thread-safe in-memory storage
- **Repository pattern** for data abstraction
- **Swagger/OpenAPI** for API documentation
- **xUnit** with FluentAssertions for integration testing

## 🔒 Error Handling

- **Validation**: Rejects blank/whitespace titles (400 Bad Request)
- **Idempotent deletes**: Unknown ID returns 404 Not Found
- **Thread-safe**: Concurrent access handled with ConcurrentDictionary
- **Graceful degradation**: Loading and error states in UI

## 📝 Development Notes

- Data is stored **in memory only** and resets on server restart
- CORS is configured for `http://localhost:4200` (Angular dev server)
- All API endpoints are documented via Swagger in development mode
- Tests cover happy paths and edge cases for both frontend and backend

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
- **Comprehensive testing** at both unit and integration levels

## 🎯 Acceptance Criteria Met

✅ Can view TODO items  
✅ Can add a TODO item  
✅ Can delete a TODO item  
✅ Uses Angular 18 and .NET 9  
✅ Data persists in memory only  
✅ Unit/integration tests included  
✅ README includes exact run instructions  
✅ Project builds/runs with standard commands  




