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
    /// Represents a surface being interactively resized.
    /// </summary>
    public class SurfaceResizeOperation : Operation
    {
        /// <summary>
        /// Initialise a new resize operation.
        /// </summary>
        /// <param name="surfaceManager">The surface manager.</param>
        /// <param name="surface">The surface being resized.</param>
        public SurfaceResizeOperation(SurfaceManager surfaceManager, Surface surface)
        {
            if (!surface.Resizable)
            {
                return;
            }

            _surfaceManager = surfaceManager;
            _surface = surface;
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
            
            ushort newWidth = (ushort)(MousePointer.X - _surface.X);
            ushort newHeight = (ushort)(MousePointer.Y - _surface.Y);

            newWidth = Math.Clamp(newWidth, _surface.MinimumWidth, _surface.MaximumWidth);
            newHeight = Math.Clamp(newHeight, _surface.MinimumHeight, _surface.MaximumHeight);

            _surface.Resize(newWidth, newHeight);
        }

        public override PointerType GetPointerType()
        {
            return PointerType.Resize;
        }

        /// <summary>
        /// The surface manager.
        /// </summary>
        private readonly SurfaceManager _surfaceManager;

        /// <summary>
        /// The surface being resized.
        /// </summary>
        private readonly Surface _surface;
    }
}
