using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toqe.Downloader.Business.Contract.Events
{
    public class DownloadEventArgs
    {
        public IDownload Download { get; set; }

        public DownloadEventArgs()
        {
        }

        public DownloadEventArgs(IDownload download)
        {
            this.Download = download;
        }
    }
}
