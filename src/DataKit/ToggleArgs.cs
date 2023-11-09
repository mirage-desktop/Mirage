/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
namespace Mirage.DataKit
{
    /// <summary>
    /// Represents the arguments of a state toggle signal.
    /// </summary>
    public class ToggleArgs : SignalArgs
    {
        /// <summary>
        /// Initialise new toggle signal arguments.
        /// </summary>
        /// <param name="state">The new state of the object.</param>
        public ToggleArgs(bool state)
        {
            State = state;
        }

        /// <summary>
        /// The new state of the object.
        /// </summary>
        public readonly bool State;
    }
}
