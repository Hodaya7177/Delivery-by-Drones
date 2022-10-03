using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class NoUpdateException : Exception
    {
        public NoUpdateException()
        {
        }

        public NoUpdateException(string message) : base(message)
        {
        }

        public NoUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}