using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            var headersFound = false;
            IEnumerable<string> fileHeaders;

            Action<HttpResponseMessage> respAction = async (r) =>
            {
                // check whatever header or content values indicate the success of this test here
                headersFound = r.Headers.TryGetValues("Authorization", out headerValues);
            };

            var repo = new FakeHttpClientRepo(respAction);

            var azureFileService = new AzureFileService(accountName, accessKey, repo);

            await azureFileService.CreateFileAsync(new Uri(uripath));

            // Check if encryption is possible with accessKey
            Assert.IsTrue(headersFound && headerValues.Any(v => v.Contains($"SharedKey {accountName}:")));
        }

        [TestMethod]
        public async Task PushRangeAsync()
        {


        }
    }
}
