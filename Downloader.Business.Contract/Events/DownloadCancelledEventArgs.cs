using System;

namespace Toqe.Downloader.Business.Contract.Events
{
    public class DownloadCancelledEventArgs : DownloadEventArgs
    {
        public DownloadCancelledEventArgs()
        {
        }

        public DownloadCancelledEventArgs(IDownload download, Exception exception)
        {
            Download = download;
            Exception = exception;
        }

        public Exception Exception { get; set; }
    }
}
