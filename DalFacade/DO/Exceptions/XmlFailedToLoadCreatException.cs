using System;

namespace DO
{
    public class XmlFailedToLoadCreatException : Exception
    {
        public string xmlFilePath;
        public XmlFailedToLoadCreatException(string xmlPath) : base() { xmlFilePath = xmlPath; }
        public XmlFailedToLoadCreatException(string xmlPath, string message) :
            base(message)
        { xmlFilePath = xmlPath; }
        public XmlFailedToLoadCreatException(string xmlPath, string message, Exception innerException) :
            base(message, innerException)
        { xmlFilePath = xmlPath; }

        public override string ToString() => base.ToString() + $", fail to load or create xml file: {xmlFilePath}";
    }
}


