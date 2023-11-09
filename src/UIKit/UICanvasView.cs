/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Drawing;
using Mirage.GraphicsKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// A view that has a canvas.
    /// </summary>
    class UICanvasView : UIView
    {
        /// <summary>
        /// Initialise a new canvas view with a specified explicit size.
        /// </summary>
        /// <param name="alpha">If the view should be alpha blended.</param>
        public UICanvasView(Size explicitSize, bool alpha = false)
        {
            ExplicitSize = explicitSize;
            Canvas = new Canvas((ushort)Size.Width, (ushort)Size.Height);
            Alpha = alpha;
        }

        /// <summary>
        /// Initialise a new canvas view from an existing canvas.
        /// <param name="alpha">If the view should be alpha blended.</param>
        /// </summary>
        public UICanvasView(Canvas canvas, bool alpha = false)
        {
            ExplicitSize = canvas.Size;
            Canvas = canvas;
            Alpha = alpha;
        }

        public override void PaintSelf()
        {
            base.PaintSelf();
            Point rootLocation = RootLocation;
            Window?.Surface.Canvas.DrawImage(rootLocation.X, rootLocation.Y, Canvas, Alpha: Alpha);
        }

        protected override Size GetImplicitSize()
        {
            return new Size(Canvas.Width, Canvas.Height);
        }

        /// <summary>
        /// The canvas of the view.
        /// </summary>
        public Canvas Canvas { get; init; }

        /// <summary>
        /// If the canvas view should alpha blended.
        /// </summary>
        public bool Alpha { get; set; }
    }
}
