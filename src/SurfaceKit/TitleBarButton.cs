/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using Mirage.GraphicsKit;

namespace Mirage.SurfaceKit
{
    /// <summary>
    /// A button in a surface's title bar.
    /// </summary>
    public class TitleBarButton
    {
        /// <summary>
        /// Initialise a new title bar button.
        /// </summary>
        /// <param name="image">Neutral image.</param>
        /// <param name="hoverImage">Hover image.</param>
        /// <param name="pressImage">Pressed image.</param>
        /// <param name="callback">Click callback.</param>
        public TitleBarButton(Canvas image, Canvas hoverImage, Canvas pressImage, Action callback)
        {
            _image = image;
            _hoverImage = hoverImage;
            _pressImage = pressImage;
            Callback = callback;
        }

        /// <summary>
        /// Neutral image.
        /// </summary>
        private readonly Canvas _image;

        /// <summary>
        /// Hover image.
        /// </summary>
        private readonly Canvas _hoverImage;

        /// <summary>
        /// Pressed image.
        /// </summary>
        private readonly Canvas _pressImage;

        /// <summary>
        /// The callback that will be called when the button is pressed.
        /// </summary>
        public Action Callback;

        /// <summary>
        /// The X coordinate of the title bar button.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate of the title bar button.
        /// </summary>
        public int Y;

        /// <summary>
        /// The width of the title bar button.
        /// </summary>
        public int Width => _image.Width;

        /// <summary>
        /// The Height of the title bar button.
        /// </summary>
        public int Height => _image.Height;

        /// <summary>
        /// If the button is pressed.
        /// </summary>
        public bool Hovered;

        /// <summary>
        /// If the button is pressed.
        /// </summary>
        public bool Pressed;

        /// <summary>
        /// Render the titlebar button.
        /// </summary>
        /// <param name="target">The target canvas to render to.</param>
        /// <param name="focus">If the title bar's surface has focus.</param>
        public void Render(Canvas target, bool focus)
        {
            Canvas image = _image;
            if (Hovered)
            {
                image = _hoverImage;
            }
            if (Pressed)
            {
                image = _pressImage;
            }

            if (!focus)
            {
                image = Filters.Opacity(0.5f, image);
            }

            target.DrawImage(X, Y, image, Alpha: true);
        }
    }
}
