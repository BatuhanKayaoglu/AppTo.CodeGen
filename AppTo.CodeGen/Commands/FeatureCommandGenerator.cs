using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppTo.CodeGen.Commands.Templates;
using AppTo.CodeGen.Services;
using AppTo.CodeGen.Models;

namespace AppTo.CodeGen.Commands
{
    /// <summary>
    /// Tek adımda Command ve CommandHandler üretimi yapar.
    /// Open/Closed prensibine uygun olarak tasarlanmıştır; ileride Query desteği kolayca eklenebilir.
    /// </summary>
    public class FeatureCommandGenerator
    {
        private readonly IApplicationLayerLocator _locator;
        private readonly IFileSystemService _fileSystem;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locator">Application katmanını bulan servis</param>
        /// <param name="fileSystem">Dosya sistemi işlemleri için servis</param>
        public FeatureCommandGenerator(IApplicationLayerLocator locator, IFileSystemService fileSystem)
        {
            _locator = locator ?? throw new ArgumentNullException(nameof(locator));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <summary>
        /// Feature üretimi yapar.
        /// </summary>
        /// <param name="featureName">Feature adı, örn: QrSaleTest</param>
        /// <param name="type">command veya query</param>
        /// <param name="endpoint">Endpoint controller adı, örn: Sale</param>
        public async Task GenerateAsync(string featureName, FeatureType type = FeatureType.Command, string endpoint = "", string projectName = null)
        {
            if (string.IsNullOrWhiteSpace(featureName))
            {
                Console.WriteLine("❌ Feature adı boş olamaz!");
                return;
            }

            // Type validation is now handled by enum, no need to check

            // 1️⃣ Application katmanını bul
            var appLayer = _locator.LocateApplicationLayer();

            // 2️⃣ Abstraction katmanını bul
            var abstractionLayer = _locator.LocateAbstractionLayer();

            // 3️⃣ Application: Feature klasörünü oluştur (Application altında direkt)
            var appFeatureFolder = Path.Combine(appLayer, featureName);
            _fileSystem.EnsureDirectory(appFeatureFolder);

            // 4️⃣ Application: Altına Commands veya Queries klasörü oluştur
            var appTypeFolder = Path.Combine(appFeatureFolder, type == FeatureType.Command ? "Commands" : "Queries");
            _fileSystem.EnsureDirectory(appTypeFolder);

            // 5️⃣ Abstraction: Feature klasörünü oluştur (Abstraction altında direkt)
            var absFeatureFolder = Path.Combine(abstractionLayer, featureName);
            _fileSystem.EnsureDirectory(absFeatureFolder);

            // 6️⃣ Abstraction: Request ve Response klasörlerini oluştur
            var requestFolder = Path.Combine(absFeatureFolder, "Request");
            var responseFolder = Path.Combine(absFeatureFolder, "Response");
            _fileSystem.EnsureDirectory(requestFolder);
            _fileSystem.EnsureDirectory(responseFolder);

            // 7️⃣ Dosya yolları
            var commandFile = Path.Combine(appTypeFolder, type == FeatureType.Command ? $"{featureName}Command.cs" : $"{featureName}Query.cs");
            var handlerFile = Path.Combine(appTypeFolder, type == FeatureType.Command ? $"{featureName}CommandHandler.cs" : $"{featureName}QueryHandler.cs");
            var requestFile = Path.Combine(requestFolder, $"{featureName}Request.cs");
            var responseFile = Path.Combine(responseFolder, $"{featureName}Response.cs");

            // 8️⃣ Namespace'ler
            var appProjectName = new DirectoryInfo(appLayer).Name; // örn: Metropol.YODA.Application
            var absProjectName = new DirectoryInfo(abstractionLayer).Name; // örn: Metropol.YODA.Abstraction
            var appNamespaceName = $"{appProjectName}.{featureName}.{(type == FeatureType.Command ? "Commands" : "Queries")}";
            var requestNamespaceName = $"{absProjectName}.{featureName}.Request";
            var responseNamespaceName = $"{absProjectName}.{featureName}.Response";

            // 9️⃣ Kodları oluştur
            string commandCode, handlerCode;
            if (type == FeatureType.Command)
            {
                commandCode = Commands.Templates.CommandGenerator.CreateCommand(appNamespaceName, featureName, type.ToString().ToLower(), projectName);
                handlerCode = Commands.Templates.CommandHandlerGenerator.CreateCommandHandler(appNamespaceName, featureName, type.ToString().ToLower(), projectName);
            }
            else
            {
                commandCode = Commands.Templates.QueryGenerator.CreateQuery(appNamespaceName, featureName, type.ToString().ToLower(), projectName);
                handlerCode = Commands.Templates.QueryHandlerGenerator.CreateQueryHandler(appNamespaceName, featureName, type.ToString().ToLower(), projectName);
            }
            var requestCode = RequestGenerator.CreateRequest(requestNamespaceName, featureName);
            var responseCode = ResponseGenerator.CreateResponse(responseNamespaceName, featureName);

            // 🔟 Dosyaları yaz
            _fileSystem.WriteFile(commandFile, commandCode);
            _fileSystem.WriteFile(handlerFile, handlerCode);
            _fileSystem.WriteFile(requestFile, requestCode);
            _fileSystem.WriteFile(responseFile, responseCode);

            var commandType = type == FeatureType.Command ? "Command" : "Query";
            var handlerType = type == FeatureType.Command ? "CommandHandler" : "QueryHandler";

            Console.WriteLine($"✅ {featureName}{commandType}.cs oluşturuldu: {commandFile}");
            Console.WriteLine($"✅ {featureName}{handlerType}.cs oluşturuldu: {handlerFile}");
            Console.WriteLine($"✅ {featureName}Request.cs oluşturuldu: {requestFile}");
            Console.WriteLine($"✅ {featureName}Response.cs oluşturuldu: {responseFile}");

            // 11️⃣ Endpoint oluştur (eğer endpoint belirtilmişse)
            if (!string.IsNullOrEmpty(endpoint))
            {
                var controllersLayer = _locator.LocateControllersLayer();
                var controllerFolder = Path.Combine(controllersLayer, endpoint);
                var controllerFile = Path.Combine(controllerFolder, $"{endpoint}Controller.cs");

                if (File.Exists(controllerFile))
                {
                    // Mevcut controller'a endpoint ekle
                    var controllerNamespace = $"{new DirectoryInfo(controllersLayer).Name}.{endpoint}";
                    var endpointCode = EndpointGenerator.CreateEndpoint(controllerNamespace, featureName, endpoint, type);

                    // Controller dosyasını oku ve endpoint'i ekle
                    var existingContent = File.ReadAllText(controllerFile);

                    // Namespace ile sarılı mı kontrol et
                    var hasNamespace = existingContent.Contains("namespace ") &&
                                     existingContent.IndexOf("namespace ") < existingContent.IndexOf("public class");

                    if (hasNamespace)
                    {
                        // Namespace ile sarılı: Son iki } karakterinden önce ekle
                        var lastBraceIndex = existingContent.LastIndexOf("}");
                        var secondLastBraceIndex = existingContent.LastIndexOf("}", lastBraceIndex - 1);

                        if (secondLastBraceIndex >= 0)
                        {
                            var newContent = existingContent.Substring(0, secondLastBraceIndex) +
                                           $"{endpointCode}\n    }}\n}}";
                            File.WriteAllText(controllerFile, newContent);
                        }
                        else
                        {
                            // Fallback: Sadece son } bulunursa
                            var newContent = existingContent.Substring(0, lastBraceIndex) +
                                           $"{endpointCode}\n}}";
                            File.WriteAllText(controllerFile, newContent);
                        }
                    }
                    else
                    {
                        // Namespace olmadan: Son } karakterinden önce ekle
                        var lastBraceIndex = existingContent.LastIndexOf("}");
                        if (lastBraceIndex >= 0)
                        {
                            var newContent = existingContent.Substring(0, lastBraceIndex) +
                                           $"{endpointCode}\n}}";
                            File.WriteAllText(controllerFile, newContent);
                        }
                    }

                    Console.WriteLine($"✅ Endpoint {featureName} eklendi: {controllerFile}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Controller bulunamadı: {controllerFile}");
                }
            }

            await Task.CompletedTask;
        }
    }
}
