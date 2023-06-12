using System;
using System.Net;
using Toqe.Downloader.Contract;
using Toqe.Downloader.Contract.Enums;
using Toqe.Downloader.Contract.Events;

namespace Toqe.Downloader.Download
{
    public class SimpleDownloader : AbstractDownloader
    {
        public SimpleDownloader(Uri url, int bufferSize, long? offset, long? maxReadBytes, IWebRequestBuilder requestBuilder, IDownloadChecker downloadChecker)
            : base(url, bufferSize, offset, maxReadBytes, requestBuilder, downloadChecker)
        {
        }

        protected override void OnStart()
        {
            try
            {
                var request = requestBuilder.CreateRequest(url, offset);

                using (var response = request.GetResponse())
                {
                    if (response is HttpWebResponse httpResponse)
                    {
                        var statusCode = httpResponse.StatusCode;

                        if (!(statusCode == HttpStatusCode.OK || (offset.HasValue && statusCode == HttpStatusCode.PartialContent)))
                        {
                            throw new InvalidOperationException("Invalid HTTP status code: " + httpResponse.StatusCode);
                        }
                    }

                    var checkResult = downloadChecker.CheckDownload(response);
                    var supportsResume = checkResult.SupportsResume;
                    long currentOffset = supportsResume && offset.HasValue ? offset.Value : 0;
                    long sumOfBytesRead = 0;

                    OnDownloadStarted(new DownloadStartedEventArgs(this, checkResult, currentOffset));

                    using (var stream = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[bufferSize];

                        while (true)
                        {
                            lock (monitor)
                            {
                                if (stopping)
                                {
                                    state = DownloadState.Stopped;
                                    break;
                                }
                            }

                            int bytesRead = stream.Read(buffer, 0, buffer.Length);

                            if (bytesRead == 0)
                            {
                                lock (monitor)
                                {
                                    state = DownloadState.Finished;
                                }

                                OnDownloadCompleted(new DownloadEventArgs(this));
                                break;
                            }

                            if (maxReadBytes.HasValue && sumOfBytesRead + bytesRead > maxReadBytes.Value)
                            {
                                var count = (int)(maxReadBytes.Value - sumOfBytesRead);

                                if (count > 0)
                                {
                                    OnDataReceived(new DownloadDataReceivedEventArgs(this, buffer, currentOffset, count));
                                }
                            }
                            else
                            {
                                OnDataReceived(new DownloadDataReceivedEventArgs(this, buffer, currentOffset, bytesRead));
                            }

                            currentOffset += bytesRead;
                            sumOfBytesRead += bytesRead;

                            if (maxReadBytes.HasValue && sumOfBytesRead >= maxReadBytes.Value)
                            {
                                lock (monitor)
                                {
                                    state = DownloadState.Finished;
                                }

                                OnDownloadCompleted(new DownloadEventArgs(this));
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (monitor)
                {
                    state = DownloadState.Cancelled;
                }

                OnDownloadCancelled(new DownloadCancelledEventArgs(this, ex));
            }
        }
    }
}