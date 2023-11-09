/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
// #define MIRAGE_UIKIT_UIWINDOW_DEBUGINFO

using Mirage.SurfaceKit;
using System.Drawing;
using System.Collections.Generic;
using Mirage.DataKit;
using Mirage.InputKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// A window with a titlebar and content.
    /// </summary>
    public class UIWindow
    {
        /// <summary>
        /// Create a new window.
        /// </summary>
        /// <param name="surfaceManager">The surface manager to add the window to.</param>
        /// <param name="width">The width of the new window.</param>
        /// <param name="height">The height of the new window.</param>
        /// <param name="title">The title of the window.</param>
        /// <param name="titlebar">If the window should have a title bar.</param>
        /// <param name="resizable">If the window should be resizable.</param>
        public UIWindow(SurfaceManager surfaceManager, ushort width, ushort height, string title = "Window", bool titlebar = true, bool resizable = true)
        {
            Surface = new Surface(
                surfaceManager,
                width,
                height,
                (surfaceManager.Width - width) / 2,
                (surfaceManager.Height - height) / 2,
                titlebar: titlebar,
                resizable: resizable)
            {
                Title = title
            };
            RootView = new UIView
            {
                ExplicitSize = new Size(width, height),
                Window = this
            };
            Surface.OnUpdate.Bind(HandleSurfaceUpdate);
            Surface.OnMouseDown.Bind(HandleSurfaceMouseDown);
            Surface.OnMouseRelease.Bind(HandleSurfaceMouseRelease);
            Surface.OnMouseDrag.Bind(HandleSurfaceMouseDrag);
            Surface.OnKeyTyped.Bind(HandleSurfaceKeyTyped);
            Surface.OnResized.Bind(HandleSurfaceResized);
            Surface.OnMouseMove.Bind(HandleSurfaceMouseMove);
            RootView.QueuePaint();
        }

        /// <summary>
        /// Get the view at the specified point relative to the window's surface.
        /// </summary>
        /// <param name="Point">The point.</param>
        /// <returns>The view at the specified point, if any.</returns>
        private UIView? GetViewAt(Point point)
        {
            List<UIView> views = RootView.GetDescendants();
            for (int i = views.Count - 1; i >= 0; i--)
            {
                UIView view = views[i];

                if (view.RootRectangle.Contains(point))
                {
                    return view;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Handle the surface's OnMouseDown signal and dispatch it to the appropriate view.
        /// </summary>
        /// <param name="args">Signal arguments.</param>
        public void HandleSurfaceMouseDown(MouseArgs args)
        {
            UIView? view = GetViewAt(args.Point);
            if (view != null)
            {
                FocusedView = view;
                _pressedView = view;
                MouseArgs localizedArgs = args.Localize(view.RootLocation);
                view.OnMouseDown.Fire(localizedArgs);
            }
        }
        
        /// <summary>
        /// Handle the surface's OnMouseRelease signal and dispatch it to the appropriate view.
        /// </summary>
        /// <param name="args">Signal arguments.</param>
        public void HandleSurfaceMouseRelease(MouseArgs args)
        {
            if (_pressedView != null)
            {
                MouseArgs localizedArgs = args.Localize(_pressedView.RootLocation);
                _pressedView.OnMouseRelease.Fire(localizedArgs);
                if (_pressedView.RootRectangle.Contains(new Point(MousePointer.X - Surface.X, MousePointer.Y - Surface.Y)))
                {
                    _pressedView.OnMouseClick.Fire(localizedArgs); // TODOTODO make handling of this better
                }
            }
        }
        
        /// <summary>
        /// Handle the surface's OnMouseDrag signal and dispatch it to the appropriate view.
        /// </summary>
        /// <param name="args">Signal arguments.</param>
        public void HandleSurfaceMouseDrag(MouseArgs args)
        {
            UIView? view = GetViewAt(args.Point);
            if (view != null)
            {
                MouseArgs localizedArgs = args.Localize(view.RootLocation);
                view.OnMouseDrag.Fire(localizedArgs);
            }
        }

        /// <summary>
        /// Handle the surface's OnMouseMove signal.
        /// </summary>
        /// <param name="args">Signal arguments.</param>
        public void HandleSurfaceMouseMove(MouseArgs args)
        {
            UIView? view = GetViewAt(args.Point);
            if (view != null)
            {
                Surface.Pointer = view.GetPointerType();
            }
            else
            {
                Surface.Pointer = PointerType.Default;
            }
        }

        /// <summary>
        /// Handle the surface's OnKeyTyped signal and dispatch it to the appropriate view.
        /// </summary>
        /// <param name="args">Signal arguments.</param>
        public void HandleSurfaceKeyTyped(KeyboardArgs args)
        {
            FocusedView?.OnKeyTyped.Fire(args);
        }

        /// <summary>
        /// Handle the surface's OnResized signal.
        /// </summary>
        /// <param name="args">Signal arguments.</param>
        public void HandleSurfaceResized(SignalArgs args)
        {
            QueuePaint(new Rectangle(0, 0, Surface.Canvas.Width, Surface.Canvas.Height));
        }
        
        /// <summary>
        /// Clear a rectangle of the window's background.
        /// </summary>
        /// <param name="rectangle">The rectangle to clear.</param>
        public void ClearRect(Rectangle rectangle)
        {    
            Surface.Canvas.DrawFilledRectangle(rectangle.X, rectangle.Y, (ushort)rectangle.Width, (ushort)rectangle.Height, 0, _backgroundColor);

            // moved to SKSurface
            // // Left border.
            // if (rectangle.X <= 0 && rectangle.Right >= 0)
            // {
            //     Surface.Canvas.DrawLine(0, rectangle.Y, 0, rectangle.Bottom, _borderColor);
            // }

            // // Right border.
            // if (rectangle.X <= Surface.Canvas.Width - 1 && rectangle.Right >= Surface.Canvas.Width - 1)
            // {
            //     Surface.Canvas.DrawLine(Surface.Canvas.Width - 1, rectangle.Y, Surface.Canvas.Width - 1, rectangle.Bottom, _borderColor);
            // }

            // // Bottom border.
            // if (rectangle.Y <= 0 && rectangle.Bottom >= 0)
            // {
            //     Surface.Canvas.DrawLine(rectangle.X, 0, rectangle.Right, 0, _borderColor);
            // }

            // // Bottom border.
            // if (rectangle.Y <= Surface.Canvas.Height - 1 && rectangle.Bottom >= Surface.Canvas.Height - 1)
            // {
            //     Surface.Canvas.DrawLine(rectangle.X, Surface.Canvas.Height - 1, rectangle.Right, Surface.Canvas.Height - 1, _borderColor);
            // }
        }

        /// <summary>
        /// Handle the surface's OnUpdate signal.
        /// </summary>
        private void HandleSurfaceUpdate(SignalArgs args)
        {
            if (_queuedPaintRects.Count == 0)
            {
                return;
            }

            // List<Rectangle> extraRectsToClear = new List<Rectangle>();

            List<UIView> views = RootView.GetDescendants();

            // foreach (UIView view in views)
            // {
            //     Rectangle viewRect = view.RootRectangle;
            //     foreach (Rectangle rect in _queuedPaintRects)
            //     {
            //         if (viewRect.IntersectsWith(rect))
            //         {
            //             extraRectsToClear.Add(viewRect);
            //             break;
            //         }
            //     }
            // }

            foreach (Rectangle rect in _queuedPaintRects)
            {
                ClearRect(rect);
            }
            // foreach (Rectangle rect in extraRectsToClear)
            // {
            //     ClearRect(rect);
            // }

            #if MIRAGE_UIKIT_UIWINDOW_DEBUGINFO
            int paints = 0;
            #endif
            foreach (UIView view in views)
            {
                foreach (Rectangle rect in _queuedPaintRects)
                {
                    if (view.RootRectangle.IntersectsWith(rect))
                    {
                        view.PaintSelf();
                        #if MIRAGE_UIKIT_UIWINDOW_DEBUGINFO
                        paints++;
                        #endif
                        break;
                    }
                }
            }

            #if MIRAGE_UIKIT_UIWINDOW_DEBUGINFO
            Surface.Canvas.DrawFilledRectangle(0, 0, 96, 33, 0, GraphicsKit.Color.Black);
            Surface.Canvas.DrawString(0, 0, $"clears: {_queuedPaintRects.Count + extraRectsToClear.Count}\npaints: {paints}", GraphicsKit.Fonts.Font.Fallback, GraphicsKit.Color.Red);
            #endif

            _queuedPaintRects.Clear();
        }

        /// <summary>
        /// Queue a rectangle to be cleared and the views inside it rendered.
        /// </summary>
        /// <param name="rect">The rectangle, in window coordinates.</param>
        public void QueuePaint(Rectangle rect)
        {
            foreach (Rectangle other in _queuedPaintRects)
            {
                if (rect.Equals(other))
                {
                    return;
                }
            }
            
            _queuedPaintRects.Add(rect);
        }

        private readonly List<Rectangle> _queuedPaintRects = new List<Rectangle>(100);

        /// <summary>
        /// The surface of the window.
        /// </summary>
        public Surface Surface { get; init; }

        /// <summary>
        /// The size of the window's surface.
        /// </summary>
        public Size Size => new Size(Surface.Canvas.Width, Surface.Canvas.Height);

        /// <summary>
        /// The root view of the window.
        /// </summary>
        public readonly UIView RootView;

        /// <summary>
        /// The currently focused view.
        /// </summary>
        public UIView? FocusedView { get; private set; } = null;

        /// <summary>
        /// The background color of the window.
        /// </summary>
        public GraphicsKit.Color BackgroundColor
        {
            get => _backgroundColor;
            set {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    QueuePaint(new Rectangle(0, 0, Surface.Canvas.Width, Surface.Canvas.Height));
                }
            }
        }

        /// <summary>
        /// The currently pressed view, if any.
        /// </summary>
        private UIView? _pressedView = null;

        private bool _exitedPressedView = false;

        /// <summary>
        /// Private backing cache for BackgroundColor.
        /// </summary>
        private GraphicsKit.Color _backgroundColor = new GraphicsKit.Color(0xFFECECEC);

        /// <summary>
        /// Close the window.
        /// </summary>
        public void Close() => Surface.Remove();
    }
}
