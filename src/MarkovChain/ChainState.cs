using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarkovChain
{
    /// <summary>Class representing a state of a <see cref="Chain{T}"/></summary>
    public class ChainState<T>: IEnumerable<KeyValuePair<ChainState<T>, int>>
    {
        /// <summary>The value associated with this state</summary>
        public T value;

        protected Dictionary<ChainState<T>, int> weightedTransitions;

        /// <summary>Creates a new <see cref="ChainState{T}"/></summary>
        /// <param name="value">The value associated with this state</param>
        public ChainState(T value)
        {
            this.value = value;
            this.weightedTransitions = new Dictionary<ChainState<T>, int>();
        }

        /// <param name="weightedTransitions">Possible transitions to other states with weights</param>
        /// <inheritdoc cref="ChainState{T}.ChainState(T)"/>
        public ChainState(T value, Dictionary<ChainState<T>, int> weightedTransitions)
        {
            this.value = value;
            this.weightedTransitions = weightedTransitions;
        }

        /// <summary>The number of posibble transitions from this state</summary>
        public int Transitions
        {
            get { return this.weightedTransitions.Count; }
        }

        /// <summary>Start transitioning through a <see cref="Chain{T}"/> starting with this state</summary>
        /// <returns>An <see cref="IEnumerator{T}"/> yielding the values while transitioning through the chain</returns>
        public IEnumerator<T> StartTransitions()
        {
            return new ChainEnumerator<T>(this);
        }

        /// <param name="rng">A <see cref="Random"/> from which to choose transitions</param>
        /// <inheritdoc cref="ChainState{T}.StartTransitions()"/>
        public IEnumerator<T> StartTransitions(Random rng)
        {
            return new ChainEnumerator<T>(this, rng);
        }

        /// <summary>Enumerate all transitions from this state with their weights</summary>
        /// <returns>A <see cref="IEnumerator{KeyValuePair{ChainState{T}, int}}"/></returns>
        public IEnumerator<KeyValuePair<ChainState<T>, int>> GetEnumerator()
        {
            return this.weightedTransitions.GetEnumerator();
        }

        /// <inheritdoc cref="ChainState{T}.GetEnumerator()"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Add a transition to another <see cref="ChainState{T}"/></summary>
        /// <param name="stateTo">State to transition to</param>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
        public void AddTransition(ChainState<T> stateTo)
        {
            this.AddTransition(stateTo, 1);
        }

        /// <param name="weight">Weight of the transition</param>
        /// <inheritdoc cref="ChainState{T}.AddTransition(ChainState{T})"/>
        public void AddTransition(ChainState<T> stateTo, int weight)
        {
            this.weightedTransitions.TryGetValue(stateTo, out int currentWeight);
            this.weightedTransitions[stateTo] = currentWeight + weight;
        }

        /// <summary>Check if there is a transition to another <see cref="ChainState{T}"/></summary>
        /// <param name="nextState">State to transition to</param>
        /// <returns>A <see langword="bool"/> indicating if a transition exists</returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.Keys.Contains"/>
        public bool HasTransition(ChainState<T> nextState)
        {
            return this.weightedTransitions.Keys.Contains(nextState);
        }

        /// <summary>Get the weight of a transition</summary>
        /// <param name="nextState">State to transition to</param>
        /// <param name="weight">Out variable containing the weight on success</param>
        /// <returns>A <see langword="bool"/> indicating if <see langword="abstract"/> transition was found</returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
        public bool TryGetWeight(ChainState<T> nextState, out int weight)
        {
            return this.weightedTransitions.TryGetValue(nextState, out weight);
        }

        /// <summary>Remove a transition to another <see cref="ChainState{T}"/></summary>
        /// <param name="stateTo">state to transition to</param>
        /// <returns>A <see langword="bool"/> indicating if a transition was found and removed</returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.Remove(TKey)"/>
        public bool RemoveTransition(ChainState<T> stateTo)
        {
            return this.weightedTransitions.Remove(stateTo);
        }

        /// <param name="weight">Weight to remove from the transition</param>
        /// <inheritdoc cref="ChainState{T}.RemoveTransition(ChainState{T})"/>
        public bool RemoveTransition(ChainState<T> stateTo, int weight)
        {
            if (this.weightedTransitions.TryGetValue(stateTo, out int currentWeight))
            {
                this.weightedTransitions[stateTo] = currentWeight - weight;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Transition to another <see cref="ChainState{T}"/></summary>
        /// <param name="rng">A <see cref="Random"/> to choose the next state with</param>
        /// <param name="state">Out variable containing the next state on success</param>
        /// <returns>A <see langword="bool"/> indicating if the transition was successfull</returns>
        /// <exception cref="InvalidSampleException">The <see cref="ChainState{T}"/> was modified during transition</exception>
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