using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
