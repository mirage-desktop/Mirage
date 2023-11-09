/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mirage.DataKit;
using Mirage.GraphicsKit;

namespace Mirage.TextKit
{
    /// <summary>
    /// Represents a block of rich text.
    /// </summary>
    public class TextBlock
    {
        /// <summary>
        /// Initialise an empty block of text.
        /// </summary>
        /// <param name="style">The initial text style.</param>
        public TextBlock(TextStyle style)
        {
            Style = style;
        }

        /// <summary>
        /// Initialise an empty block of text. with the default style
        /// </summary>
        public TextBlock()
        {
            Style = _defaultStyle;
        }

        /// <summary>
        /// Initialise a block of text.
        /// </summary>
        /// <param name="style">The text style.</param>
        /// <param name="text">The initial text.</param>
        public TextBlock(TextStyle style, string text)
        {
            Style = style;
            Append(text);
        }

        /// <summary>
        /// Initialise a block of text with the default style.
        /// </summary>
        /// <param name="text">The initial text.</param>
        public TextBlock(string text)
        {
            Style = _defaultStyle;
            Append(text);
        }

        /// <summary>
        /// Append a string of text to the text block.
        /// </summary>
        /// <param name="text">The text to append.</param>
        public void Append(string text)
        {
            TextFragment fragment = new TextFragment(text, Style);
            Append(fragment);
        }

        /// <summary>
        /// Append a text fragment to the text block.
        /// </summary>
        /// <param name="fragment">The text fragment to append.</param>
        public void Append(TextFragment fragment)
        {
            if (Fragments.Count > 0 && Fragments[^1].Style.Equals(fragment.Style))
            {
                Fragments[^1].Text += fragment.Text;
            }
            else
            {
                Fragments.Add(fragment);
            }
            OnChanged.Fire(new());
        }

        /// <summary>
        /// Clear the text block.
        /// </summary>
        public void Clear()
        {
            Fragments.Clear();
            OnChanged.Fire(new());
        }

        /// <summary>
        /// The current rich style of the text block.
        /// This property only applies to newly appended text.
        /// </summary>
        public TextStyle Style { get; set; }

        /// <summary>
        /// The contents of the text block.
        /// </summary>
        public List<TextFragment> Fragments { get; } = new List<TextFragment>();

        // /// <summary>
        // /// Combine sequential fragments of the same style.
        // /// </summary>
        // private void Defragment()
        // {
        //     for (int i = 0; i < Fragments.Count - 1; i++)
        //     {
        //         if (Fragments[i].Style.Equals(Fragments[i + 1].Style))
        //         {
        //             Fragments[i].Text += Fragments[i + 1].Text;
        //             Fragments.RemoveAt(i + 1);
        //         }
        //     }
        // }

        /// <summary>
        /// The plain text of the text block.
        /// You should prefer using the Append method rather than setting this property,
        /// as it will preserve existing rich text styles and allow for greater performance.
        /// </summary>
        public string Text
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (TextFragment fragment in Fragments)
                {
                    builder.Append(fragment.Text);
                }
                return builder.ToString();
            }

            set
            {
                Fragments.Clear();
                Append(value);
            }
        }

        // /// <summary>
        // /// Describes a range of fragments and a starting and ending character index for the first and last fragments.
        // /// </summary>
        // private struct FragmentRange
        // {
        //     public int FragmentBeginIndex;
        //     public int FragmentEndIndex;
        //     public int FirstFragmentBeginIndex;
        //     public int LastFragmentEndIndex;
        // }

        // /// <summary>
        // /// Get the fragment range for a selection.
        // /// </summary>
        // /// <param name="selectionBegin">The starting index of the selection.</param>
        // /// <param name="selectionEnd">The ending index of the selection.</param>
        // /// <returns>The fragment range.</returns>
        // private FragmentRange GetFragmentRange(int selectionBegin, int selectionEnd)
        // {
        //     int i = 0;
        //     int selectionBegin = 0;
        //     foreach (TextFragment fragment in Fragments)
        //     {
        //         i += fragment.Text.Length;
        //         if (selectionBegin == 0 && i == selectionBegin)
        //         {

        //         }
        //     }
        //     // FragmentRange range = new FragmentRange
        //     // {
        //     //     FragmentBeginIndex = fragmentBeginIndex,
        //     //     FragmentEndIndex = fragmentEndIndex,
        //     // };
        // }

        // public void Replace(int startIndex, int endIndex, TextFragment newFragment)
        // {
        //     int count = 0;
        //     int insertionIndex = 0;
        //     for (int i = 0; i < Fragments.Count; i++)
        //     {
        //         TextFragment fragment = Fragments[i];
        //         if (count >= startIndex)
        //         {
        //             fragment.Text = fragment.Text.Substring(Math.Max(0, count - startIndex));
        //         }
        //         count += fragment.Text.Length;
        //         if (count >= endIndex)
        //         {
        //             insertionIndex = i + 1;
        //             break;
        //         }
        //     }
        //     Fragments.Insert(insertionIndex, newFragment);
        //     OnChanged.Fire(new());
        // }

        /// <summary>
        /// The length of the content.
        /// </summary>
        public int Length
        {
            get {
                int length = 0;
                foreach (TextFragment fragment in Fragments)
                {
                    length += fragment.Text.Length;
                }
                return length;
            }
        }

        /// <summary>
        /// Fired when the text block is modified.
        /// </summary>
        public readonly Signal<SignalArgs> OnChanged = new();

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// The default style for text blocks.
        /// </summary>
        private readonly TextStyle _defaultStyle = new TextStyle(Resources.Cantarell, Color.Black);
    }
}
