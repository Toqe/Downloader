using System;

namespace Toqe.Downloader.Business.Contract.Exceptions
{
    public class TooManyRetriesException : Exception
    {
        public TooManyRetriesException()
            : base()
        {
        }
    }
}
