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
            result.Size = response.ContentLength;
            result.StatusCode = (int?)(response as HttpWebResponse)?.StatusCode;
            result.Success = true;
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
            catch (WebException ex)
            {
                return new DownloadCheckResult() { Exception = ex, StatusCode = (int)(ex.Response as HttpWebResponse)?.StatusCode };
            }
            catch (Exception ex)
            {
                return new DownloadCheckResult() { Exception = ex };
            }
        }
    }
}