using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Download;

namespace Toqe.Downloader.Business.DownloadBuilder
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

            if (downloadBuilder == null)
                throw new ArgumentNullException("downloadBuilder");

            this.timeForHeartbeat = timeForHeartbeat;
            this.timeToRetry = timeToRetry;
            this.maxRetries = maxRetries;
            this.downloadBuilder = downloadBuilder;
        }

        public IDownload Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new ResumingDownload(url, bufferSize, offset, maxReadBytes, this.timeForHeartbeat, this.timeToRetry, this.maxRetries, this.downloadBuilder);
        }
    }
}