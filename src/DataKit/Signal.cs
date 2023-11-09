/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Collections.Generic;

namespace Mirage.DataKit
{
    /// <summary>
    /// A class which broadcasts signals to its subscribers.
    /// </summary>
    public class Signal<T> where T : SignalArgs
    {
        /// <summary>
        /// Bind a callback to the signal.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> callback that will be binded to this signal.</param>
        public void Bind(Action<T> action)
        {
            _subscribers.Add(action);
        }

        /// <summary>
        /// Fire the signal.
        /// </summary>
        /// <param name="args">The arguments of the signal.</param>
        public void Fire(T args)
        {
            foreach (Action<T> action in _subscribers)
            {
                action.Invoke(args);
            }
        }

        /// <summary>
        /// The <see cref="Action"/> callbacks that subscribe to the event.
        /// </summary>
        private List<Action<T>> _subscribers = new();
    }
}
