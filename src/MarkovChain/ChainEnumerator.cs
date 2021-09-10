using System;
using System.Collections;
using System.Collections.Generic;

namespace MarkovChain
{
    /// <summary><see cref="IEnumerator{T}"/> yielding the values while transitioning through a <see cref="Chain{T}"/></summary>
    public class ChainEnumerator<T>: IEnumerator<T>
    {
        protected ChainState<T> nextState;

        protected T currentValue;

        protected Random rng;

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public T Current
        {
            get { return this.currentValue; }
        }

        /// <inheritdoc cref="ChainEnumerator{T}.Current"/>
        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        /// <summary>Creates a new <see cref="ChainEnumerator{T}"/></summary>
        /// <param name="nextState">The <see cref="ChainState{T}"/> from which to start transitioning</param>
        public ChainEnumerator(ChainState<T> nextState)
        {
            this.nextState = nextState;
            this.rng = new Random();
        }

        /// <param name="rng">A <see cref="Random"/> from which to choose transitions</param>
        /// <inheritdoc cref="ChainEnumerator{T}.ChainEnumerator(ChainState{T})"/>
        public ChainEnumerator(ChainState<T> nextState, Random rng)
        {
            this.nextState = nextState;
            this.rng = rng;
        }

        /// <summary>Transition to the next state in the <see cref="Chain{T}"/></summary>
        /// <remarks>The value of the initial state is yielded first</remarks>
        /// <returns>A <see langword="bool"/> indicating if the transition was succesfull</returns>
        /// <exception cref="InvalidOperationException">The current <see cref="ChainState{T}"/> was modified while transitioning</exception>
        /// <inheritdoc cref="IEnumerator{T}.MoveNext"/>
        public bool MoveNext()
        {
            if (this.nextState == null)
            {
                return false;
            }
            else
            {
                this.currentValue = this.nextState.value;
                try
                {
                    this.nextState.TryTransition(rng, out this.nextState);
                }
                catch (InvalidSampleException e)
                {
                    throw new InvalidOperationException("state was modified", e);
                }
                return true;
            }
        }

        /// <remarks>This operation is currently not supported</remarks>
        /// <inheritdoc cref="IEnumerator{T}.Reset"/>
        public void Reset()
        {
            throw new NotSupportedException();
        }

        /// <remarks>This method currently does nothing</remarks>
        /// <inheritdoc cref="IEnumerator{T}.Dispose"/>
        public void Dispose() {}
    }
}