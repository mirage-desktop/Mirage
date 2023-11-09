/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
namespace Mirage.SurfaceKit
{
    /// <summary>
    /// Describes the visual type of a mouse cursor.
    /// </summary>
    public enum PointerType
    {
        /// <summary>
        /// Default pointer.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Moving pointer.
        /// </summary>
        Move = 1,

        /// <summary>
        /// Resizing pointer.
        /// </summary>
        Resize = 2,

        /// <summary>
        /// I-beam pointer.
        /// </summary>
        IBeam = 3,
    }
}
