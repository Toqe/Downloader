using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Toqe.Downloader.Business.Contract;

namespace Toqe.Downloader.Business.Utils
{
    public class SimpleWebRequestBuilder : IWebRequestBuilder
    {
        public SimpleWebRequestBuilder(IWebProxy proxy)
        {
            this.proxy = proxy;
        }

        public SimpleWebRequestBuilder()
            : this(null)
        {
        }

        public IWebProxy proxy { get; private set; }

        public HttpWebRequest CreateRequest(Uri url, int? offset)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            if (proxy != null)
            {
                request.Proxy = proxy;
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddRange(offset.Value);
            }

            return request;
        }
    }
}
