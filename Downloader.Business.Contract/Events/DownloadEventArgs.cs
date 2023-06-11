namespace Toqe.Downloader.Business.Contract.Events
{
    public class DownloadEventArgs
    {
        public IDownload Download { get; set; }

        public DownloadEventArgs()
        {
        }

        public DownloadEventArgs(IDownload download)
        {
            Download = download;
        }
    }
}
