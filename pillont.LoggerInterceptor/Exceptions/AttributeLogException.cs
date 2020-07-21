using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace pillont.LoggerInterceptors.Exceptions
{
    public class AttributeLogException : ArgumentException
    {
        public AttributeLogException()
        { }

        public AttributeLogException(string message) : base(message)
        { }

        public AttributeLogException(string message, Exception innerException) : base(message, innerException)
        { }

        public AttributeLogException(string message, string paramName) : base(message, paramName)
        { }

        public AttributeLogException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        { }

        protected AttributeLogException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}