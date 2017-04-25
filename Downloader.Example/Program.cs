using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Contract.Events;
using Toqe.Downloader.Business.Download;
using Toqe.Downloader.Business.DownloadBuilder;
using Toqe.Downloader.Business.Observer;
using Toqe.Downloader.Business.Utils;

namespace Downloader.Example
{
    public class Program
    {
        static bool finished = false;

        public static void Main()
        {
            // Please insert an URL of a large file here, otherwise the download will be finished too quickly to really demonstrate the functionality.
            var url = new Uri("http://speedtest.tele2.net/10GB.zip");
            var file = new System.IO.FileInfo("README.md");
            var requestBuilder = new SimpleWebRequestBuilder();
            var dlChecker = new DownloadChecker();
            var httpDlBuilder = new SimpleDownloadBuilder(requestBuilder, dlChecker);
            var timeForHeartbeat = 3000;
            var timeToRetry = 5000;
            var maxRetries = 5;
            var resumingDlBuilder = new ResumingDownloadBuilder(timeForHeartbeat, timeToRetry, maxRetries, httpDlBuilder);
            List<DownloadRange> alreadyDownloadedRanges = null;
            var bufferSize = 4096;
            var numberOfParts = 4;
            var download = new MultiPartDownload(url, bufferSize, numberOfParts, resumingDlBuilder, requestBuilder, dlChecker, alreadyDownloadedRanges);
            var speedMonitor = new DownloadSpeedMonitor(maxSampleCount: 32);
            speedMonitor.Attach(download);
            var progressMonitor = new DownloadProgressMonitor();
            progressMonitor.Attach(download);
            var dlSaver = new DownloadToFileSaver(file);
            dlSaver.Attach(download);
            download.DownloadCompleted += OnCompleted;
            download.Start();

            while (!finished)
            {
                Thread.Sleep(1000);

                var percent = progressMonitor.GetCurrentProgressPercentage(download) * 100;
                var alreadyDownloadedSize = (progressMonitor.GetCurrentProgressInBytes(download) / 1024);
                var totalDownloadSize = (progressMonitor.GetTotalFilesizeInBytes(download) / 1024);
                var speed = (speedMonitor.GetCurrentBytesPerSecond() / 1024);
                var remainingTime = (progressMonitor.GetTotalFilesizeInBytes(download) - progressMonitor.GetCurrentProgressInBytes(download)) / speedMonitor.GetCurrentBytesPerSecond();

                Console.WriteLine(
                    "Progress: " + percent + "% " + "(" + alreadyDownloadedSize + " of " + totalDownloadSize + " KiB)" +
                    "   Speed: " + speed + " KiB/sec." +
                    "   Remaining time: " + remainingTime + " sec.");
            }
        }

        static void OnCompleted(DownloadEventArgs args)
        {
            // this is an important thing to do after a download isn't used anymore, otherwise you will run into a memory leak.
            args.Download.DetachAllHandlers();
            Console.WriteLine("Download has finished!");
            finished = true;
        }
    }
}