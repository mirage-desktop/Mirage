/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mirage.TextKit
{
    /// <summary>
    /// Represents a single, immutable text layout.
    /// </summary>
    public class TextLayout
    {
        /// <summary>
        /// Lay out a block of text.
        /// </summary>
        /// <param name="block">The text block to lay out.</param>
        /// <param name="containerWidth">The width of the container. If this is null, the text will be unbounded and not wrap.</param>
        /// <param name="wrapping">If text wrapping is enabled.</param>
        /// <param name="horizontalAlignment">The horizontal alignment of the text.</param>
        /// <returns>The computed layout.</returns>
        public static TextLayout LayOut(TextBlock block, int? containerWidth = null, bool wrapping = false, TextAlignment horizontalAlignment = TextAlignment.Left)
        {
            TextLayout layout = new TextLayout(block, containerWidth, wrapping, horizontalAlignment);
            layout.ComputeLayout();
            return layout;
        }

        /// <summary>
        /// Initialise a new text layout.
        /// </summary>
        /// <param name="block">The text block.</param>
        /// <param name="containerWidth">The width of the container.</param>
        /// <param name="wrapping">If text wrapping is enabled.</param>
        /// <param name="alignment">The alignment of the text. The container width must be set for this to have an affect on the layout.</param>
        private TextLayout(TextBlock block, int? containerWidth, bool wrapping, TextAlignment horizontalAlignment)
        {
            _block = block;
            _containerWidth = containerWidth;
            _wrapping = wrapping;
            _horizontalAlignment = horizontalAlignment;
        }

        /// <summary>
        /// Lay out the text block. Can only be called once.
        /// </summary>
        void ComputeLayout()
        {
            if (_laidOut)
            {
                return;
            }

            int x = 0;
            int returnX = x;

            int y = 0;

            bool newLineEntry = true;

            bool ignore = false;

            GlyphLine currentLine = new GlyphLine();
            GlyphLines.Add(currentLine);

            // _indexLut = new int[TextBlock.Length];

            foreach (TextFragment fragment in _block.Fragments)
            {
                int wrappableGlyphs = 0;
                int wrappableWidth = 0;

                for (int i = 0; i < fragment.Text.Length; i++)
                {
                    if (newLineEntry)
                    {
                        y += fragment.Style.FontFace.GetHeight();
                        newLineEntry = false;
                    }

                    char c = fragment.Text[i];

                    currentLine.Height = Math.Max(currentLine.Height, fragment.Style.FontFace.GetHeight());

                    Glyph? glyph = fragment.Style.FontFace.GetGlyph(c);
                    if (glyph == null)
                    {
                        continue;
                    }

                    if (c == '\n')
                    {
                        // currentLine.OutputGlyphs.Add(new OutputGlyph(x, y, glyph, fragment.Style));
                        currentLine.EndsWithNewline = true;

                        x = returnX;
                        newLineEntry = true;
                        wrappableGlyphs = 0;

                        currentLine = new GlyphLine();
                        GlyphLines.Add(currentLine);

                        continue;
                    }
                    
                    int glyphPaintX = x + glyph.Left;
                    int glyphPaintY = y - glyph.Top;

                    // BUGBUG: this might get stuck if the _containerWidth is tiny
                    bool shouldBreak = _wrapping && _containerWidth != null && glyphPaintX + glyph.Width > _containerWidth.Value;
                    if (shouldBreak)
                    {
                        bool canWrapWord = wrappableGlyphs > 0 &&
                            wrappableGlyphs < currentLine.OutputGlyphs.Count &&
                            wrappableWidth < _containerWidth!.Value;

                        if (canWrapWord)
                        {
                            i -= wrappableGlyphs + 1;
                            currentLine.OutputGlyphs.RemoveRange(currentLine.OutputGlyphs.Count - wrappableGlyphs, wrappableGlyphs);
                            currentLine.Width -= wrappableWidth;
                            if (i < fragment.Text.Length && char.IsWhiteSpace(fragment.Text[i]))
                            {
                                ignore = true;
                            }
                        }

                        x = returnX;
                        newLineEntry = true;
                        wrappableGlyphs = 0;
                        wrappableWidth = 0;

                        currentLine = new GlyphLine();
                        GlyphLines.Add(currentLine);

                        i--;

                        continue;
                    }

                    OutputGlyph outputGlyph = new OutputGlyph(x, y, glyph, fragment.Style);
                    currentLine.OutputGlyphs.Add(outputGlyph);

                    bool move = !ignore;

                    if (move)
                    {
                        x += glyph.AdvanceX;
                        currentLine.Width += glyph.AdvanceX;
                    }

                    if (c != '-' && !char.IsWhiteSpace(c))
                    {
                        wrappableGlyphs++;
                        if (move)
                        {
                            wrappableWidth += glyph.AdvanceX;
                        }
                    }
                    else
                    {
                        wrappableGlyphs = 0;
                        wrappableWidth = 0;
                    }

                    ignore = false;
                }
            }

            int width = 0;
            foreach (GlyphLine line in GlyphLines)
            {
                width = Math.Max(width, line.Width);
            }

            foreach (GlyphLine line in GlyphLines)
            {
                line.Align(_horizontalAlignment, _containerWidth ?? width);
                foreach (OutputGlyph outputGlyph in line.OutputGlyphs)
                {
                    OutputGlyphs.Add(outputGlyph);
                }
            }

            _laidOut = true;
        }
        
        // <summary>
        /// Get the text index at a point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The index of text at the point.</returns>
        public int GetIndexAtPoint(Point point)
        {
            if (!_laidOut || GlyphLines.Count == 0)
            {
                return 0;
            }
            int y = 0;
            int idx = 0;
            GlyphLine? lineAtPoint = null;
            foreach (GlyphLine line in GlyphLines)
            {
                y += line.Height;
                if (point.Y < y)
                {
                    lineAtPoint = line;
                    break;
                }
                idx += line.OutputGlyphs.Count + (line.EndsWithNewline ? 1 : 0);
            }
            lineAtPoint ??= GlyphLines[^1];
            int x = 0;
            foreach (OutputGlyph outGlyph in lineAtPoint.OutputGlyphs)
            {
                int finalX = x + outGlyph.Glyph.AdvanceX;
                x += outGlyph.Glyph.AdvanceX / 2;
                if (point.X < x)
                {
                    return idx;
                }
                x = finalX;
                idx++;
            }
            return idx;
        }

        /// <summary>
        /// Get caret point and height at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Caret info. The point's Y coordinate is relative to the text's baseline.</returns>
        public (Point Point, int Height) GetCaretInfoAtIndex(int index)
        {
            if (!_laidOut)
            {
                return (
                    new Point(0, _block.Style.FontFace.GetHeight()),
                    _block.Style.FontFace.GetHeight()
                );
            }
            int i = 0;
            int y = 0;
            foreach (GlyphLine line in GlyphLines)
            {
                if (index <= i + line.OutputGlyphs.Count)
                {
                    int idxr = index - i;
                    if (line.OutputGlyphs.Count == 0 || idxr < 0)
                    {  
                        return (
                            new Point(0, y + _block.Style.FontFace.GetHeight()),
                            _block.Style.FontFace.GetHeight()
                        );
                    }
                    if (idxr >= line.OutputGlyphs.Count)
                    {
                        OutputGlyph pglyph = line.OutputGlyphs[^1];
                        return (
                            new Point(pglyph.X + pglyph.Glyph.AdvanceX, y + _block.Style.FontFace.GetHeight()),
                            _block.Style.FontFace.GetHeight()
                        );
                    }
                    OutputGlyph glyph = line.OutputGlyphs[idxr];
                    return (
                        new Point(glyph.X, glyph.Y),
                        glyph.Style.FontFace.GetHeight()
                    );
                }
                i += line.OutputGlyphs.Count;
                if (line.EndsWithNewline)
                {
                    i++;
                }
                y += line.Height;
            }
            return (new(0, 0), 0);
        }

        /// <summary>
        /// Get the total width of the laid out text.
        /// </summary>
        /// <returns>The total width of the laid out text.</returns>
        public int GetWidth()
        {
            if (!_laidOut)
            {
                return 0;
            }

            int width = 0;
            foreach (GlyphLine line in GlyphLines)
            {
                width = Math.Max(0, line.Width);
            }
            return width;
        }

        /// <summary>
        /// Get the total height of the laid out text.
        /// </summary>
        /// <returns>The total height of the laid out text.</returns>
        public int GetHeight()
        {
            if (!_laidOut)
            {
                return 0;
            }

            int height = 0;
            foreach (GlyphLine line in GlyphLines)
            {
                height += line.Height;
            }
            return height;
        }
        
        /// <summary>
        /// Glyph lines.
        /// </summary>
        public readonly List<GlyphLine> GlyphLines = new List<GlyphLine>();

        /// <summary>
        /// The laid out glyphs.
        /// </summary>
        public readonly List<OutputGlyph> OutputGlyphs = new();

        /// <summary>
        /// If the text has been laid out.
        /// </summary>
        private bool _laidOut = false;

        /// <summary>
        /// The text block.
        /// </summary>
        private readonly TextBlock _block;

        /// <summary>
        /// The size of the container.
        /// If this is null, the text will be unbounded and not wrap.
        /// </summary>
        private readonly int? _containerWidth;

        /// <summary>
        /// If text wrapping is enabled.
        /// </summary>
        private readonly bool _wrapping;

        /// <summary>
        /// The horizontal alignment of the text.
        /// </summary>
        private readonly TextAlignment _horizontalAlignment;
    }
}
