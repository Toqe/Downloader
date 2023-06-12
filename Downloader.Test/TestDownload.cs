using System;
using Myitian.Downloader.Contract;
using Myitian.Downloader.Contract.Events;
using Myitian.Downloader.Download;

namespace Downloader.Test
{
    public class TestDownload : AbstractDownloader
    {
        public TestDownload(Uri url, int bufferSize)
            : base(url, bufferSize, null, null, null, null)
        {
        }

        public void OnDataReceived(byte[] data, long offset, int count)
        {
            OnDataReceived(new DownloadDataReceivedEventArgs(this, data, offset, count));
        }

        public void OnDownloadStarted(DownloadCheckResult checkResult)
        {
            OnDownloadStarted(new DownloadStartedEventArgs(this, checkResult));
        }

        public void OnDownloadCompleted()
        {
            OnDownloadCompleted(new DownloadEventArgs(this));
        }

        public void OnDownloadStopped()
        {
            OnDownloadStopped(new DownloadEventArgs(this));
        }

        public void OnDownloadCancelled(Exception ex)
        {
            OnDownloadCancelled(new DownloadCancelledEventArgs(this, ex));
        }
    }
}
