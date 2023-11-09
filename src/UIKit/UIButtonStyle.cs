/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.GraphicsKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// Represents a style for a button's background.
    /// </summary>
    public abstract class UIButtonStyle
    {
        /// <summary>
        /// Render the button.
        /// </summary>
        /// <param name="target">Target canvas.</param>
        /// <param name="x">X coordinate of the button.</param>
        /// <param name="y">Y coordinate of the button.</param>
        /// <param name="width">Width of the button.</param>
        /// <param name="height">Height of the button.</param>
        /// <param name="hovered">Hovered state of the button. (currently unsupported)</param>
        /// <param name="pressed">Pressed state of the button.</param>
        /// <param name="@checked">Checked state of the button.</param>
        public abstract void Render(Canvas target, int x, int y, ushort width, ushort height, bool hovered, bool pressed, bool @checked);
    }
}
