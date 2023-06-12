using System;
using System.Net;

namespace Myitian.Downloader.Contract
{
    public interface IWebRequestBuilder
    {
        HttpWebRequest CreateRequest(Uri url, long? offset);
    }
}
