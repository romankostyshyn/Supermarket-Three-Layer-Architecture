using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Validation
{
    public class MarketException : Exception
    {
        public MarketException(string message) : base(message)
        {
        }

        public MarketException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
