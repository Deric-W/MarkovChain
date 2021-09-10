using System.Collections;
using System.Collections.Generic;

namespace MarkovChain.Cli
{
    /// <summary>Interface for Enumerators which support looking at the next element in advance</summary>
    public interface IPeekableEnumerator<T> : IEnumerator<T>
    {
        /// <summary>Look at the next element</summary>
        /// <param name="nextValue">Out variable containing the next element on success</param>
        /// <returns>A <see langword="bool"/> indicating if there is a next element</returns>
        public bool TryPeek(out T nextValue);
    }

    /// <summary>Adapter which implements <see cref="IPeekableEnumerator{T}"/> for every enumerator</summary>
    /// <remarks>The wrapped <see cref="IEnumerator{T}"/> is advanced on class creation and the element is stored</remarks>
    public class PeekableEnumeratorAdapter<T> : IPeekableEnumerator<T>
    {
        protected IEnumerator<T> enumerator;

        protected bool fetchedNext;

        protected T lastValue;

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public T Current
        {
            get { return this.lastValue; }
        }

        /// <inheritdoc cref="PeekableEnumeratorAdapter{T}.Current"/>
        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        /// <summary>Create a new <see cref="PeekableEnumeratorAdapter{T}"/></summary>
        /// <param name="enumerator"><see cref="IEnumerator{T}"/> to wrap</param>
        public PeekableEnumeratorAdapter(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
            this.fetchedNext = enumerator.MoveNext();
        }

        /// <inheritdoc cref="IEnumerator{T}.MoveNext()"/>
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

        /// <inheritdoc cref="IEnumerator{T}.Reset()"/>
        public void Reset()
        {
            this.enumerator.Reset();
            this.fetchedNext = this.enumerator.MoveNext();
        }

        /// <remarks>This method disposes the wrapped enumerator</remarks>
        /// <inheritdoc cref="IEnumerator{T}.Dispose()"/>
        public void Dispose()
        {
            this.enumerator.Dispose();
        }

        /// <inheritdoc cref="IPeekableEnumerator{T}.TryPeek(out T)"/>
        public bool TryPeek(out T nextValue)
        {
            if (this.fetchedNext)
            {
                nextValue = this.enumerator.Current;
                return true;
            }
            else
            {
                nextValue = default;
                return false;
            }
        }
    }
}
