using System;
using System.Collections.Generic;
using System.IO;
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

        static readonly AutoResetEvent done = new(false);
        static readonly AutoResetEvent exit = new(false);

        public static void Main()
        {
            bool useDownloadSpeedThrottling = false;

            // Please insert an URL of a large file here, otherwise the download will be finished too quickly to really demonstrate the functionality.
            var uri = new Uri("https://download.visualstudio.microsoft.com/download/pr/1b55b379-5ef2-4f21-8fad-aba058913cbc/c26ee3ba55cb40407a79564e28ed6d98/dotnet-sdk-8.0.100-preview.4.23260.5-win-x64.exe");
            var file = new FileInfo("dotnet-sdk-8.0.100-preview.4.23260.5-win-x64.exe");

            Console.WriteLine("Download " + uri);
            Console.WriteLine("Save to " + file);

            var requestBuilder = new SimpleWebRequestBuilder();
            var dlChecker = new DownloadChecker();
            var httpDlBuilder = new SimpleDownloadBuilder(requestBuilder, dlChecker);
            var timeForHeartbeat = 3000;
            var timeToRetry = 5000;
            var maxRetries = 5;
            var resumingDlBuilder = new ResumingDownloadBuilder(timeForHeartbeat, timeToRetry, maxRetries, httpDlBuilder);
            var alreadyDownloadedRanges = new List<DownloadRange>();
            var bufferSize = 4096;
            var numberOfParts = 4;
            var download = new MultiPartDownload(uri, bufferSize, numberOfParts, resumingDlBuilder, requestBuilder, dlChecker, alreadyDownloadedRanges);
            var speedMonitor = new DownloadSpeedMonitor(maxSampleCount: 128);
            speedMonitor.Attach(download);
            var progressMonitor = new DownloadProgressMonitor();
            progressMonitor.Attach(download);

            if (useDownloadSpeedThrottling)
            {
                var downloadThrottling = new DownloadThrottling(maxBytesPerSecond: 200 * 1024, maxSampleCount: 128);
                downloadThrottling.Attach(download);
            }

            Console.WriteLine("\r\nDownload has started!");

            var dlSaver = new DownloadToFileSaver(file);
            dlSaver.Attach(download);
            download.DownloadCompleted += OnCompleted;
            download.Start();

            while (!finished)
            {
                Thread.Sleep(200);

                var alreadyDownloadedSizeInBytes = progressMonitor.GetCurrentProgressInBytes(download);
                var totalDownloadSizeInBytes = progressMonitor.GetTotalFilesizeInBytes(download);
                var currentSpeedInBytesPerSecond = speedMonitor.GetCurrentBytesPerSecond();

                var currentProgressInPercent = progressMonitor.GetCurrentProgressPercentage(download) * 100;
                var alreadyDownloadedSizeInKiB = alreadyDownloadedSizeInBytes / 1024;
                var totalDownloadSizeInKiB = totalDownloadSizeInBytes / 1024;
                var currentSpeedInKiBPerSecond = currentSpeedInBytesPerSecond / 1024;
                var remainingTimeInSeconds = currentSpeedInBytesPerSecond == 0 ? 0 : (totalDownloadSizeInBytes - alreadyDownloadedSizeInBytes) / currentSpeedInBytesPerSecond;

                Console.Write($"\r{currentProgressInPercent,6:f2}% ({alreadyDownloadedSizeInKiB,7} of {totalDownloadSizeInKiB,7} KiB) {currentSpeedInKiBPerSecond,7} KiB/sec. ETA: {remainingTimeInSeconds,5} sec.");
            }

            done.Set();
            exit.WaitOne();
            done.Dispose();
            exit.Dispose();
        }

        static void OnCompleted(DownloadEventArgs args)
        {
            finished = true;
            done.WaitOne(); // make sure that the finish message is showed after the last progress message

            // this is an important thing to do after a download isn't used anymore, otherwise you will run into a memory leak.
            args.Download.DetachAllHandlers();

            Console.WriteLine("\r\nDownload has finished!");
            exit.Set();
        }
    }
}