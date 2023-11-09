/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Drawing;
using Cosmos.System;
using Mirage.DataKit;

namespace Mirage.InputKit
{   
    /// <summary>
    /// Signal arguments for a mouse event.
    /// </summary>
    public class MouseArgs : SignalArgs
    {
        public MouseArgs(int screenX, int screenY, MouseState state)
        {
            ScreenX = screenX;
            ScreenY = screenY;
            X = screenX;
            Y = screenY;
            State = state;
        }

        public MouseArgs(int screenX, int screenY, int x, int y, MouseState state)
        {
            ScreenX = screenX;
            ScreenY = screenY;
            X = x;
            Y = y;
            State = state;
        }

        /// <summary>
        /// The X coordinate of the event relative to the screen.
        /// </summary>
        public int ScreenX { get; init; }

        /// <summary>
        /// The Y coordinate of the event relative to the screen.
        /// </summary>
        public int ScreenY { get; init; }

        /// <summary>
        /// The point of the event relative to the screen.
        /// </summary>
        public Point ScreenPoint => new Point(ScreenX, ScreenY);

        /// <summary>
        /// The X coordinate of the event relative to the object that received it.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y coordinate of the event relative to the object that received it.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The point of the event relative to the object that received it.
        /// </summary>
        public Point Point => new Point(X, Y);

        /// <summary>
        /// The state of the mouse.
        /// </summary>
        public MouseState State { get; init; }

        /// <summary>
        /// Localize the coordinates of the event.
        /// </summary>
        /// <param name="x">The X coordinate of the object being localized to, relative to the X of the event.</param>
        /// <param name="x">The Y coordinate of the object being localized to, relative to the Y of the event.</param>
        /// <returns>The new localized event.</returns>
        public MouseArgs Localize(int x, int y)
        {
            return new MouseArgs(ScreenX, ScreenY, X - x, Y - y, State);
        }

        /// <summary>
        /// Localize the coordinates of the event.
        /// </summary>
        /// <param name="point">The point of the object being localized to, relative to the point of the event.</param>
        /// <returns>The new localized event.</returns>
        public MouseArgs Localize(Point point)
        {
            return Localize(point.X, point.Y);
        }
    }
}
