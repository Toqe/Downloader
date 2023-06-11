namespace Toqe.Downloader.Business.Contract.Events
{
    public class DownloadStartedEventArgs : DownloadEventArgs
    {
        public DownloadStartedEventArgs()
        {
        }

        public DownloadStartedEventArgs(IDownload download, DownloadCheckResult checkResult, long alreadyDownloadedSize = 0)
        {
            Download = download;
            CheckResult = checkResult;
            AlreadyDownloadedSize = alreadyDownloadedSize;
        }

        public DownloadCheckResult CheckResult { get; set; }

        public long AlreadyDownloadedSize { get; set; }
    }
}
