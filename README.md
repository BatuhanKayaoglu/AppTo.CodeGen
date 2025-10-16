# AppTo.CodeGen

A powerful .NET global tool for generating CQRS (Command Query Responsibility Segregation) pattern code following Clean Architecture principles.

## 🚀 Features

- **Command & Query Generation**: Generate Command, CommandHandler, Query, and QueryHandler classes
- **Validator Generation**: Automatically create Command and Query validators
- **Request & Response Models**: Automatically create Request and Response classes
- **API Endpoints**: Generate REST API endpoints with proper HTTP methods
- **Smart Project Detection**: Automatically detect project structure and naming
- **Flexible Configuration**: Support for custom project names and endpoint controllers
- **Clean Architecture**: Follows Clean Architecture folder structure conventions
- **Modern Architecture**: Built with Clean Architecture, Dependency Injection, and SOLID principles

## 🏗️ Architecture

This tool is built using modern software architecture principles:

- **Clean Architecture**: Separation of concerns with Core, Application, Infrastructure, and Presentation layers
- **Dependency Injection**: Built-in DI container for better testability and maintainability
- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion
- **CQRS Pattern**: Command Query Responsibility Segregation for better separation of concerns
- **Template Engine**: Pluggable template system for code generation

## 📦 Installation

```bash
dotnet tool install --global AppTo.CodeGen
```

## 🎯 Usage

### Basic Command Generation

```bash
# Generate a command with automatic project detection
codegen add feature QrSale --type command --ep Sale

# Generate a query
codegen add feature GetUser --type query --ep User

# Generate without validator (validator is enabled by default)
codegen add feature Login --type command --validator:false
```

### Advanced Usage

```bash
# Specify custom project name
codegen add feature AddCard --type command --ep Card --projectName Metropol.LUKE

# Generate query with custom project name
codegen add feature GetProducts --type query --ep Product --projectName MyCompany.API
```

## 📁 Generated Structure

When you run `codegen add feature QrSale --type command --ep Sale`, it creates:

```
Application/
└── QrSale/
    └── Commands/
        ├── QrSaleCommand.cs
        ├── QrSaleCommandHandler.cs
        └── QrSaleCommandValidator.cs

Abstraction/
└── QrSale/
    ├── Request/
    │   └── QrSaleRequest.cs
    └── Response/
        └── QrSaleResponse.cs

Controllers/
└── SaleController.cs (endpoint added)
```

## 🔧 Command Options

| Option          | Description                          | Example                                                      |
| --------------- | ------------------------------------ | ------------------------------------------------------------ |
| `featureName`   | Name of the feature to generate      | `QrSale`                                                     |
| `--type`        | Type: `command` or `query`           | `--type command`                                             |
| `--ep`          | Endpoint controller name             | `--ep Sale`                                                  |
| `--projectName` | Custom project name (optional)       | `--projectName Metropol.LUKE`                                |
| `--prop-req`    | Request property                  | `--prop-req "Name:string,Email:string,Age:int,OrderId:int"`  |
| `--prop-resp`   | Response property                 | `--prop-resp "Name:string,Email:string,Age:int,OrderId:int"` |
| `--validator`   | Validator create (varsayılan: true) | `--validator:false`                                          |

## 📝 Properties Examples

```bash
# Sadece Request properties
codegen add feature UserRegistration --type command --prop-req "Name:string,Email:string,Age:int,OrderId:int"

# Sadece Response properties
codegen add feature GetUser --type query --prop-resp "Name:string,Email:string,Age:int,OrderId:int"

# Hem Request hem Response properties
codegen add feature CreateProduct --type command --prop-req "Name:string,Price:decimal" --prop-resp "Id:int,Name:string,Price:decimal,CreatedDate:DateTime"

# Hiçbir property belirtmezseniz (boş sınıflar)
codegen add feature SimpleCommand --type command
```

## 📝 Generated Code Examples

### Request Class

```csharp
namespace MyProject.Abstraction.UserRegistration.Request;

public class UserRegistrationRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public int OrderId { get; set; }
}
```

### Response Class

```csharp
namespace MyProject.Abstraction.UserRegistration.Response;

public class UserRegistrationResponse
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public int OrderId { get; set; }
}
```

### Command Class

```csharp
using MyProject.Infrastructure.CQRS.Concrete;
using MyProject.Abstraction.QrSale.Response;

namespace MyProject.Application.QrSale.Commands;

public class QrSaleCommand : MetropolCommand<QrSaleResponse>
{
    // TODO: Add Business Logic here.
}
```

### Command Handler

```csharp
using MyProject.Infrastructure.CQRS.Concrete;
using MyProject.Abstraction.QrSale.Response;

namespace MyProject.Application.QrSale.Commands;

public class QrSaleCommandHandler : MetropolCommandHandler<QrSaleCommand, QrSaleResponse>
{
    public override async Task<QrSaleResponse?> Handle(QrSaleCommand request, CancellationToken cancellationToken)
    {
        // TODO: Add Business Logic here.
        return new QrSaleResponse();
    }
}
```

### Command Validator

```csharp
using MyProject.Infrastructure.Validation;

namespace MyProject.Application.QrSale.Commands;

public class QrSaleCommandValidator : MetropolValidator<QrSaleCommand>
{
    public QrSaleCommandValidator()
    {

    }
}
```

### API Endpoint

```csharp
[HttpPost]
[Route("qr-sale")]
[ProducesResponseType(typeof(MetropolApiResponse<QrSaleResponse>), (int)System.Net.HttpStatusCode.OK)]
public async Task<MetropolApiResponse<QrSaleResponse>> QrSale(
    [FromBody] QrSaleRequest request,
    CancellationToken cancellationToken)
{
    var response = await _cqrsProcessor.ProcessAsync(new QrSaleCommand(), cancellationToken);
    return SetResponse(response);
}
```

## 🏗️ Project Structure Requirements

The tool expects the following folder structure:

```
YourProject/
├── src/
│   ├── YourProject.Application/
│   ├── YourProject.Abstraction/
│   └── YourProject.Controllers/
└── YourProject.sln
```

## 🔍 Project Name Detection

The tool automatically detects your project name using this priority:

1. **src/ folder**: Uses the first directory name in the `src/` folder
2. **Manual override**: Use `--projectName` parameter to specify custom name

## 🛠️ Requirements

- .NET 9.0 or later
- Clean Architecture project structure
- CQRS pattern implementation

**Made with ❤️ by Batuhan Kayaoğlu**
