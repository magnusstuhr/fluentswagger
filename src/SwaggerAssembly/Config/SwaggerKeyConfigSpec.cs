namespace SwaggerAssembly.Config
{
    public class SwaggerConfigKeySpec
    {
        public readonly string Description;
        public readonly string DocUrl;
        public readonly string TermsOfService;
        public readonly string Title;
        public readonly string UseBearerAuth;

        public SwaggerConfigKeySpec(string descriptionKey, string docUrlKey, string termsOfServiceKey, string titleKey,
            string userBearerAuthNameKey = nameof(UseBearerAuth))
        {
            Description = descriptionKey;
            DocUrl = docUrlKey;
            TermsOfService = termsOfServiceKey;
            Title = titleKey;
            UseBearerAuth = userBearerAuthNameKey;
        }

        public static SwaggerConfigKeySpec CreateDefaultConfigKeySpec()
        {
            return new SwaggerConfigKeySpec(nameof(Description), nameof(DocUrl), nameof(TermsOfService), nameof(Title));
        }
    }
}