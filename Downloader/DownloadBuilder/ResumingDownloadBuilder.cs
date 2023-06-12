using System;
using Myitian.Downloader.Contract;
using Myitian.Downloader.Download;

namespace Myitian.Downloader.DownloadBuilder
{
    public class ResumingDownloadBuilder : IDownloadBuilder
    {
        public readonly int TimeForHeartbeat;

        public readonly int TimeToRetry;

        public readonly int? MaxRetries;

        private readonly IDownloadBuilder downloadBuilder;

        public ResumingDownloadBuilder(int timeForHeartbeat, int timeToRetry, int? maxRetries, IDownloadBuilder downloadBuilder)
        {
            if (timeForHeartbeat <= 0)
                throw new ArgumentException("timeForHeartbeat <= 0");

            if (timeToRetry <= 0)
                throw new ArgumentException("timeToRetry <= 0");
            this.TimeForHeartbeat = timeForHeartbeat;
            this.TimeToRetry = timeToRetry;
            this.MaxRetries = maxRetries;
            this.downloadBuilder = downloadBuilder ?? throw new ArgumentNullException(nameof(downloadBuilder));
        }

        public IDownloader Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new ResumingDownloader(url, bufferSize, offset, maxReadBytes, TimeForHeartbeat, TimeToRetry, MaxRetries, downloadBuilder);
        }
    }
}