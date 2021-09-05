using System.Collections;
using System.Collections.Generic;

namespace MarkovChain
{
    public class Chain<T>: ICollection<T>
    {
        protected Dictionary<T, ChainState<T>> states;

        public int Count
        {
            get { return this.states.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<T, ChainState<T>>>)this.states).IsReadOnly; }
        }

        public Chain()
        {
            this.states = new Dictionary<T, ChainState<T>>();
        }

        public Chain(Dictionary<T, ChainState<T>> states)
        {
            this.states = states;
        }

        public bool TryAddState(T value)
        {
            return this.TryAddState(value, new ChainState<T>(value));
        }

        public bool TryAddState(T value, ChainState<T> state)
        {
            return this.states.TryAdd(value, state);
        }

        public void Add(T value)
        {
            this.states.Add(value, new ChainState<T>(value));
        }

        public void Add(T value, ChainState<T> state)
        {
            this.states.Add(value, state);
        }

        public bool Remove(T value)
        {
            return this.states.Remove(value);
        }

        public void Clear()
        {
            this.states.Clear();
        }

        public bool Contains(T value)
        {
            return this.states.ContainsKey(value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.states.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.states.Keys.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ChainState<T>> EnumerateStates()
        {
            return this.states.Values.GetEnumerator();
        }

        public bool TryGetState(T value, out ChainState<T> state)
        {
            return this.states.TryGetValue(value, out state);
        }

        public void AddTransition(T valueFrom, T valueTo)
        {
            this.AddTransition(valueFrom, valueTo, 1);
        }

        public void AddTransition(T valueFrom, T valueTo, int weight)
        {
            ChainState<T> stateTo;
            ChainState<T> stateFrom;
            if (!this.states.TryGetValue(valueTo, out stateTo))
            {
                this.states[valueTo] = stateTo = new ChainState<T>(valueTo);
            }
            if (!this.states.TryGetValue(valueFrom, out stateFrom))
            {
                this.states[valueFrom] = stateFrom = new ChainState<T>(valueFrom);
            }
            stateFrom.AddTransition(stateTo, weight);
        }

        public void AddTransitions(IEnumerator<(T, int)> transitions)
        {
            ChainState<T> newState;
            ChainState<T> lastState;    // reuse last state 
            T value;
            int weight;
           if (transitions.MoveNext())
           {
                (value, _) = transitions.Current;   // first weight is ignored
                if (!this.TryGetState(value, out lastState))
                {
                    this.states[value] = lastState = new ChainState<T>(value);
                }

                while (transitions.MoveNext())
                {
                    (value, weight) = transitions.Current;
                    if (!this.TryGetState(value, out newState))
                    {
                        this.states[value] = newState = new ChainState<T>(value);
                    }
                    lastState.AddTransition(newState, weight);
                    lastState = newState;
                }
           }
        }

        public void AddTransitions(IEnumerator<T> transitions)
        {
            this.AddTransitions(this.AddWeights(transitions));
        }

        private IEnumerator<(T, int)> AddWeights(IEnumerator<T> transitions)
        {
            while (transitions.MoveNext())
            {
                yield return (transitions.Current, 1);
            }
        }
    }
}
