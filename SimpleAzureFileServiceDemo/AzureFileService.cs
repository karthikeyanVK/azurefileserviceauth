using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SimpleAzureFileServiceDemo.Repositories;

namespace SimpleAzureFileServiceDemo
{
    /// <summary>
    /// Implements the Azure FileService REST API
    /// https://docs.microsoft.com/en-us/rest/api/storageservices/file-service-rest-api
    /// </summary>
    public class AzureFileService
    {
        private readonly string _accessKey;
        private readonly string _accoutName;
        private readonly IHttpClientRepo _clientRepo;

        /// <summary>
        /// Initialize FileService
        /// </summary>
        public AzureFileService(string accountName, string accessKey, IHttpClientRepo clientRepo = null)
        {
            _accessKey = accessKey;
            _accoutName = accountName;
            // set the default repo in case of null
            if (clientRepo == null) clientRepo = new HttpClientRepo();

            _clientRepo = clientRepo;
        }

        /// <summary>
        /// Create File Request
        /// </summary>
        /// <returns></returns>
        public async Task CreateFileAsync(Uri storageUri)
        {
            //Instantiate the request message with a empty payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, storageUri)
            { Content = new StringContent("") })
            {
                var now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");

                // Required. This header specifies the maximum size for the file, up to 1 TiB.
                httpRequestMessage.Headers.Add("x-ms-content-length", httpRequestMessage.Content.Headers.ContentLength.ToString());
                httpRequestMessage.Headers.Add("x-ms-type", "file");

                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   _accoutName, _accessKey, now, httpRequestMessage);

                // Send the request.
                using (var httpResponseMessage = await _clientRepo.Instance().SendAsync(httpRequestMessage, CancellationToken.None))
                {
                    if (!httpResponseMessage.IsSuccessStatusCode)
                    {
                        Trace.TraceError(storageUri.AbsoluteUri);
                        Trace.TraceError(httpResponseMessage.StatusCode.ToString());
                        Trace.TraceError(await httpResponseMessage.Content.ReadAsStringAsync());
                    }
                }
            }
        }

        /// <summary>
        /// Put Range Request
        /// </summary>
        /// <returns></returns>
        public async Task PutRangeAsync()
        {

        }
    }
}
