using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarkovChain
{
    public class ChainState<T>: IEnumerable<T>
    {
        public T value;

        protected Dictionary<ChainState<T>, int> weightedTransitions;

        public ChainState(T value)
        {
            this.value = value;
            this.weightedTransitions = new Dictionary<ChainState<T>, int>();
        }

        public ChainState(T value, Dictionary<ChainState<T>, int> weightedTransitions)
        {
            this.value = value;
            this.weightedTransitions = weightedTransitions;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ChainEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ChainEnumerator<T>(this);
        }

        public IEnumerator<T> GetEnumerator(Random rng)
        {
            return new ChainEnumerator<T>(this, rng);
        }

        public void AddTransition(ChainState<T> stateTo)
        {
            this.AddTransition(stateTo, 1);
        }

        public void AddTransition(ChainState<T> stateTo, int weight)
        {
            int currentWeight = 0;
            this.weightedTransitions.TryGetValue(stateTo, out currentWeight);
            this.weightedTransitions[stateTo] = currentWeight + weight;
        }

        public IEnumerator<KeyValuePair<ChainState<T>, int>> EnumerateTransitions()
        {
            return this.weightedTransitions.GetEnumerator();
        }

        public bool RemoveTransition(ChainState<T> stateTo)
        {
            return this.weightedTransitions.Remove(stateTo);
        }

        public bool RemoveTransition(ChainState<T> stateTo, int weight)
        {
            int currentWeight;
            if (this.weightedTransitions.TryGetValue(stateTo, out currentWeight))
            {
                this.weightedTransitions[stateTo] = currentWeight - weight;
                return true;
            }
            else
            {
                return false;
            }
        }

        public ChainState<T> PickNextState(Random rng)
        {
            if (this.weightedTransitions.Count == 0)
            {
                throw new MissingTransitionException("state contains no transitions to choose from");
            }
            int weightSum = this.weightedTransitions.Values.Sum();
            int sample = rng.Next(weightSum + 1);
            foreach (KeyValuePair<ChainState<T>, int> transition in this.weightedTransitions)
            {
                if (sample <= transition.Value) {
                    return transition.Key;
                }
                sample -= transition.Value;
            }
            // the transitions where modified after generating the sum
            throw new InvalidSampleException($"sample '{sample + weightSum}' exeeds weight sum of '{weightSum}'");
        }
    }
}