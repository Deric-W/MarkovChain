using System;
using System.Linq;
using NUnit.Framework;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class ChainEnumerator
    {
        [Test]
        public void MoveNext()
        {
            ChainState<int> state;
            int[] states = new int[]{3, 2, 1, 0};
            Chain<int> chain = new Chain<int>();
            chain.AddTransition(3, 2);
            chain.AddTransition(2, 1);
            chain.AddTransition(1, 0);
            Assert.IsTrue(chain.TryGetState(3, out state));
            Assert.IsTrue(state.SequenceEqual(states));
            Assert.IsTrue(chain.TryGetState(2, out state));
            Assert.IsTrue(state.SequenceEqual(new ArraySegment<int>(states, 1, 3)));
            Assert.IsTrue(chain.TryGetState(1, out state));
            Assert.IsTrue(state.SequenceEqual(new ArraySegment<int>(states, 2, 2)));
            Assert.IsTrue(chain.TryGetState(0, out state));
            Assert.IsTrue(state.SequenceEqual(new ArraySegment<int>(states, 3, 1)));
        }
    }
}