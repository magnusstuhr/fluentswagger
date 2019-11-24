using System;
using System.Text.RegularExpressions;

namespace SwaggerAssembly.Config
{
    internal sealed class SwaggerVersionBuilder
    {
        private const string VersionFragmentSeparator = ".";
        private const string VersionAnnotation = "v";
        private const string VersionNamePattern = "\\$\\{VersionName\\}";

        internal static string EnsureTitleHasCorrectVersion(string title, string versionName)
        {
            return EnsureCorrectVersion(title, versionName);
        }

        internal static string EnsureDocUrlHasCorrectVersion(string docUrl, string versionName)
        {
            if (!HasVersionNameAnnotation(docUrl))
            {
                const string escapeCharacter = "\\\\";
                var versionNamePatternWithoutEscapedCharacters =
                    $"{ReplaceRegexPattern(VersionNamePattern, escapeCharacter, string.Empty)} " +
                    "annotation in the URL, ";
                throw new ArgumentException("The Swagger doc URL should have a " +
                                            versionNamePatternWithoutEscapedCharacters +
                                            $"so that it gets the correct version name from the executing assembly.",
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
            var versionFragments = versionNumber.Split(VersionFragmentSeparator);
            return versionFragments;
        }

        private static string CreateVersionName(string versionNumber)
        {
            return $"{VersionAnnotation}{versionNumber}";
        }
    }
}