using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Contract.Events;

namespace Toqe.Downloader.Business.Observer
{
    public class DownloadToFileSaver : AbstractDownloadObserver
    {
        private FileInfo file;

        private FileStream fileStream;

        public DownloadToFileSaver(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            this.file = new FileInfo(filename);
        }

        public DownloadToFileSaver(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            this.file = file;
        }

        protected override void OnAttach(IDownload download)
        {
            download.DownloadStarted += downloadStarted;
            download.DownloadCancelled += downloadCancelled;
            download.DownloadCompleted += downloadCompleted;
            download.DownloadStopped += downloadStopped;
            download.DataReceived += downloadDataReceived;
        }

        protected override void OnDetach(IDownload download)
        {
            download.DownloadStarted -= downloadStarted;
            download.DownloadCancelled -= downloadCancelled;
            download.DownloadCompleted -= downloadCompleted;
            download.DownloadStopped -= downloadStopped;
            download.DataReceived -= downloadDataReceived;
        }

        private void OpenFileIfNecessary()
        {
            lock (this.monitor)
            {
                if (this.fileStream == null)
                {
                    this.fileStream = this.file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
            }
        }

        private void WriteToFile(byte[] data, long offset, int count)
        {
            lock (this.monitor)
            {
                this.OpenFileIfNecessary();

                this.fileStream.Position = offset;
                this.fileStream.Write(data, 0, count);
            }
        }

        private void CloseFile()
        {
            lock (this.monitor)
            {
                if (this.fileStream != null)
                {
                    this.fileStream.Flush();
                    this.fileStream.Close();
                    this.fileStream.Dispose();
                    this.fileStream = null;
                }
            }
        }

        private void downloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            lock (this.monitor)
            {
                this.WriteToFile(args.Data, args.Offset, args.Count);
            }
        }

        private void downloadStarted(DownloadStartedEventArgs args)
        {
            lock (this.monitor)
            {
                this.OpenFileIfNecessary();
            }
        }

        private void downloadCompleted(DownloadEventArgs args)
        {
            lock (this.monitor)
            {
                this.CloseFile();
            }
        }

        private void downloadStopped(DownloadEventArgs args)
        {
            lock (this.monitor)
            {
                this.CloseFile();
            }
        }

        private void downloadCancelled(DownloadCancelledEventArgs args)
        {
            lock (this.monitor)
            {
                this.CloseFile();
            }
        }

        public override void Dispose()
        {
            lock (this.monitor)
            {
                this.CloseFile();
            }

            base.Dispose();
        }
    }
}
