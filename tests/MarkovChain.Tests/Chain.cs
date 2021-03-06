using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class Chain
    {
        [Test]
        public void Count()
        {
            Chain<int> chain = new Chain<int>();
            Assert.AreEqual(chain.Count, 0);
            Assert.IsTrue(chain.TryAddState(0));
            Assert.AreEqual(chain.Count, 1);
        }

        [Test]
        public void TryAddState()
        {
            Chain<int> chain = new Chain<int>();
            Assert.IsTrue(chain.TryAddState(0));
            Assert.IsTrue(chain.TryAddState(1));
            Assert.IsFalse(chain.TryAddState(0));
            Assert.IsTrue(chain.TryAddState(2, new ChainState<int>(42)));
            Assert.AreEqual(chain.Count, 3);
            ChainState<int> state;
            Assert.IsTrue(chain.TryGetState(2, out state));
            Assert.AreEqual(state.value, 42);
        }

        [Test]
        public void TryGetState()
        {
            ChainState<int> state;
            Chain<int> chain = new Chain<int>();
            Assert.IsTrue(chain.TryAddState(42));
            Assert.IsTrue(chain.TryGetState(42, out state));
            Assert.AreEqual(state.value, 42);
            Assert.IsFalse(chain.TryGetState(0, out state));
            Assert.IsNull(state);
        }

        [Test]
        public void Add()
        {
            Chain<int> chain = new Chain<int>();
            chain.Add(0);
            Assert.IsTrue(chain.Contains(0));
            Assert.Throws<ArgumentException>(() => chain.Add(0));
            Assert.AreEqual(chain.Count, 1);
        }

        [Test]
        public void Remove()
        {
            Chain<int> chain = new Chain<int>();
            Assert.IsTrue(chain.TryAddState(0));
            Assert.IsTrue(chain.TryAddState(42));
            Assert.IsTrue(chain.Remove(0));
            Assert.IsFalse(chain.Remove(0));
            Assert.AreEqual(chain.Count, 1);
        }

        [Test]
        public void Clear()
        {
            Chain<int> chain = new Chain<int>();
            Assert.IsTrue(chain.TryAddState(0));
            Assert.IsTrue(chain.TryAddState(42));
            chain.Clear();
            Assert.AreEqual(chain.Count, 0);
        }

        [Test]
        public void Contains()
        {
            Chain<int> chain = new Chain<int>();
            Assert.IsTrue(chain.TryAddState(42));
            Assert.IsFalse(chain.Contains(0));
            Assert.IsTrue(chain.Contains(42));
        }

        [Test]
        public void CopyTo()
        {
            Chain<int> chain = new Chain<int>();
            Assert.IsTrue(chain.TryAddState(0));
            Assert.IsTrue(chain.TryAddState(42));
            int[] array = new int[]{1, 2, 3};
            chain.CopyTo(array, 1);
            Assert.IsTrue(
                array.SequenceEqual(new int[]{1, 0, 42}) || array.SequenceEqual(new int[]{1, 42, 0})
            );
        }

        [Test]
        public void AddTransition()
        {
            ChainState<int> state1;
            ChainState<int> state2;
            Chain<int> chain = new Chain<int>();
            chain.AddTransition(0, 1);
            Assert.IsTrue(chain.Contains(0));
            Assert.IsTrue(chain.TryGetState(1, out state1));
            chain.AddTransition(1, 2, 42);
            Assert.IsTrue(chain.TryGetState(1, out state2));
            Assert.AreSame(state1, state2);
            Assert.IsTrue(chain.TryGetState(2, out state2));
            Assert.IsTrue(state1.TryGetWeight(state2, out int weight));
            Assert.AreEqual(42, weight);
            Assert.AreEqual(1, state1.Transitions);
            Assert.AreEqual(0, state2.Transitions);
        }

        [Test]
        public void AddTransitions()
        {
            ChainState<int> state1;
            ChainState<int> state2;
            int weight;
            Chain<int> chain = new Chain<int>();
            chain.AddTransitions(
                new (int, int)[]{}.Cast<(int, int)>().GetEnumerator()
            );
            Assert.AreEqual(0, chain.Count);
            chain.AddTransition(2, 42);
            chain.AddTransitions(
                new (int, int)[]{ (3, 42), (2, 1), (1, 9) }.Cast<(int, int)>().GetEnumerator()
            );
            Assert.AreEqual(chain.Count, 4);
            Assert.IsTrue(chain.TryGetState(3, out state1));
            Assert.IsTrue(chain.TryGetState(2, out state2));
            Assert.IsTrue(state1.TryGetWeight(state2, out weight));
            Assert.AreEqual(1, weight);
            state1 = state2;
            Assert.IsTrue(chain.TryGetState(42, out state2));
            Assert.IsTrue(state1.TryGetWeight(state2, out weight));
            Assert.AreEqual(1, weight);
            Assert.IsTrue(chain.TryGetState(1, out state2));
            Assert.IsTrue(state1.TryGetWeight(state2, out weight));
            Assert.AreEqual(9, weight);
            Assert.AreEqual(0, state2.Transitions);
        }
    }
}