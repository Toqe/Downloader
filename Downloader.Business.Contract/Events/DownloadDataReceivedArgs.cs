namespace Toqe.Downloader.Business.Contract.Events
{
    public class DownloadDataReceivedEventArgs : DownloadEventArgs
    {
        public DownloadDataReceivedEventArgs()
        {
        }

        public DownloadDataReceivedEventArgs(IDownload download, byte[] data, long offset, int count)
        {
            Download = download;
            Data = data;
            Offset = offset;
            Count = count;
        }

        public byte[] Data { get; set; }

        public long Offset { get; set; }

        public int Count { get; set; }
    }
}
