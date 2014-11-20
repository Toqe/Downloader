Downloader
==========

A library for resuming and multi-part/multi-threaded downloads in .NET written in C#

The library uses .NET 3.5 and threads to support all platforms from Windows Vista on.

Example for usage:

```
            var url = new Uri("http://www.test.com");
            var file = new FileInfo("myfile.html");
            var requestBuilder = new SimpleWebRequestBuilder();
            var dlChecker = new DownloadChecker();
            var httpDlBuilder = new SimpleDownloadBuilder(requestBuilder, dlChecker);
            var rdlBuilder = new ResumingDownloadBuilder(3000, 5000, 5, httpDlBuilder);
            var alreadyDownloadedRanges = null;
            var speedMonitor = new DownloadSpeedMonitor(32);
            this.download = new MultiPartDownload(url, 4096, 4, rdlBuilder, requestBuilder, dlChecker, alreadyDownloadedRanges);
            speedMonitor.Attach(this.download);
            this.download.DataReceived += dl_DataReceived;
            this.download.DownloadCompleted += (args) => { /* download has finished! */ };
            var dlSaver = new DownloadToFileSaver(file);
            dlSaver.Attach(this.download);
            this.download.Start();
```
