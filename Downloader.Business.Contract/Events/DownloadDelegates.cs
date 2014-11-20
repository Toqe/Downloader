using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toqe.Downloader.Business.Contract.Events
{
    public class DownloadDelegates
    {
        public delegate void DownloadDataReceivedHandler(DownloadDataReceivedEventArgs args);

        public delegate void DownloadStartedHandler(DownloadStartedEventArgs args);

        public delegate void DownloadCompletedHandler(DownloadEventArgs args);

        public delegate void DownloadStoppedHandler(DownloadEventArgs args);

        public delegate void DownloadCancelledHandler(DownloadCancelledEventArgs args);

        public delegate void VoidAction();
    }
}