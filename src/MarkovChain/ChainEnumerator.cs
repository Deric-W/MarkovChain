using System;
using System.Collections;
using System.Collections.Generic;

namespace MarkovChain
{
    public class ChainEnumerator<T>: IEnumerator<T>
    {
        protected ChainState<T> currentState;

        protected Random rng;

        public T Current
        {
            get { return this.currentState.value; }
        }

        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        public ChainEnumerator(ChainState<T> currentState)
        {
            this.currentState = currentState;
            this.rng = new Random();
        }

        public ChainEnumerator(ChainState<T> currentState, Random rng)
        {
            this.currentState = currentState;
            this.rng = rng;
        }

        public bool MoveNext()
        {
            try
            {
                this.currentState = this.currentState.PickNextState(this.rng);
            }
            catch (MissingTransitionException)
            {
                return false;
            }
            catch (InvalidSampleException e)
            {
                throw new InvalidOperationException("state was modified", e);
            }
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose() {}
    }
}