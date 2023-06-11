using System;

namespace Toqe.Downloader.Business.Contract.Exceptions
{
    public class DownloadCheckNotSuccessfulException : Exception
    {
        public DownloadCheckNotSuccessfulException(string message, Exception ex, DownloadCheckResult downloadCheckResult) : base(message, ex)
        {
            DownloadCheckResult = downloadCheckResult;
        }

        public DownloadCheckResult DownloadCheckResult { get; private set; }
    }
}
