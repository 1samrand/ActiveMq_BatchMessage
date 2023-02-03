using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchMessage.ActiveMQ
{
    public class ActiveMQException : Exception
    {
        public ActiveMQException()
            : base()
        {
        }

        public ActiveMQException(string message)
            : base(message)
        {
        }

        public ActiveMQException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
