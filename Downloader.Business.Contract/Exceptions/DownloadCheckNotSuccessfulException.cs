using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toqe.Downloader.Business.Contract.Exceptions
{
    public class DownloadCheckNotSuccessfulException : Exception
    {
        public DownloadCheckNotSuccessfulException(string message, Exception ex, DownloadCheckResult downloadCheckResult) : base(message, ex)
        {
            this.DownloadCheckResult = downloadCheckResult;
        }

        public DownloadCheckResult DownloadCheckResult { get; private set; }
    }
}
