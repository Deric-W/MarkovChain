using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace MarkovChain.Cli
{
    /// <summary>Enumerator which joins lines in text which are seperated with a minus</summary>
    public class LineJoiningEnumerator : IEnumerator<char>
    {
        protected IPeekableEnumerator<char> enumerator;

        /// <inheritdoc cref="IEnumerator{char}.Current"/>
        public char Current
        {
            get { return this.enumerator.Current; }
        }

        /// <inheritdoc cref="LineJoiningEnumerator.Current"/>
        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        /// <summary>Creates a new <see cref="LineJoiningEnumerator"/></summary>
        /// <remarks>A <see cref="PeekableEnumeratorAdapter{char}"/> will be created if the wrapped enumerator does not implement <see cref="IPeekableEnumerator{char}"/></remarks>
        /// <param name="enumerator"><see cref="IEnumerator{char}"/> over the text to wrap</param>
        public LineJoiningEnumerator(IPeekableEnumerator<char> enumerator)
        {
            this.enumerator = enumerator;
        }

        /// <inheritdoc cref="LineJoiningEnumerator.LineJoiningEnumerator(IPeekableEnumerator{char})"/>
        public LineJoiningEnumerator(IEnumerator<char> enumerator)
        {
            this.enumerator = new PeekableEnumeratorAdapter<char>(enumerator);
        }

        /// <inheritdoc cref="IEnumerator{char}.MoveNext()"/>
        public bool MoveNext()
        {
            if (this.enumerator.MoveNext())
            {
                if (this.enumerator.Current == '-' && this.enumerator.TryPeek(out char nextChar))
                {
                    if (this.IsEndOfLine(nextChar))
                    {
                        this.enumerator.MoveNext();
                        do
                        {
                            if (!this.enumerator.MoveNext())
                            {
                                return false;
                            }
                        }
                        while (this.IsEndOfLine(this.enumerator.Current));
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Checks if a character marks the end of a line</summary>
        /// <param name="c">The character to check</param>
        /// <returns>A <see langword="bool"/> indicating if the caracter marks the end of a line</returns>
        public bool IsEndOfLine(char c)
        {
            return c == '\n' || c == '\r' || Char.GetUnicodeCategory(c) == UnicodeCategory.LineSeparator;
        }

        /// <inheritdoc cref="IEnumerator{char}.Reset()"/>
        public void Reset()
        {
            this.enumerator.Reset();
        }

        /// <remarks>This method disposes the wrapped enumerator</remarks>
        /// <inheritdoc cref="IEnumerator{char}.Dispose()"/>
        public void Dispose()
        {
            this.enumerator.Dispose();
        }
    }
}