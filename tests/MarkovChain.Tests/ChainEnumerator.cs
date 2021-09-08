using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class ChainEnumerator
    {
        private int[] EnumeratorToArray(IEnumerator<int> enumerator)
        {
            List<int> buffer = new List<int>();
            while (enumerator.MoveNext())
            {
                buffer.Add(enumerator.Current);
            }
            return buffer.ToArray();
        }

        [Test]
        public void MoveNext()
        {
            ChainState<int> state;
            Random rng = new Random();
            int[] states = new int[]{3, 2, 1, 0};
            Chain<int> chain = new Chain<int>();
            chain.AddTransition(3, 2);
            chain.AddTransition(2, 1);
            chain.AddTransition(1, 0);
            Assert.IsTrue(chain.TryGetState(3, out state));
            Assert.AreEqual(states, this.EnumeratorToArray(state.StartTransitions(rng)));
            Assert.IsTrue(chain.TryGetState(2, out state));
            Assert.AreEqual(new ArraySegment<int>(states, 1, 3), this.EnumeratorToArray(state.StartTransitions(rng)));
            Assert.IsTrue(chain.TryGetState(1, out state));
            Assert.AreEqual(new ArraySegment<int>(states, 2, 2), this.EnumeratorToArray(state.StartTransitions(rng)));
            Assert.IsTrue(chain.TryGetState(0, out state));
            Assert.AreEqual(new ArraySegment<int>(states, 3, 1), this.EnumeratorToArray(state.StartTransitions(rng)));
        }
    }
}