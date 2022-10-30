using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Exceptions
{
    public class SaveAlreadyExistsException : Exception
    {
        public SaveAlreadyExistsException()
        {

        }

        public SaveAlreadyExistsException(string message) : base(message)
        {

        }
    }
}
