using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class EditionException :  Exception
    {
        public EditionException(string? message) : base(message)
        {

        }
    }
}
