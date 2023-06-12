namespace Myitian.Downloader.Contract.Events
{
    public class DownloadEventArgs
    {
        public IDownloader Download { get; set; }

        public DownloadEventArgs()
        {
        }

        public DownloadEventArgs(IDownloader download)
        {
            Download = download;
        }
    }
}
