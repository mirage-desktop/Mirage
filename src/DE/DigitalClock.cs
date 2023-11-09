/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Drawing;
using Mirage.DataKit;
using Mirage.SurfaceKit;
using Mirage.UIKit;

namespace Mirage.DE
{
    /// <summary>
    /// Digital clock application.
    /// </summary>
    class DigitalClock : UIApplication
    {
        /// <summary>
        /// Initialise the application.
        /// </summary>
        public DigitalClock(SurfaceManager surfaceManager) : base(surfaceManager)
        {
            MainWindow = new UIWindow(surfaceManager, 160, 64, "Digital Clock", titlebar: true, resizable: false)
            {
                BackgroundColor = GraphicsKit.Color.Black
            };
            // MainWindow.Surface.BorderColor = GraphicsKit.Color.Black;
            _clock = new UITextView(new TextKit.TextStyle(Resources.Segment, GraphicsKit.Color.Red))
            {
                HorizontalAlignment = TextKit.TextAlignment.Center,
                FillWidth = true,
                Location = new Point(0, 8)
            };
            MainWindow.RootView.Add(_clock);
            _clock.Content.Text = GetClockText();
            MainWindow.Surface.OnUpdate.Bind(HandleSurfaceUpdate);
        }

        /// <summary>
        /// Updates the clock if the second has changed.
        /// </summary>
        private void HandleSurfaceUpdate(SignalArgs args)
        {
            if (DateTime.Now.Second != _previousSecond)
            {
                _previousSecond = DateTime.Now.Second;
                _clock.Content.Text = GetClockText();
            }
        }

        /// <summary>
        /// Get the text for the clock.
        /// </summary>
        /// <returns>The text for the clock.</returns>
        private static string GetClockText()
        {
            DateTime now = DateTime.Now;
            return $"{now.Hour.ToString().PadLeft(2, '0')}:{now.Minute.ToString().PadLeft(2, '0')}:{now.Second.ToString().PadLeft(2, '0')}";
        }

        /// <summary>
        /// The clock text view.
        /// </summary>
        private readonly UITextView _clock;

        /// <summary>
        /// Second tracker for the clock.
        /// </summary>
        private int _previousSecond = DateTime.Now.Second;
    }
}
