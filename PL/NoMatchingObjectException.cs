using System;
using System.Runtime.Serialization;

namespace PL
{
    [Serializable]
    public class NoMatchingObjectException : Exception
    {
        public NoMatchingObjectException()
        {
        }

        public NoMatchingObjectException(string message) : base(message)
        {
        }

        public NoMatchingObjectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoMatchingObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
