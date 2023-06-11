using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Contract.Events;

namespace Toqe.Downloader.Business.Observer
{
    public class DownloadThrottling : IDownloadObserver, IDisposable
    {
        private readonly int maxBytesPerSecond;

        private readonly List<IDownload> downloads = new List<IDownload>();

        private readonly int maxSampleCount;

        private List<DownloadDataSample> samples;

        private double floatingWaitingTimeInMilliseconds = 0;

        private object monitor = new object();

        public DownloadThrottling(int maxBytesPerSecond, int maxSampleCount)
        {
            if (maxBytesPerSecond <= 0)
                throw new ArgumentException("maxBytesPerSecond <= 0");

            if (maxSampleCount < 2)
                throw new ArgumentException("sampleCount < 2");

            this.maxBytesPerSecond = maxBytesPerSecond;
            this.maxSampleCount = maxSampleCount;
            samples = new List<DownloadDataSample>();
        }

        public void Attach(IDownload download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            download.DataReceived += downloadDataReceived;

            lock (monitor)
            {
                downloads.Add(download);
            }
        }

        public void Detach(IDownload download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            download.DataReceived -= downloadDataReceived;

            lock (monitor)
            {
                downloads.Remove(download);
            }
        }

        public void DetachAll()
        {
            lock (monitor)
            {
                foreach (var download in downloads)
                {
                    Detach(download);
                }
            }
        }

        public void Dispose()
        {
            DetachAll();
        }

        private void AddSample(int count)
        {
            lock (monitor)
            {
                var sample = new DownloadDataSample()
                {
                    Count = count,
                    Timestamp = DateTime.UtcNow
                };

                samples.Add(sample);

                if (samples.Count > maxSampleCount)
                {
                    samples.RemoveAt(0);
                }
            }
        }

        private int CalculateWaitingTime()
        {
            lock (monitor)
            {
                if (samples.Count < 2)
                {
                    return 0;
                }

                var averageBytesPerCall = samples.Average(s => s.Count);
                double sumOfTicksBetweenCalls = 0;

                // 1 tick = 100 nano seconds, 1 ms ^= 10.000 ticks
                for (var i = 0; i < samples.Count - 1; i++)
                {
                    sumOfTicksBetweenCalls += (samples[i + 1].Timestamp - samples[i].Timestamp).Ticks;
                }

                var averageTicksBetweenCalls = sumOfTicksBetweenCalls / (samples.Count - 1);
                var timePerNetworkRequestInMilliseconds = averageTicksBetweenCalls / 10000 - floatingWaitingTimeInMilliseconds;

                var currentBytesPerMillisecond = averageBytesPerCall / averageTicksBetweenCalls * 10000;
                var maxBytesPerMillisecond = (double)maxBytesPerSecond / 1000;

                var waitingTimeInMilliseconds = averageBytesPerCall / maxBytesPerMillisecond - timePerNetworkRequestInMilliseconds;
                floatingWaitingTimeInMilliseconds = waitingTimeInMilliseconds * 0.6 + floatingWaitingTimeInMilliseconds * 0.4;

                return (int)waitingTimeInMilliseconds;
            }
        }

        private void downloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            AddSample(args.Count);
            var waitingTime = CalculateWaitingTime();

            if (waitingTime > 0)
            {
                Thread.Sleep(waitingTime);
            }
        }
    }
}