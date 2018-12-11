using System;
using System.Net.Http;
using SimpleAzureFileServiceDemo.Repositories;

namespace SimpleAzureFileServiceDemoTests.Repositories
{
    public class FakeHttpClientRepo : IHttpClientRepo
    {
        private readonly Action<HttpResponseMessage> _responseAction;

        public FakeHttpClientRepo(Action<HttpResponseMessage> responseAction)
        {
            _responseAction = responseAction;
        }

        public HttpClient Instance()
        {
            return new FakeHttpClient(_responseAction);
        }
    }
}
