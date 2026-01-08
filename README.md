# Swegon Recruitment CodeTest Backend

A comprehensive .NET 9.0 backend solution demonstrating clean architecture principles, RESTful API design, and production-quality code.

## 📁 Solution Structure

```
Swegon.Recruitment.CodeTest.Backend/
├── Swegon.Recruitment.CodeTest.Backend.Api/                    # Main API project
├── Swegon.Recruitment.CodeTest.Backend.Api.Client/             # Client SDK
├── Swegon.Recruitment.CodeTest.Backend.Api.Contracts/          # Shared contracts/DTOs
└── Swegon.Recruitment.CodeTest.Backend.Api.Tests/              # Unit/Integration tests
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022, VS Code, or Rider
- Git

### Build and Run

```bash
# Clone the repository
git clone <repository-url>
cd backend

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run the API
cd Swegon.Recruitment.CodeTest.Backend.Api
dotnet run

# Run tests
cd ../Swegon.Recruitment.CodeTest.Backend.Api.Tests
dotnet test
```

The API will be available at `https://localhost:5001` (or the port shown in the console).

## 📚 API Documentation

When running in Development mode, Swagger UI is available at the root URL (`https://localhost:5001`).

### Available Endpoints

#### Products
- `GET /api/products` - Get all products with pagination and filtering
- `GET /api/products/{id}` - Get a specific product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update an existing product
- `DELETE /api/products/{id}` - Delete a product
- `POST /api/products/search` - Search products

#### Calculations
- `POST /api/calculations` - Perform a calculation
- `GET /api/calculations/history/{productId}` - Get calculation history
- `GET /api/calculations/{id}` - Get a specific calculation
- `POST /api/calculations/batch` - Perform batch calculations

#### Configuration
- `GET /api/configuration` - Get all configurations
- `GET /api/configuration/{key}` - Get configuration by key
- `POST /api/configuration` - Create or update configuration
- `DELETE /api/configuration/{key}` - Delete configuration

#### Health
- `GET /api/health` - Health check endpoint

## 🏗️ Architecture

### Projects

#### 1. **Swegon.Recruitment.CodeTest.Backend.Api**
Main ASP.NET Core Web API project containing:
- **Controllers** - API endpoints
- **Services** - Business logic (ProductService, CalculationService, CacheService, etc.)
- **Calculators** - Calculation engines (PrimaryCalculator, DiscountCalculator, TaxCalculator)
- **Models** - Domain models
- **Middleware** - Custom middleware for error handling, logging, caching
- **Filters** - Action filters for validation and caching
- **Helpers** - Utility classes
- **Validators** - Validation logic
- **Config** - Configuration classes
- **Extensions** - Extension methods

#### 2. **Swegon.Recruitment.CodeTest.Backend.Api.Contracts**
Shared contracts library containing:
- **Requests** - Request DTOs
- **Responses** - Response DTOs
- **Enums** - Shared enumerations
- **Interfaces** - Service interfaces
- **Common** - Common models (ApiResult, PaginationMetadata, FilterCriteria)

#### 3. **Swegon.Recruitment.CodeTest.Backend.Api.Client**
Client SDK library containing:
- **Clients** - API client implementations (ProductClient, CalculationClient, etc.)
- **Configuration** - Client configuration
- **Extensions** - DI registration extensions
- **Exceptions** - Client-specific exceptions
- **Handlers** - HTTP message handlers for authentication and logging

#### 4. **Swegon.Recruitment.CodeTest.Backend.Api.Tests**
Test project containing:
- **Unit Tests** - Unit tests for services, calculators, and helpers
- **Integration Tests** - Integration tests for controllers
- **Fixtures** - Test fixtures for setup
- **TestData** - Test data builders
- **Utilities** - Test utilities

## 🔧 Configuration

Configuration is managed through `appsettings.json` and `appsettings.Development.json`.

Key settings:
- **AppSettings:Cache** - Cache configuration
- **AppSettings:Features** - Feature flags
- **Database** - Database configuration (currently in-memory)
- **Validation** - Validation settings

## 🧪 Testing

The solution includes comprehensive unit and integration tests:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Swegon.Recruitment.CodeTest.Backend.Api.Tests
```

### Test Coverage
- **Unit Tests**: Services, Calculators, Helpers, Validators
- **Integration Tests**: Controllers, End-to-End scenarios
- **Test Infrastructure**: Fixtures, Builders, Utilities

## 📦 NuGet Packages

### API Project
- Swashbuckle.AspNetCore - API documentation
- AutoMapper.Extensions.Microsoft.DependencyInjection - Object mapping
- Microsoft.Extensions.Caching.Memory - In-memory caching

### Client Project
- Microsoft.Extensions.Http - HttpClient factory
- Microsoft.Extensions.DependencyInjection - DI support

### Tests Project
- xUnit - Testing framework
- Moq - Mocking framework
- FluentAssertions - Fluent assertions
- Microsoft.AspNetCore.Mvc.Testing - Integration testing

## 🎯 Features

### Core Features
- ✅ RESTful API design
- ✅ Clean architecture
- ✅ Dependency injection
- ✅ Async/await throughout
- ✅ In-memory caching
- ✅ Comprehensive error handling
- ✅ Request/response logging
- ✅ Model validation
- ✅ Swagger/OpenAPI documentation

### Advanced Features
- ✅ Complex calculation engines
- ✅ Batch operations
- ✅ Search and filtering
- ✅ Pagination
- ✅ Export functionality (CSV/JSON)
- ✅ Health checks
- ✅ CORS support
- ✅ Client SDK

## 🔐 Authentication

Authentication middleware is included but currently allows all requests for demo purposes. To enable API key authentication, set the `X-API-Key` header in requests.

## 📝 Code Style

The solution follows C# coding conventions and includes:
- **EditorConfig** - Code style configuration
- **XML Documentation** - Comprehensive XML comments
- **Nullable reference types** - Enabled
- **Implicit usings** - Enabled

## 🚢 Deployment

The API is configured for easy deployment:

```bash
# Publish for production
dotnet publish -c Release -o ./publish

# Run published version
cd publish
dotnet Swegon.Recruitment.CodeTest.Backend.Api.dll
```

## 📊 Project Statistics

- **Total Files**: ~100+ files
- **Total Lines of Code**: ~15,000+ lines
- **Projects**: 4
- **Test Coverage**: Comprehensive unit and integration tests
- **API Endpoints**: 15+ endpoints
- **Services**: 8 services
- **Calculators**: 7 calculators
- **Controllers**: 4 controllers

## 🤝 Contributing

This is a recruitment code test project. For questions or issues, please contact the recruitment team.

## 📄 License

Copyright © Swegon 2026. All rights reserved.

## 🙏 Acknowledgments

Built with:
- ASP.NET Core 9.0
- xUnit
- Swagger/OpenAPI
- AutoMapper
- And many other great open-source libraries

---

**Note**: This project demonstrates production-quality code architecture and best practices for a recruitment code test.
