/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Diagnostics.CodeAnalysis;
using Mirage.GraphicsKit;

namespace Mirage.TextKit
{
    /// <summary>
    /// Represents a rich text style.
    /// </summary>
    public struct TextStyle
    {
        /// <summary>
        /// Initialise the default system text style.
        /// </summary>
        public TextStyle()
        {
            FontFace = Resources.Cantarell;
            Color = Color.Black;
        }

        /// <summary>
        /// Initialise a new rich text style.
        /// </summary>
        /// <param name="fontFace">The font face.</param>
        /// <param name="color">The color.</param>
        public TextStyle(FontFace fontFace, Color color)
        {
            FontFace = fontFace;
            Color = color;
        }

        /// <summary>
        /// The font face.
        /// </summary>
        public FontFace FontFace;

        /// <summary>
        /// The color.
        /// </summary>
        public Color Color;
    
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is TextStyle style)
            {
                return style.Color == Color && style.FontFace == FontFace;
            }
            return false;
        }
    }
}
