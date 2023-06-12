namespace Toqe.Downloader.Contract
{
    public interface IDownloadObserver
    {
        void Attach(IDownloader download);

        void Detach(IDownloader download);

        void DetachAll();
    }
}