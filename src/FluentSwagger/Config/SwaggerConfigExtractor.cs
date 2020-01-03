using System.Collections.Generic;
using System.Reflection;
using FluentSwagger.Reflection;
using Microsoft.Extensions.Configuration;

namespace FluentSwagger.Config
{
    public class SwaggerConfigExtractor
    {
        private const string SwaggerName = "Swagger";

        public readonly SwaggerConfig SwaggerConfig;

        private readonly IConfigurationSection _swaggerConfigSection;
        private readonly AssemblyMetaDataExtractor _assemblyMetaDataExtractor;

        public SwaggerConfigExtractor(IConfiguration swaggerConfig, Assembly executingAssembly)
        {
            _swaggerConfigSection = swaggerConfig.GetSection(SwaggerName);
            _assemblyMetaDataExtractor = new AssemblyMetaDataExtractor(executingAssembly);

            SwaggerConfig = new SwaggerConfig();

            PopulateStringProperties();
        }

        private void PopulateStringProperties()
        {
            PopulateVersionProperties();
            PopulateMandatoryProperties();
        }

        private void PopulateVersionProperties()
        {
            var versionNumber = _assemblyMetaDataExtractor.GetProductVersion();
            var versionName = SwaggerVersionBuilder.ExtractVersionNameFromVersionNumber(versionNumber);
            SwaggerConfig.VersionName = versionName;
            SwaggerConfig.VersionNumber = versionNumber;

            var docUrl = ExtractAndValidateConfigValue("DocUrl");
            SwaggerConfig.DocUrl = SwaggerVersionBuilder.EnsureDocUrlHasCorrectVersion(docUrl, versionName);

            var title = ExtractAndValidateConfigValue("Title");
            SwaggerConfig.Title = SwaggerVersionBuilder.EnsureTitleHasCorrectVersion(title, versionName);
        }

        private void PopulateMandatoryProperties()
        {
            SwaggerConfig.Description = ExtractAndValidateConfigValue("Description");
            SwaggerConfig.XmlFileName = _assemblyMetaDataExtractor.GetAssemblyXmlFilename();
        }

        private string ExtractAndValidateConfigValue(string key)
        {
            var configValue = ExtractConfigValue(key);
            ValidateConfigValue(configValue, key);
            return configValue;
        }

        private string ExtractConfigValue(string key)
        {
            return _swaggerConfigSection[key];
        }

        private static void ValidateConfigValue(object configValue, string configKey)
        {
            if (configValue is null)
            {
                throw new KeyNotFoundException(
                    $"The key '{configKey}' not found in the '{SwaggerName}' config section.");
            }
        }
    }
}
