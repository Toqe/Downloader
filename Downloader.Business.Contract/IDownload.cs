using System;
using System.Collections.Generic;
using System.Text;
using Toqe.Downloader.Business.Contract.Enums;
using Toqe.Downloader.Business.Contract.Events;

namespace Toqe.Downloader.Business.Contract
{
    public interface IDownload : IDisposable
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