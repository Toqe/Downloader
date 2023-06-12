using System.Collections.Generic;
using Toqe.Downloader.Contract;
using Toqe.Downloader.Contract.Events;

namespace Toqe.Downloader.Observer
{
    public class DownloadProgressMonitor : AbstractDownloadObserver
    {
        private readonly Dictionary<IDownloader, long> downloadSizes = new Dictionary<IDownloader, long>();

        private readonly Dictionary<IDownloader, long> alreadyDownloadedSizes = new Dictionary<IDownloader, long>();

        public float GetCurrentProgressPercentage(IDownloader download)
        {
            lock (monitor)
            {
                if (!downloadSizes.ContainsKey(download) || !alreadyDownloadedSizes.ContainsKey(download) || downloadSizes[download] <= 0)
                {
                    return 0;
                }

                return (float)alreadyDownloadedSizes[download] / downloadSizes[download];
            }
        }

        public long GetCurrentProgressInBytes(IDownloader download)
        {
            lock (monitor)
            {
                if (!alreadyDownloadedSizes.ContainsKey(download))
                {
                    return 0;
                }

                return alreadyDownloadedSizes[download];
            }
        }

        public long GetTotalFilesizeInBytes(IDownloader download)
        {
            lock (monitor)
            {
                if (!downloadSizes.ContainsKey(download) || downloadSizes[download] <= 0)
                {
                    return 0;
                }

                return downloadSizes[download];
            }
        }

        protected override void OnAttach(IDownloader download)
        {
            download.DownloadStarted += OnDownloadStarted;
            download.DataReceived += OnDownloadDataReceived;
            download.DownloadCompleted += OnDownloadCompleted;
        }

        protected override void OnDetach(IDownloader download)
        {
            download.DownloadStarted -= OnDownloadStarted;
            download.DataReceived -= OnDownloadDataReceived;
            download.DownloadCompleted -= OnDownloadCompleted;

            lock (monitor)
            {
                if (downloadSizes.ContainsKey(download))
                {
                    downloadSizes.Remove(download);
                }

                if (alreadyDownloadedSizes.ContainsKey(download))
                {
                    alreadyDownloadedSizes.Remove(download);
                }
            }
        }

        private void OnDownloadStarted(DownloadStartedEventArgs args)
        {
            lock (monitor)
            {
                downloadSizes[args.Download] = args.CheckResult.Size;
                alreadyDownloadedSizes[args.Download] = args.AlreadyDownloadedSize;
            }
        }

        private void OnDownloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            lock (monitor)
            {
                if (!alreadyDownloadedSizes.ContainsKey(args.Download))
                {
                    alreadyDownloadedSizes[args.Download] = 0;
                }

                alreadyDownloadedSizes[args.Download] += args.Count;
            }
        }

        private void OnDownloadCompleted(DownloadEventArgs args)
        {
            lock (monitor)
            {
                alreadyDownloadedSizes[args.Download] = downloadSizes[args.Download];
            }
        }
    }
}