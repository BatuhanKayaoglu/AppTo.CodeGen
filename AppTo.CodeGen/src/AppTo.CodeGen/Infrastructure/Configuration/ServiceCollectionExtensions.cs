using AppTo.CodeGen.Application.Services;
using AppTo.CodeGen.Core.Interfaces;
using AppTo.CodeGen.Infrastructure.FileSystem;
using AppTo.CodeGen.Infrastructure.Templates;
using AppTo.CodeGen.Presentation.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AppTo.CodeGen.Infrastructure.Configuration;

/// <summary>
/// Extension methods for configuring services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all required services to the service collection
    /// </summary>
    public static IServiceCollection AddAppToCodeGen(this IServiceCollection services)
    {
        // Core services
        services.AddSingleton<IFileSystemService, FileSystemService>();
        services.AddSingleton<IProjectLocatorService, ProjectLocatorService>();
        services.AddSingleton<ITemplateEngine, TemplateEngine>();

        // Application services
        services.AddScoped<IFeatureGenerator, FeatureGeneratorService>();

        // Presentation services
        services.AddScoped<GenerateFeatureCommandHandler>();

        return services;
    }
}
