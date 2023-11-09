/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using Mirage.GraphicsKit;

namespace Mirage.TextKit
{
    /// <summary>
    /// Handles rendering of <see cref="TextLayout"/> objects.
    /// </summary>
    public class TextRenderer
    {
        /// <summary>
        /// Initialise a new <see cref="TextRenderer"/>.
        /// </summary>
        public TextRenderer()
        {
        }

        // /// <summary>
        // /// Get the bounds of the text block.
        // /// </summary>
        // /// <returns>The bounds of the text block.</returns>
        // private Rectangle GetBounds()
        // {
        //     LayOut();

        //     if (_boundsCache != null)
        //     {
        //         return _boundsCache.Value;
        //     }

        //     int minX = 0, minY = 0, maxX = 0, maxY = 0;
        //     foreach (OutputGlyph outputGlyph in _outputGlyphs)
        //     {
        //         Glyph glyph = outputGlyph.Glyph;

        //         minX = Math.Min(minX, outputGlyph.X);
        //         minX = Math.Min(minY, outputGlyph.Y);

        //         maxX = Math.Max(maxX, outputGlyph.X + glyph.Width);
        //         maxY = Math.Max(maxX, outputGlyph.Y + glyph.Height);
        //     }

        //     Rectangle bounds = new Rectangle(
        //         minX,
        //         minY,
        //         maxX - minX,
        //         maxY - minY
        //     );
        //     _boundsCache = bounds;

        //     return bounds;
        // }

        /// <summary>
        /// Render text.
        /// </summary>
        /// <param name="layout">The layout to render.
        /// <param name="target">The target canvas to render to.
        /// The canvas should be sized to at least the size of the bounds of the text block,
        /// which can be retrieved using the Bounds property.</param>
        /// <param name="x">The X coordinate of the text block.</param>
        /// <param name="y">The Y coordinate of the text block, relative to the first baseline.</param>
        /// <param name="highlightStart">Text highlight starting index.</param>
        /// <param name="highlightEnd">Text highlight ending index.</param>
        public unsafe void Render(TextLayout layout, Canvas target, int x, int y, int highlightStart = 0, int highlightEnd = 0)
        {
            // fix params
            int tmp = Math.Min(highlightStart, highlightEnd);
            highlightEnd = Math.Max(highlightStart, highlightEnd);
            highlightStart = tmp;

            int targetWidth = target._Width;
            int targetHeight = target._Height;

            int charIdx = 0;
            foreach (GlyphLine line in layout.GlyphLines)
            {
                for (int i = 0; i < line.OutputGlyphs.Count; i++)
                {
                    OutputGlyph outputGlyph = line.OutputGlyphs[i];

                    if (charIdx >= highlightStart && charIdx < highlightEnd)
                    {
                        int width = outputGlyph.Glyph.AdvanceX;
                        int height = outputGlyph.Style.FontFace.GetHeight();
                        target.DrawFilledRectangle(
                            x + outputGlyph.X,
                            y + outputGlyph.Y - height,
                            (ushort)width,
                            (ushort)height,
                            0,
                            _highlightColor
                            );
                    }

                    Glyph glyph = outputGlyph.Glyph;
                    int glyphPaintX = outputGlyph.X + outputGlyph.Glyph.Left;
                    int glyphPaintY = outputGlyph.Y - outputGlyph.Glyph.Top;

                    if (glyphPaintX + x >= targetWidth || glyphPaintY + y >= targetHeight ||
                        glyphPaintX + x + glyph.Width < 0 || glyphPaintY + y + glyph.Height < 0)
                    {
                        charIdx++;
                        continue;
                    }

                    int glyphHeight = glyph.Height;
                    int glyphWidth = glyph.Width;

                    uint glyphColorArgb = outputGlyph.Style.Color.ARGB;

                    for (int row = 0; row < glyphHeight; row++)
                    {
                        int canvasRow = y + glyphPaintY + row;
                        if (canvasRow < 0 || canvasRow >= target._Height)
                        {
                            continue;
                        }

                        for (int column = 0; column < glyphWidth; column++)
                        {
                            int canvasColumn = x + glyphPaintX + column;
                            if (canvasColumn < 0 || canvasColumn >= target._Width)
                            {
                                continue;
                            }

                            byte alpha = glyph.Bitmap[(row * glyphWidth) + column];

                            if (alpha == 0)
                            {
                                continue;
                            }

                            int invAlpha = 256 - alpha;

                            int canvasIdx = (canvasRow * target._Width) + canvasColumn;

                            uint backgroundArgb = target.Internal[canvasIdx];

                            unchecked
                            {
                                byte backgroundA = (byte)((backgroundArgb >> 24) & 0xFF);

                                byte backgroundR = (byte)((backgroundArgb >> 16) & 0xFF);
                                byte backgroundG = (byte)((backgroundArgb >> 8) & 0xFF);
                                byte backgroundB = (byte)(backgroundArgb & 0xFF);

                                byte foregroundR = (byte)((glyphColorArgb >> 16) & 0xFF);
                                byte foregroundG = (byte)((glyphColorArgb >> 8) & 0xFF);
                                byte foregroundB = (byte)((glyphColorArgb) & 0xFF);

                                // TODOTODO port over alpha background support from canvas

                                byte a = 255;
                                byte r = (byte)((alpha * foregroundR + invAlpha * backgroundR) >> 8);
                                byte g = (byte)((alpha * foregroundG + invAlpha * backgroundG) >> 8);
                                byte b = (byte)((alpha * foregroundB + invAlpha * backgroundB) >> 8);

                                uint color = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;

                                target.Internal[canvasIdx] = color;
                            }
                        }
                    }

                    charIdx++;
                }

                charIdx += line.EndsWithNewline ? 1 : -1;
            }
        }

        /// <summary>
        /// Text highlight color.
        /// </summary>
        private readonly Color _highlightColor = new Color(0xFFB4D5FE);
    }
}
