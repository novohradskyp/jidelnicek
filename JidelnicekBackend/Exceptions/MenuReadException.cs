using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Exceptions
{
    [Serializable]
    public class MenuReadException : System.Exception
    {
        public MenuReadException()
        {
        }

        public MenuReadException(string message) : base(message)
        {
        }

        public MenuReadException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected MenuReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
