/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.Collections.Generic;
using Cosmos.System.ScanMaps;
using Mirage.GraphicsKit;
using Mirage.SurfaceKit;
using Mirage.TextKit;
using Mirage.UIKit;

namespace Mirage.DE
{
    /// <summary>
    /// Out of box experience application.
    /// </summary>
    class OOBE : UIApplication
    {
        private static readonly List<(string, Cosmos.System.ScanMapBase)> _scanMaps = new()
        {
            ("American English", new USStandardLayout()),
            ("British English", new GBStandardLayout()),
            ("German (Deutsch)", new DEStandardLayout()),
            ("Spanish (Español)", new ESStandardLayout()),
            ("French (Français)", new FRStandardLayout()),
            ("Turkish (Türkçe)", new TRStandardLayout())
        };

        /// <summary>
        /// Initialise the application.
        /// </summary>
        public OOBE(SurfaceManager surfaceManager) : base(surfaceManager)
        {
            MainWindow = new UIWindow(surfaceManager, 480, 360, "Setup", resizable: false);

            UICanvasView icon = new UICanvasView(Resources.Keyboard, alpha: true)
            {
                Location = new(24, 24),
            };

            TextBlock headerBlock = new TextBlock(new TextStyle(Resources.CantarellLarge, Color.Black));
            headerBlock.Append("Keyboard Layout\n");
            headerBlock.Style = new TextStyle(Resources.Cantarell, Color.DeepGray);
            headerBlock.Append("Select your keyboard layout.");

            UITextView label = new UITextView(headerBlock)
            {
                Location = new(112, 24),  
            };

            UIBoxLayout layout = new UIBoxLayout(UIBoxOrientation.Vertical)
            {
                Location = new(112, 88),
            };
            List<UIButton> options = new List<UIButton>();
            foreach (var (Name, ScanMap) in _scanMaps)
            {
                UIButton option = new UIButton(Name)
                {
                    HorizontalPadding = 32,
                    Style = new UIRadioButtonStyle(),
                    Checkable = true,
                    AllowUnchecking = false,
                };
                options.Add(option);
                layout.Add(option);
                option.OnCheckedChange.Bind((args) => {
                    if (args.State)
                    {
                        foreach (UIButton other in options)
                        {
                            if (option != other)
                            {
                                other.Checked = false;
                            }
                        }
                        Cosmos.System.KeyboardManager.SetKeyLayout(ScanMap);
                    }
                });
            }
            layout.LayOut();
            options[0].Checked = true;

            UIButton close = new UIButton("OK");
            close.Location = new(
                MainWindow.Surface.Canvas.Width - close.Size.Width - 24,
                MainWindow.Surface.Canvas.Height - close.Size.Height - 24
            );
            close.OnMouseClick.Bind((args) => MainWindow.Close());

            MainWindow.Surface.OnRemoved.Bind((args) => {
                SurfaceManager.SystemMenu = new SystemMenu(SurfaceManager);
            });

            MainWindow.RootView.Add(icon);
            MainWindow.RootView.Add(label);
            MainWindow.RootView.Add(layout);
            MainWindow.RootView.Add(close);
        }
    }
}
