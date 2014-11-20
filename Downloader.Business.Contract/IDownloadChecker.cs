using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Toqe.Downloader.Business.Contract
{
    public interface IDownloadChecker
    {
        DownloadCheckResult CheckDownload(WebResponse response);

        DownloadCheckResult CheckDownload(Uri url, IWebRequestBuilder requestBuilder);
    }
}
