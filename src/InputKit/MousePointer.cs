/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Drawing;
using Mirage.GraphicsKit;
using MouseManager = Cosmos.System.MouseManager;

namespace Mirage.InputKit
{
    /// <summary>
    /// The user's mouse pointer.
    /// </summary>
    public static class MousePointer
    {
        /// <summary>
        /// The X coordinate of the mouse pointer.
        /// </summary>
        public static int X => (int)MouseManager.X;

        /// <summary>
        /// The Y coordinate of the mouse pointer.
        /// </summary>
        public static int Y => (int)MouseManager.Y;

        /// <summary>
        /// The location of the mouse pointer.
        /// </summary>
        public static Point Location => new Point(X, Y);
    }
}
