using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Toqe.Downloader.Business.Contract;

namespace Toqe.Downloader.Business.Utils
{
    public class DownloadChecker : IDownloadChecker
    {
        public DownloadCheckResult CheckDownload(WebResponse response)
        {
            var result = new DownloadCheckResult();
            var acceptRanges = response.Headers["Accept-Ranges"];
            result.SupportsResume = !string.IsNullOrEmpty(acceptRanges) && acceptRanges.ToLower().Contains("bytes");
            result.Size = (int)response.ContentLength;
            return result;
        }

        public DownloadCheckResult CheckDownload(Uri url, IWebRequestBuilder requestBuilder)
        {
            try
            {
                var request = requestBuilder.CreateRequest(url, null);

                using (var response = request.GetResponse())
                {
                    return CheckDownload(response);
                }
            }
            catch (Exception ex)
            {
                return new DownloadCheckResult() { Exception = ex };
            }
        }
    }
}