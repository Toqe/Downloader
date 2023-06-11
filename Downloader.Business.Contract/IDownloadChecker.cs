using System;
using System.Net;

namespace Toqe.Downloader.Business.Contract
{
    public interface IDownloadChecker
    {
        DownloadCheckResult CheckDownload(WebResponse response);

        DownloadCheckResult CheckDownload(Uri url, IWebRequestBuilder requestBuilder);
    }
}
