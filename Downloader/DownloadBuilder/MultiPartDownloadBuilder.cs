using System;
using System.Collections.Generic;
using Myitian.Downloader.Contract;
using Myitian.Downloader.Download;

namespace Myitian.Downloader.DownloadBuilder
{
    public class MultiPartDownloadBuilder : IDownloadBuilder
    {
        public readonly int NumberOfParts;

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
            this.NumberOfParts = numberOfParts;
            this.downloadBuilder = downloadBuilder ?? throw new ArgumentNullException(nameof(downloadBuilder));
            this.requestBuilder = requestBuilder ?? throw new ArgumentNullException(nameof(requestBuilder));
            this.downloadChecker = downloadChecker ?? throw new ArgumentNullException(nameof(downloadChecker));
            this.alreadyDownloadedRanges = alreadyDownloadedRanges ?? new List<DownloadRange>();
        }

        public IDownloader Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new MultiPartDownloader(url, bufferSize, NumberOfParts, downloadBuilder, requestBuilder, downloadChecker, alreadyDownloadedRanges);
        }
    }
}