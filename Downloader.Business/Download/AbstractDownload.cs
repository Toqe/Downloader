using System;
using System.Threading;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Contract.Enums;
using Toqe.Downloader.Business.Contract.Events;

namespace Toqe.Downloader.Business.Download
{
    public abstract class AbstractDownload : IDownload
    {
        public event DownloadDelegates.DownloadDataReceivedHandler DataReceived;

        public event DownloadDelegates.DownloadStartedHandler DownloadStarted;

        public event DownloadDelegates.DownloadCompletedHandler DownloadCompleted;

        public event DownloadDelegates.DownloadStoppedHandler DownloadStopped;

        public event DownloadDelegates.DownloadCancelledHandler DownloadCancelled;

        protected DownloadState state = DownloadState.Undefined;

        protected Uri url;

        protected int bufferSize;

        protected long? offset;

        protected long? maxReadBytes;

        protected IWebRequestBuilder requestBuilder;

        protected IDownloadChecker downloadChecker;

        protected bool stopping = false;

        protected readonly object monitor = new object();

        public AbstractDownload(Uri url, int bufferSize, long? offset, long? maxReadBytes, IWebRequestBuilder requestBuilder, IDownloadChecker downloadChecker)
        {
            if (bufferSize < 0)
                throw new ArgumentException("bufferSize < 0");

            if (offset.HasValue && offset.Value < 0)
                throw new ArgumentException("offset < 0");

            if (maxReadBytes.HasValue && maxReadBytes.Value < 0)
                throw new ArgumentException("maxReadBytes < 0");

            this.url = url ?? throw new ArgumentNullException(nameof(url));
            this.bufferSize = bufferSize;
            this.offset = offset;
            this.maxReadBytes = maxReadBytes;
            this.requestBuilder = requestBuilder;
            this.downloadChecker = downloadChecker;

            state = DownloadState.Initialized;
        }

        public DownloadState State
        {
            get { return state; }
        }

        public virtual void Start()
        {
            lock (monitor)
            {
                if (state != DownloadState.Initialized)
                {
                    throw new InvalidOperationException("Invalid state: " + state);
                }

                state = DownloadState.Running;
            }

            OnStart();
        }

        public virtual void Stop()
        {
            DoStop(DownloadStopType.WithNotification);
        }

        protected virtual void DoStop(DownloadStopType stopType)
        {
            lock (monitor)
            {
                stopping = true;
            }

            OnStop();

            if (stopType == DownloadStopType.WithNotification)
            {
                OnDownloadStopped(new DownloadEventArgs(this));
            }
        }

        public virtual void Dispose()
        {
            Stop();
        }

        public virtual void DetachAllHandlers()
        {
            if (DataReceived != null)
            {
                foreach (var i in DataReceived.GetInvocationList())
                {
                    DataReceived -= (DownloadDelegates.DownloadDataReceivedHandler)i;
                }
            }

            if (DownloadCancelled != null)
            {
                foreach (var i in DownloadCancelled.GetInvocationList())
                {
                    DownloadCancelled -= (DownloadDelegates.DownloadCancelledHandler)i;
                }
            }

            if (DownloadCompleted != null)
            {
                foreach (var i in DownloadCompleted.GetInvocationList())
                {
                    DownloadCompleted -= (DownloadDelegates.DownloadCompletedHandler)i;
                }
            }

            if (DownloadStopped != null)
            {
                foreach (var i in DownloadStopped.GetInvocationList())
                {
                    DownloadStopped -= (DownloadDelegates.DownloadStoppedHandler)i;
                }
            }

            if (DownloadStarted != null)
            {
                foreach (var i in DownloadStarted.GetInvocationList())
                {
                    DownloadStarted -= (DownloadDelegates.DownloadStartedHandler)i;
                }
            }
        }

        protected virtual void OnStart()
        {
            // Implementations should start their work here.
        }

        protected virtual void OnStop()
        {
            // This happens, when the Stop method is called.
            // Implementations should clean up and free their ressources here.
            // The stop event must not be triggered in here, it is triggered in the context of the Stop method.
        }

        protected virtual void StartThread(DownloadDelegates.VoidAction func, string name)
        {
            var thread = new Thread(new ThreadStart(func)) { Name = name };
            thread.Start();
        }

        protected virtual void OnDataReceived(DownloadDataReceivedEventArgs args)
        {
            DataReceived?.Invoke(args);
        }

        protected virtual void OnDownloadStarted(DownloadStartedEventArgs args)
        {
            DownloadStarted?.Invoke(args);
        }

        protected virtual void OnDownloadCompleted(DownloadEventArgs args)
        {
            DownloadCompleted?.Invoke(args);
        }

        protected virtual void OnDownloadStopped(DownloadEventArgs args)
        {
            DownloadStopped?.Invoke(args);
        }

        protected virtual void OnDownloadCancelled(DownloadCancelledEventArgs args)
        {
            DownloadCancelled?.Invoke(args);
        }
    }
}