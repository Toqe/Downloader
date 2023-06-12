using System;
using System.Collections.Generic;
using System.Linq;
using Toqe.Downloader.Contract;
using Toqe.Downloader.Contract.Events;

namespace Toqe.Downloader.Observer
{
    public class DownloadSpeedMonitor : AbstractDownloadObserver
    {
        private readonly int maxSampleCount;

        private readonly List<DownloadDataSample> samples = new List<DownloadDataSample>();

        public DownloadSpeedMonitor(int maxSampleCount)
        {
            if (maxSampleCount < 2)
                throw new ArgumentException("maxSampleCount < 2");

            this.maxSampleCount = maxSampleCount;
        }

        public int GetCurrentBytesPerSecond()
        {
            lock (monitor)
            {
                if (samples.Count < 2)
                {
                    return 0;
                }

                var sumOfBytesFromCalls = samples.Sum(s => s.Count);
                var ticksBetweenCalls = (DateTime.UtcNow - samples[0].Timestamp).Ticks;

                return (int)((double)sumOfBytesFromCalls / ticksBetweenCalls * 10000 * 1000);
            }
        }

        protected override void OnAttach(IDownloader download)
        {
            download.DataReceived += downloadDataReceived;
        }

        protected override void OnDetach(IDownloader download)
        {
            download.DataReceived -= downloadDataReceived;
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

        private void downloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            AddSample(args.Count);
        }
    }
}