using System;

namespace MarkovChain
{
    public class InvalidSampleException : Exception
    {
        public InvalidSampleException()
        {
        }

        public InvalidSampleException(string message)
            : base(message)
        {
        }

        public InvalidSampleException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}