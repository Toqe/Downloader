using System;
using System.Collections.Generic;
using System.Text;

namespace Toqe.Downloader.Business.Contract
{
    public class DownloadCheckResult
    {
        public int Size { get; set; }

        public bool SupportsResume { get; set; }

        public Exception Exception { get; set; }
    }
}