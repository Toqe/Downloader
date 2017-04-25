using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;

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
