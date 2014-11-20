using System;
using System.Collections.Generic;
using System.Text;
using Toqe.Downloader.Business.Contract;

namespace Toqe.Downloader.Business.Utils
{
    public class DownloadRangeHelper
    {
        public bool RangesCollide(DownloadRange range1, DownloadRange range2)
        {
            return range1.Start <= range2.End && range2.Start <= range1.End;
        }

        public List<DownloadRange> RangeDifference(DownloadRange fullRange, DownloadRange range)
        {
            var result = new List<DownloadRange>();

            // no intersection
            if (!RangesCollide(fullRange, range))
            {
                result.Add(fullRange);
                return result;
            }

            // fullRange is part of range --> difference is empty
            if (fullRange.Start >= range.Start && fullRange.End <= range.End)
            {
                return result;
            }

            if (fullRange.Start < range.Start)
            {
                result.Add(new DownloadRange(fullRange.Start, range.Start - fullRange.Start));
            }

            if (fullRange.End > range.End)
            {
                result.Add(new DownloadRange(range.End + 1, fullRange.End - range.End));
            }

            return result;
        }
    }
}