using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentSwaggerTests.Swagger.Endpoint;
using Newtonsoft.Json;
using Xunit;

namespace FluentSwaggerTests.Swagger
{
    public class SwaggerRuntimeValidator
    {
        private readonly HttpClient _client;
        private readonly string _swaggerDocPath;

        public SwaggerRuntimeValidator(HttpClient client, string swaggerDocPath)
        {
            _client = client;
            _swaggerDocPath = swaggerDocPath;
        }

        public async Task VerifySwaggerIsRunningCorrectly(SwaggerRuntimeInfo expectedSwaggerRuntimeInfo)
        {
            var actualSwaggerRuntimeInfo = await ExtractSwaggerRuntimeInfo();

            Assert.Equal(expectedSwaggerRuntimeInfo, actualSwaggerRuntimeInfo);
        }

        private async Task<SwaggerRuntimeInfo> ExtractSwaggerRuntimeInfo()
        {
            var response = await _client.GetAsync(_swaggerDocPath);

            ValidateHttpOkStatus(response);

            var swaggerEndpointConfig = await DeserializeSwaggerJsonConfig(response);

            var swaggerRuntimeInfo = BuildSwaggerRuntimeInfoFromEndpoint(swaggerEndpointConfig);
            return swaggerRuntimeInfo;
        }

        private static async Task<SwaggerEndpointConfig> DeserializeSwaggerJsonConfig(
            HttpResponseMessage httpResponseMessage)
        {
            var responseString = await GetStringContentFromHttpResponse(httpResponseMessage);

            var hasJsonParsingThrownException = false;
            SwaggerEndpointConfig swaggerEndpointConfig = null;
            try
            {
                swaggerEndpointConfig = JsonConvert.DeserializeObject<SwaggerEndpointConfig>(responseString);
            }
            catch (Exception)
            {
                hasJsonParsingThrownException = true;
            }

            if (hasJsonParsingThrownException || swaggerEndpointConfig?.Info is null)
            {
                var httpRequestUri = ExtractRequestUri(httpResponseMessage);
                throw new JsonReaderException($"Could not parse the JSON from the Swagger endpoint {httpRequestUri}.");
            }

            return swaggerEndpointConfig;
        }

        private static SwaggerRuntimeInfo BuildSwaggerRuntimeInfoFromEndpoint(
            SwaggerEndpointConfig swaggerEndpointConfig)
        {
            var swaggerEndpointInfo = swaggerEndpointConfig.Info;

            return new SwaggerRuntimeInfo
            {
                Title = swaggerEndpointInfo.Title,
                Description = swaggerEndpointInfo.Description,
                Version = swaggerEndpointInfo.Version,
            };
        }

        private static void ValidateHttpOkStatus(HttpResponseMessage httpResponseMessage)
        {
            var responseStatusCode = ExtractStatusCode(httpResponseMessage);

            if (responseStatusCode != HttpStatusCode.OK)
            {
                ThrowHttpRequestExceptionForSwagger(httpResponseMessage);
            }
        }

        private static void ThrowHttpRequestExceptionForSwagger(HttpResponseMessage httpResponseMessage)
        {
            var responseStatusCode = ExtractStatusCode(httpResponseMessage);
            var httpRequestUri = ExtractRequestUri(httpResponseMessage);
            var responseStatusCodeInteger = (int) responseStatusCode;

            var exceptionMessage =
                $"The URL {httpRequestUri} returned a {responseStatusCodeInteger} {responseStatusCode} status code.";

            throw new HttpRequestException(exceptionMessage);
        }

        private static HttpStatusCode ExtractStatusCode(HttpResponseMessage httpResponseMessage)
        {
            return httpResponseMessage.StatusCode;
        }

        private static Uri ExtractRequestUri(HttpResponseMessage httpResponseMessage)
        {
            return httpResponseMessage.RequestMessage.RequestUri;
        }

        private static async Task<string> GetStringContentFromHttpResponse(HttpResponseMessage httpResponseMessage)
        {
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}