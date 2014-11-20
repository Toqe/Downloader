using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toqe.Downloader.Business.Contract
{
    public interface IDownloadObserver
    {
        void Attach(IDownload download);

        void Detach(IDownload download);

        void DetachAll();
    }
}