using System.Collections;
using System.Collections.Generic;

namespace MarkovChain.Cli
{
    public class LineJoiningEnumerator : IEnumerator<char>
    {
        protected PeekableEnumeratorAdapter<char> enumerator;

        public char Current
        {
            get { return this.enumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        public LineJoiningEnumerator(IEnumerator<char> enumerator)
        {
            this.enumerator = new PeekableEnumeratorAdapter<char>(enumerator);
        }

        public bool MoveNext()
        {
            char nextChar;
            if (this.enumerator.MoveNext())
            {
                if (this.enumerator.Current == '-' && this.enumerator.TryPeek(out nextChar))
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

        public bool IsEndOfLine(char c)
        {
            return c == '\n' || c == '\r';
        }

        public void Reset()
        {
            this.enumerator.Reset();
        }

        public void Dispose()
        {
            this.enumerator.Dispose();
        }
    }
}