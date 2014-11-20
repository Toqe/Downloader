using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;

namespace Downloader.Test
{
    public class TestDownloadChecker : IDownloadChecker
    {
        public DownloadCheckResult CheckDownload(Uri url, IWebRequestBuilder requestBuilder)
        {
            return this.CheckDownload(null);
        }

        public DownloadCheckResult CheckDownload(System.Net.WebResponse response)
        {
            return new DownloadCheckResult() { Size = 4000, SupportsResume = true };
        }
    }
}
