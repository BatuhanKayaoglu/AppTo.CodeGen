using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppTo.CodeGen.Commands.Templates;
using AppTo.CodeGen.Services;

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
        public async Task GenerateAsync(string featureName, string type = "command", string endpoint = "")
        {
            if (string.IsNullOrWhiteSpace(featureName))
            {
                Console.WriteLine("❌ Feature adı boş olamaz!");
                return;
            }

            type = type.ToLower();

            if (type != "command" && type != "query")
            {
                Console.WriteLine("❌ Type 'command' veya 'query' olmalı!");
                return;
            }

            // 1️⃣ Application katmanını bul
            var appLayer = _locator.LocateApplicationLayer();

            // 2️⃣ Abstraction katmanını bul
            var abstractionLayer = _locator.LocateAbstractionLayer();

            // 3️⃣ Application: Feature klasörünü oluştur (Application altında direkt)
            var appFeatureFolder = Path.Combine(appLayer, featureName);
            _fileSystem.EnsureDirectory(appFeatureFolder);

            // 4️⃣ Application: Altına Commands veya Queries klasörü oluştur
            var appTypeFolder = Path.Combine(appFeatureFolder, type.Equals("command") ? "Commands" : "Queries");
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
            var commandFile = Path.Combine(appTypeFolder, $"{featureName}Command.cs");
            var handlerFile = Path.Combine(appTypeFolder, $"{featureName}CommandHandler.cs");
            var requestFile = Path.Combine(requestFolder, $"{featureName}Request.cs");
            var responseFile = Path.Combine(responseFolder, $"{featureName}Response.cs");

            // 8️⃣ Namespace'ler
            var appProjectName = new DirectoryInfo(appLayer).Name; // örn: Metropol.YODA.Application
            var absProjectName = new DirectoryInfo(abstractionLayer).Name; // örn: Metropol.YODA.Abstraction
            var appNamespaceName = $"{appProjectName}.{featureName}.{(type.Equals("command") ? "Commands" : "Queries")}";
            var requestNamespaceName = $"{absProjectName}.{featureName}.Request";
            var responseNamespaceName = $"{absProjectName}.{featureName}.Response";

            // 9️⃣ Kodları oluştur
            var commandCode = CommandGenerator.CreateCommand(appNamespaceName, featureName, type);
            var handlerCode = CommandHandlerGenerator.CreateCommandHandler(appNamespaceName, featureName, type);
            var requestCode = RequestGenerator.CreateRequest(requestNamespaceName, featureName);
            var responseCode = ResponseGenerator.CreateResponse(responseNamespaceName, featureName);

            // 🔟 Dosyaları yaz
            _fileSystem.WriteFile(commandFile, commandCode);
            _fileSystem.WriteFile(handlerFile, handlerCode);
            _fileSystem.WriteFile(requestFile, requestCode);
            _fileSystem.WriteFile(responseFile, responseCode);

            Console.WriteLine($"✅ {featureName}Command.cs oluşturuldu: {commandFile}");
            Console.WriteLine($"✅ {featureName}CommandHandler.cs oluşturuldu: {handlerFile}");
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

                    // Controller'ın son } karakterini bul ve endpoint'i ekle
                    var lastBraceIndex = existingContent.LastIndexOf("}");
                    if (lastBraceIndex >= 0)
                    {
                        var newContent = existingContent.Substring(0, lastBraceIndex) +
                                       $"{endpointCode}\n}}";
                        File.WriteAllText(controllerFile, newContent);
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
