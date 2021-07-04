using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class ChainState
    {
        protected static ChainState<int>[] states = {
            new ChainState<int>(0),
            new ChainState<int>(1)
        };

        public bool EnumeratorContains<T>(IEnumerator<T> enumerator, HashSet<T> values)
        {
            int items = 0;
            while (enumerator.MoveNext())
            {
                if (!values.Contains(enumerator.Current))
                {
                    return false;
                }
                items += 1;
            }
            return items == values.Count;
        }

        [TestCase(0)]
        [TestCase(42)]
        public void Value(int value)
        {
            Assert.AreEqual(new ChainState<int>(value).value, value);
        }

        [Test]
        public void AddTransition()
        {
            HashSet<KeyValuePair<ChainState<int>, int>> stateSet = new HashSet<KeyValuePair<ChainState<int>, int>>();
            ChainState<int> state = new ChainState<int>(-1);

            state.AddTransition(states[0]);
            stateSet.Add(new KeyValuePair<ChainState<int>, int>(states[0], 1));
            Assert.True(this.EnumeratorContains(state.EnumerateTransitions(), stateSet));

            state.AddTransition(states[1], 42);
            stateSet.Add(new KeyValuePair<ChainState<int>, int>(states[1], 42));
            Assert.True(this.EnumeratorContains(state.EnumerateTransitions(), stateSet));

            state.AddTransition(states[0], 9);
            stateSet.Remove(new KeyValuePair<ChainState<int>, int>(states[0], 1));
            stateSet.Add(new KeyValuePair<ChainState<int>, int>(states[0], 10));
            Assert.True(this.EnumeratorContains(state.EnumerateTransitions(), stateSet));
        }

        [Test]
        public void RemoveTransition()
        {
            HashSet<KeyValuePair<ChainState<int>, int>> stateSet = new HashSet<KeyValuePair<ChainState<int>, int>>(
                new KeyValuePair<ChainState<int>, int>[] {
                    new KeyValuePair<ChainState<int>, int>(states[0], 1),
                    new KeyValuePair<ChainState<int>, int>(states[1], 42)
                }
            );
            ChainState<int> state = new ChainState<int>(
                -1,
                new Dictionary<ChainState<int>, int>(stateSet)
            );

            Assert.True(state.RemoveTransition(states[1], 40));
            stateSet.Remove(new KeyValuePair<ChainState<int>, int>(states[1], 42));
            stateSet.Add(new KeyValuePair<ChainState<int>, int>(states[1], 2));
            Assert.True(this.EnumeratorContains(state.EnumerateTransitions(), stateSet));

            Assert.True(state.RemoveTransition(states[0]));
            stateSet.Remove(new KeyValuePair<ChainState<int>, int>(states[0], 1));
            Assert.True(this.EnumeratorContains(state.EnumerateTransitions(), stateSet));

            Assert.False(state.RemoveTransition(states[0]));
        }

        [Test]
        public void PickNextState()
        {
            ChainState<int> state = new ChainState<int>(
                -1,
                new Dictionary<ChainState<int>, int>(
                    new KeyValuePair<ChainState<int>, int>[] {
                        new KeyValuePair<ChainState<int>, int>(states[0], 30),
                        new KeyValuePair<ChainState<int>, int>(states[1], 70)
                    }
                )
            );

            Random rng = new Random(42);

            int state0 = 0;
            int state1 = 0;

            for (int i = 0; i < 100; i++)
            {
                switch (state.PickNextState(rng).value)
                {
                    case 0:
                        state0 += 1;
                        break;
                    case 1:
                        state1 += 1;
                        break;
                    default:
                        Assert.Fail("picked invalid state");
                        break;
                }
            }
            Assert.Greater(state1, state0);
        }
    }
}
