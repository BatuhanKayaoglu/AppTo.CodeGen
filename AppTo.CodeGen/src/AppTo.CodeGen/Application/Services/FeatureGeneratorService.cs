using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppTo.CodeGen.Core.Interfaces;
using AppTo.CodeGen.Core.Models;

namespace AppTo.CodeGen.Application.Services;

/// <summary>
/// Service for generating features
/// </summary>
public class FeatureGeneratorService : IFeatureGenerator
{
    private readonly IProjectLocatorService _projectLocator;
    private readonly IFileSystemService _fileSystem;
    private readonly ITemplateEngine _templateEngine;

    public FeatureGeneratorService(
        IProjectLocatorService projectLocator,
        IFileSystemService fileSystem,
        ITemplateEngine templateEngine)
    {
        _projectLocator = projectLocator ?? throw new ArgumentNullException(nameof(projectLocator));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
    }

    public async Task<FeatureGenerationResult> GenerateAsync(FeatureGenerationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FeatureName))
            {
                return new FeatureGenerationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Feature name cannot be empty"
                };
            }

            var projectStructure = _projectLocator.LocateProjectStructure();
            var generatedFiles = new List<string>();
            var messages = new List<string>();

            // Create directories
            var directories = CreateDirectories(request, projectStructure);
            foreach (var dir in directories)
            {
                _fileSystem.EnsureDirectory(dir);
            }

            // Generate files
            await GenerateFeatureFiles(request, projectStructure, generatedFiles, messages);

            return new FeatureGenerationResult
            {
                IsSuccess = true,
                GeneratedFiles = generatedFiles,
                Messages = messages
            };
        }
        catch (Exception ex)
        {
            return new FeatureGenerationResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private List<string> CreateDirectories(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var directories = new List<string>();

        // Application layer directories
        var appFeatureFolder = Path.Combine(projectStructure.ApplicationLayer, request.FeatureName);
        var appTypeFolder = Path.Combine(appFeatureFolder, request.Type == Core.Enums.FeatureType.Command ? "Commands" : "Queries");
        directories.Add(appFeatureFolder);
        directories.Add(appTypeFolder);

        // Abstraction layer directories
        var absFeatureFolder = Path.Combine(projectStructure.AbstractionLayer, request.FeatureName);
        var requestFolder = Path.Combine(absFeatureFolder, "Request");
        var responseFolder = Path.Combine(absFeatureFolder, "Response");
        directories.Add(absFeatureFolder);
        directories.Add(requestFolder);
        directories.Add(responseFolder);

        return directories;
    }

    private async Task GenerateFeatureFiles(
        FeatureGenerationRequest request,
        ProjectStructure projectStructure,
        List<string> generatedFiles,
        List<string> messages)
    {
        var appTypeFolder = Path.Combine(projectStructure.ApplicationLayer, request.FeatureName,
            request.Type == Core.Enums.FeatureType.Command ? "Commands" : "Queries");
        var requestFolder = Path.Combine(projectStructure.AbstractionLayer, request.FeatureName, "Request");
        var responseFolder = Path.Combine(projectStructure.AbstractionLayer, request.FeatureName, "Response");

        // Generate main feature files
        var featureFile = Path.Combine(appTypeFolder, $"{request.FeatureName}{request.Type}.cs");
        var handlerFile = Path.Combine(appTypeFolder, $"{request.FeatureName}{request.Type}Handler.cs");
        var requestFile = Path.Combine(requestFolder, $"{request.FeatureName}Request.cs");
        var responseFile = Path.Combine(responseFolder, $"{request.FeatureName}Response.cs");

        string featureCode, handlerCode;
        if (request.Type == Core.Enums.FeatureType.Command)
        {
            featureCode = _templateEngine.GenerateCommand(request, projectStructure);
            handlerCode = _templateEngine.GenerateCommandHandler(request, projectStructure);
        }
        else
        {
            featureCode = _templateEngine.GenerateQuery(request, projectStructure);
            handlerCode = _templateEngine.GenerateQueryHandler(request, projectStructure);
        }

        _fileSystem.WriteFile(featureFile, featureCode);
        _fileSystem.WriteFile(handlerFile, handlerCode);
        _fileSystem.WriteFile(requestFile, _templateEngine.GenerateRequest(request, projectStructure));
        _fileSystem.WriteFile(responseFile, _templateEngine.GenerateResponse(request, projectStructure));

        generatedFiles.AddRange(new[] { featureFile, handlerFile, requestFile, responseFile });
        messages.AddRange(new[]
        {
            $"✅ {request.FeatureName}{request.Type}.cs created",
            $"✅ {request.FeatureName}{request.Type}Handler.cs created",
            $"✅ {request.FeatureName}Request.cs created",
            $"✅ {request.FeatureName}Response.cs created"
        });

        // Generate validator if requested
        if (request.GenerateValidator)
        {
            var validatorFile = Path.Combine(appTypeFolder, $"{request.FeatureName}{request.Type}Validator.cs");
            string validatorCode = request.Type == Core.Enums.FeatureType.Command
                ? _templateEngine.GenerateCommandValidator(request, projectStructure)
                : _templateEngine.GenerateQueryValidator(request, projectStructure);

            _fileSystem.WriteFile(validatorFile, validatorCode);
            generatedFiles.Add(validatorFile);
            messages.Add($"✅ {request.FeatureName}{request.Type}Validator.cs created");
        }

        // Generate endpoint if specified
        if (!string.IsNullOrEmpty(request.Endpoint))
        {
            await GenerateEndpoint(request, projectStructure, generatedFiles, messages);
        }

        await Task.CompletedTask;
    }

    private async Task GenerateEndpoint(
        FeatureGenerationRequest request,
        ProjectStructure projectStructure,
        List<string> generatedFiles,
        List<string> messages)
    {
        var controllerFolder = Path.Combine(projectStructure.ControllersLayer, request.Endpoint!);
        var controllerFile = Path.Combine(controllerFolder, $"{request.Endpoint}Controller.cs");

        if (_fileSystem.FileExists(controllerFile))
        {
            var existingContent = _fileSystem.ReadFile(controllerFile);
            var endpointCode = _templateEngine.GenerateEndpoint(request, projectStructure);

            // Add endpoint to existing controller
            var updatedContent = AddEndpointToController(existingContent, endpointCode);
            _fileSystem.UpdateFile(controllerFile, updatedContent);

            messages.Add($"✅ Endpoint {request.FeatureName} added to {request.Endpoint}Controller.cs");
        }
        else
        {
            messages.Add($"⚠️ Controller not found: {controllerFile}");
        }

        await Task.CompletedTask;
    }

    private string AddEndpointToController(string existingContent, string endpointCode)
    {
        // Check if endpoints are inside namespace or outside
        var namespaceStartIndex = existingContent.IndexOf("namespace ");
        if (namespaceStartIndex == -1)
        {
            // No namespace found, add at the end
            var lastBraceIndex = existingContent.LastIndexOf("}");
            if (lastBraceIndex >= 0)
            {
                return existingContent.Substring(0, lastBraceIndex) +
                       $"{endpointCode}\n}}";
            }
            return existingContent + endpointCode;
        }

        // Check for modern namespace syntax (with semicolon)
        var namespaceLineEnd = existingContent.IndexOf('\n', namespaceStartIndex);
        if (namespaceLineEnd == -1) namespaceLineEnd = existingContent.Length;

        var namespaceLine = existingContent.Substring(namespaceStartIndex, namespaceLineEnd - namespaceStartIndex);
        var isModernNamespace = namespaceLine.Contains(';');

        if (isModernNamespace)
        {
            // Modern namespace syntax: namespace Name;
            // Endpoints are always outside the namespace
            var lastBraceIndex = existingContent.LastIndexOf("}");
            if (lastBraceIndex >= 0)
            {
                return existingContent.Substring(0, lastBraceIndex) +
                       $"{endpointCode}\n}}";
            }
            return existingContent + endpointCode;
        }
        else
        {
            // Classic namespace syntax: namespace Name { ... }
            var namespaceEndIndex = existingContent.IndexOf("{", namespaceStartIndex);
            if (namespaceEndIndex == -1)
            {
                // Namespace found but no opening brace, add at the end
                var lastBraceIndex = existingContent.LastIndexOf("}");
                if (lastBraceIndex >= 0)
                {
                    return existingContent.Substring(0, lastBraceIndex) +
                           $"{endpointCode}\n}}";
                }
                return existingContent + endpointCode;
            }

            // Find the last method/endpoint before the namespace closing brace
            var namespaceClosingBraceIndex = FindNamespaceClosingBrace(existingContent, namespaceEndIndex);
            if (namespaceClosingBraceIndex == -1)
            {
                // Couldn't find namespace closing brace, add at the end
                var lastBraceIndex = existingContent.LastIndexOf("}");
                if (lastBraceIndex >= 0)
                {
                    return existingContent.Substring(0, lastBraceIndex) +
                           $"{endpointCode}\n}}";
                }
                return existingContent + endpointCode;
            }

            // Check if there are endpoints inside the namespace
            var contentInsideNamespace = existingContent.Substring(namespaceEndIndex + 1, namespaceClosingBraceIndex - namespaceEndIndex - 1);
            var hasEndpointsInsideNamespace = contentInsideNamespace.Contains("[HttpPost]") ||
                                             contentInsideNamespace.Contains("[HttpGet]") ||
                                             contentInsideNamespace.Contains("[HttpPut]") ||
                                             contentInsideNamespace.Contains("[HttpDelete]");

            if (hasEndpointsInsideNamespace)
            {
                // Endpoints are inside namespace, add before the second-to-last closing brace
                var secondToLastBraceIndex = FindSecondToLastClosingBrace(existingContent, namespaceClosingBraceIndex);
                if (secondToLastBraceIndex >= 0)
                {
                    return existingContent.Substring(0, secondToLastBraceIndex) +
                           $"{endpointCode}\n    " +
                           existingContent.Substring(secondToLastBraceIndex);
                }
                else
                {
                    // Fallback: add before the namespace closing brace
                    return existingContent.Substring(0, namespaceClosingBraceIndex) +
                           $"{endpointCode}\n    " +
                           existingContent.Substring(namespaceClosingBraceIndex);
                }
            }
            else
            {
                // Endpoints are outside namespace, add after the namespace closing brace
                return existingContent.Substring(0, namespaceClosingBraceIndex + 1) +
                       $"\n{endpointCode}" +
                       existingContent.Substring(namespaceClosingBraceIndex + 1);
            }
        }
    }

    private int FindNamespaceClosingBrace(string content, int startIndex)
    {
        var braceCount = 1;
        for (int i = startIndex + 1; i < content.Length; i++)
        {
            if (content[i] == '{')
                braceCount++;
            else if (content[i] == '}')
            {
                braceCount--;
                if (braceCount == 0)
                    return i;
            }
        }
        return -1;
    }

    private int FindSecondToLastClosingBrace(string content, int namespaceClosingBraceIndex)
    {
        // Find the last closing brace before the namespace closing brace
        var lastBraceIndex = -1;
        for (int i = namespaceClosingBraceIndex - 1; i >= 0; i--)
        {
            if (content[i] == '}')
            {
                lastBraceIndex = i;
                break;
            }
        }
        return lastBraceIndex;
    }
}
