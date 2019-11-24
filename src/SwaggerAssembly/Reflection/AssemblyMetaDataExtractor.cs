using System.Diagnostics;
using System.Reflection;

namespace SwaggerAssembly.Reflection
{
    internal class AssemblyMetaDataExtractor
    {
        private readonly Assembly _executingAssembly;

        internal AssemblyMetaDataExtractor(Assembly assembly)
        {
            _executingAssembly = assembly;
        }

        internal string GetProductVersion()
        {
            return FileVersionInfo.GetVersionInfo(_executingAssembly.Location).ProductVersion;
        }


        internal string GetAssemblyXmlFilename()
        {
            const string xmlFileExtension = "xml";
            return $"{GetExecutingAssemblyName()}.{xmlFileExtension}";
        }

        private string GetExecutingAssemblyName()
        {
            return _executingAssembly.GetName().Name;
        }
    }
}