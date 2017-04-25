using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Download;

namespace Toqe.Downloader.Business.DownloadBuilder
{
    public class SimpleDownloadBuilder : IDownloadBuilder
    {
        private readonly IWebRequestBuilder requestBuilder;

        private readonly IDownloadChecker downloadChecker;

        public SimpleDownloadBuilder(IWebRequestBuilder requestBuilder, IDownloadChecker downloadChecker)
        {
            if (requestBuilder == null)
                throw new ArgumentNullException("requestBuilder");

            if (downloadChecker == null)
                throw new ArgumentNullException("downloadChecker");

            this.requestBuilder = requestBuilder;
            this.downloadChecker = downloadChecker;
        }

        public IDownload Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            return new SimpleDownload(url, bufferSize, offset, maxReadBytes, this.requestBuilder, this.downloadChecker);
        }
    }
}