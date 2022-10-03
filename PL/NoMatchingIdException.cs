using System;
using System.Runtime.Serialization;

namespace PL
{
    [Serializable]
    public class NoMatchingIdException : Exception
    {
        public NoMatchingIdException()
        {
        }

        public NoMatchingIdException(string message) : base(message)
        {
        }

        public NoMatchingIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoMatchingIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}