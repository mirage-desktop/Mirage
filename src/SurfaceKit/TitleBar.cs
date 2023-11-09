/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;
using Mirage.GraphicsKit;
using Mirage.InputKit;
using Mirage.TextKit;

namespace Mirage.SurfaceKit
{
    /// <summary>
    /// The title bar of a surface.
    /// </summary>
    public class TitleBar
    {
        /// <summary>
        /// Initialise a new title bar for a surface.
        /// </summary>
        /// <param name="surface">The surface the title bar belongs to.</param>
        public TitleBar(Surface surface)
        {
            _surface = surface;
            _title = surface.Title;
            _contentAreaCanvas = new Canvas(ContentAreaWidth, TITLEBAR_HEIGHT);

            _buttons.Add(new TitleBarButton(Resources.Close, Resources.Close_Hover, Resources.Close_Press, CloseButtonClicked));
            if (_surface.Resizable)
            {
                _buttons.Add(new TitleBarButton(Resources.Max, Resources.Max_Hover, Resources.Max_Press, () => {}));
                _buttons.Add(new TitleBarButton(Resources.Min, Resources.Min_Hover, Resources.Min_Press, () => {}));
            }
            LayOutButtons();

            PaintContentArea();
        }

        /// <summary>
        /// Handle the close button of the title bar being clicked.
        /// </summary>
        private void CloseButtonClicked() => _surface.Remove();

        /// <summary>
        /// The width of the content area.
        /// </summary>
        private ushort ContentAreaWidth =>
            (ushort)(Math.Max(0, _surface.Canvas.Width - Resources.TitleBar_Left.Width - Resources.TitleBar_Right.Width) + 2);

        /// <summary>
        /// Paint the background of the content area.
        /// </summary>
        private void PaintContentAreaBackground()
        {
            for (int i = 0; i < _contentAreaCanvas.Width; i++)
            {
                _contentAreaCanvas.DrawImage(i, 0, Resources.TitleBar_Middle, Alpha: false);
            }
        }

        /// <summary>
        /// Paint the title of the surface.
        /// </summary>
        private void PaintTitle()
        {
            if (_title == string.Empty)
            {
                return;
            }

            TextStyle style = _surface.Focused ? TITLE_STYLE_FOCUSED : TITLE_STYLE_UNFOCUSED;

            TextBlock block = new TextBlock(style, _title);
            TextLayout layout = TextLayout.LayOut(block, _contentAreaCanvas.Width, false, TextAlignment.Center);

            TextRenderer renderer = new TextRenderer();
            renderer.Render(layout, _contentAreaCanvas, 0, 0);
        }

        private void LayOutButtons()
        {
            int x = _contentAreaCanvas.Width;
            for (int i = 0; i < _buttons.Count; i++)
            {
                TitleBarButton button = _buttons[i];
                x -= button.Width;
                button.X = x;
                button.Y = (_contentAreaCanvas.Height - button.Height) / 2;
                x -= BUTTON_SPACING;
            }
        }

        /// <summary>
        /// Paint the title bar's buttons.
        /// </summary>
        private void PaintButtons()
        {
            foreach (TitleBarButton button in _buttons)
            {
                button.Render(_contentAreaCanvas, _surface.Focused);
            }
        }

        /// <summary>
        /// Update the state of a title bar button.
        /// </summary>
        /// <param name="button">The button whose state should be updated.</param>
        /// <param name="button">Whether to repaint the titlebar's content area if the state changes.</param>
        /// <returns>If a state change occurred.</returns>
        public bool UpdateButton(TitleBarButton button, bool doRepaint = true)
        {
            bool stateChanged = false;
            bool newHovered = _surface.SurfaceManager.IsTitleBarHovered(this) &&
                GetButtonScreenRect(button).Contains(MousePointer.Location);
            bool newPressed = newHovered && MouseManager.MouseState == MouseState.Left;
            if (button.Hovered != newHovered || button.Pressed != newPressed)
            {
                stateChanged = true;
            }
            button.Hovered = newHovered;
            button.Pressed = newPressed;
            if (stateChanged && doRepaint)
            {
                PaintContentArea();
            }
            return stateChanged;
        }

        /// <summary>
        /// Update the state of the titlebar's buttons.
        /// </summary>
        public void UpdateButtons()
        {
            bool doRepaint = false;
            foreach (TitleBarButton button in _buttons)
            {
                bool stateChanged = UpdateButton(button, doRepaint: false);
                if (stateChanged)
                {
                    doRepaint = true;
                }
            }
            if (doRepaint)
            {
                PaintContentArea();
            }
        }

        /// <summary>
        /// Repaint the content area of the title bar.
        /// </summary>
        public void PaintContentArea()
        {
            PaintContentAreaBackground();
            PaintTitle();
            PaintButtons();
        }

        /// <summary>
        /// Get the rectangle of a title bar button, relative to the screen.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>The rectangle of the button, relative to the screen.</returns>
        private Rectangle GetButtonScreenRect(TitleBarButton button)
        {
            return new Rectangle(
                button.X + Resources.TitleBar_Left.Width + Rectangle.X,
                button.Y + Rectangle.Y,
                button.Width,
                button.Height
            );
        }

        /// <summary>
        /// Handle a press on the title bar.
        /// </summary>
        /// <param name="surfaceManager">The surface manager.</param>
        public void HandlePress(SurfaceManager surfaceManager)
        {
            // content area relative coordinates
            int x = (int)(MouseManager.X - Rectangle.X - Resources.TitleBar_Left.Width);
            int y = (int)(MouseManager.Y - Rectangle.Y);

            foreach (TitleBarButton button in _buttons)
            {
                if (x >= button.X && y >= button.Y && x < button.X + button.Width && y < button.Y + button.Height)
                {
                    Rectangle buttonScreenRect = GetButtonScreenRect(button);
                    TitleBarButtonOperation operation = new TitleBarButtonOperation(surfaceManager, this, button, buttonScreenRect);
                    surfaceManager.BeginOperation(operation);
                    return;
                }
            }
            surfaceManager.BeginOperation(new SurfaceDragOperation(surfaceManager, _surface));
        }

        /// <summary>
        /// Render the titlebar.
        /// </summary>
        /// <param name="target">The target canvas to render the titlebar to.</param>
        public void Render(Canvas target)
        {
            bool repaintContentArea = false;
            if (_contentAreaCanvas.Width != ContentAreaWidth)
            {
                _contentAreaCanvas.Width = ContentAreaWidth;
                LayOutButtons();
                repaintContentArea = true;
            }
            if (_title != _surface.Title)
            {
                _title = _surface.Title;
                repaintContentArea = true;
            }
            if (repaintContentArea)
            {
                PaintContentArea();
            }

            int x = _surface.X - 1; // 1 for window border
            int y = _surface.Y - TITLEBAR_HEIGHT;

            // Left edge.
            target.DrawImage(x, y, Resources.TitleBar_Left, Alpha: true);

            // Content area.
            target.DrawImage(x + Resources.TitleBar_Left.Width, y, _contentAreaCanvas, Alpha: false);

            // Right edge.
            target.DrawImage(x + Resources.TitleBar_Left.Width + _contentAreaCanvas.Width, y, Resources.TitleBar_Right, Alpha: true);
        }

        /// <summary>
        /// Get the rectangle of the title bar, relative to the screen.
        /// </summary>
        public Rectangle Rectangle => new Rectangle(_surface.X, _surface.Y - TITLEBAR_HEIGHT, _surface.Canvas.Width, TITLEBAR_HEIGHT);

        /// <summary>
        /// The total height of the titlebar.
        /// </summary>
        public const int TITLEBAR_HEIGHT = 24;

        /// <summary>
        /// The spacing between buttons in the title bar.
        /// </summary>
        private const int BUTTON_SPACING = 2;

        /// <summary>
        /// The title bar's buttons.
        /// </summary>
        private readonly List<TitleBarButton> _buttons = new List<TitleBarButton>();

        /// <summary>
        /// The rich text style of the titlebar's title when the window is focused.
        /// </summary>
        private static readonly TextStyle TITLE_STYLE_FOCUSED = new TextStyle(Resources.Cantarell, GraphicsKit.Color.Black);

        /// <summary>
        /// The rich text style of the titlebar's title when the window is unfocused..
        /// </summary>
        private static readonly TextStyle TITLE_STYLE_UNFOCUSED = new TextStyle(Resources.Cantarell, GraphicsKit.Color.LightGray);

        // /// <summary>
        // /// The rich text style of the titlebar's title shadow.
        // /// </summary>
        // private readonly TextStyle TITLE_SHADOW_STYLE = new TextStyle(Resources.Cantarell, new GraphicsKit.Color(127, 255, 255, 255));

        /// <summary>
        /// The surface this title bar belongs to.
        /// </summary>
        private Surface _surface;

        /// <summary>
        /// The canvas of the titlebar's content area (i.e. the whole thing, excluding the left and right edges.)
        /// </summary>
        private Canvas _contentAreaCanvas;

        /// <summary>
        /// The title displayed in the titlebar.
        /// </summary>
        private string _title;
    }
}
