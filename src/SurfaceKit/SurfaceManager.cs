/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Collections.Generic;
using Mirage.GraphicsKit;
using Mirage.GraphicsKit.Hardware;
using Mirage.InputKit;
using Cosmos.System;
using System;

namespace Mirage.SurfaceKit
{
    /// <summary>
    /// Manages and renders <see cref="Surface"/>s.
    /// </summary>
    public class SurfaceManager
    {
        /// <summary>
        /// Create a new surface manager and display.
        /// </summary>
        public SurfaceManager()
        {
            _display = Display.GetDisplay(640, 480);
            DisplaySplash();
            MouseManager.ScreenWidth = _display.Width;
            MouseManager.ScreenHeight = _display.Height;
            MouseManager.X = (uint)(_display.Width / 2);
            MouseManager.Y = (uint)(_display.Height / 2);
            Desktop = new DE.Desktop(this);
        }

        /// <summary>
        /// Display the splash screen.
        /// </summary>
        private void DisplaySplash()
        {
            _display.Clear(Color.Black);
            string message = "Starting Mirage...";
            int x = (_display.Width - GraphicsKit.Fonts.Font.Fallback.MeasureString(message)) / 2;
            int y = (_display.Height - GraphicsKit.Fonts.Font.Fallback.Size) / 2;
            _display.DrawString(x, y, message, GraphicsKit.Fonts.Font.Fallback, Color.White);
            _display.Update();
        }

        /// <summary>
        /// Handle user input.
        /// </summary>
        private void HandleInput()
        {
            if (_currentOperation != null)
            {
                _currentOperation.Update();
                return;
            }

            _hoveredTitleBar?.UpdateButtons();

            _hoveredTitleBar = null;

            _mouseIsOverResizeZone = false;

            // resize
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                Surface surface = _surfaces[i];

                if (surface.Resizable && surface.IsMouseOverResizeZone)
                {
                    _mouseIsOverResizeZone = true;

                    if (MouseManager.LastMouseState == MouseState.None &&
                        MouseManager.MouseState == MouseState.Left)
                    {
                        if (surface.Focusable)
                        {
                            Focus = surface;
                        }
                        else
                        {
                            BringToFront(surface);
                        }
                        BeginOperation(new SurfaceResizeOperation(this, surface));
                        return;
                    }
                }
            }

            // set focus
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                Surface surface = _surfaces[i];

                bool isMouseOverTitlebar = surface.TitleBar != null && IsTitleBarHovered(surface.TitleBar);

                if ((surface.IsMouseOver || isMouseOverTitlebar) &&
                    MouseManager.MouseState != MouseState.None)
                {
                    if (!surface.Modal)
                    {
                        for (int j = _surfaces.Count - 1; j >= 0; j--)
                        {
                            if (_surfaces[j].Modal && _surfaces[j] != surface)
                            {
                                BeginOperation(new ModalRemoveOperation(this, _surfaces[j]));
                                return;
                            }
                        }
                    }

                    if (surface.Focusable)
                    {
                        Focus = surface;
                    }
                    else
                    {
                        BringToFront(surface);
                    }
                    break;
                }
            }

            // titlebar stuff
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                Surface surface = _surfaces[i];

                if (surface.IsMouseOver)
                {
                    break;
                }

                bool isMouseOverTitlebar = surface.TitleBar != null && IsTitleBarHovered(surface.TitleBar);

                if (isMouseOverTitlebar)
                {
                    if (MouseManager.LastMouseState == MouseState.None &&
                        MouseManager.MouseState == MouseState.Left)
                    {
                        surface.TitleBar!.HandlePress(this);
                    }

                    _hoveredTitleBar = surface.TitleBar;
                    _hoveredTitleBar?.UpdateButtons();

                    break;
                }
            }

            // signalling
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                Surface surface = _surfaces[i];

                if (surface.IsMouseOver)
                {
                    if (MouseManager.LastMouseState == MouseState.None &&
                        MouseManager.MouseState != MouseState.None)
                    {
                        BeginOperation(new SurfacePressOperation(this, surface));
                    }

                    if (Math.Abs(MouseManager.DeltaX) > 0 || Math.Abs(MouseManager.DeltaY) > 0)
                    {
                        MouseArgs args = new MouseArgs(MousePointer.X, MousePointer.Y, MouseManager.MouseState);
                        args = args.Localize(surface.X, surface.Y);
                        surface.OnMouseMove.Fire(args);
                    }

                    break;
                }
            }

            // keys
            if (KeyboardManager.TryReadKey(out KeyEvent @event))
            {
                KeyboardArgs args = new KeyboardArgs(@event.Key, @event.KeyChar, @event.Modifiers);
                ActiveSurface?.OnKeyTyped.Fire(args);
            }
        }

        /// <summary>
        /// Check if a title bar is being hovered over.
        /// </summary>
        /// <param name="titlebar">The title bar.</param>
        public bool IsTitleBarHovered(TitleBar titlebar)
        {
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                Surface surface = _surfaces[i];

                if (surface.TitleBar != null && surface.TitleBar.Rectangle.Contains(MousePointer.Location))
                {
                    return titlebar == surface.TitleBar;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Send update signals to the surfaces.
        /// </summary>
        private void UpdateSurfaces()
        {
            foreach (Surface surface in _surfaces)
            {
                surface.OnUpdate.Fire(new());
            }
        }

        /// <summary>
        /// Get the pointer image for a pointer type.
        /// </summary>
        /// <param name="pointerType">The pointer type.</param>
        /// <returns>Pointer image for this pointer type.</returns>
        private Canvas GetImageForPointerType(PointerType pointerType)
        {
            return pointerType switch
            {
                PointerType.Resize => Resources.PointerResize,
                PointerType.Move => Resources.PointerMove,
                PointerType.IBeam => Resources.PointerIBeam,
                _ => Resources.Pointer,
            };
        }

        /// <summary>
        /// Get the drawing offset for a pointer type.
        /// </summary>
        /// <param name="pointerType">Pointer type.</param>
        /// <returns>Drawing offset.</returns>
        private (int X, int Y) GetOffsetForPointerType(PointerType pointerType)
        {
            return pointerType switch
            {
                PointerType.Resize => (-Resources.PointerResize.Width, -Resources.PointerResize.Height),
                PointerType.Move => (-(Resources.PointerMove.Width / 2), -(Resources.PointerMove.Height / 2)),
                PointerType.IBeam => (-(Resources.PointerIBeam.Width / 2), -(Resources.PointerIBeam.Height / 2)),
                _ => (0, 0),
            };
        }

        private void RenderPointer()
        {
            PointerType pointerType = PointerType.Default;
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                TitleBar? titleBar = _surfaces[i].TitleBar;
                if (titleBar != null && titleBar.Rectangle.Contains(MousePointer.Location))
                {
                    break;
                }
                if (_surfaces[i].IsMouseOver)
                {
                    pointerType = _surfaces[i].Pointer;
                    break;
                }
            }
            // if (_mouseIsOverResizeZone)
            // {
            //     pointerType = PointerType.Resize;
            // }
            if (_currentOperation != null)
            {
                pointerType = _currentOperation.GetPointerType();
            }
            (int X, int Y) pointerOffset = GetOffsetForPointerType(pointerType);
            Canvas pointerImage = GetImageForPointerType(pointerType);
            _display.DrawImage(MousePointer.X + pointerOffset.X, MousePointer.Y + pointerOffset.Y, pointerImage);
        }

        /// <summary>
        /// Render the screen.
        /// </summary>
        private void Render()
        {
            // _display.Clear(_backgroundColor);
            foreach (Surface surface in _surfaces)
            {
                surface.Render(_display);
            }
            RenderPointer();
            _display.Update();
        }

        /// <summary>
        /// Run sleep mode.
        /// </summary>
        private void HandleSleep()
        {
            _display.Clear(Color.Black);
            _display.Update();
            System.Console.ReadKey(true);
            _sleeping = false;
        }

        /// <summary>
        /// Update the surfaces and handle user input.
        /// </summary>
        public void Update()
        {
            if (_sleeping)
            {
                HandleSleep();
                return;
            }
            HandleInput();
            UpdateSurfaces();
            Render();
        }

        /// <summary>
        /// Remove all surfaces.
        /// </summary>
        /// <param name="removeShellSurfaces">If shell surfaces should be removed.</param>
        public void RemoveAllSurfaces(bool removeShellSurfaces = true)
        {
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                Surface surface = _surfaces[i];
                if (removeShellSurfaces || !surface.IsShell)
                {
                    _surfaces.RemoveAt(i);
                    // BUGBUG: Why does the below line cause a crash?
                    // surface.OnRemoved.Fire(new());
                }
            }
        }

        /// <summary>
        /// The currently focused surface.
        /// </summary>
        public Surface? Focus
        {
            get => _focus;

            set
            {
                if (value != null && !value.Focusable)
                {
                    return;
                }

                Surface? previousFocus = _focus;
                if (previousFocus == value)
                {
                    return;
                }
                
                _focus = value;

                previousFocus?.TitleBar?.PaintContentArea();

                if (_focus != null)
                {
                    _focus.TitleBar?.PaintContentArea();
                    BringToFront(_focus);
                }

                ActiveSurface = _focus;
            }
        }

        /// <summary>
        /// Get the active surface. Note that this is different from the focused surface.
        /// </summary>
        public Surface? ActiveSurface { get; private set; }

        private int GetSurfaceInsertionIndex()
        {
            int index = _surfaces.Count;
            for (int i = _surfaces.Count - 1; i >= 0; i--)
            {
                if (_surfaces[i].AlwaysOnTop)
                {
                    index--;
                }
                else
                {
                    return index;
                }
            }
            return index;
        }

        /// <summary>
        /// Bring a surface to the front.
        /// </summary>
        /// <param name="surface">The surface to bring to the front.</param>
        public void BringToFront(Surface surface)
        {
            if (!_surfaces.Contains(surface) ||
                _surfaces[^1] == surface ||
                !surface.CanRaise)
            {
                return;
            }

            _surfaces.Remove(surface);
            if (surface.AlwaysOnTop)
            {
                _surfaces.Add(surface);
            }
            else
            {
                _surfaces.Insert(GetSurfaceInsertionIndex(), surface);
            }

            ActiveSurface = surface;
        }

        /// <summary>
        /// Begin an interactive operation.
        /// If an operation is already active, this method will do nothing.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public void BeginOperation(Operation operation)
        {
            if (_currentOperation != null)
            {
                return;
            }
            
            _currentOperation = operation;
        }

        /// <summary>
        /// Cancel the current interactive operation, if any.
        /// </summary>
        public void CancelOperation()
        {
            _currentOperation = null;
        }

        /// <summary>
        /// Check whether the <see cref="SurfaceManager"/> contains a surface.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <returns>Whether or not the <see cref="SurfaceManager"/> contains the surface.</returns>
        public bool HasSurface(Surface surface)
        {
            return _surfaces.Contains(surface);
        }

        /// <summary>
        /// Add a surface at a specific location.
        /// </summary>
        /// <param name="surface">The surface to add.</param>
        /// <param name="x">The X coordinate to place the surface at.</param>
        /// <param name="y">The Y coordinate to place the surface at.</param>
        public void AddSurface(Surface surface)
        {
            if (HasSurface(surface))
            {
                return;
            }

            _surfaces.Insert(GetSurfaceInsertionIndex(), surface);
        }

        /// <summary>
        /// Remove a surface from the surface manager.
        /// </summary>
        /// <param name="surface">The surface to remove.</param>
        public void RemoveSurface(Surface surface)
        {
            _surfaces.Remove(surface);
            surface.OnRemoved.Fire(new());
        }

        /// <summary>
        /// Enter sleep mode.
        /// </summary>
        public void Sleep()
        {
            _sleeping = true;
        }

        /// <summary>
        /// If shadows are enabled.
        /// </summary>
        public bool ShadowsEnabled { get; set; } = true;

        /// <summary>
        /// The width of the surface manager's display.
        /// </summary>
        public int Width => _display.Width;

        /// <summary>
        /// The height of the surface manager's display.
        /// </summary>
        public int Height => _display.Height;
        
        /// <summary>
        /// The current interactive operation.
        /// </summary>
        private Operation? _currentOperation = null;

        /// <summary>
        /// The currently hovered title bar.
        /// </summary>
        private TitleBar? _hoveredTitleBar = null;

        /// <summary>
        /// The Display that surfaces will be rendered to.
        /// </summary>
        private readonly Display _display;

        /// <summary>
        /// A list of all surfaces tracked by the <see cref="SurfaceManager"/>, in back-to-front order.
        /// </summary>
        private readonly List<Surface> _surfaces = new List<Surface>();

        /// <summary>
        /// The background color.
        /// </summary>
        private readonly Color _backgroundColor = new Color(0xFF4C7ABF);

        /// <summary>
        /// The currently focused surface.
        /// </summary>
        private Surface? _focus = null;

        /// <summary>
        /// If the mouse is over the resize zone of any surface.
        /// </summary>
        private bool _mouseIsOverResizeZone = false;

        /// <summary>
        /// If the system is in sleep mode.
        /// </summary>
        private bool _sleeping = false;

        /// <summary>
        /// Desktop.
        /// </summary>
        public DE.Desktop? Desktop { get; init; }

        /// <summary>
        /// System menu bar.
        /// </summary>
        public DE.SystemMenu? SystemMenu { get; set; }
    }
}
