using System;
using System.Threading;
using Toqe.Downloader.Contract;
using Toqe.Downloader.Contract.Enums;
using Toqe.Downloader.Contract.Events;
using Toqe.Downloader.Contract.Exceptions;

namespace Toqe.Downloader.Download
{
    public class ResumingDownloader : AbstractDownloader
    {
        private readonly int timeForHeartbeat;

        private readonly int timeToRetry;

        private readonly int? maxRetries;

        private readonly IDownloadBuilder downloadBuilder;

        private bool downloadStartedNotified;

        private long currentOffset;

        private long sumOfBytesRead;

        private IDownloader currentDownload;

        private DateTime lastHeartbeat;

        private int currentRetry = 0;

        public ResumingDownloader(Uri url, int bufferSize, long? offset, long? maxReadBytes, int timeForHeartbeat, int timeToRetry, int? maxRetries, IDownloadBuilder downloadBuilder)
            : base(url, bufferSize, offset, maxReadBytes, null, null)
        {
            if (timeForHeartbeat <= 0)
                throw new ArgumentException("timeForHeartbeat <= 0");

            if (timeToRetry <= 0)
                throw new ArgumentException("timeToRetry <= 0");
            this.timeForHeartbeat = timeForHeartbeat;
            this.timeToRetry = timeToRetry;
            this.maxRetries = maxRetries;
            this.downloadBuilder = downloadBuilder ?? throw new ArgumentNullException("downloadBuilder");
        }

        protected override void OnStart()
        {
            StartThread(StartDownload, string.Format("ResumingDownload offset {0} length {1} Main", offset, maxReadBytes));
            StartThread(CheckHeartbeat, string.Format("ResumingDownload offset {0} length {1} Heartbeat", offset, maxReadBytes));
        }

        protected override void OnStop()
        {
            lock (monitor)
            {
                stopping = true;
                DoStopIfNecessary();
            }
        }

        private void StartDownload()
        {
            lock (monitor)
            {
                StartNewDownload();
            }
        }

        private void StartNewDownload()
        {
            currentOffset = offset ?? 0;
            BuildDownload();
        }

        private void CheckHeartbeat()
        {
            while (true)
            {
                Thread.Sleep(timeForHeartbeat);

                lock (monitor)
                {
                    if (DoStopIfNecessary())
                    {
                        return;
                    }

                    if (DateTime.Now - lastHeartbeat > TimeSpan.FromMilliseconds(timeForHeartbeat))
                    {
                        CountRetryAndCancelIfMaxRetriesReached();

                        if (currentDownload != null)
                        {
                            CloseDownload();
                            StartThread(BuildDownload, Thread.CurrentThread.Name + "-byHeartbeat");
                        }
                    }
                }
            }
        }

        private void CountRetryAndCancelIfMaxRetriesReached()
        {
            if (maxRetries.HasValue && currentRetry >= maxRetries)
            {
                state = DownloadState.Cancelled;
                OnDownloadCancelled(new DownloadCancelledEventArgs(this, new TooManyRetriesException()));
                DoStop(DownloadStopType.WithoutNotification);
            }

            currentRetry++;
        }

        private void BuildDownload()
        {
            lock (monitor)
            {
                if (DoStopIfNecessary())
                {
                    return;
                }

                long? currentMaxReadBytes = maxReadBytes.HasValue ? (long?)maxReadBytes.Value - sumOfBytesRead : null;

                currentDownload = downloadBuilder.Build(url, bufferSize, currentOffset, currentMaxReadBytes);
                currentDownload.DownloadStarted += downloadStarted;
                currentDownload.DownloadCancelled += downloadCancelled;
                currentDownload.DownloadCompleted += downloadCompleted;
                currentDownload.DataReceived += downloadDataReceived;
                StartThread(currentDownload.Start, Thread.CurrentThread.Name + "-buildDownload");
            }
        }

        private bool DoStopIfNecessary()
        {
            if (stopping)
            {
                CloseDownload();

                lock (monitor)
                {
                    state = DownloadState.Stopped;
                }
            }

            return stopping;
        }

        private void SleepThenBuildDownload()
        {
            Thread.Sleep(timeToRetry);
            BuildDownload();
        }

        private void CloseDownload()
        {
            if (currentDownload != null)
            {
                currentDownload.DetachAllHandlers();
                currentDownload.Stop();
                currentDownload = null;
            }
        }

        private void downloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            var download = args.Download;
            var count = args.Count;
            var data = args.Data;
            long previousOffset = 0;

            lock (monitor)
            {
                if (currentDownload == download)
                {
                    if (DoStopIfNecessary())
                    {
                        return;
                    }

                    previousOffset = currentOffset;

                    lastHeartbeat = DateTime.Now;
                    currentOffset += count;
                    sumOfBytesRead += count;
                }
            }

            OnDataReceived(new DownloadDataReceivedEventArgs(this, data, previousOffset, count));
        }

        private void downloadStarted(DownloadStartedEventArgs args)
        {
            var download = args.Download;
            bool shouldNotifyDownloadStarted = false;

            lock (monitor)
            {
                if (download == currentDownload)
                {
                    if (!downloadStartedNotified)
                    {
                        shouldNotifyDownloadStarted = true;
                        downloadStartedNotified = true;
                    }
                }
            }

            if (shouldNotifyDownloadStarted)
            {
                OnDownloadStarted(new DownloadStartedEventArgs(this, args.CheckResult, args.AlreadyDownloadedSize));
            }
        }

        private void downloadCompleted(DownloadEventArgs args)
        {
            lock (monitor)
            {
                CloseDownload();
                state = DownloadState.Finished;
                stopping = true;
            }

            OnDownloadCompleted(new DownloadEventArgs(this));
        }

        private void downloadCancelled(DownloadCancelledEventArgs args)
        {
            var download = args.Download;

            lock (monitor)
            {
                if (download == currentDownload)
                {
                    CountRetryAndCancelIfMaxRetriesReached();

                    if (currentDownload != null)
                    {
                        currentDownload = null;
                        StartThread(SleepThenBuildDownload, Thread.CurrentThread.Name + "-afterCancel");
                    }
                }
            }
        }
    }
}