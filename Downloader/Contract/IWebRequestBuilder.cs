using System;
using System.Net;

namespace Toqe.Downloader.Contract
{
    public interface IWebRequestBuilder
    {
        HttpWebRequest CreateRequest(Uri url, long? offset);
    }
}
