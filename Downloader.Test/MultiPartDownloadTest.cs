using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract.Events;
using Toqe.Downloader.Business.DownloadBuilder;
using Xunit;

namespace Downloader.Test
{
    public class MultiPartDownloadTest
    {
        private static readonly Uri url = new Uri("http://test.com");

        private static readonly int bufferSize = 4096;

        private static readonly int numberOfParts = 4;

        [Fact]
        public void TestMultiPartDownloadListsDuringDownload()
        {
            var dlBuilder = new TestDownloadBuilder();
            var requestBuilder = new TestWebRequestBuilder();
            var dlChecker = new TestDownloadChecker();
            var mpdlBuilder = new MultiPartDownloadBuilder(numberOfParts, dlBuilder, requestBuilder, dlChecker, null);
            var dl = mpdlBuilder.Build(url, bufferSize, null, null);

            var dataReceivedList = new List<DownloadDataReceivedEventArgs>();
            var downloadStartedList = new List<DownloadStartedEventArgs>();
            var downloadCompletedList = new List<DownloadEventArgs>();
            var downloadStoppedList = new List<DownloadEventArgs>();
            var downloadCancelledList = new List<DownloadCancelledEventArgs>();

            // TODO: Register events and add args to list, if handler is called

            dl.Start();

            // TODO: wait for download to build up

            // TODO: simulate download parts and check for correct results
        }
    }
}
