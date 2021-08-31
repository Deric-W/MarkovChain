using System.Text;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class LineJoiningEnumerator
    {
        [Test]
        public void MoveNext()
        {
            string text = "This is a Test.\n\rWe test multiple line end-\rings and multiple lin-\n\n\r\n\n\r\x2028es.\n";
            Cli.LineJoiningEnumerator enumerator = new Cli.LineJoiningEnumerator(text.GetEnumerator());
            StringBuilder builder = new StringBuilder(67);
            while (enumerator.MoveNext())
            {
                builder.Append(enumerator.Current);
            }
            Assert.AreEqual(builder.ToString(), "This is a Test.\n\rWe test multiple line endings and multiple lines.\n");
        }

        [Test]
        public void IsEndOfLine()
        {
            Cli.LineJoiningEnumerator enumerator = new Cli.LineJoiningEnumerator(new char[]{}.Cast<char>().GetEnumerator());
            Assert.IsTrue(enumerator.IsEndOfLine('\n'));
            Assert.IsTrue(enumerator.IsEndOfLine('\r'));
            Assert.IsTrue(enumerator.IsEndOfLine('\x2028'));
            Assert.IsFalse(enumerator.IsEndOfLine('X'));
            Assert.IsFalse(enumerator.IsEndOfLine(' '));
        }

        [Test]
        public void Reset()
        {
            var enumeratorMock = new Mock<IEnumerator<char>>();
            enumeratorMock.Setup(e => e.Reset());
            new Cli.LineJoiningEnumerator(enumeratorMock.Object).Reset();
            enumeratorMock.Verify(e => e.Reset(), Times.Once());
        }

        [Test]
        public void Dispose()
        {
            var enumeratorMock = new Mock<IEnumerator<char>>();
            enumeratorMock.Setup(e => e.Dispose());
            new Cli.LineJoiningEnumerator(enumeratorMock.Object).Dispose();
            enumeratorMock.Verify(e => e.Dispose(), Times.Once());
        }
    }
}
