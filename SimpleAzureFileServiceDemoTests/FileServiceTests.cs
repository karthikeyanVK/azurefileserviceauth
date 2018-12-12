using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAzureFileServiceDemo;
using SimpleAzureFileServiceDemoTests.Repositories;

namespace SimpleAzureFileServiceDemoTests
{
    [TestClass]
    public class FileServiceTests
    {
        const string accountName = "YOURACCOUNTNAME";
        const string accessKey = "YOURACCESSKEY";
        const string uripath = "https://ACCOUNTNAME.file.core.windows.net/CONTAINER/FILE";

        [TestMethod]
        public async Task CreateFileAsync()
        {
            IEnumerable<string> headerValues = new List<string>();

            Action<HttpResponseMessage> respAction = (r) =>
            {
                // check whatever header or content values indicate the success of this test here
                r.Headers.TryGetValues("Authorization", out headerValues);
            };

            var repo = new FakeHttpClientRepo(respAction);
            //var repo = new HttpClientRepo();

            var azureFileService = new AzureFileService(accountName, accessKey, repo);

            await azureFileService.CreateFileAsync(new Uri(uripath));

            // Check if encryption is possible with accessKey
            Assert.IsTrue(headerValues.Any(v => v.Contains($"SharedKey {accountName}:")));
        }

        [TestMethod]
        public async Task PutRangeAsync()
        {
            const string testContent = "Content as String";
            IEnumerable<string> headerValues = new List<string>();
            IEnumerable<string> xmsheaderValues = new List<string>();
            long? contentLengthReq = null;
            Action<HttpResponseMessage> respAction = (r) =>
            {
                r.Headers.TryGetValues("Authorization", out headerValues);
                r.Headers.TryGetValues("x-ms-range", out xmsheaderValues);
                contentLengthReq = r.Content.Headers.ContentLength;
            };

            var repo = new FakeHttpClientRepo(respAction);
            //var repo = new HttpClientRepo();

            var azureFileService = new AzureFileService(accountName, accessKey, repo);

            byte[] bytes = Encoding.UTF8.GetBytes(testContent);

            await azureFileService.PutRangeAsync(
                new Uri(uripath + "?comp=range"),
                bytes);

            var xmsStringComparison = $"bytes=0-{(bytes.Length - 1).ToString()}";

            Assert.IsTrue(
                bytes.Length == contentLengthReq
                && xmsheaderValues.Any(hv => hv.Contains(xmsStringComparison))
                && headerValues.Any(v => v.Contains($"SharedKey {accountName}:")));
        }
    }
}
