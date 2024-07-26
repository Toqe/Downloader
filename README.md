Downloader
==========

# Update on 27.07.2024
Please consider this library archived as it has been written 10 years ago with targetting .NET 3.5 in mind, using from today's perspective ancient tools (explicit Threads instead of Tasks/Channels, much explicit locking instead of concurrent data structures, etc.). I also haven't used it myself in a long time, so I can't keep supporting it.

If you are looking for a more recent fork, please check out https://github.com/Myitian/Downloader which also provides a NuGet package for easy use.

If you are looking for a more modern library using .NET Standard / .NET (Core) 6+, have a look at https://github.com/bezzad/Downloader

And to you, DELL SupportAssist team: Thank you for crediting me at https://www.dell.com/support/manuals/de-de/support-assist-os-recovery/saosr_licensing_doc/toqe.downloader.business-license - but next time I would suggest using separate namespaces for your own code, so that the error stacktraces floating around the web won't be credited to this library ;)

# General Information

A library for resuming and multi-part/multi-threaded downloads in .NET written in C#

The library uses .NET 3.5 and threads to support all platforms from Windows Vista on.

Example for usage:
Start a resuming download with 4 parts of this README.md to local file README.md.

```C#
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
var bufferSize = 4096;
var numberOfParts = 4;
var download = new MultiPartDownload(url, bufferSize, numberOfParts, rdlBuilder, requestBuilder, dlChecker, alreadyDownloadedRanges);
download.DownloadCompleted += (args) => Console.WriteLine("download has finished!");
var dlSaver = new DownloadToFileSaver(file);
dlSaver.Attach(download);
download.Start();
```

For a more sophisticated example also demonstrating the download observers functionality, please have a look at the Downloader.Example project.

## Note on the number of concurrent/parallel downloads ##
.NET by default limits the number of concurrent connections. You can bypass this limit by manually setting the static `System.Net.ServicePointManager.DefaultConnectionLimit` property to a value appropriate to your application. Please also have a look at the documentation in the [MSDN](https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.defaultconnectionlimit.aspx)
