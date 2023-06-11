using System;
using System.Collections.Generic;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Download;

namespace Toqe.Downloader.Business.DownloadBuilder
{
    public class MultiPartDownloadBuilder : IDownloadBuilder
    {
        private readonly int numberOfParts;

        private readonly IDownloadBuilder downloadBuilder;

        private readonly IWebRequestBuilder requestBuilder;

        private readonly IDownloadChecker downloadChecker;

        private readonly List<DownloadRange> alreadyDownloadedRanges;

        public MultiPartDownloadBuilder(
            int numberOfParts,
            IDownloadBuilder downloadBuilder,
            IWebRequestBuilder requestBuilder,
            IDownloadChecker downloadChecker,
            List<DownloadRange> alreadyDownloadedRanges)
        {
            if (numberOfParts <= 0)
                throw new ArgumentException("numberOfParts <= 0");
            this.numberOfParts = numberOfParts;
            this.downloadBuilder = downloadBuilder ?? throw new ArgumentNullException(nameof(downloadBuilder));
            this.requestBuilder = requestBuilder ?? throw new ArgumentNullException(nameof(requestBuilder));
            this.downloadChecker = downloadChecker ?? throw new ArgumentNullException(nameof(downloadChecker));
            this.alreadyDownloadedRanges = alreadyDownloadedRanges ?? new List<DownloadRange>();
        }

        public IDownload Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new MultiPartDownload(url, bufferSize, numberOfParts, downloadBuilder, requestBuilder, downloadChecker, alreadyDownloadedRanges);
        }
    }
}