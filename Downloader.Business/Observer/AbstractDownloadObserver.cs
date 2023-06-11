using System;
using System.Collections.Generic;
using Toqe.Downloader.Business.Contract;

namespace Toqe.Downloader.Business.Observer
{
    public abstract class AbstractDownloadObserver : IDownloadObserver, IDisposable
    {
        protected List<IDownload> attachedDownloads = new List<IDownload>();

        protected object monitor = new object();

        public void Attach(IDownload download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            lock (monitor)
            {
                attachedDownloads.Add(download);
            }

            OnAttach(download);
        }

        public void Detach(IDownload download)
        {
            lock (monitor)
            {
                attachedDownloads.Remove(download);
            }

            OnDetach(download);
        }

        public void DetachAll()
        {
            List<IDownload> downloadsCopy;

            lock (monitor)
            {
                downloadsCopy = new List<IDownload>(attachedDownloads);
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

        protected virtual void OnAttach(IDownload download)
        {
        }

        protected virtual void OnDetach(IDownload download)
        {
        }
    }
}