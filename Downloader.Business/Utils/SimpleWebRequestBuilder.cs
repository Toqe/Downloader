using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
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

        public HttpWebRequest CreateRequest(Uri url, long? offset)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            if (proxy != null)
            {
                request.Proxy = proxy;
            }

            if (offset.HasValue && offset.Value > 0)
            {
                // Only works with long values starting with .NET 4:
                // request.AddRange(offset.Value);

                this.AddLongRangeInDotNet3_5(request, offset.Value);
            }

            return request;
        }

        private void AddLongRangeInDotNet3_5(HttpWebRequest request, long offset)
        {
            var method = typeof(WebHeaderCollection).GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);

            string key = "Range";
            string val = string.Format("bytes={0}-", offset);

            method.Invoke(request.Headers, new object[] { key, val });
        }
    }
}
