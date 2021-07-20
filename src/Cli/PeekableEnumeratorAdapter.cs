using System.Collections;
using System.Collections.Generic;

namespace MarkovChain.Cli
{
    public class PeekableEnumeratorAdapter<T> : IEnumerator<T>
    {
        protected IEnumerator<T> enumerator;

        protected bool fetchedNext;

        protected T lastValue;

        public T Current
        {
            get { return this.lastValue; }
        }

        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        public PeekableEnumeratorAdapter(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
            this.fetchedNext = enumerator.MoveNext();
        }

        public bool MoveNext()
        {
            if (this.TryPeek(out this.lastValue))
            {
                this.fetchedNext = this.enumerator.MoveNext();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            this.enumerator.Reset();
        }

        public void Dispose()
        {
            this.enumerator.Dispose();
        }

        public bool TryPeek(out T nextValue)
        {
            if (this.fetchedNext)
            {
                nextValue = this.enumerator.Current;
                return true;
            }
            else
            {
                nextValue = default(T);
                return false;
            }
        }
    }
}
