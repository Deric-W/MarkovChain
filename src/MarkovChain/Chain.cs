using System.Collections;
using System.Collections.Generic;

namespace MarkovChain
{
    /// <summary>Class representing a <see href="https://en.wikipedia.org/wiki/Markov_chain">Markov Chain</see></summary>
    public class Chain<T>: ICollection<T>
    {
        protected Dictionary<T, ChainState<T>> states;

        /// <summary>The number of states in the chain.</summary>
        /// <returns>The number of states in the chain as <see langword="int"/>.</returns>
        public int Count
        {
            get { return this.states.Count; }
        }

        /// <summary>Indicates if the chain is read only</summary>
        /// <returns>A <see langword="bool"/> indicating if the chain is read only</returns>
        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<T, ChainState<T>>>)this.states).IsReadOnly; }
        }

        /// <summary>Creates a new empty chain</summary>
        public Chain()
        {
            this.states = new Dictionary<T, ChainState<T>>();
        }

        /// <summary>Creates a new chain containing specific states</summary>
        /// <param name="states">A <see cref="IEnumerable{KeyValuePair{T, ChainState{T}}}"/> yielding values with their associated state</param>
        public Chain(IEnumerable<KeyValuePair<T, ChainState<T>>> states)
        {
            this.states = new Dictionary<T, ChainState<T>>(states);
        }

        /// <summary>Creates a new chain containing specific states</summary>
        /// <param name="states">A <see cref="Dictionary{TKey, TValue}"/> mapping values to states</param>
        public Chain(Dictionary<T, ChainState<T>> states)
        {
            this.states = states;
        }

        /// <summary>Add a new state to the chain if it does not exist</summary>
        /// <param name="value">The value associated with the state</param>
        /// <returns>A <see langword="bool"/> indicating if a new state was added succesfully</return>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.TryAdd(TKey, TValue)"/>
        public bool TryAddState(T value)
        {
            return this.TryAddState(value, new ChainState<T>(value));
        }

        /// <param name="state">The state to add to the chain</param>
        /// <returns>A <see langword="bool"/> indicating if <paramref name="state"/> was added succesfully</return>
        /// <inheritdoc cref="Chain{T}.TryAddState(T)"/>
        public bool TryAddState(T value, ChainState<T> state)
        {
            return this.states.TryAdd(value, state);
        }

        /// <summary>Add a new state for <paramref name="value"/> to the chain</summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is already in the chain</exception>
        public void Add(T value)
        {
            this.states.Add(value, new ChainState<T>(value));
        }

        /// <param name="state">State to add to the chain</param>
        /// <inheritdoc cref="Chain{T}.Add(T)"/>
        public void Add(T value, ChainState<T> state)
        {
            this.states.Add(value, state);
        }

        /// <summary>Remove the state associated with a value from the chain</summary>
        /// <param name="value">The value which state is to be removed</param>
        /// <returns>A <see langword="bool"/> indicating if the state was removed</returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.Remove(TKey)"/>
        public bool Remove(T value)
        {
            return this.states.Remove(value);
        }

        /// <summary>Remove all states from the chain</summary>
        public void Clear()
        {
            this.states.Clear();
        }

        /// <summary>Check if the chain contains a specific value</summary>
        /// <returns>A <see langword="bool"/> indicating if the value was found</returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.ContainsKey(TKey)"/>
        public bool Contains(T value)
        {
            return this.states.ContainsKey(value);
        }

        /// <summary>Enumerates all values in the chain</summary>
        /// <returns>An <see cref="IEnumerator{T}"/> yielding all values in the chain</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.states.Keys.GetEnumerator();
        }

        /// <returns>An <see cref="IEnumerator"/> yielding all values in the chain</returns>
        /// <inheritdoc cref="Chain{T}.GetEnumerator()"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Copies the values in the chain to an existing one-dimensional <see cref="System.Array"/>, starting at the specified array index</summary>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.Keys.CopyTo"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.states.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>Enumerates all states in the chain</summary>
        /// <returns>An <see cref="IEnumerator{T}"/> yielding all states in the chain</returns>
        public IEnumerator<ChainState<T>> EnumerateStates()
        {
            return this.states.Values.GetEnumerator();
        }

        /// <summary>Get the state associated with a value</summary>
        /// <param name="value">The value associated with the state</param>
        /// <param name="state">Out variable containing the associated state on success</param>
        /// <returns>A <see langword="bool"/> indicating if the state was found</returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
        public bool TryGetState(T value, out ChainState<T> state)
        {
            return this.states.TryGetValue(value, out state);
        }

        /// <summary>Add a transition between two values to the chain</summary>
        /// <param name="valueFrom">Value from which the transitions starts</param>
        /// <param name="valueTo">Value on which the transition ends</param>
        /// <exception cref="ArgumentNullException">Some of the values are null</exception>
        public void AddTransition(T valueFrom, T valueTo)
        {
            this.AddTransition(valueFrom, valueTo, 1);
        }

        /// <param name="weight">Weight of the transition compared to others</param>
        /// <inheritdoc cref="Chain{T}.AddTransition(T, T)"/>
        public void AddTransition(T valueFrom, T valueTo, int weight)
        {
            if (!this.states.TryGetValue(valueTo, out ChainState<T> stateTo))
            {
                this.states[valueTo] = stateTo = new ChainState<T>(valueTo);
            }
            if (!this.states.TryGetValue(valueFrom, out ChainState<T> stateFrom))
            {
                this.states[valueFrom] = stateFrom = new ChainState<T>(valueFrom);
            }
            stateFrom.AddTransition(stateTo, weight);
        }

        /// <summary>Add transitions between the values yielded from an <see cref="IEnumerator{(T, int)}"/> to the chain.</summary>
        /// <remarks>This method is more efficient than calling <see cref="Chain{T}.AddTransition(T, T, int)"/> multiple times.</remarks>
        /// <param name="transitions">A <see cref="IEnumerator{(T, int)}"/> yielding the values between which transitions should be added with their weights</param>
        /// <exception cref="ArgumentNullException">Some of the values are null</exception>
        public void AddTransitions(IEnumerator<(T, int)> transitions)
        {
            T value;
            int weight;
           if (transitions.MoveNext())
           {
                (value, _) = transitions.Current;   // first weight is ignored
                if (!this.TryGetState(value, out ChainState<T> lastState))
                {
                    this.states[value] = lastState = new ChainState<T>(value);  // no exception because the state is not in the chain
                }

                while (transitions.MoveNext())
                {
                    (value, weight) = transitions.Current;
                    if (!this.TryGetState(value, out ChainState<T> newState))
                    {
                        this.states[value] = newState = new ChainState<T>(value);
                    }
                    lastState.AddTransition(newState, weight);
                    lastState = newState;   // reuse last state
                }
           }
        }

        /// <summary>Add transitions between the values yielded from an <see cref="IEnumerator{T}"/> to the chain.</summary>
        /// <param name="transitions">A <see cref="IEnumerator{T}"/> yielding the values between which transitions should be added</param>
        /// <inheritdoc cref="Chain{T}.AddTransitions(IEnumerator{(T, int)})"/>
        public void AddTransitions(IEnumerator<T> transitions)
        {
            this.AddTransitions(Chain<T>.AddWeights(transitions));
        }

        private static IEnumerator<(T, int)> AddWeights(IEnumerator<T> transitions)
        {
            while (transitions.MoveNext())
            {
                yield return (transitions.Current, 1);
            }
        }
    }
}
