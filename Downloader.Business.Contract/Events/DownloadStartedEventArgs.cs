using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toqe.Downloader.Business.Contract.Events
{
    public class DownloadStartedEventArgs : DownloadEventArgs
    {
        public DownloadStartedEventArgs()
        {
        }

        public DownloadStartedEventArgs(IDownload download, DownloadCheckResult checkResult)
        {
            this.Download = download;
            this.CheckResult = checkResult;
        }

        public DownloadCheckResult CheckResult { get; set; }
    }
}
