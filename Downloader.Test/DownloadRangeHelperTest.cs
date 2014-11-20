using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Utils;
using Xunit;

namespace Downloader.Test
{
    public class DownloadRangeHelperTest
    {
        private DownloadRangeHelper helper = new DownloadRangeHelper();

        [Fact]
        public void NoIntersection()
        {
            var fullRange = new DownloadRange(1, 8);
            var range1 = new DownloadRange(9, 3);
            var range2 = new DownloadRange(0, 1);

            Assert.False(helper.RangesCollide(fullRange, range1));
            Assert.False(helper.RangesCollide(fullRange, range2));

            var differenceWithRange1 = helper.RangeDifference(fullRange, range1);
            var differenceWithRange2 = helper.RangeDifference(fullRange, range2);

            Assert.Equal(1, differenceWithRange1.Count);
            Assert.Equal(1, differenceWithRange2.Count);
            Assert.Contains(fullRange, differenceWithRange1);
            Assert.Contains(fullRange, differenceWithRange2);
        }

        [Fact]
        public void FullOverlay()
        {
            var fullRange = new DownloadRange(1, 8);
            var range1 = new DownloadRange(0, 10);
            var range2 = new DownloadRange(1, 8);

            Assert.True(helper.RangesCollide(fullRange, range1));
            Assert.True(helper.RangesCollide(fullRange, range2));

            var differenceWithRange1 = helper.RangeDifference(fullRange, range1);
            var differenceWithRange2 = helper.RangeDifference(fullRange, range2);

            Assert.Empty(differenceWithRange1);
            Assert.Empty(differenceWithRange2);
        }

        [Fact]
        public void PartialIntersectionWithOneResult()
        {
            var fullRange = new DownloadRange(1, 8);
            var range1 = new DownloadRange(0, 5);
            var range2 = new DownloadRange(1, 4);
            var range3 = new DownloadRange(7, 4);
            var range4 = new DownloadRange(8, 4);

            Assert.True(helper.RangesCollide(fullRange, range1));
            Assert.True(helper.RangesCollide(fullRange, range2));
            Assert.True(helper.RangesCollide(fullRange, range3));
            Assert.True(helper.RangesCollide(fullRange, range4));

            var differenceWithRange1 = helper.RangeDifference(fullRange, range1);
            var differenceWithRange2 = helper.RangeDifference(fullRange, range2);
            var differenceWithRange3 = helper.RangeDifference(fullRange, range3);
            var differenceWithRange4 = helper.RangeDifference(fullRange, range4);

            Assert.Equal(1, differenceWithRange1.Count);
            Assert.Equal(1, differenceWithRange2.Count);
            Assert.Equal(1, differenceWithRange3.Count);
            Assert.Equal(1, differenceWithRange4.Count);
            Assert.Contains(new DownloadRange(5, 4), differenceWithRange1);
            Assert.Contains(new DownloadRange(5, 4), differenceWithRange2);
            Assert.Contains(new DownloadRange(1, 6), differenceWithRange3);
            Assert.Contains(new DownloadRange(1, 7), differenceWithRange4);
        }

        [Fact]
        public void PartialIntersectionWithTwoResults()
        {
            var fullRange = new DownloadRange(1, 8);
            var range1 = new DownloadRange(2, 1);
            var range2 = new DownloadRange(3, 3);

            Assert.True(helper.RangesCollide(fullRange, range1));
            Assert.True(helper.RangesCollide(fullRange, range2));

            var differenceWithRange1 = helper.RangeDifference(fullRange, range1);
            var differenceWithRange2 = helper.RangeDifference(fullRange, range2);

            Assert.Equal(2, differenceWithRange1.Count);
            Assert.Equal(2, differenceWithRange2.Count);
            Assert.Contains(new DownloadRange(1, 1), differenceWithRange1);
            Assert.Contains(new DownloadRange(3, 6), differenceWithRange1);
            Assert.Contains(new DownloadRange(1, 2), differenceWithRange2);
            Assert.Contains(new DownloadRange(6, 3), differenceWithRange2);
        }
    }
}