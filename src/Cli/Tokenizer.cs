using System;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace MarkovChain.Cli
{
    public enum CharacterCategory
    {
        Single,     // Token contains only this char
        Delimiter,  // marks end of a word
        Other
    }


    public class Tokenizer : IEnumerator<string>
    {
        protected IPeekableEnumerator<char> enumerator;

        protected StringBuilder buffer;

        public Tokenizer(IPeekableEnumerator<char> enumerator, StringBuilder buffer)
        {
            this.enumerator = enumerator;
            this.buffer = buffer;
        }

        public Tokenizer(IPeekableEnumerator<char> enumerator)
        {
            this.enumerator = enumerator;
            this.buffer = new StringBuilder();
        }

        public Tokenizer(IEnumerator<char> enumerator, StringBuilder buffer)
        {
            this.enumerator = new PeekableEnumeratorAdapter<char>(enumerator);
            this.buffer = buffer;
        }

        public Tokenizer(IEnumerator<char> enumerator)
        {
            this.enumerator = new PeekableEnumeratorAdapter<char>(enumerator);
            this.buffer = new StringBuilder();
        }

        public string Current
        {
            get { return this.buffer.ToString(); }
        }

        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        public bool MoveNext()
        {
            this.buffer.Clear();
            while (this.enumerator.MoveNext())
            {
                CharacterCategory category = this.GetCharacterCategory(this.enumerator.Current);
                if (category != CharacterCategory.Delimiter)
                {
                    this.buffer.Append(this.enumerator.Current);
                    if (category == CharacterCategory.Single)
                    {
                        break;
                    }
                    if (this.enumerator.TryPeek(out char nextChar))
                    {
                        category = this.GetCharacterCategory(nextChar);
                        if (category == CharacterCategory.Delimiter || category == CharacterCategory.Single)
                        {
                            break;
                        }
                    }
                }
            }
            return this.buffer.Length != 0;
        }

        public CharacterCategory GetCharacterCategory(char c)
        {
            switch (Char.GetUnicodeCategory(c))
            {
                case UnicodeCategory.OpenPunctuation:
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.InitialQuotePunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.MathSymbol:
                    return CharacterCategory.Single;
                case UnicodeCategory.Control:
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                    return CharacterCategory.Delimiter;
                default:
                    return CharacterCategory.Other;
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
    }
}