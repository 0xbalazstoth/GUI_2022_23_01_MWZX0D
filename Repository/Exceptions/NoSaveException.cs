using System;

namespace Repository.Exceptions
{
    public class NoSaveException : Exception
    {
        public NoSaveException()
        {

        }

        public NoSaveException(string message) : base(message)
        {

        }
    }
}
