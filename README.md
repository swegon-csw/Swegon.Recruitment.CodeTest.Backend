# Swegon Recruitment Backend

ASP.NET Core Web API for Swegon recruitment code test.

## Prerequisites

-   .NET 9.0 SDK ([download](https://dotnet.microsoft.com/download/dotnet/9.0))

## Installation

Download and install the .NET 9.0 SDK from the link above, then verify:

```bash
dotnet --version
```

Clone the repository and restore dependencies:

```bash
git clone <repository-url>
dotnet restore
```

## Running the Application

### Option 1: Command Line

Build and run from the command line:

```bash
dotnet build
cd Swegon.Recruitment.CodeTest.Backend.Api
dotnet run
```

The API will be available at `https://localhost:5078`

### Option 2: IDE (Recommended)

Open the solution in your preferred IDE and start debugging:

-   **Visual Studio**: Open `Swegon.Recruitment.CodeTest.Backend.sln` and press F5
-   **VS Code**: Open the workspace and use Run and Debug (F5)
-   **Rider**: Open the solution and click the Run button

The API will be available at `https://localhost:5078`

## Running Tests

```bash
dotnet test
```

## Tech Stack

-   ASP.NET Core 9.0
-   .NET 9.0
-   Swagger/OpenAPI
-   xUnit
