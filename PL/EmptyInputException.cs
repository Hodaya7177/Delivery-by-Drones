using System;
using System.Runtime.Serialization;

namespace PL
{
    [Serializable]
    internal class EmptyInputException : Exception
    {
        public EmptyInputException()
        {
        }

        public EmptyInputException(string message) : base(message)
        {
        }

        public EmptyInputException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyInputException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}