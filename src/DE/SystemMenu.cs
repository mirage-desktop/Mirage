/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using Mirage.DataKit;
using Mirage.GraphicsKit;
using Mirage.GraphicsKit.Animation;
using Mirage.SurfaceKit;
using Mirage.TextKit;
using Mirage.UIKit;

namespace Mirage.DE
{
    /// <summary>
    /// System menu bar.
    /// </summary>
    public class SystemMenu
    {
        /// <summary>
        /// Initialise the system menu bar.
        /// </summary>
        public SystemMenu(SurfaceManager surfaceManager)
        {
            _surfaceManager = surfaceManager;
            _window = new UIWindow(surfaceManager, (ushort)_surfaceManager.Width, MENU_HEIGHT, "Menu bar", titlebar: false, resizable: false);
            _window.Surface.X = 0;
            _window.Surface.Y = (int)_animationY;
            _window.Surface.Focusable = false;
            _window.Surface.AlwaysOnTop = true;
            _window.Surface.BorderColor = Color.Transparent;
            _window.Surface.IsShell = true;
            UIButton button = new UIButton("Mirage")
            {
                Location = new(8, 0),
                ExplicitHeight = MENU_HEIGHT,
                HorizontalPadding = 16,
                Style = new UIMenuBarButtonStyle(),
                Checkable = true,
                HorizontalAlignment = TextAlignment.Center,
            };
            UIContextMenu contextMenu = new UIContextMenu(new()
            {
                new()  
                {
                    new("Edit", () => _ = new Edit(surfaceManager)),
                    new("Digital Clock", () => _ = new DigitalClock(surfaceManager)),
                    new("DVD Screensaver", () => _ = new DVDScreensaver(surfaceManager)),
                },
                new()
                {
                    new("System Preferences", () => Preferences.Open(surfaceManager)),
                    new("About This Computer", () => _ = new About(surfaceManager)),
                },
                new()
                {
                    new("Sleep", () => surfaceManager.Sleep()),
                    new("Power Off...", () => PowerDialogue.ShowPowerDialogue(surfaceManager, isRebooting: false)),
                    new("Restart...", () => PowerDialogue.ShowPowerDialogue(surfaceManager, isRebooting: true)),
                },
            });
            button.OnCheckedChange.Bind((args) => {
                if (args.State)
                {
                    UIWindow contextMenuWindow = contextMenu.Show(_surfaceManager, button.ScreenLocation.X, button.ScreenLocation.Y + button.Size.Height);
                    contextMenuWindow.Surface.OnRemoved.Bind((args) => button.Checked = false);
                }
            });
            _clock = new UITextView(GetClockText())
            {
                HorizontalAlignment = TextAlignment.Right,
                ExplicitSize = new System.Drawing.Size(48, MENU_HEIGHT),
                Location = new System.Drawing.Point(_window.Size.Width - 64, 0)
            };
            _window.Surface.OnUpdate.Bind(HandleSurfaceUpdate);
            _window.RootView.Add(button);
            _window.RootView.Add(_clock);
        }

        /// <summary>
        /// Updates the clock if the minute has changed. Also updates the animation.
        /// </summary>
        private void HandleSurfaceUpdate(SignalArgs args)
        {
            if (DateTime.Now.Minute != _previousMinute)
            {
                _previousMinute = DateTime.Now.Minute;
                _clock.Content.Text = GetClockText();
            }

            // animation
            if (HideMenu && (int)Math.Floor(_animationY) > -MENU_HEIGHT)
            {
                _animationY -= (_animationY + MENU_HEIGHT) / 12;
                if ((int)Math.Floor(_animationY) <= -MENU_HEIGHT)
                {
                    OnMenuHideCompleted.Fire(new());
                }
            }
            else if ((int)_animationY < 0)
            {
                _animationY -= _animationY / 12;
            }
            _window.Surface.Y = (int)_animationY;
        }

        /// <summary>
        /// Y coordinate of the system menu animation.
        /// </summary>
        private float _animationY = -MENU_HEIGHT;

        /// <summary>
        /// Get the text for the clock.
        /// </summary>
        /// <returns>The text for the clock.</returns>
        private static string GetClockText()
        {
            DateTime now = DateTime.Now;
            return $"{now.Hour.ToString().PadLeft(2, '0')}:{now.Minute.ToString().PadLeft(2, '0')}";
        }

        /// <summary>
        /// Height of the system menu.
        /// </summary>
        public const int MENU_HEIGHT = 24;

        /// <summary>
        /// Menu bar window.
        /// </summary>
        private readonly UIWindow _window;

        /// <summary>
        /// Clock view.
        /// </summary>
        private readonly UITextView _clock;

        /// <summary>
        /// The surface manager.
        /// </summary>
        private readonly SurfaceManager _surfaceManager;

        /// <summary>
        /// Minute tracker for the clock.
        /// </summary>
        private int _previousMinute = DateTime.Now.Minute;

        /// <summary>
        /// If the system menu should be hidden.
        /// </summary>
        public bool HideMenu { get; set; } = false;

        /// <summary>
        /// Fired when the menu hide animation is completed.
        /// </summary>
        public readonly Signal<SignalArgs> OnMenuHideCompleted = new();
    }
}
