using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SwaggerAssembly.Reflection;

namespace SwaggerAssembly.Config
{
    public class SwaggerConfig
    {
        public string Description { get; private set; }

        public string DocUrl { get; private set; }

        public string TermsOfService { get; private set; }

        public string Title { get; private set; }

        public string VersionName { get; private set; }

        public string VersionNumber { get; private set; }

        public string XmlFileName { get; private set; }

        private const string SwaggerName = "Swagger";

        private readonly IConfigurationSection _swaggerConfig;
        private readonly SwaggerConfigKeySpec _swaggerConfigKeySpec;
        private readonly AssemblyMetaDataExtractor _assemblyMetaDataExtractor;

        public SwaggerConfig(IConfigurationRoot swaggerConfig, Assembly executingAssembly,
            SwaggerConfigKeySpec swaggerConfigKeySpec = null)
        {
            _swaggerConfig = swaggerConfig.GetSection(SwaggerName);
            _assemblyMetaDataExtractor = new AssemblyMetaDataExtractor(executingAssembly);
            _swaggerConfigKeySpec = swaggerConfigKeySpec ?? SwaggerConfigKeySpec.CreateDefaultConfigKeySpec();

            PopulateStringProperties();
        }

        private void PopulateStringProperties()
        {
            PopulateVersionProperties();
            PopulateMandatoryProperties();
            PopulateOptionalProperties();
        }

        private void PopulateVersionProperties()
        {
            var versionNumber = _assemblyMetaDataExtractor.GetProductVersion();
            var versionName = SwaggerVersionBuilder.ExtractVersionNameFromVersionNumber(versionNumber);
            VersionName = versionName;
            VersionNumber = versionNumber;

            var docUrl = ExtractAndValidateConfigValue(_swaggerConfigKeySpec.DocUrl);
            DocUrl = SwaggerVersionBuilder.EnsureDocUrlHasCorrectVersion(docUrl, versionName);

            var title = ExtractAndValidateConfigValue(_swaggerConfigKeySpec.Title);
            Title = SwaggerVersionBuilder.EnsureTitleHasCorrectVersion(title, versionName);
        }

        private void PopulateMandatoryProperties()
        {
            Description = ExtractAndValidateConfigValue(_swaggerConfigKeySpec.Description);
            XmlFileName = _assemblyMetaDataExtractor.GetAssemblyXmlFilename();
        }

        private void PopulateOptionalProperties()
        {
            TermsOfService = ExtractConfigValue(_swaggerConfigKeySpec.TermsOfService);
        }

        private string ExtractAndValidateConfigValue(string key)
        {
            var configValue = ExtractConfigValue(key);
            ValidateConfigValue(configValue, key);
            return configValue;
        }

        private string ExtractConfigValue(string key)
        {
            return _swaggerConfig[key];
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