# AppTo.CodeGen

A powerful .NET global tool for generating CQRS (Command Query Responsibility Segregation) pattern code following Clean Architecture principles.

## 🚀 Features

- **Command & Query Generation**: Generate Command, CommandHandler, Query, and QueryHandler classes
- **Request & Response Models**: Automatically create Request and Response classes
- **API Endpoints**: Generate REST API endpoints with proper HTTP methods
- **Smart Project Detection**: Automatically detect project structure and naming
- **Flexible Configuration**: Support for custom project names and endpoint controllers
- **Clean Architecture**: Follows Clean Architecture folder structure conventions

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
        └── QrSaleCommandHandler.cs

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

| Option          | Description                     | Example                       |
| --------------- | ------------------------------- | ----------------------------- |
| `featureName`   | Name of the feature to generate | `QrSale`                      |
| `--type`        | Type: `command` or `query`      | `--type command`              |
| `--ep`          | Endpoint controller name        | `--ep Sale`                   |
| `--projectName` | Custom project name (optional)  | `--projectName Metropol.LUKE` |

## 📝 Generated Code Examples

### Command Class

```csharp
using MyProject.Infrastructure.CQRS.Concrete;
using MyProject.Abstraction.QrSale.Response;

namespace MyProject.Application.QrSale.Commands;

public class QrSaleCommand : MetropolCommand<QrSaleResponse>
{
    // TODO: Command özelliklerini buraya ekleyin
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
        // TODO: İş mantığını burada uygula...
        return new QrSaleResponse();
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

## 🎨 Supported Patterns

- **CQRS Pattern**: Commands and Queries with separate handlers
- **Clean Architecture**: Application, Abstraction, and Controllers layers
- **REST API**: HTTP POST for commands, HTTP GET for queries
- **MediatR Integration**: Compatible with MediatR pattern

## 🛠️ Requirements

- .NET 9.0 or later
- Clean Architecture project structure
- CQRS pattern implementation

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📞 Support

If you have any questions or need help, please open an issue on GitHub.

---

**Made with ❤️ by Batuhan Kayaoğlu**
