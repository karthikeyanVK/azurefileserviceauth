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

        public async Task CreateFileAsync(Uri storageUri)
        {
            //Instantiate the request message with a empty payload.
            using (var httpRequestMessage =
                new HttpRequestMessage(HttpMethod.Put, storageUri)
                { Content = new StringContent("") })
            {
                var now = DateTime.UtcNow;

                SetDefaultHeaders(httpRequestMessage, now);
                // Required. This header specifies the maximum size for the file, up to 1 TiB.
                httpRequestMessage.Headers.Add("x-ms-content-length", "200");
                httpRequestMessage.Headers.Add("x-ms-type", "file");

                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   _accoutName, _accessKey, now, httpRequestMessage);

                // Send the request.
                await SendAndTraceOnErrorAsync(httpRequestMessage, storageUri);
            }
        }

        /// <summary>
        /// Put Range Request
        /// </summary>
        /// <returns></returns>
        public async Task PutRangeAsync(Uri storageUri, byte[] bytes, int startBytes = 0, string writeMode = "Update")
        {
            if (string.IsNullOrEmpty(storageUri.Query) || !storageUri.Query.Contains("comp=range")) throw new Exception("Missing Query String comp=range");

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, storageUri))
            {
                httpRequestMessage.Content = new ByteArrayContent(bytes);

                var contentlength = httpRequestMessage.Content.Headers.ContentLength - 1;

                var now = DateTime.UtcNow;
                SetDefaultHeaders(httpRequestMessage, now);

                httpRequestMessage.Headers.Add("x-ms-range", $"bytes={startBytes}-{contentlength.ToString()}");
                httpRequestMessage.Headers.Add("x-ms-write", writeMode);

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                    _accoutName, _accessKey, now, httpRequestMessage);

                await SendAndTraceOnErrorAsync(httpRequestMessage, storageUri);

            }
        }

        #region Helpers

        /// <summary>
        /// Set the default headers which are mandatory in all requests
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="now"></param>
        private void SetDefaultHeaders(HttpRequestMessage httpRequestMessage, DateTime now)
        {
            httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
            httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");
        }

        /// <summary>
        /// Uses HttpClient to send the request Async and traces Errors if Status ist not succeeded
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="storageUri"></param>
        /// <returns></returns>
        private async Task SendAndTraceOnErrorAsync(HttpRequestMessage httpRequestMessage, Uri storageUri)
        {
            using (var httpResponseMessage =
                await _clientRepo.Instance().SendAsync(httpRequestMessage, CancellationToken.None))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    Trace.TraceError(storageUri.AbsoluteUri);
                    Trace.TraceError(httpResponseMessage.StatusCode.ToString());
                    Trace.TraceError(await httpResponseMessage.Content.ReadAsStringAsync());
                }
            }
        }

        #endregion
    }
}
