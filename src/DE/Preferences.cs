/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.DataKit;
using Mirage.SurfaceKit;
using Mirage.UIKit;

namespace Mirage.DE
{
    /// <summary>
    /// System preferences application.
    /// </summary>
    class Preferences : UIApplication
    {
        /// <summary>
        /// Initialise the application.
        /// </summary>
        public Preferences(SurfaceManager surfaceManager) : base(surfaceManager)
        {
            MainWindow = new UIWindow(surfaceManager, 300, 128, "System Preferences", resizable: false);
            
            UIButton shadows = new UIButton("Window shadows")
            {
                Location = new(24, 24),
                Checkable = true,
                Style = new UICheckBoxButtonStyle(),
                HorizontalPadding = 32,
                Checked = SurfaceManager.ShadowsEnabled
            };
            shadows.OnCheckedChange.Bind(HandleShadowsCheckedChange);
            
            UIButton close = new UIButton("Close");
            close.Location = new(
                MainWindow.Surface.Canvas.Width - close.Size.Width - 24,
                MainWindow.Surface.Canvas.Height - close.Size.Height - 24
            );
            close.OnMouseClick.Bind((args) => MainWindow.Close());

            MainWindow.RootView.Add(shadows);
            MainWindow.RootView.Add(close);

            MainWindow.Surface.OnRemoved.Bind((args) => _preferencesInstance = null);
        }

        /// <summary>
        /// Existing preferences instance.
        /// </summary>
        private static Preferences? _preferencesInstance;

        /// <summary>
        /// Start the system preferences application, or focus the existing instance if one is already open.
        /// </summary>
        /// <param name="surfaceManager">The surface manager.</param>
        public static void Open(SurfaceManager surfaceManager)
        {
            if (_preferencesInstance != null && _preferencesInstance.MainWindow != null)
            {
                _preferencesInstance.MainWindow.Surface.SurfaceManager.Focus = _preferencesInstance.MainWindow.Surface;
                return;
            }
            _preferencesInstance = new Preferences(surfaceManager);
        }

        /// <summary>
        /// Handle the window shadows checkbox being toggled.
        /// </summary>
        /// <param name="args">Signal args.</param>
        private void HandleShadowsCheckedChange(ToggleArgs args)
        {
            SurfaceManager.ShadowsEnabled = args.State;
        }
    }
}
