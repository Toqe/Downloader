using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                throw new ArgumentNullException("download");

            lock (this.monitor)
            {
                this.attachedDownloads.Add(download);
            }

            this.OnAttach(download);
        }

        public void Detach(IDownload download)
        {
            lock (this.monitor)
            {
                this.attachedDownloads.Remove(download);
            }

            this.OnDetach(download);
        }

        public void DetachAll()
        {
            List<IDownload> downloadsCopy;

            lock (this.monitor)
            {
                downloadsCopy = new List<IDownload>(this.attachedDownloads);
            }

            foreach (var download in downloadsCopy)
            {
                this.Detach(download);
            }
        }

        public virtual void Dispose()
        {
            this.DetachAll();
        }

        protected virtual void OnAttach(IDownload download)
        {
        }

        protected virtual void OnDetach(IDownload download)
        {
        }
    }
}