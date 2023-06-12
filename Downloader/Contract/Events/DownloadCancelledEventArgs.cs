using System;

namespace Toqe.Downloader.Contract.Events
{
    public class DownloadCancelledEventArgs : DownloadEventArgs
    {
        public DownloadCancelledEventArgs()
        {
        }

        public DownloadCancelledEventArgs(IDownloader download, Exception exception)
        {
            Download = download;
            Exception = exception;
        }

        public Exception Exception { get; set; }
    }
}
