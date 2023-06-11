namespace Toqe.Downloader.Business.Contract
{
    public class DownloadRange
    {
        public DownloadRange()
        {
        }

        public DownloadRange(long start, long length)
        {
            Start = start;
            Length = length;
        }

        public long Start { get; set; }

        public long Length { get; set; }

        public long End
        {
            get { return Start + Length - 1; }
        }

        public override bool Equals(object obj)
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
            if (r is null)
            {
                return false;
            }

            return (Start == r.Start) && (Length == r.Length);
        }

        public override int GetHashCode()
        {
            return (int)(13 * Start * End);
        }

        public static bool operator ==(DownloadRange a, DownloadRange b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((a is null) || (b is null))
            {
                return false;
            }

            return a is null ? b.Equals(a) : a.Equals(b);
        }

        public static bool operator !=(DownloadRange a, DownloadRange b)
        {
            return !(a == b);
        }
    }
}