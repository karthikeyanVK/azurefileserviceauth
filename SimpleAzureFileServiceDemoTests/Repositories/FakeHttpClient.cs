using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleAzureFileServiceDemoTests.Repositories
{
    public class FakeHttpClient : HttpClient
    {
        private readonly Action<HttpResponseMessage> _responseAction;

        public FakeHttpClient(Action<HttpResponseMessage> responseAction)
        {
            _responseAction = responseAction;
        }

        /// <summary>
        /// does not actually send anything.
        /// Just returns the request as response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            // Set the headers of the original request in the response for testing
            foreach (var httpRequestHeader in request.Headers)
            {
                response.Headers.TryAddWithoutValidation(httpRequestHeader.Key, httpRequestHeader.Value);
            }

            response.Content = request.Content;

            // set the original content for testing

            _responseAction.Invoke(response);

            return response;
        }
    }
}
