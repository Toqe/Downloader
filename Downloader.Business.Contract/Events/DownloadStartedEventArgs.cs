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

        public DownloadStartedEventArgs(IDownload download, DownloadCheckResult checkResult, int alreadyDownloadedSize = 0)
        {
            this.Download = download;
            this.CheckResult = checkResult;
            this.AlreadyDownloadedSize = alreadyDownloadedSize;
        }

        public DownloadCheckResult CheckResult { get; set; }

        public int AlreadyDownloadedSize { get; set; }
    }
}
