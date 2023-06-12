using System;
using Myitian.Downloader.Contract;
using Myitian.Downloader.Download;

namespace Myitian.Downloader.DownloadBuilder
{
    public class ResumingDownloadBuilder : IDownloadBuilder
    {
        private readonly int timeForHeartbeat;

        private readonly int timeToRetry;

        private readonly int? maxRetries;

        private readonly IDownloadBuilder downloadBuilder;

        public ResumingDownloadBuilder(int timeForHeartbeat, int timeToRetry, int? maxRetries, IDownloadBuilder downloadBuilder)
        {
            if (timeForHeartbeat <= 0)
                throw new ArgumentException("timeForHeartbeat <= 0");

            if (timeToRetry <= 0)
                throw new ArgumentException("timeToRetry <= 0");
            this.timeForHeartbeat = timeForHeartbeat;
            this.timeToRetry = timeToRetry;
            this.maxRetries = maxRetries;
            this.downloadBuilder = downloadBuilder ?? throw new ArgumentNullException(nameof(downloadBuilder));
        }

        public IDownloader Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new ResumingDownloader(url, bufferSize, offset, maxReadBytes, timeForHeartbeat, timeToRetry, maxRetries, downloadBuilder);
        }
    }
}