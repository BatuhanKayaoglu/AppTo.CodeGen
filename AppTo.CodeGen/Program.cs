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
var propReqOption = new Option<string>("--prop-req", "Request özellikleri (örn: 'Name:string,Email:string,Age:int,OrderId:int')");
var propRespOption = new Option<string>("--prop-resp", "Response özellikleri (örn: 'Name:string,Email:string,Age:int,OrderId:int')");
var validatorOption = new Option<bool>("--validator", () => true, "Validator oluştur (varsayılan: true)");

featureCommand.AddArgument(featureNameArgument);
featureCommand.AddOption(typeOption);
featureCommand.AddOption(moduleOption);
featureCommand.AddOption(endpointOption);
featureCommand.AddOption(projectNameOption);
featureCommand.AddOption(propReqOption);
featureCommand.AddOption(propRespOption);
featureCommand.AddOption(validatorOption);

featureCommand.SetHandler(async (string featureName, FeatureType type, string module, string endpoint, string projectName, string propReq, string propResp, bool validator) =>
{
    try
    {
        var locator = new ApplicationLayerLocator();
        var fileSystem = new FileSystemService();
        var generator = new FeatureCommandGenerator(locator, fileSystem);

        await generator.GenerateAsync(featureName, type, endpoint, projectName, propReq, propResp, validator);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"\n✅ {featureName} {type} başarıyla oluşturuldu!");
        var commandType = type == FeatureType.Command ? "Command" : "Query";
        Console.WriteLine($"📁 Handler: {featureName}{commandType}Handler.cs");
        Console.WriteLine($"📁 {commandType}: {featureName}{commandType}.cs");
        if (validator)
        {
            var validatorType = type == FeatureType.Command ? "CommandValidator" : "QueryValidator";
            Console.WriteLine($"📁 Validator: {featureName}{validatorType}.cs");
        }
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
}, featureNameArgument, typeOption, moduleOption, endpointOption, projectNameOption, propReqOption, propRespOption, validatorOption);

addCommand.AddCommand(featureCommand);
rootCommand.AddCommand(addCommand);

return await rootCommand.InvokeAsync(args);