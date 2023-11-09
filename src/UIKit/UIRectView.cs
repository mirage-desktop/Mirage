/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.GraphicsKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// A solid colored rectangle.
    /// </summary>
    class UIRectView : UIView
    {
        public override void PaintSelf()
        {
            if (Window == null)
            {
                return;
            }
            base.PaintSelf();
            System.Drawing.Rectangle rr = RootRectangle;
            Window.Surface.Canvas.DrawFilledRectangle(rr.X, rr.Y, (ushort)rr.Width, (ushort)rr.Height, 0, Color);
        }

        /// <summary>
        /// The color of the rectangle.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    QueuePaint();
                }
            }
        }

        /// <summary>
        /// Private backing cache for Color.
        /// </summary>
        private Color _color = Color.White;
    }
}
