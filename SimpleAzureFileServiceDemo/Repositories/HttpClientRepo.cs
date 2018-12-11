using System.Net.Http;

namespace SimpleAzureFileServiceDemo.Repositories
{
    /// <inheritdoc />
    /// <summary>
    /// Default Repository for HttpClient Tests
    /// </summary>
    public class HttpClientRepo : IHttpClientRepo
    {
        public HttpClient Instance()
        {
            return new HttpClient();
        }
    }
}
