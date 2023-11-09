/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.GraphicsKit;

namespace Mirage.TextKit
{
    /// <summary>
    /// A positioned glyph with a style.
    /// </summary>
    public struct OutputGlyph
    {
        /// <summary>
        /// Initialise a new output glyph.
        /// </summary>
        /// <param name="x">The X coordinate of the glyph.</param>
        /// <param name="y">The Y coordinate of the glyph.</param>
        /// <param name="glyph">The glyph.</param>
        /// <param name="style">The style of the glyph.</param>
        public OutputGlyph(int x, int y, Glyph glyph, TextStyle style)
        {
            X = x;
            Y = y;
            Glyph = glyph;
            Style = style;
        }

        /// <summary>
        /// The X coordinate of the glyph.
        /// </summary>
        public int X;

        /// <summary>
        /// The baseline Y coordinate of the glyph.
        /// </summary>
        public int Y;

        /// <summary>
        /// The glyph.
        /// </summary>
        public Glyph Glyph;
        
        /// <summary>
        /// The style of the glyph.
        /// /// </summary>
        public TextStyle Style;
    }
}
