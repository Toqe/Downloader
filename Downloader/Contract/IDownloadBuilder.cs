using System;

namespace Myitian.Downloader.Contract
{
    public interface IDownloadBuilder
    {
        IDownloader Build(Uri url, int bufferSize, long? offset, long? maxReadBytes);
    }
}