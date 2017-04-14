using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Contract.Events;

namespace Toqe.Downloader.Business.Observer
{
    public class DownloadProgressMonitor : AbstractDownloadObserver
    {
        private readonly Dictionary<IDownload, long> downloadSizes = new Dictionary<IDownload, long>();

        private readonly Dictionary<IDownload, long> alreadyDownloadedSizes = new Dictionary<IDownload, long>();

        public float GetCurrentProgressPercentage(IDownload download)
        {
            lock (this.monitor)
            {
                if (!downloadSizes.ContainsKey(download) || !alreadyDownloadedSizes.ContainsKey(download) || downloadSizes[download] <= 0)
                {
                    return 0;
                }

                return (float)alreadyDownloadedSizes[download] / downloadSizes[download];
            }
        }

        public long GetCurrentProgressInBytes(IDownload download)
        {
            lock (this.monitor)
            {
                if (!alreadyDownloadedSizes.ContainsKey(download))
                {
                    return 0;
                }

                return alreadyDownloadedSizes[download];
            }
        }

        public long GetTotalFilesizeInBytes(IDownload download)
        {
            lock (this.monitor)
            {
                if (!downloadSizes.ContainsKey(download) || downloadSizes[download] <= 0)
                {
                    return 0;
                }

                return downloadSizes[download];
            }
        }

        protected override void OnAttach(IDownload download)
        {
            download.DownloadStarted += OnDownloadStarted;
            download.DataReceived += OnDownloadDataReceived;
            download.DownloadCompleted += OnDownloadCompleted;
        }

        protected override void OnDetach(IDownload download)
        {
            download.DownloadStarted -= OnDownloadStarted;
            download.DataReceived -= OnDownloadDataReceived;
            download.DownloadCompleted -= OnDownloadCompleted;

            lock (this.monitor)
            {
                if (this.downloadSizes.ContainsKey(download))
                {
                    this.downloadSizes.Remove(download);
                }

                if (this.alreadyDownloadedSizes.ContainsKey(download))
                {
                    this.alreadyDownloadedSizes.Remove(download);
                }
            }
        }

        private void OnDownloadStarted(DownloadStartedEventArgs args)
        {
            lock (this.monitor)
            {
                this.downloadSizes[args.Download] = args.CheckResult.Size;
                this.alreadyDownloadedSizes[args.Download] = args.AlreadyDownloadedSize;
            }
        }

        private void OnDownloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            lock (this.monitor)
            {
                if (!alreadyDownloadedSizes.ContainsKey(args.Download))
                {
                    this.alreadyDownloadedSizes[args.Download] = 0;
                }

                this.alreadyDownloadedSizes[args.Download] += args.Count;
            }
        }

        private void OnDownloadCompleted(DownloadEventArgs args)
        {
            lock (this.monitor)
            {
                this.alreadyDownloadedSizes[args.Download] = this.downloadSizes[args.Download];
            }
        }
    }
}