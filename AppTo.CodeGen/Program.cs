using System;
using System.CommandLine;
using AppTo.CodeGen.Commands;
using AppTo.CodeGen.Services;

var rootCommand = new RootCommand("AppTo Code Generator - CQRS pattern için kod üretici");

var addCommand = new Command("add", "Yeni feature ekle");
var featureCommand = new Command("feature", "Feature oluştur");

var featureNameArgument = new Argument<string>("featureName", "Feature adı (örn: QrSale)");
var typeOption = new Option<string>("--type", () => "command", "Tip: command veya query");
var moduleOption = new Option<string>("--module", "Modül adı");
var endpointOption = new Option<string>("--ep", "Endpoint controller adı (örn: Sale)");

featureCommand.AddArgument(featureNameArgument);
featureCommand.AddOption(typeOption);
featureCommand.AddOption(moduleOption);
featureCommand.AddOption(endpointOption);

featureCommand.SetHandler(async (string featureName, string type, string module, string endpoint) =>
{
    try
    {
        var locator = new ApplicationLayerLocator();
        var fileSystem = new FileSystemService();
        var generator = new FeatureCommandGenerator(locator, fileSystem);

        await generator.GenerateAsync(featureName, type, endpoint);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✅ {featureName} {type} başarıyla oluşturuldu!");
        Console.WriteLine($"📁 Command: {featureName}Command.cs");
        Console.WriteLine($"📁 Handler: {featureName}CommandHandler.cs");
        Console.WriteLine($"📁 Request: {featureName}Request.cs");
        Console.WriteLine($"📁 Response: {featureName}Response.cs");
        if (!string.IsNullOrEmpty(endpoint))
        {
            Console.WriteLine($"📁 Endpoint: {endpoint}Controller.cs içine eklendi");
        }
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ Hata: {ex.Message}");
        Console.ResetColor();
    }
}, featureNameArgument, typeOption, moduleOption, endpointOption);

addCommand.AddCommand(featureCommand);
rootCommand.AddCommand(addCommand);

return await rootCommand.InvokeAsync(args);