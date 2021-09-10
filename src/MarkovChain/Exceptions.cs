using System;

namespace MarkovChain
{
    /// <summary>Represents errors that occur while choosing the next <see cref="ChainState{T}"/> to transition to</summary>
    /// <inheritdoc cref="Exception"/>
    public class InvalidSampleException : Exception
    {
        /// <inheritdoc cref="Exception.Exception()"/>
        public InvalidSampleException()
        {
        }

        /// <inheritdoc cref="Exception.Exception(string?)"/>
        public InvalidSampleException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="Exception.Exception(string?, Exception?)"/>
        public InvalidSampleException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}