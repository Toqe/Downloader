using System;

namespace Toqe.Downloader.Business.Contract
{
    public interface IDownloadBuilder
    {
        IDownload Build(Uri url, int bufferSize, long? offset, long? maxReadBytes);
    }
}