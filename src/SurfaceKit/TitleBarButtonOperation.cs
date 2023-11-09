/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Drawing;
using Cosmos.System;
using Mirage.InputKit;

namespace Mirage.SurfaceKit
{
    /// <summary>
    /// Represents one of a title bar's buttons being pressed.
    /// </summary>
    public class TitleBarButtonOperation : Operation
    {
        /// <summary>
        /// Initialise a new interactive title bar button operation.
        /// </summary>
        /// <param name="surfaceManager">The surface manager.</param>
        /// <param name="button">The title bar.</param>
        /// <param name="button">The title bar button being pressed.</param>
        /// <param name="buttonScreenRect">The rectangle of the button, in screen coordinates.</param>
        public TitleBarButtonOperation(SurfaceManager surfaceManager, TitleBar titleBar, TitleBarButton button, Rectangle buttonScreenRect)
        {
            _surfaceManager = surfaceManager;
            _titleBar = titleBar;
            _button = button;
            _buttonScreenRect = buttonScreenRect;

            _titleBar.UpdateButton(_button);
        }

        /// <summary>
        /// Update the interactive title bar button operation.
        /// </summary>
        public override void Update()
        {
            bool mouseDown = MouseManager.MouseState == MouseState.Left;
            bool mouseOverButton = _buttonScreenRect.Contains(MousePointer.Location);

            if (mouseOverButton == false)
            {
                _exited = true;
            }

            _titleBar.UpdateButton(_button);

            if (!mouseDown)
            {
                if (mouseOverButton && !_exited)
                {
                    _button.Callback?.Invoke();
                }
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

        /// <summary>
        /// The title bar button being pressed.
        /// </summary>
        private readonly TitleBar _titleBar;

        /// <summary>
        /// The title bar button being pressed.
        /// </summary>
        private readonly TitleBarButton _button;

        /// <summary>
        /// The rectangle of the button, in screen coordinates.
        /// </summary>
        private readonly Rectangle _buttonScreenRect;

        /// <summary>
        /// If the mouse left the button during the operation.
        /// </summary>
        private bool _exited = false;
    }
}
