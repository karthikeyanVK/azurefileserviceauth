using System.Net.Http;

namespace SimpleAzureFileServiceDemo.Repositories
{
    /// <summary>
    /// Factory Pattern for unit testing
    /// </summary>
    public interface IHttpClientRepo
    {
        HttpClient Instance();
    }
}
