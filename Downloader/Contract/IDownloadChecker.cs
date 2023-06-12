using System;
using System.Net;

namespace Toqe.Downloader.Contract
{
    public interface IDownloadChecker
    {
        DownloadCheckResult CheckDownload(WebResponse response);

        DownloadCheckResult CheckDownload(Uri url, IWebRequestBuilder requestBuilder);
    }
}
