using System;
using Toqe.Downloader.Contract.Enums;
using Toqe.Downloader.Contract.Events;

namespace Toqe.Downloader.Contract
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