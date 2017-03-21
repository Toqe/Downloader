using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Download;
using Toqe.Downloader.Business.DownloadBuilder;
using Toqe.Downloader.Business.Observer;
using Toqe.Downloader.Business.Utils;

namespace Downloader.Example
{
    public class Program
    {
        public static void Main()
        {
            var url = new Uri("https://raw.githubusercontent.com/Toqe/Downloader/master/README.md");
            var file = new System.IO.FileInfo("README.md");
            var requestBuilder = new SimpleWebRequestBuilder();
            var dlChecker = new DownloadChecker();
            var httpDlBuilder = new SimpleDownloadBuilder(requestBuilder, dlChecker);
            var timeForHeartbeat = 3000;
            var timeToRetry = 5000;
            var maxRetries = 5;
            var rdlBuilder = new ResumingDownloadBuilder(timeForHeartbeat, timeToRetry, maxRetries, httpDlBuilder);
            List<DownloadRange> alreadyDownloadedRanges = null;
            var speedMonitor = new DownloadSpeedMonitor(maxSampleCount: 32);
            var bufferSize = 4096;
            var numberOfParts = 4;
            var download = new MultiPartDownload(url, bufferSize, numberOfParts, rdlBuilder, requestBuilder, dlChecker, alreadyDownloadedRanges);
            speedMonitor.Attach(download);
            download.DownloadCompleted += (args) => Console.WriteLine("download has finished!");
            var dlSaver = new DownloadToFileSaver(file);
            dlSaver.Attach(download);
            download.Start();
        }
    }
}