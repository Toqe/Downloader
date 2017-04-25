using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Toqe.Downloader.Business.Contract
{
    public interface IWebRequestBuilder
    {
        HttpWebRequest CreateRequest(Uri url, long? offset);
    }
}
