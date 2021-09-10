using System;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace MarkovChain.Cli
{
    /// <summary>How the <see cref="Tokenizer"/> treats characters</summary>
    public enum CharacterCategory
    {
        /// <summary>Character is an independent token</summary>
        Single,

        /// <summary>Chracter marks end of a token</summary>
        Delimiter,

        /// <summary>Character is part of another token</summary>
        Other
    }

    /// <summary>Class which splits text into its tokens</summary>
    public class Tokenizer : IEnumerator<string>
    {
        protected IPeekableEnumerator<char> enumerator;

        protected StringBuilder buffer;

        /// <summary>Create a new <see cref="Tokenizer"/></summary>
        /// <param name="enumerator"><see cref="IPeekableEnumerator{char}"/> over the text to wrap</param>
        public Tokenizer(IPeekableEnumerator<char> enumerator)
        {
            this.enumerator = enumerator;
            this.buffer = new StringBuilder();
        }

        /// <param name="buffer"><see cref="StringBuilder"/> to build tokens in</param>
        /// <inheritdoc cref="Tokenizer.Tokenizer(IPeekableEnumerator{char})"/>
        public Tokenizer(IPeekableEnumerator<char> enumerator, StringBuilder buffer)
        {
            this.enumerator = enumerator;
            this.buffer = buffer;
        }

        /// <param name="enumerator"><see cref="IEnumerator{char}"/> over the text to wrap</param>
        /// <inheritdoc cref="Tokenizer.Tokenizer(IPeekableEnumerator{char})"/>
        public Tokenizer(IEnumerator<char> enumerator)
        {
            this.enumerator = new PeekableEnumeratorAdapter<char>(enumerator);
            this.buffer = new StringBuilder();
        }

        /// <param name="enumerator"><see cref="IEnumerator{char}"/> over the text to wrap</param>
        /// <inheritdoc cref="Tokenizer.Tokenizer(IPeekableEnumerator{char}, StringBuilder)"/>
        public Tokenizer(IEnumerator<char> enumerator, StringBuilder buffer)
        {
            this.enumerator = new PeekableEnumeratorAdapter<char>(enumerator);
            this.buffer = buffer;
        }

        /// <inheritdoc cref="IEnumerator{string}.Current"/>
        public string Current
        {
            get { return this.buffer.ToString(); }
        }

        /// <inheritdoc cref="Tokenizer.Current"/>
        object IEnumerator.Current
        {
            get { return (object)this.Current; }
        }

        /// <inheritdoc cref="IEnumerator{string}.MoveNext()"/>
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

        /// <summary>Determine the <see cref="CharacterCategory"/> of a character</summary>
        /// <param name="c">Character which <see cref="CharacterCategory"/> is to be determined</param>
        /// <returns>The <see cref="CharacterCategory"/> of <paramref name="c"/></returns>
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

        /// <inheritdoc cref="IEnumerator{string}.Reset()"/>
        public void Reset()
        {
            this.enumerator.Reset();
        }

        /// <remarks>This method disposes the wrapped enumerator</remarks>
        /// <inheritdoc cref="IEnumerator{string}.Dispose()"/>
        public void Dispose()
        {
            this.enumerator.Dispose();
        }
    }
}