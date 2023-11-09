/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Collections.Generic;
using System.Drawing;
using Mirage.DataKit;
using Mirage.InputKit;
using Mirage.SurfaceKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// A view in the scene graph.
    /// </summary>
    public class UIView
    {
        /// <summary>
        /// Add a <see cref="UIView"/> to this view.
        /// </summary>
        /// <param name="view">The view to add.</param>
        public void Add(UIView view)
        {
            if (!_children.Contains(view))
            {
                _children.Add(view);
                view.Window = Window;
                view._parent = this;
                Window?.QueuePaint(RootRectangle);
            }
        }

        /// <summary>
        /// Remove a <see cref="UIView"/> from this view.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        public void Remove(UIView view)
        {
            if (_children.Contains(view))
            {
                _children.Remove(view);
                view._parent = null;
            }
        }

        // /// <summary>
        // /// Queue the view and its descendants to be painted by the window.
        // /// </summary>
        // public void RecursiveQueuePaint()
        // {
        //     QueuePaint();
        //     //todo
        // }

        /// <summary>
        /// Queue the view to be painted by the window.
        /// </summary>
        public void QueuePaint()
        {
            if (PreviousRenderRect != RootRectangle)
            {
                Window?.QueuePaint(PreviousRenderRect);
            }
            Window?.QueuePaint(RootRectangle);
        }

        /// <summary>
        /// Paint the view on the window's surface.
        /// Calling this will paint the view on top of all other views irrespective to the window background, views behind it, or views in front of it.
        /// Unless you have a specific reason to call this method, you should call QueuePaint.
        /// </summary>
        public virtual void PaintSelf()
        {
            PreviousRenderRect = RootRectangle;
            Rectangle rr = RootRectangle;
            #if MIRAGE_UIKIT_UIVIEW_DEBUGBOUNDS
            Window?.Surface.Canvas.DrawRectangle(rr.X, rr.Y, (ushort)(rr.Width - 1), (ushort)(rr.Height - 1), 0, GraphicsKit.Color.Red);
            #endif
        }

        // /// <summary>
        // /// Render the view and its children.
        // /// </summary>
        // public void Render()
        // {
        //     if (Window == null)
        //     {
        //         return;
        //     }

        //     RenderSelf();
        //     foreach (UIView child in _children)
        //     {
        //         child.Render();
        //     }
        // }

        /// <summary>
        /// Get the view's descendants.
        /// </summary>
        /// <returns>A list of all of the descendants of this view.</returns>
        public List<UIView> GetDescendants()
        {
            List<UIView> descendants = new List<UIView>();
            
            foreach (UIView child in _children)
            {
                descendants.Add(child);
                descendants.AddRange(child.GetDescendants());
            }

            return descendants;
        }

        /// <summary>
        /// The parent of this <see cref="UIView"/>.
        /// </summary>
        public UIView? Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (value == _parent)
                {
                    return;
                }

                _parent?.Remove(this);
                value?.Add(this);
                _parent = value;
            }
        }

        /// <summary>
        /// The view's offset relative to its parent.
        /// </summary>
        public Point Location
        {
            get => _location;

            set
            {
                if (_location != value)
                {
                    _location = value;
                    QueuePaint();
                }
            }
        }

        /// <summary>
        /// The view's location relative to the root view.
        /// </summary>
        public Point RootLocation
        {
            get
            {
                if (_parent == null)
                {
                    return Location;
                }

                Point rootLocation = _parent.RootLocation;
                rootLocation.Offset(Location);

                return rootLocation;
            }
        }

        /// <summary>
        /// The view's location relative to the screen.
        /// </summary>
        public Point ScreenLocation
        {
            get
            {
                if (_window == null)
                {
                    return RootLocation;
                }

                Point screenLocation = RootLocation;
                screenLocation.Offset(new Point(_window.Surface.X, _window.Surface.Y));

                return screenLocation;
            }
        }

        /// <summary>
        /// Get the implicit size of the view.
        /// </summary>
        /// <returns>The implicit size of the view.</returns>
        protected virtual Size GetImplicitSize() => new Size(100, 100);

        /// <summary>
        /// The actual size of the view.
        /// If either the ExplicitWidth or ExplicitHeight are null, those dimensions will take the implicit size of the view.
        /// If FillWidth or FillHeight are true and the parents width or height are non-zero, they will override both the implicit and explicit sizes of their respective dimensions and take the size of the parent view.
        /// </summary>
        public Size Size
        {
            get
            {
                Size implicitSize = GetImplicitSize();
                int width = (FillWidth && Parent != null && Parent.Size.Width > 0) ? Parent.Size.Width : (ExplicitWidth ?? implicitSize.Width);
                int height = (FillHeight && Parent != null && Parent.Size.Height > 0) ? Parent.Size.Height : (ExplicitHeight ?? implicitSize.Height);
                return new Size(width, height);
            }
        }

        /// <summary>
        /// The explicit width of the view.
        /// </summary>
        public int? ExplicitWidth
        {
            get => _explicitWidth;
            set
            {
                Size oldSize = Size;
                _explicitWidth = value;
                if (oldSize.Width != Size.Width)
                {
                    OnSizeChanged.Fire(new SignalArgs());
                }
            }
        }

        /// <summary>
        /// The explicit height of the view.
        /// </summary>
        public int? ExplicitHeight
        {
            get => _explicitHeight;
            set
            {
                Size oldSize = Size;
                _explicitHeight = value;
                if (oldSize.Height != Size.Height)
                {
                    OnSizeChanged.Fire(new SignalArgs());
                }
            }
        }
        
        /// <summary>
        /// If the view should fill its parents width.
        /// </summary>
        public bool FillWidth
        {
            get => _fillWidth;
            set
            {
                Size oldSize = Size;
                _fillWidth = value;
                if (oldSize.Width != Size.Width)
                {
                    OnSizeChanged.Fire(new SignalArgs());
                }
            }
        }
        
        /// <summary>
        /// If the view should fill its parents width.
        /// </summary>
        public bool FillHeight
        {
            get => _fillHeight;
            set
            {
                Size oldSize = Size;
                _fillHeight = value;
                if (oldSize.Height != Size.Height)
                {
                    OnSizeChanged.Fire(new SignalArgs());
                }
            }
        }

        /// <summary>
        /// The explicit size of the view.
        /// </summary>
        public Size? ExplicitSize
        {
            set
            {
                Size oldSize = Size;
                if (value != null)
                {
                    _explicitWidth = value.Value.Width;
                    _explicitHeight = value.Value.Height;
                }
                else
                {
                    _explicitWidth = null;
                    _explicitHeight = null;
                }
                if (oldSize != Size)
                {
                    OnSizeChanged.Fire(new SignalArgs());
                }
            }
        }

        /// <summary>
        /// The implicit size of the view.
        /// </summary>
        public Size ImplicitSize
        {
            get => GetImplicitSize();
        }

        /// <summary>
        /// The window the view is in.
        /// </summary>
        public UIWindow? Window {
            get => _window;
            set
            {
                _window = value;
                foreach (UIView child in _children)
                {
                    child.Window = value;
                }
            }
        }

        /// <summary>
        /// The rectangle of the view in parent-relative coordinates.
        /// </summary>
        public Rectangle Rectangle {
            get
            {
                Size size = Size;
                return new Rectangle(
                    Location.X,
                    Location.Y,
                    size.Width,
                    size.Height
                );
            }
        }

        /// <summary>
        /// The rectangle of the view in root-relative coordinates.
        /// </summary>
        public Rectangle RootRectangle {
            get
            {
                Point rootLocation = RootLocation;
                Size size = Size;
                return new Rectangle(
                    rootLocation.X,
                    rootLocation.Y,
                    size.Width,
                    size.Height
                );
            }
        }

        /// <summary>
        /// Get the pointer type to be displayed when the pointer enters the view.
        /// </summary>
        /// <returns>The pointer type.</returns>
        public virtual PointerType GetPointerType()
        {
            return PointerType.Default;
        }

        /// <summary>
        /// The rectangle of the view at the time of the last render, in root-relative coordinates.
        /// </summary>
        public Rectangle PreviousRenderRect { get; set; } = Rectangle.Empty;

        /// <summary>
        /// Fired when the size of the view changes.
        /// </summary>
        public readonly Signal<SignalArgs> OnSizeChanged = new();

        /// <summary>
        /// Fired when the mouse pointer is pressed on the view.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseDown = new();

        /// <summary>
        /// Fired when the mouse pointer is dragged on the view.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseDrag = new();

        /// <summary>
        /// Fired when the mouse pointer is released from the view.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseRelease = new();
        
        // /// <summary>
        // /// Fired when the mouse pointer enters the view.
        // /// </summary>
        // public readonly Signal<MouseArgs> OnMouseEnter = new();
        
        // /// <summary>
        // /// Fired when the mouse pointer leaves the view.
        // /// </summary>
        // public readonly Signal<MouseArgs> OnMouseLeave = new();

        /// <summary>
        /// Fired when a key is typed in the view.
        /// </summary>
        public readonly Signal<KeyboardArgs> OnKeyTyped = new();

        /// <summary>
        /// Fired when the view is clicked.
        /// </summary>
        public readonly Signal<MouseArgs> OnMouseClick = new();

        /// <summary>
        /// The private backing cache for Location.
        /// </summary>
        private Point _location = new();
        
        /// <summary>
        /// The private backing cache for ExplicitWidth.
        /// </summary>
        private int? _explicitWidth = null;
        
        /// <summary>
        /// The private backing cache for ExplicitHeight.
        /// </summary>
        private int? _explicitHeight = null;
        
        /// <summary>
        /// The private backing cache for FillWidth.
        /// </summary>
        private bool _fillWidth = false;
        
        /// <summary>
        /// The private backing cache for FillHeight.
        /// </summary>
        private bool _fillHeight = false;

        /// <summary>
        /// The parent of the view. Do not set this field directly; rather use the public Parent property.
        /// </summary>
        protected UIView? _parent = null;

        /// <summary>
        /// The children of the view. Do not modify this list directly; rather use the appropriate public methods.
        /// </summary>
        protected readonly List<UIView> _children = new List<UIView>();

        /// <summary>
        /// The private backing cache for Window.
        /// </summary>
        private UIWindow? _window;
    }
}
