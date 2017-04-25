using System;
using System.Collections.Generic;
using System.Text;

namespace Toqe.Downloader.Business.Contract
{
    public class DownloadCheckResult
    {
        public bool Success { get; set; }

        public long Size { get; set; }

        public int? StatusCode { get; set; }

        public bool SupportsResume { get; set; }

        public Exception Exception { get; set; }
    }
}