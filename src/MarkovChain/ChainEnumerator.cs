using System;
using System.Collections;
using System.Collections.Generic;

namespace MarkovChain
{
    public class ChainEnumerator<T>: IEnumerator<T>
    {
        protected ChainState<T> nextState;

        protected T currentValue;

        protected Random rng;

        public T Current
        {
            get { return this.currentValue; }
        }

        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        public ChainEnumerator(ChainState<T> nextState)
        {
            this.nextState = nextState;
            this.rng = new Random();
        }

        public ChainEnumerator(ChainState<T> nextState, Random rng)
        {
            this.nextState = nextState;
            this.rng = rng;
        }

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

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose() {}
    }
}