using AppTo.CodeGen.Application.DTOs;
using AppTo.CodeGen.Core.Enums;
using AppTo.CodeGen.Infrastructure.Configuration;
using AppTo.CodeGen.Presentation.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

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
    // Setup dependency injection
    var services = new ServiceCollection();
    services.AddAppToCodeGen();
    var serviceProvider = services.BuildServiceProvider();

    // Create command
    var command = new GenerateFeatureCommand(
        featureName,
        type,
        endpoint,
        projectName,
        module,
        propReq,
        propResp,
        validator);

    // Execute command
    var handler = serviceProvider.GetRequiredService<GenerateFeatureCommandHandler>();
    var exitCode = await handler.HandleAsync(command);

    Environment.Exit(exitCode);
}, featureNameArgument, typeOption, moduleOption, endpointOption, projectNameOption, propReqOption, propRespOption, validatorOption);

addCommand.AddCommand(featureCommand);
rootCommand.AddCommand(addCommand);

return await rootCommand.InvokeAsync(args);
