using System;
using Myitian.Downloader.Contract;

namespace Downloader.Test
{
    public class TestDownloadChecker : IDownloadChecker
    {
        public DownloadCheckResult CheckDownload(Uri url, IWebRequestBuilder requestBuilder)
        {
            return CheckDownload(null);
        }

        public DownloadCheckResult CheckDownload(System.Net.WebResponse response)
        {
            return new DownloadCheckResult() { Size = 4000, SupportsResume = true };
        }
    }
}
