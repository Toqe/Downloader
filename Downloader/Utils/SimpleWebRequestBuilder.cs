using System;
using System.Net;
using Toqe.Downloader.Contract;

namespace Toqe.Downloader.Utils
{
    public class SimpleWebRequestBuilder : IWebRequestBuilder
    {
        public SimpleWebRequestBuilder(IWebProxy proxy)
        {
            Proxy = proxy;
        }

        public SimpleWebRequestBuilder()
            : this(null)
        {
        }

        public IWebProxy Proxy { get; private set; }

        public HttpWebRequest CreateRequest(Uri url, long? offset)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddRange(offset.Value);
            }

            return request;
        }
    }
}
