/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Collections.Generic;

namespace Mirage.TextKit
{
    /// <summary>
    /// Represents glyphs laid out on a single line.
    /// </summary>
    public class GlyphLine
    {
        /// <summary>
        /// The glyphs on the line.
        /// </summary>
        public readonly List<OutputGlyph> OutputGlyphs = new();

        // /// <summary>
        // /// Get the width of the glyph line.
        // /// </summary>
        // /// <returns>The width of the glyph line.</returns>
        // public int GetWidth()
        // {
        //     // int minX = 0, maxX = 0;
        //     // foreach (OutputGlyph outputGlyph in OutputGlyphs)
        //     // {
        //     //     minX = Math.Min(minX, outputGlyph.X);
        //     //     maxX = Math.Max(maxX, outputGlyph.X + outputGlyph.Glyph.Width);
        //     // }
        //     // return maxX - minX;
        // }

        /// <summary>
        /// Offset the glyphs X coordinates to align them in a box.
        /// </summary>
        /// <param name="alignment">The horizontal alignment of the glyph run.</param>
        /// <param name="width">The width of the container.</param>
        public void Align(TextAlignment alignment, int containerWidth)
        {
            if (alignment == TextAlignment.Left)
            {
                return;
            }

            int lineWidth = Width;

            int offset = 0;
            switch (alignment)
            {
                case TextAlignment.Center:
                    offset = (containerWidth / 2) - (lineWidth / 2);
                    break;
                case TextAlignment.Right:
                    offset = containerWidth - lineWidth;
                    break;
            }

            for (int i = 0; i < OutputGlyphs.Count; i++)
            {
                OutputGlyphs[i] = new OutputGlyph(
                    OutputGlyphs[i].X + offset,
                    OutputGlyphs[i].Y,
                    OutputGlyphs[i].Glyph,
                    OutputGlyphs[i].Style
                );
            }
        }

        /// <summary>
        /// The width of the line.
        /// </summary>
        public int Width = 0;

        /// <summary>
        /// The height of the line.
        /// </summary>
        public int Height = 0;

        public bool EndsWithNewline = false;
    }
}
