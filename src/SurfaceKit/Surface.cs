/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Drawing;
using Mirage.DataKit;
using Mirage.GraphicsKit;
using Mirage.InputKit;

namespace Mirage.SurfaceKit
{
    /// <summary>
    /// A surface to be displayed on the screen.
    /// </summary>
    public class Surface
    {
        /// <summary>
        /// Initialise a new <see cref="Surface"/>.
        /// </summary>
        /// <param name="surfaceManager">The SurfaceManager that manages the surface.</param>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="x">The X coordinate of the surface.</param>
        /// <param name="y">The Y coordinate of the surface.</param>
        /// <param name="titlebar">Whether to add a title bar to the surface.</param>
        /// <param name="shadow">Whether to add a shadow to the surface.</param>
        /// <param name="resizable">Whether the surface should be resizable by the user.</param>
        public Surface(SurfaceManager surfaceManager, ushort width, ushort height, int x, int y, bool titlebar = false, bool shadow = true, bool resizable = true)
        {
            SurfaceManager = surfaceManager;
            X = x;
            Y = y;
            Shadow = shadow;
            Resizable = resizable;
            Canvas = new Canvas(width, height);
            if (titlebar)
            {
                TitleBar = new TitleBar(this);
            }
            surfaceManager.AddSurface(this);
        }

        /// <summary>
        /// Render the surface to a target canvas.
        /// </summary>
        /// <param name="target">The target canvas to render on.</param>
        internal void Render(Canvas target)
        {
            if (Shadow && SurfaceManager.ShadowsEnabled)
            {
                target.DrawImage(X - SHADOW_RADIUS, Y - SHADOW_RADIUS, Resources.ShadowCornerTopLeft, Alpha: true);
                target.DrawImage(X + Canvas.Width, Y - SHADOW_RADIUS, Resources.ShadowCornerTopRight, Alpha: true);
                target.DrawImage(X - SHADOW_RADIUS, Y + Canvas.Height, Resources.ShadowCornerBottomLeft, Alpha: true);
                target.DrawImage(X + Canvas.Width, Y + Canvas.Height, Resources.ShadowCornerBottomRight, Alpha: true);
                for (int i = 0; i < Canvas.Height; i++)
                {
                    target.DrawImage(X - SHADOW_RADIUS, Y + i, Resources.ShadowEdgeLeft, Alpha: true);
                    target.DrawImage(X + Canvas.Width, Y + i, Resources.ShadowEdgeRight, Alpha: true);
                }
                for (int i = 0; i < Canvas.Width; i++)
                {
                    if (TitleBar == null)
                    {
                        target.DrawImage(X + i, Y - SHADOW_RADIUS, Resources.ShadowEdgeTop, Alpha: true);
                    }
                    target.DrawImage(X + i, Y + Canvas.Height, Resources.ShadowEdgeBottom, Alpha: true);
                }
            }

            // border
            if (BorderColor.A != 0)
            {
                target.DrawFilledRectangle(X - 1, Y - 1, (ushort)(Canvas.Width + 2), (ushort)(Canvas.Height + 2), 0, BorderColor);
            }

            // titlebar
            TitleBar?.Render(target);
        
            // content
            target.DrawImage(X, Y, Canvas, Alpha: false);
        }

        public void Resize(ushort width, ushort height)
        {
            // CAUSES MEMORY CORRUPTION FOR SOME REASON!
            // Canvas.Width = width;
            // Canvas.Height = height;
            // OnResized.Fire(new());
        }

        /// <summary>
        /// Get the rectangle of the surface.
        /// </summary>
        public Rectangle Rectangle => new Rectangle(X, Y, Canvas.Width, Canvas.Height);

        /// <summary>
        /// If the mouse is over the surface.
        /// </summary>
        public bool IsMouseOver => Rectangle.Contains(MousePointer.Location);

        /// <summary>
        /// If the mouse is over the surface's resize zone.
        /// </summary>
        public bool IsMouseOverResizeZone
        {
            get
            {
                Rectangle resizeZone = new Rectangle(X + Canvas.Width, Y + Canvas.Height, RESIZE_ZONE_SIZE, RESIZE_ZONE_SIZE);
                return resizeZone.Contains(MousePointer.Location);
            }
        }

        /// <summary>
        /// If the surface is focused.
        /// </summary>
        public bool Focused => SurfaceManager.Focus == this;

        /// <summary>
        /// If the surface should be focusable.
        /// </summary>
        public bool Focusable { get; set; } = true;

        /// <summary>
        /// If the surface should always appear on top of other surfaces.
        /// </summary>
        public bool AlwaysOnTop { get; set; } = false;

        /// <summary>
        /// If the surface can be raised.
        /// </summary>
        public bool CanRaise { get; set; } = true;

        /// <summary>
        /// The surface's designated PrismAPI canvas.
        /// </summary>
        public Canvas Canvas { get; init; }

        /// <summary>
        /// The SurfaceManager that manages the surface.
        /// </summary>
        public SurfaceManager SurfaceManager { get; init; }

        /// <summary>
        /// The surface's titlebar.
        /// </summary>
        public TitleBar? TitleBar { get; init; }

        /// <summary>
        /// Fired every frame.
        /// </summary>
        public readonly Signal<SignalArgs> OnUpdate = new();

        /// <summary>
        /// Fired when the mouse pointer is pressed on the surface.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseDown = new();

        /// <summary>
        /// Fired when the mouse pointer is dragged on the surface.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseDrag = new();

        /// <summary>
        /// Fired when the mouse pointer is moved on the surface.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseMove = new();

        /// <summary>
        /// Fired when the mouse pointer is released from the surface.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseRelease = new();

        /// <summary>
        /// Fired when a key is typed in the surface.
        /// </summary>
        public readonly Signal<KeyboardArgs> OnKeyTyped = new();

        /// <summary>
        /// Fired when the surface is clicked.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseClick = new();

        /// <summary>
        /// Fired when the surface is removed.
        /// </summary>
        public readonly Signal<SignalArgs> OnRemoved = new();

        /// <summary>
        /// Fired when the surface is resized.
        /// </summary>
        public readonly Signal<SignalArgs> OnResized = new();

        /// <summary>
        /// The type of mouse pointer that is displayed while hovering over the view.
        /// </summary>
        public PointerType Pointer { get; set; } = PointerType.Default;

        /// <summary>
        /// The X coordinate of the surface.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The Y coordinate of the surface.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The minimum width of the surface.
        /// </summary>
        public ushort MinimumWidth = 100;
        /// <summary>
        /// The minimum height of the surface.
        /// </summary>
        public ushort MinimumHeight = 100;

        /// <summary>
        /// The maximum width of the surface.
        /// </summary>
        public ushort MaximumWidth = 640;
        /// <summary>
        /// The maximum height of the surface.
        /// </summary>
        public ushort MaximumHeight = 480;

        /// <summary>
        /// Remove the surface from the associated surface manager.
        /// </summary>
        public void Remove() => SurfaceManager.RemoveSurface(this);

        /// <summary>
        /// The title of the surface.
        /// </summary>
        public string Title {
            get => _title;
            
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// The private backing cache for Title.
        /// </summary>
        private string _title = string.Empty;

        /// <summary>
        /// The size of the surface's resize zone.
        /// </summary>
        private const int RESIZE_ZONE_SIZE = 15;

        /// <summary>
        /// The border color of the surface. The border is only visible when the surface has a title bar.
        /// </summary>
        public GraphicsKit.Color BorderColor { get; set; } = new GraphicsKit.Color(0xFFBFBFBF);

        /// <summary>
        /// Whether the surface should have a shadow.
        /// </summary>
        public bool Shadow;

        /// <summary>
        /// If the surface allows resizing by the user.
        /// </summary>
        public bool Resizable { get; init; }

        /// <summary>
        /// If the surface is part of the system shell.
        /// </summary>
        public bool IsShell { get; set; } = false;

        /// <summary>
        /// If the surface should close when the user interacts with any surface outside of its bounds.
        /// </summary>
        public bool Modal { get; set; } = false;

        private const int SHADOW_RADIUS = 24;
    }
}
