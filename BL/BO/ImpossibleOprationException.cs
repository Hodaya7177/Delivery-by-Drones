using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class ImpossibleOprationException : Exception
    {
        public ImpossibleOprationException() : base() { }

        public ImpossibleOprationException(string message) : base(message) { }


        public ImpossibleOprationException(string message, Exception innerException) : base(message, innerException) { }

        public override string ToString()
        {
            return Message;
        }
    }
}