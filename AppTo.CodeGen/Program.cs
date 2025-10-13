using System;
using System.CommandLine;
using AppTo.CodeGen.Commands;
using AppTo.CodeGen.Services;
using AppTo.CodeGen.Models;

var rootCommand = new RootCommand("AppTo Code Generator - CQRS pattern için kod üretici");

var addCommand = new Command("add", "Yeni feature ekle");
var featureCommand = new Command("feature", "Feature oluştur");

var featureNameArgument = new Argument<string>("featureName", "Feature adı (örn: QrSale)");
var typeOption = new Option<FeatureType>("--type", () => FeatureType.Command, "Tip: command veya query");
var moduleOption = new Option<string>("--module", "Modül adı");
var endpointOption = new Option<string>("--ep", "Endpoint controller adı (örn: Sale)");
var projectNameOption = new Option<string>("--projectName", "Proje adı (örn: Metropol.LUKE)");

featureCommand.AddArgument(featureNameArgument);
featureCommand.AddOption(typeOption);
featureCommand.AddOption(moduleOption);
featureCommand.AddOption(endpointOption);
featureCommand.AddOption(projectNameOption);

featureCommand.SetHandler(async (string featureName, FeatureType type, string module, string endpoint, string projectName) =>
{
    try
    {
        var locator = new ApplicationLayerLocator();
        var fileSystem = new FileSystemService();
        var generator = new FeatureCommandGenerator(locator, fileSystem);

        await generator.GenerateAsync(featureName, type, endpoint, projectName);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"\n✅ {featureName} {type} başarıyla oluşturuldu!");
        var commandType = type == FeatureType.Command ? "Command" : "Query";
        Console.WriteLine($"📁 Handler: {featureName}{commandType}Handler.cs");
        Console.WriteLine($"📁 {commandType}: {featureName}{commandType}.cs");
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
}, featureNameArgument, typeOption, moduleOption, endpointOption, projectNameOption);

addCommand.AddCommand(featureCommand);
rootCommand.AddCommand(addCommand);

return await rootCommand.InvokeAsync(args);