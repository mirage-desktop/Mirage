/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using Mirage.DataKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// An item for use in a <see cref="UIContextMenu"/>.
    /// </summary>
    public class UIContextMenuItem
    {
        /// <summary>
        /// Initialise a new context menu item.
        /// </summary>
        /// <param name="text">The text of the context menu item.</param>
        /// <param name="callback">The callback to be called when the context menu item is clicked.</param>
        public UIContextMenuItem(string text, Action callback)
        {
            Text = text;
            Callback = callback;
        }

        /// <summary>
        /// Create a button representing this context menu item.
        /// </summary>
        /// <returns>The button.</returns>
        public UIButton CreateButton()
        {
            UIButton button = new UIButton(Text)
            {
                Style = new UIContextMenuButtonStyle(),
                ExplicitHeight = 24,
                HorizontalPadding = 15,
                FillWidth = true,
            };
            return button;
        }

        /// <summary>
        /// The text of the context menu item.
        /// </summary>
        public string Text { get; init; }

        /// <summary>
        /// The callback to be called when the context menu item is clicked.
        /// </summary>
        public Action Callback { get; set; }
    }
}
