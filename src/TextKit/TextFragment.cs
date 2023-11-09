/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
namespace Mirage.TextKit
{
    /// <summary>
    /// Represents a string of text associated with a <see cref="Style"/>.
    /// </summary>
    public class TextFragment
    {
        public TextFragment(string text, TextStyle style)
        {
            Text = text;
            Style = style;
        }

        /// <summary>
        /// The content of the text fragment.
        /// </summary>
        public string Text;

        /// <summary>
        /// The style of the text fragment.
        /// </summary>
        public TextStyle Style;

        public override string ToString()
        {
            return Text;
        }
    }
}
