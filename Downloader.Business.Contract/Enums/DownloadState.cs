using System;
using System.Collections.Generic;
using System.Text;

namespace Toqe.Downloader.Business.Contract.Enums
{
    public enum DownloadState
    {
        Undefined = 0,
        Initialized = 1,
        Running = 2,
        Finished = 3,
        Stopped = 4,
        Cancelled = 5
    }
}