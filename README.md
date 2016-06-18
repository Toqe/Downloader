Downloader
==========

A library for resuming and multi-part/multi-threaded downloads in .NET written in C#

The library uses .NET 3.5 and threads to support all platforms from Windows Vista on.

Example for usage:
Start a resuming download with 4 parts of test.com to local file myfile.html.

```
var url = new Uri("http://www.test.com");
var file = new System.IO.FileInfo("myfile.html");
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
```
