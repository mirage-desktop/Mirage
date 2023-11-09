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
    /// Interactive operation of a surface being pressed.
    /// </summary>
    class SurfacePressOperation : Operation
    {
        /// <summary>
        /// Initialise a new interactive surface press operation.
        /// </summary>
        /// <param name="surfaceManager">The surface manager.</param>
        /// <param name="surface">The surface being pressed.</param>
        public SurfacePressOperation(SurfaceManager surfaceManager, Surface surface)
        {
            _surfaceManager = surfaceManager;
            _surface = surface;

            SignalDown();
        }

        private void SignalDown()
        {
            MouseArgs args = new MouseArgs(MousePointer.X, MousePointer.Y, MouseManager.MouseState);
            args = args.Localize(_surface.X, _surface.Y);
            _surface.OnMouseDown.Fire(args);
        }

        private void SignalRelease()
        {
            MouseArgs args = new MouseArgs(MousePointer.X, MousePointer.Y, MouseManager.LastMouseState);
            args = args.Localize(_surface.X, _surface.Y);
            _surface.OnMouseRelease.Fire(args);
        }
    
        private void SignalDragAndMove()
        {
            MouseArgs args = new MouseArgs(MousePointer.X, MousePointer.Y, MouseManager.MouseState);
            args = args.Localize(_surface.X, _surface.Y);
            _surface.OnMouseMove.Fire(args);
            _surface.OnMouseDrag.Fire(args);
        }

        private void SignalClick()
        {
            MouseArgs args = new MouseArgs(MousePointer.X, MousePointer.Y, MouseManager.LastMouseState);
            args = args.Localize(_surface.X, _surface.Y);
            _surface.OnMouseClick.Fire(args);
        }

        /// <summary>
        /// Update the interactive surface press operation.
        /// </summary>
        public override void Update()
        {   if (!_surface.IsMouseOver)
            {
                _exited = true;
            }

            if (_previousMouseOverSurface == false && _surface.IsMouseOver == true)
            {
                SignalDown();
            }
            if (_previousMouseOverSurface == true && _surface.IsMouseOver == false)
            {
                SignalRelease();
            }

            if (MouseManager.MouseState == MouseState.None)
            {
                SignalRelease();
                if (!_exited)
                {
                    SignalClick();
                }
                _surfaceManager.CancelOperation();
                return;
            }

            if (Math.Abs(MouseManager.DeltaX) > 0 || Math.Abs(MouseManager.DeltaY) > 0)
            {
                SignalDragAndMove();
            }

            _previousMouseOverSurface = _surface.IsMouseOver;
        }

        public override PointerType GetPointerType()
        {
            return _surface.Pointer;
        }

        /// <summary>
        /// The surface manager.
        /// </summary>
        private readonly SurfaceManager _surfaceManager;

        /// <summary>
        /// The surface being pressed.
        /// </summary>
        private readonly Surface _surface;

        /// <summary>
        /// Whether the mouse was over the surface last frame.
        /// </summary>
        private bool _previousMouseOverSurface = true;

        /// <summary>
        /// If the mouse left the surface during the operation.
        /// </summary>
        private bool _exited = false;
    }
}
