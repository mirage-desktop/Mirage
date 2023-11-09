/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
namespace Mirage.SurfaceKit
{
    /// <summary>
    /// An interactive operation.
    /// </summary>
    public abstract class Operation
    {
        /// <summary>
        /// Update the interactive operation.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Get the pointer's display type for this operation.
        /// </summary>
        /// <returns>Pointer type.</returns>
        public abstract PointerType GetPointerType();
    }
}
