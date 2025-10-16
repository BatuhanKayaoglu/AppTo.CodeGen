using System.Threading.Tasks;
using AppTo.CodeGen.Core.Models;

namespace AppTo.CodeGen.Core.Interfaces;

/// <summary>
/// Service for generating features
/// </summary>
public interface IFeatureGenerator
{
    Task<FeatureGenerationResult> GenerateAsync(FeatureGenerationRequest request);
}
