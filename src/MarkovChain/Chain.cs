using System;
using System.Collections.Generic;

namespace MarkovChain
{
    public class Chain<T>
    {
        protected Dictionary<T, ChainState<T>> states;

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
            if (!this.states.TryGetValue(valueTo, out stateTo))
            {
                this.states[valueTo] = stateTo = new ChainState<T>(valueTo);
            }
            ChainState<T> stateFrom;
            if (this.states.TryGetValue(valueFrom, out stateFrom))
            {
                stateFrom.AddTransition(stateTo);
            }
            else
            {
                stateFrom = new ChainState<T>(valueFrom);
                stateFrom.AddTransition(stateTo, weight);
                this.states[valueFrom] = stateFrom;
            }
        }
    }
}
