using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;

namespace Downloader.Test
{
    public class TestDownloadBuilder : IDownloadBuilder
    {
        public TestDownloadBuilder()
        {
            this.ReturnedDownloads = new List<TestDownload>();
        }

        public List<TestDownload> ReturnedDownloads { get; set; }

        public IDownload Build(Uri url, int bufferSize, long? offset, long? maxReadBytes)
        {
            var download = new TestDownload(url, bufferSize);
            this.ReturnedDownloads.Add(download);
            return download;
        }
    }
}
