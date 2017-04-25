using System;
using System.Collections.Generic;
using System.Text;

namespace Toqe.Downloader.Business.Contract
{
    public class DownloadRange
    {
        public DownloadRange()
        {
        }

        public DownloadRange(long start, long length)
        {
            this.Start = start;
            this.Length = length;
        }

        public long Start { get; set; }

        public long Length { get; set; }

        public long End
        {
            get { return this.Start + this.Length - 1; }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            DownloadRange r = obj as DownloadRange;

            return Equals(r);
        }

        public bool Equals(DownloadRange r)
        {
            if ((Object)r == null)
            {
                return false;
            }

            return (this.Start == r.Start) && (this.Length == r.Length);
        }

        public override int GetHashCode()
        {
            return (int)(13 * this.Start * this.End);
        }

        public static bool operator ==(DownloadRange a, DownloadRange b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return (object)a == null ? b.Equals(a) : a.Equals(b);
        }

        public static bool operator !=(DownloadRange a, DownloadRange b)
        {
            return !(a == b);
        }
    }
}