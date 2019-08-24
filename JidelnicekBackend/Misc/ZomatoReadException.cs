using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Misc
{
    public class ZomatoReadException : Exception
    {
        public ZomatoReadException()
        {
        }

        public ZomatoReadException(string message) : base(message)
        {
        }

        public ZomatoReadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
