using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class PeekableEnumeratorAdapter
    {
        [Test]
        public void MoveNext()
        {
            int peeked;
            var enumerator = new Cli.PeekableEnumeratorAdapter<int>(
                new int[]{1, 2}.Cast<int>().GetEnumerator()
            );
            Assert.IsTrue(enumerator.TryPeek(out peeked));
            Assert.AreEqual(peeked, 1);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.TryPeek(out peeked));
            Assert.AreEqual(peeked, 2);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);
            Assert.IsFalse(enumerator.TryPeek(out peeked));
            Assert.AreEqual(peeked, 0);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void Reset()
        {
            var enumeratorMock = new Mock<IEnumerator<int>>();
            enumeratorMock.Setup(e => e.Reset());
            new Cli.PeekableEnumeratorAdapter<int>(enumeratorMock.Object).Reset();
            enumeratorMock.Verify(e => e.Reset(), Times.Once());
        }

        [Test]
        public void Dispose()
        {
            var enumeratorMock = new Mock<IEnumerator<int>>();
            enumeratorMock.Setup(e => e.Dispose());
            new Cli.PeekableEnumeratorAdapter<int>(enumeratorMock.Object).Dispose();
            enumeratorMock.Verify(e => e.Dispose(), Times.Once());
        }
    }
}
