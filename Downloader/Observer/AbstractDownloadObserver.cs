using System;
using System.Collections.Generic;
using Toqe.Downloader.Contract;

namespace Toqe.Downloader.Observer
{
    public abstract class AbstractDownloadObserver : IDownloadObserver, IDisposable
    {
        protected List<IDownloader> attachedDownloads = new List<IDownloader>();

        protected object monitor = new object();

        public void Attach(IDownloader download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            lock (monitor)
            {
                attachedDownloads.Add(download);
            }

            OnAttach(download);
        }

        public void Detach(IDownloader download)
        {
            lock (monitor)
            {
                attachedDownloads.Remove(download);
            }

            OnDetach(download);
        }

        public void DetachAll()
        {
            List<IDownloader> downloadsCopy;

            lock (monitor)
            {
                downloadsCopy = new List<IDownloader>(attachedDownloads);
            }

            foreach (var download in downloadsCopy)
            {
                Detach(download);
            }
        }

        public virtual void Dispose()
        {
            DetachAll();
        }

        protected virtual void OnAttach(IDownloader download)
        {
        }

        protected virtual void OnDetach(IDownloader download)
        {
        }
    }
}