using System;

namespace FluentSwaggerTests.Swagger
{
    public class SwaggerRuntimeInfo : IEquatable<SwaggerRuntimeInfo>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SwaggerRuntimeInfo);
        }

        public bool Equals(SwaggerRuntimeInfo other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Title, other.Title) && Equals(Description, other.Description) &&
                   Equals(Version, other.Version);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Description, Version);
        }

        private static bool Equals(string s1, string s2)
        {
            return string.Equals(s1, s2);
        }
    }
}