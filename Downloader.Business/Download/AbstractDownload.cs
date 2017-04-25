using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
            if (url == null)
                throw new ArgumentNullException("url");

            if (bufferSize < 0)
                throw new ArgumentException("bufferSize < 0");

            if (offset.HasValue && offset.Value < 0)
                throw new ArgumentException("offset < 0");

            if (maxReadBytes.HasValue && maxReadBytes.Value < 0)
                throw new ArgumentException("maxReadBytes < 0");

            this.url = url;
            this.bufferSize = bufferSize;
            this.offset = offset;
            this.maxReadBytes = maxReadBytes;
            this.requestBuilder = requestBuilder;
            this.downloadChecker = downloadChecker;

            this.state = DownloadState.Initialized;
        }

        public DownloadState State
        {
            get { return this.state; }
        }

        public virtual void Start()
        {
            lock (this.monitor)
            {
                if (this.state != DownloadState.Initialized)
                {
                    throw new InvalidOperationException("Invalid state: " + this.state);
                }

                this.state = DownloadState.Running;
            }

            this.OnStart();
        }

        public virtual void Stop()
        {
            this.DoStop(DownloadStopType.WithNotification);
        }

        protected virtual void DoStop(DownloadStopType stopType)
        {
            lock (this.monitor)
            {
                this.stopping = true;
            }

            this.OnStop();

            if (stopType == DownloadStopType.WithNotification)
            {
                this.OnDownloadStopped(new DownloadEventArgs(this));
            }
        }

        public virtual void Dispose()
        {
            this.Stop();
        }

        public virtual void DetachAllHandlers()
        {
            if (this.DataReceived != null)
            {
                foreach (var i in this.DataReceived.GetInvocationList())
                {
                    this.DataReceived -= (DownloadDelegates.DownloadDataReceivedHandler)i;
                }
            }

            if (this.DownloadCancelled != null)
            {
                foreach (var i in this.DownloadCancelled.GetInvocationList())
                {
                    this.DownloadCancelled -= (DownloadDelegates.DownloadCancelledHandler)i;
                }
            }

            if (this.DownloadCompleted != null)
            {
                foreach (var i in this.DownloadCompleted.GetInvocationList())
                {
                    this.DownloadCompleted -= (DownloadDelegates.DownloadCompletedHandler)i;
                }
            }

            if (this.DownloadStopped != null)
            {
                foreach (var i in this.DownloadStopped.GetInvocationList())
                {
                    this.DownloadStopped -= (DownloadDelegates.DownloadStoppedHandler)i;
                }
            }

            if (this.DownloadStarted != null)
            {
                foreach (var i in this.DownloadStarted.GetInvocationList())
                {
                    this.DownloadStarted -= (DownloadDelegates.DownloadStartedHandler)i;
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
            if (this.DataReceived != null)
            {
                this.DataReceived(args);
            }
        }

        protected virtual void OnDownloadStarted(DownloadStartedEventArgs args)
        {
            if (this.DownloadStarted != null)
            {
                this.DownloadStarted(args);
            }
        }

        protected virtual void OnDownloadCompleted(DownloadEventArgs args)
        {
            if (this.DownloadCompleted != null)
            {
                this.DownloadCompleted(args);
            }
        }

        protected virtual void OnDownloadStopped(DownloadEventArgs args)
        {
            if (this.DownloadStopped != null)
            {
                this.DownloadStopped(args);
            }
        }

        protected virtual void OnDownloadCancelled(DownloadCancelledEventArgs args)
        {
            if (this.DownloadCancelled != null)
            {
                this.DownloadCancelled(args);
            }
        }
    }
}