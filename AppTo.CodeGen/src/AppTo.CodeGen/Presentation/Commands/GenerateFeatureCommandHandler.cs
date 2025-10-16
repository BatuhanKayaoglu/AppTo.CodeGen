using System;
using System.Threading.Tasks;
using AppTo.CodeGen.Application.DTOs;
using AppTo.CodeGen.Core.Interfaces;

namespace AppTo.CodeGen.Presentation.Commands;

/// <summary>
/// Handler for generate feature command
/// </summary>
public class GenerateFeatureCommandHandler
{
    private readonly IFeatureGenerator _featureGenerator;

    public GenerateFeatureCommandHandler(IFeatureGenerator featureGenerator)
    {
        _featureGenerator = featureGenerator ?? throw new ArgumentNullException(nameof(featureGenerator));
    }

    public async Task<int> HandleAsync(GenerateFeatureCommand command)
    {
        try
        {
            var result = await _featureGenerator.GenerateAsync(command);

            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"\n✅ {command.FeatureName} {command.Type} başarıyla oluşturuldu!");

                foreach (var message in result.Messages)
                {
                    Console.WriteLine(message);
                }

                if (!string.IsNullOrEmpty(command.Endpoint))
                {
                    Console.WriteLine($"📁 Endpoint: {command.Endpoint}Controller.cs içine eklendi");
                }

                Console.ResetColor();
                return 0;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Hata: {result.ErrorMessage}");
                Console.ResetColor();
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Hata: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }
}
