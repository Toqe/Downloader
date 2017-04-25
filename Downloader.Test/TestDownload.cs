using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Contract.Events;
using Toqe.Downloader.Business.Download;

namespace Downloader.Test
{
    public class TestDownload : AbstractDownload
    {
        public TestDownload(Uri url, int bufferSize)
            : base(url, bufferSize, null, null, null, null)
        {
        }

        public void OnDataReceived(byte[] data, long offset, int count)
        {
            this.OnDataReceived(new DownloadDataReceivedEventArgs(this, data, offset, count));
        }

        public void OnDownloadStarted(DownloadCheckResult checkResult)
        {
            this.OnDownloadStarted(new DownloadStartedEventArgs(this, checkResult));
        }

        public void OnDownloadCompleted()
        {
            this.OnDownloadCompleted(new DownloadEventArgs(this));
        }

        public void OnDownloadStopped()
        {
            this.OnDownloadStopped(new DownloadEventArgs(this));
        }

        public void OnDownloadCancelled(Exception ex)
        {
            this.OnDownloadCancelled(new DownloadCancelledEventArgs(this, ex));
        }
    }
}
