/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using Cosmos.System;
using Mirage.InputKit;

namespace Mirage.SurfaceKit
{
    /// <summary>
    /// Represents a surface being interactively dragged.
    /// </summary>
    public class SurfaceDragOperation : Operation
    {
        /// <summary>
        /// Initialise a new drag operation.
        /// </summary>
        /// <param name="surfaceManager">The surface manager.</param>
        /// <param name="surface">The surface being dragged.</param>
        public SurfaceDragOperation(SurfaceManager surfaceManager, Surface surface)
        {
            _surfaceManager = surfaceManager;
            _surface = surface;

            _surfaceStartX = surface.X;
            _surfaceStartY = surface.Y;

            _mouseStartX = MousePointer.X;
            _mouseStartY = MousePointer.Y;
        }

        /// <summary>
        /// Update the interactive drag operation.
        /// </summary>
        public override void Update()
        {
            if (MouseManager.MouseState != MouseState.Left)
            {
                _surfaceManager.CancelOperation();
                return;
            }
            
            int minY = (_surfaceManager.SystemMenu != null ? DE.SystemMenu.MENU_HEIGHT : 0) +
                (_surface.TitleBar != null ? TitleBar.TITLEBAR_HEIGHT : 0);

            _surface.X = _surfaceStartX + (MousePointer.X - _mouseStartX);
            _surface.Y = Math.Max(minY, _surfaceStartY + (MousePointer.Y - _mouseStartY));
        }

        public override PointerType GetPointerType()
        {
            return PointerType.Move;
        }

        /// <summary>
        /// The surface manager.
        /// </summary>
        private readonly SurfaceManager _surfaceManager;

        /// <summary>
        /// The surface being dragged.
        /// </summary>
        private readonly Surface _surface;

        /// <summary>
        /// The X coordinate of the surface when the operation started.
        /// </summary>
        private readonly int _surfaceStartX;

        /// <summary>
        /// The Y coordinate of the surface when the operation started.
        /// </summary>
        private readonly int _surfaceStartY;

        /// <summary>
        /// The X coordinate of the mouse when the operation started.
        /// </summary>
        private readonly int _mouseStartX;

        /// <summary>
        /// The Y coordinate of the mouse when the operation started.
        /// </summary>
        private readonly int _mouseStartY;
    }
}
