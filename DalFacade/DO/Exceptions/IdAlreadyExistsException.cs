using System;
using System.Runtime.Serialization;

namespace DO
{
    [Serializable]
    public class IdAlreadyExistsException : Exception
    {
        public IdAlreadyExistsException()
        {
        }

        public IdAlreadyExistsException(string message) : base(message)
        {
        }

        public IdAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}