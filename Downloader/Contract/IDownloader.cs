using System;
using Myitian.Downloader.Contract.Enums;
using Myitian.Downloader.Contract.Events;

namespace Myitian.Downloader.Contract
{
    public interface IDownloader : IDisposable
    {
        event DownloadDelegates.DownloadDataReceivedHandler DataReceived;

        event DownloadDelegates.DownloadStartedHandler DownloadStarted;

        event DownloadDelegates.DownloadCompletedHandler DownloadCompleted;

        event DownloadDelegates.DownloadStoppedHandler DownloadStopped;

        event DownloadDelegates.DownloadCancelledHandler DownloadCancelled;

        DownloadState State { get; }

        void Start();

        void Stop();

        void DetachAllHandlers();
    }
}