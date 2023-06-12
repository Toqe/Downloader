using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Myitian.Downloader.Contract;
using Myitian.Downloader.Contract.Events;
using Myitian.Downloader.Download;
using Myitian.Downloader.DownloadBuilder;
using Myitian.Downloader.Observer;
using Myitian.Downloader.Utils;

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
            var uri = new Uri(Console.ReadLine());
            var filepath = Console.ReadLine();
            var file = new FileInfo(string.IsNullOrWhiteSpace(filepath) ? Path.GetFileName(uri.LocalPath) : filepath);

            Console.WriteLine();
            Console.WriteLine("Download " + uri);
            Console.WriteLine("Save to " + file.FullName);

            var requestBuilder = new SimpleWebRequestBuilder();
            var dlChecker = new DownloadChecker();
            var httpDlBuilder = new SimpleDownloadBuilder(requestBuilder, dlChecker);
            var timeForHeartbeat = 3000;
            var timeToRetry = 2000;
            var maxRetries = 10;
            var resumingDlBuilder = new ResumingDownloadBuilder(timeForHeartbeat, timeToRetry, maxRetries, httpDlBuilder);
            var alreadyDownloadedRanges = new List<DownloadRange>();
            var bufferSize = 4096;
            var numberOfParts = 4;
            using var downloader = new MultiPartDownloader(uri, bufferSize, numberOfParts, resumingDlBuilder, requestBuilder, dlChecker, alreadyDownloadedRanges);
            var speedMonitor = new DownloadSpeedMonitor(maxSampleCount: 128);
            speedMonitor.Attach(downloader);
            var progressMonitor = new DownloadProgressMonitor();
            progressMonitor.Attach(downloader);

            if (useDownloadSpeedThrottling)
            {
                var downloadThrottling = new DownloadThrottling(maxBytesPerSecond: 200 * 1024, maxSampleCount: 128);
                downloadThrottling.Attach(downloader);
            }

            Console.WriteLine("\r\nDownload has started!");

            var dlSaver = new DownloadToFileSaver(file);
            dlSaver.Attach(downloader);
            downloader.DownloadCompleted += OnCompleted;
            downloader.Start();

            while (!finished)
            {
                Thread.Sleep(200);

                var alreadyDownloadedSizeInBytes = progressMonitor.GetCurrentProgressInBytes(downloader);
                var totalDownloadSizeInBytes = progressMonitor.GetTotalFilesizeInBytes(downloader);
                var currentSpeedInBytesPerSecond = speedMonitor.GetCurrentBytesPerSecond();

                var currentProgressInPercent = progressMonitor.GetCurrentProgressPercentage(downloader) * 100;
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