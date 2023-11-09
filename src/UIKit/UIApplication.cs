/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Collections.Generic;
using Mirage.SurfaceKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// An application which manages a set of windows.
    /// </summary>
    abstract class UIApplication
    {
        /// <summary>
        /// Initialise a new application instance.
        /// </summary>
        public UIApplication(SurfaceManager surfaceManager)
        {
            SurfaceManager = surfaceManager;
        }

        /// <summary>
        /// Quit the application by closing all of its windows.
        /// </summary>
        public void Quit()
        {
            foreach (UIWindow window in ApplicationWindows)
            {
                window.Close();
            }
            ApplicationWindows.Clear();
            _mainWindow = null;
        }

        /// <summary>
        /// Add a window to the application.
        /// </summary>
        public void AddApplicationWindow(UIWindow window)
        {
            if (!ApplicationWindows.Contains(window))
            {
                ApplicationWindows.Add(window);
            }
        }

        /// <summary>
        /// The main window of the application.
        /// Setting this will automatically add the window to the application if it isn't already added.
        /// </summary>
        public UIWindow? MainWindow {
            get => _mainWindow;
            set {
                if (value != null)
                {
                    ApplicationWindows.Add(value);
                    value.Surface.SurfaceManager.Focus = value.Surface;
                }
                _mainWindow = value;
            }
        }

        /// <summary>
        /// The surface manager of the application.
        /// </summary>
        public SurfaceManager SurfaceManager { get; init; }

        /// <summary>
        /// The windows of the application.
        /// </summary>
        public List<UIWindow> ApplicationWindows { get; } = new List<UIWindow>();

        /// <summary>
        /// Private backing cache for MainWindow.
        /// </summary>
        private UIWindow? _mainWindow;
    }
}
