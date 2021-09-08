using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class Tokenizer
    {
        [TestCase("Hello, I _would_ like to know your \"name\".", new string[] { "Hello", ",", "I", "_would_", "like", "to", "know", "your", "\"", "name", "\"", "." })]
        [TestCase("Test-Case nr. 24a (12*2)", new string[] { "Test-Case", "nr", ".", "24a", "(", "12", "*", "2", ")" })]
        public void MoveNext(string text, string[] tokens)
        {
            var enumerator1 = tokens.GetEnumerator();
            var enumerator2 = new Cli.Tokenizer(text.GetEnumerator());
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                Assert.AreEqual(enumerator1.Current, enumerator2.Current);
            }
            Assert.IsFalse(enumerator1.MoveNext());
            Assert.IsFalse(enumerator2.MoveNext());
        }

        [Test]
        public void Reset()
        {
            var enumerator = new Cli.Tokenizer("Test-Case nr. 24a (12*2)".GetEnumerator());
            enumerator.MoveNext();
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, "nr");
            enumerator.Reset();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(enumerator.Current, "Test-Case");
        }

        [Test]
        public void Dispose()
        {
            var enumeratorMock = new Mock<IEnumerator<char>>();
            enumeratorMock.Setup(e => e.Dispose());
            new Cli.Tokenizer(enumeratorMock.Object).Dispose();
            enumeratorMock.Verify(e => e.Dispose(), Times.Once());
        }
    }
}