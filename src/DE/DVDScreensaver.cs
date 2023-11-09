/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using Mirage.DataKit;
using Mirage.GraphicsKit;
using Mirage.SurfaceKit;
using Mirage.UIKit;

namespace Mirage.DE
{
    /// <summary>
    /// DVD screensaver demo.
    /// </summary>
    class DVDScreensaver : UIApplication
    {
        /// <summary>
        /// Initialise the application.
        /// </summary>
        public DVDScreensaver(SurfaceManager surfaceManager) : base(surfaceManager)
        {
            MainWindow = new UIWindow(surfaceManager, 480, 360, "DVD Screensaver", titlebar: true, resizable: false);
            _view = new UICanvasView(MainWindow.Size);
            MainWindow.RootView.Add(_view);
            MainWindow.Surface.OnUpdate.Bind(HandleSurfaceUpdate);
            Update();
        }

        private static Random _random = new Random();

        private static Color PickRandomColor()
        {
            byte[] bytes = new byte[3];
            _random.NextBytes(bytes);
            return new Color(bytes[0], bytes[1], bytes[2]);
        }

        private int _x = 0;
        private int _y = 0;
        private int _velocityX = 1;
        private int _velocityY = 1;
        private Color _color = PickRandomColor();

        private void Update()
        {
            _view.Canvas.Clear(Color.Black);
            _view.Canvas.DrawFilledRectangle(_x, _y, Resources.DVD.Width, Resources.DVD.Height, 0, _color);
            _view.Canvas.DrawImage(_x, _y, Resources.DVD, Alpha: true);
            _x += _velocityX;
            _y += _velocityY;
            if (_x < 0 || _x >= _view.Canvas.Width - Resources.DVD.Width)
            {
                _velocityX = -_velocityX;
                _color = PickRandomColor();
            }
            if (_y < 0 || _y >= _view.Canvas.Height - Resources.DVD.Height)
            {
                _velocityY = -_velocityY;
                _color = PickRandomColor();
            }
            _view.QueuePaint();
        }

        private void HandleSurfaceUpdate(SignalArgs args)
        {
            Update();
        }

        /// <summary>
        /// The game canvas view.
        /// </summary>
        private readonly UICanvasView _view;
    }
}
