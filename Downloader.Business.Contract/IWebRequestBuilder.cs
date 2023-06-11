using System;
using System.Net;

namespace Toqe.Downloader.Business.Contract
{
    public interface IWebRequestBuilder
    {
        HttpWebRequest CreateRequest(Uri url, long? offset);
    }
}
