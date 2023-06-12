using System;
using Myitian.Downloader.Contract;
using Myitian.Downloader.Download;

namespace Myitian.Downloader.DownloadBuilder
{
    public class SimpleDownloadBuilder : IDownloadBuilder
    {
        private readonly IWebRequestBuilder requestBuilder;

        private readonly IDownloadChecker downloadChecker;

        public SimpleDownloadBuilder(IWebRequestBuilder requestBuilder, IDownloadChecker downloadChecker)
        {
            this.requestBuilder = requestBuilder ?? throw new ArgumentNullException(nameof(requestBuilder));
            this.downloadChecker = downloadChecker ?? throw new ArgumentNullException(nameof(downloadChecker));
        }

        public IDownloader Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new SimpleDownloader(url, bufferSize, offset, maxReadBytes, requestBuilder, downloadChecker);
        }
    }
}