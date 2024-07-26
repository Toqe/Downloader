using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Myitian.Downloader.Contract;
using Myitian.Downloader.Contract.Enums;
using Myitian.Downloader.Contract.Events;
using Myitian.Downloader.Contract.Exceptions;
using Myitian.Downloader.Utils;

namespace Myitian.Downloader.Download
{
    public class MultiPartDownloader : AbstractDownloader
    {
        private readonly DownloadRangeHelper downloadRangeHelper = new DownloadRangeHelper();

        private readonly int numberOfParts;

        private readonly IDownloadBuilder downloadBuilder;

        private readonly Dictionary<IDownloader, DownloadRange> downloads = new Dictionary<IDownloader, DownloadRange>();

        public MultiPartDownloader(
            Uri url,
            int bufferSize,
            int numberOfParts,
            IDownloadBuilder downloadBuilder,
            IWebRequestBuilder requestBuilder,
            IDownloadChecker downloadChecker,
            List<DownloadRange> alreadyDownloadedRanges)
            : base(url, bufferSize, null, null, requestBuilder, downloadChecker)
        {
            if (numberOfParts <= 0)
                throw new ArgumentException("numberOfParts <= 0");
            this.numberOfParts = numberOfParts;
            this.downloadBuilder = downloadBuilder ?? throw new ArgumentNullException(nameof(downloadBuilder));
            AlreadyDownloadedRanges = alreadyDownloadedRanges ?? new List<DownloadRange>();

            if (ServicePointManager.DefaultConnectionLimit < numberOfParts)
            {
                ServicePointManager.DefaultConnectionLimit = numberOfParts;
            }
        }

        public List<DownloadRange> AlreadyDownloadedRanges { get; private set; }

        public List<DownloadRange> ToDoRanges { get; private set; }

        protected override void OnStart()
        {
            var downloadCheck = PerformInitialDownloadCheck();
            DetermineFileSizeAndStartDownloads(downloadCheck);
        }

        protected override void OnStop()
        {
            List<IDownloader> currentDownloads = new List<IDownloader>();

            lock (monitor)
            {
                if (downloads != null && downloads.Count > 0)
                {
                    currentDownloads = new List<IDownloader>(downloads.Keys);
                }
            }

            foreach (var download in currentDownloads)
            {
                download.DetachAllHandlers();
                download.Stop();
            }

            lock (monitor)
            {
                state = DownloadState.Stopped;
            }
        }

        private DownloadCheckResult PerformInitialDownloadCheck()
        {
            var downloadCheck = downloadChecker.CheckDownload(url, requestBuilder);

            if (!downloadCheck.Success)
                throw new DownloadCheckNotSuccessfulException("Download check was not successful. HTTP status code: " + downloadCheck.StatusCode, downloadCheck.Exception, downloadCheck);

            if (!downloadCheck.SupportsResume)
                throw new ResumingNotSupportedException();

            OnDownloadStarted(new DownloadStartedEventArgs(this, downloadCheck, AlreadyDownloadedRanges.Sum(x => x.Length)));

            return downloadCheck;
        }

        private void DetermineFileSizeAndStartDownloads(DownloadCheckResult downloadCheck)
        {
            lock (monitor)
            {
                if (AlreadyDownloadedRanges.Count == 1 && AlreadyDownloadedRanges[0].Length == downloadCheck.Size)
                {
                    state = DownloadState.Finished;
                    OnDownloadCompleted(new DownloadEventArgs(this));
                    return;
                }

                ToDoRanges = DetermineToDoRanges(downloadCheck.Size, AlreadyDownloadedRanges);
                SplitToDoRangesForNumberOfParts();

                for (int i = 0; i < ToDoRanges.Count; i++)
                {
                    var todoRange = ToDoRanges[i];
                    StartDownload(todoRange);
                }
            }
        }

        private void SplitToDoRangesForNumberOfParts()
        {
            while (ToDoRanges.Count < numberOfParts)
            {
                var maxRange = ToDoRanges.FirstOrDefault(r => r.Length == ToDoRanges.Max(r2 => r2.Length));
                if (maxRange == null)
                {
                    return;
                }

                ToDoRanges.Remove(maxRange);
                var range1Start = maxRange.Start;
                var range1Length = maxRange.Length / 2;
                var range2Start = range1Start + range1Length;
                var range2Length = maxRange.End - range2Start + 1;
                ToDoRanges.Add(new DownloadRange(range1Start, range1Length));
                ToDoRanges.Add(new DownloadRange(range2Start, range2Length));
            }
        }

        private void StartDownload(DownloadRange range)
        {
            var download = downloadBuilder.Build(url, bufferSize, range.Start, range.Length);
            download.DataReceived += downloadDataReceived;
            download.DownloadCancelled += downloadCancelled;
            download.DownloadCompleted += downloadCompleted;
            download.Start();

            lock (monitor)
            {
                downloads.Add(download, range);
            }
        }

        private List<DownloadRange> DetermineToDoRanges(long fileSize, List<DownloadRange> alreadyDoneRanges)
        {
            var result = new List<DownloadRange>();

            var initialRange = new DownloadRange(0, fileSize);
            result.Add(initialRange);

            if (alreadyDoneRanges != null && alreadyDoneRanges.Count > 0)
            {
                foreach (var range in alreadyDoneRanges)
                {
                    var newResult = new List<DownloadRange>(result);

                    foreach (var resultRange in result)
                    {
                        if (downloadRangeHelper.RangesCollide(range, resultRange))
                        {
                            newResult.Remove(resultRange);
                            var difference = downloadRangeHelper.RangeDifference(resultRange, range);
                            newResult.AddRange(difference);
                        }
                    }

                    result = newResult;
                }
            }

            return result;
        }

        private void StartDownloadOfNextRange()
        {
            DownloadRange nextRange = null;

            lock (monitor)
            {
                nextRange = ToDoRanges.FirstOrDefault(r => !downloads.Values.Any(r2 => downloadRangeHelper.RangesCollide(r, r2)));
            }

            if (nextRange != null)
            {
                StartDownload(nextRange);
            }

            if (!downloads.Any())
            {
                lock (monitor)
                {
                    state = DownloadState.Finished;
                }

                OnDownloadCompleted(new DownloadEventArgs(this));
            }
        }

        private void downloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            var offset = args.Offset;
            var count = args.Count;
            var data = args.Data;

            lock (monitor)
            {
                var justDownloadedRange = new DownloadRange(offset, count);

                var todoRange = ToDoRanges.FirstOrDefault(r => downloadRangeHelper.RangesCollide(r, justDownloadedRange));

                if (todoRange != null)
                {
                    ToDoRanges.Remove(todoRange);
                    var differences = downloadRangeHelper.RangeDifference(todoRange, justDownloadedRange);
                    ToDoRanges.AddRange(differences);
                }

                var alreadyDoneRange = AlreadyDownloadedRanges.FirstOrDefault(r => r.End + 1 == justDownloadedRange.Start);

                if (alreadyDoneRange == null)
                {
                    alreadyDoneRange = justDownloadedRange;
                    AlreadyDownloadedRanges.Add(alreadyDoneRange);
                }
                else
                {
                    alreadyDoneRange.Length += justDownloadedRange.Length;
                }

                var neighborRange = AlreadyDownloadedRanges.FirstOrDefault(r => r.Start == alreadyDoneRange.End + 1);

                if (neighborRange != null)
                {
                    AlreadyDownloadedRanges.Remove(alreadyDoneRange);
                    AlreadyDownloadedRanges.Remove(neighborRange);
                    var combinedRange = new DownloadRange(alreadyDoneRange.Start, alreadyDoneRange.Length + neighborRange.Length);
                    AlreadyDownloadedRanges.Add(combinedRange);
                }
            }

            OnDataReceived(new DownloadDataReceivedEventArgs(this, data, offset, count));
        }

        private void downloadCompleted(DownloadEventArgs args)
        {
            lock (monitor)
            {
                var resumingDownload = (ResumingDownloader)args.Download;
                downloads.Remove(resumingDownload);
            }

            StartDownloadOfNextRange();
        }

        private void downloadCancelled(DownloadCancelledEventArgs args)
        {
            StartDownloadOfNextRange();
        }
    }
}