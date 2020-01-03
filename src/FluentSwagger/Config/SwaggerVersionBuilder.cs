using System;
using System.Text.RegularExpressions;

namespace FluentSwagger.Config
{
    internal sealed class SwaggerVersionBuilder
    {
        private const string VersionNamePattern = "\\$\\{VersionName\\}";

        internal static string EnsureTitleHasCorrectVersion(string title, string versionName)
        {
            return EnsureCorrectVersion(title, versionName);
        }

        internal static string EnsureDocUrlHasCorrectVersion(string docUrl, string versionName)
        {
            if (!HasVersionNameAnnotation(docUrl))
            {
                throw new ArgumentException("The Swagger doc URL should have a ${VersionName} annotation in the URL, " +
                                            "so that it gets the correct version name from the executing assembly.",
                    nameof(docUrl));
            }

            return EnsureCorrectVersion(docUrl, versionName);
        }

        internal static string ExtractVersionNameFromVersionNumber(string versionNumber)
        {
            var versionFragments = SplitVersionNumbers(versionNumber);

            const int majorVersionIndex = 0;
            var majorVersion = versionFragments[majorVersionIndex];

            return CreateVersionName(majorVersion);
        }

        private static string EnsureCorrectVersion(string value, string versionName)
        {
            return ReplaceRegexPattern(value, VersionNamePattern, versionName);
        }

        private static bool HasVersionNameAnnotation(string value)
        {
            return CreateIgnoreCaseRegex(VersionNamePattern).IsMatch(value);
        }

        private static string ReplaceRegexPattern(string input, string regexPattern, string replacementForRegexPattern)
        {
            return CreateIgnoreCaseRegex(regexPattern).Replace(input, replacementForRegexPattern);
        }

        private static Regex CreateIgnoreCaseRegex(string regexPattern)
        {
            return new Regex(regexPattern, RegexOptions.IgnoreCase);
        }

        private static string[] SplitVersionNumbers(string versionNumber)
        {
            const string versionFragmentSeparator = ".";
            return versionNumber.Split(versionFragmentSeparator);
        }

        private static string CreateVersionName(string versionNumber)
        {
            const string versionAnnotation = "v";
            return $"{versionAnnotation}{versionNumber}";
        }
    }
}