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

    public class MissingTransitionException : Exception
    {
        public MissingTransitionException()
        {
        }

        public MissingTransitionException(string message)
            : base(message)
        {
        }

        public MissingTransitionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}