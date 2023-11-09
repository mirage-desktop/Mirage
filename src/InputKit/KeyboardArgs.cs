/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Drawing;
using Cosmos.System;
using Mirage.DataKit;

namespace Mirage.InputKit
{   
    /// <summary>
    /// Signal arguments for a keyboard event.
    /// </summary>
    public class KeyboardArgs : SignalArgs
    {
        public KeyboardArgs(ConsoleKeyEx key, char character, ConsoleModifiers modifiers)
        {
            Key = key;
            Character = character;
            Modifiers = modifiers;
        }

        /// <summary>
        /// The key.
        /// </summary>
        public ConsoleKeyEx Key { get; init; }

        /// <summary>
        /// The resulting character of the key.
        /// </summary>
        public char Character { get; init; }

        /// <summary>
        /// The modifier keys that were held.
        /// </summary>
        public ConsoleModifiers Modifiers { get; init; }
    }
}
