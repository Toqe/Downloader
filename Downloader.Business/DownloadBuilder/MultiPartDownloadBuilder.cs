using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            if (downloadBuilder == null)
                throw new ArgumentNullException("downloadBuilder");

            if (requestBuilder == null)
                throw new ArgumentNullException("requestBuilder");

            if (downloadChecker == null)
                throw new ArgumentNullException("downloadChecker");

            this.numberOfParts = numberOfParts;
            this.downloadBuilder = downloadBuilder;
            this.requestBuilder = requestBuilder;
            this.downloadChecker = downloadChecker;
            this.alreadyDownloadedRanges = alreadyDownloadedRanges ?? new List<DownloadRange>();
        }

        public IDownload Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new MultiPartDownload(url, bufferSize, this.numberOfParts, this.downloadBuilder, this.requestBuilder, this.downloadChecker, this.alreadyDownloadedRanges);
        }
    }
}