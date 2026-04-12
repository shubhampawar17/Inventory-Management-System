using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Exceptions
{
    public class TransactionNotFoundException : Exception
    {
        public TransactionNotFoundException(string message) : base(message) { }
    }
}
