namespace Myitian.Downloader.Contract.Events
{
    public class DownloadStartedEventArgs : DownloadEventArgs
    {
        public DownloadStartedEventArgs()
        {
        }

        public DownloadStartedEventArgs(IDownloader download, DownloadCheckResult checkResult, long alreadyDownloadedSize = 0)
        {
            Download = download;
            CheckResult = checkResult;
            AlreadyDownloadedSize = alreadyDownloadedSize;
        }

        public DownloadCheckResult CheckResult { get; set; }

        public long AlreadyDownloadedSize { get; set; }
    }
}
