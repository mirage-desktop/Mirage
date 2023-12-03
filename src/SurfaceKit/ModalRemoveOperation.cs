/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Cosmos.System;

namespace Mirage.SurfaceKit
{
    /// <summary>
    /// Represents a modal being removed by interacting with a surface outside of its bounds.
    /// </summary>
    public class ModalRemoveOperation : Operation
    {
        /// <summary>
        /// Initialise a new modal remove operation.
        /// </summary>
        /// <param name="surfaceManager">The surface manager.</param>
        /// <param name="modal">The modal being removed.</param>
        public ModalRemoveOperation(SurfaceManager surfaceManager, Surface modal)
        {
            _surfaceManager = surfaceManager;
            modal.Remove();
        }

        /// <summary>
        /// Update the interactive modal remove operation.
        /// </summary>
        public override void Update()
        {
            if (MouseManager.MouseState == MouseState.None)
            {
                _surfaceManager.CancelOperation();
                return;
            }
        }

        public override PointerType GetPointerType()
        {
            return PointerType.Default;
        }

        /// <summary>
        /// The surface manager.
        /// </summary>
        private readonly SurfaceManager _surfaceManager;
    }
}
