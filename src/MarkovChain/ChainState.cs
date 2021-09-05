using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarkovChain
{
    public class ChainState<T>: IEnumerable<KeyValuePair<ChainState<T>, int>>
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

        public int Transitions
        {
            get { return this.weightedTransitions.Count; }
        }

        public IEnumerator<T> StartTransitions()
        {
            return new ChainEnumerator<T>(this);
        }

        public IEnumerator<T> StartTransitions(Random rng)
        {
            return new ChainEnumerator<T>(this, rng);
        }

        public IEnumerator<KeyValuePair<ChainState<T>, int>> GetEnumerator()
        {
            return this.weightedTransitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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

        public bool HasTransition(ChainState<T> nextState)
        {
            return this.weightedTransitions.Keys.Contains(nextState);
        }

        public bool TryGetWeight(ChainState<T> nextState, out int weight)
        {
            return this.weightedTransitions.TryGetValue(nextState, out weight);
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

        public bool TryTransition(Random rng, out ChainState<T> state)
        {
            if (this.weightedTransitions.Count == 0)
            {
                state = null;
                return false;
            }
            int weightSum = this.weightedTransitions.Values.Sum();
            int sample = rng.Next(weightSum + 1);
            foreach (KeyValuePair<ChainState<T>, int> transition in this.weightedTransitions)
            {
                if (sample <= transition.Value) {
                    state = transition.Key;
                    return true;
                }
                sample -= transition.Value;
            }
            // the transitions where modified after generating the sum
            throw new InvalidSampleException($"sample '{sample + weightSum}' exeeds weight sum of '{weightSum}'");
        }
    }
}