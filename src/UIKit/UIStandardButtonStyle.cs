/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.GraphicsKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// Standard button style.
    /// </summary>
    public class UIStandardButtonStyle : UIButtonStyle
    {
        public override void Render(Canvas target, int x, int y, ushort width, ushort height, bool hovered, bool pressed, bool @checked)
        {
            Slice slice = (pressed || @checked) ? _slicePress : _slice;
            slice.Render(target, x, y, width, height);
        }

        /// <summary>
        /// Neutral slice.
        /// </summary>
        private static readonly Slice _slice = new Slice(Resources.Button, 4);

        /// <summary>
        /// Pressed slice.
        /// </summary>
        private static readonly Slice _slicePress = new Slice(Resources.ButtonPress, 4);
    }
}
