/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using Mirage.DataKit;
using Mirage.SurfaceKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// A context menu that displays <see cref="UIContextMenuItem"/> objects.
    /// </summary>
    public class UIContextMenu
    {
        /// <summary>
        /// Initialise a new context menu.
        /// </summary>
        /// <param name="sections">The list of items, grouped into sections.</param>
        public UIContextMenu(List<List<UIContextMenuItem>> sections)
        {
            _sections = sections;
        }

        /// <summary>
        /// Show the context menu.
        /// </summary>
        /// <param name="surfaceManager">The surface manager to show the context menu on.</param>
        /// <param name="x">The X coordinate of the context menu.</param>
        /// <param name="y">The Y coordinate of the context menu.</param>
        /// <returns>The window of the context menu.</returns>
        public UIWindow Show(SurfaceManager surfaceManager, int x, int y)
        {
            List<(UIButton, UIContextMenuItem)> buttonItemPairs = new();

            UIBoxLayout menuLayout = new UIBoxLayout(UIBoxOrientation.Vertical)
            {
                Location = new Point(0, VERTICAL_PADDING)
            };
            for (int i = 0; i < _sections.Count; i++)
            {
                foreach (UIContextMenuItem item in _sections[i])
                {
                    UIButton button = item.CreateButton();
                    buttonItemPairs.Add((button, item));
                    menuLayout.Add(button);
                }

                if (i < _sections.Count - 1)
                {
                    UIRectView divider = new UIRectView()
                    {
                        Color = GraphicsKit.Color.LightGray,
                        ExplicitHeight = 1,
                        FillWidth = true,
                    };
                    menuLayout.Add(divider);
                }
            }
            menuLayout.LayOut();

            UIWindow window = new UIWindow(
                surfaceManager,
                (ushort)menuLayout.Size.Width,
                (ushort)(menuLayout.Size.Height + (VERTICAL_PADDING * 2)),
                title: string.Empty, 
                titlebar: false
            );

            // 1px offset to account for the border
            window.Surface.X = x + 1;
            window.Surface.Y = y + 1;

            window.Surface.Modal = true;
            window.Surface.Focusable = false;

            window.Surface.SurfaceManager.BringToFront(window.Surface);

            foreach ((UIButton, UIContextMenuItem) pair in buttonItemPairs)
            {
                UIButton button = pair.Item1;
                UIContextMenuItem item = pair.Item2;

                button.OnMouseClick.Bind((args) => {
                    window.Close();
                    item.Callback.Invoke();
                });
            }

            window.RootView.Add(menuLayout);

            return window;   
        }

        /// <summary>
        /// The items of the context menu, grouped into sections.
        /// </summary>
        private readonly List<List<UIContextMenuItem>> _sections;

        /// <summary>
        /// The vertical padding of the context menu.
        /// </summary>
        private const int VERTICAL_PADDING = 5;

        /// <summary>
        /// The spacing between context menu items.
        /// </summary>
        private const int SPACING = 0;
    }
}
