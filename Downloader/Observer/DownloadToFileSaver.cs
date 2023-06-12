using System;
using System.IO;

using Toqe.Downloader.Contract;
using Toqe.Downloader.Contract.Events;

namespace Toqe.Downloader.Observer
{
    public class DownloadToFileSaver : AbstractDownloadObserver
    {
        private FileInfo file;

        private FileStream fileStream;

        public DownloadToFileSaver(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            file = new FileInfo(filename);
        }

        public DownloadToFileSaver(FileInfo file)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        protected override void OnAttach(IDownloader download)
        {
            download.DownloadStarted += downloadStarted;
            download.DownloadCancelled += downloadCancelled;
            download.DownloadCompleted += downloadCompleted;
            download.DownloadStopped += downloadStopped;
            download.DataReceived += downloadDataReceived;
        }

        protected override void OnDetach(IDownloader download)
        {
            download.DownloadStarted -= downloadStarted;
            download.DownloadCancelled -= downloadCancelled;
            download.DownloadCompleted -= downloadCompleted;
            download.DownloadStopped -= downloadStopped;
            download.DataReceived -= downloadDataReceived;
        }

        private void OpenFileIfNecessary()
        {
            lock (monitor)
            {
                fileStream = fileStream ?? file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
        }

        private void WriteToFile(byte[] data, long offset, int count)
        {
            lock (monitor)
            {
                OpenFileIfNecessary();

                fileStream.Position = offset;
                fileStream.Write(data, 0, count);
            }
        }

        private void CloseFile()
        {
            lock (monitor)
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                    fileStream.Dispose();
                    fileStream = null;
                }
            }
        }

        private void downloadDataReceived(DownloadDataReceivedEventArgs args)
        {
            lock (monitor)
            {
                WriteToFile(args.Data, args.Offset, args.Count);
            }
        }

        private void downloadStarted(DownloadStartedEventArgs args)
        {
            lock (monitor)
            {
                OpenFileIfNecessary();
            }
        }

        private void downloadCompleted(DownloadEventArgs args)
        {
            lock (monitor)
            {
                CloseFile();
            }
        }

        private void downloadStopped(DownloadEventArgs args)
        {
            lock (monitor)
            {
                CloseFile();
            }
        }

        private void downloadCancelled(DownloadCancelledEventArgs args)
        {
            lock (monitor)
            {
                CloseFile();
            }
        }

        public override void Dispose()
        {
            lock (monitor)
            {
                CloseFile();
            }

            base.Dispose();
        }
    }
}
