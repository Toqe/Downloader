using System;
using Toqe.Downloader.Contract;

namespace Downloader.Test
{
    public class TestWebRequestBuilder : IWebRequestBuilder
    {
        public System.Net.HttpWebRequest CreateRequest(Uri url, long? offset)
        {
            throw new NotImplementedException();
        }
    }
}
