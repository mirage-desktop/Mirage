/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.SurfaceKit;
using Mirage.UIKit;

namespace Mirage.DE
{
    /// <summary>
    /// Text editor application.
    /// </summary>
    class Edit : UIApplication
    {
        /// <summary>
        /// Initialise the application.
        /// </summary>
        public Edit(SurfaceManager surfaceManager) : base(surfaceManager)
        {
            MainWindow = new UIWindow(surfaceManager, 320, 240, "Untitled - Edit", resizable: false)
            {
                BackgroundColor = GraphicsKit.Color.White
            };
            _textView = new UITextView
            {
                ExplicitSize = MainWindow.Size,
                Wrapping = false,
                Editable = true,
            };
            _textView.Content.Style = new TextKit.TextStyle(Resources.LiberationSerif, GraphicsKit.Color.Black);
            _textView.Content.Append("Welcome to Edit.");
            _textView.SelectionStart = _textView.SelectionEnd = _textView.Content.Length;
            MainWindow.RootView.Add(_textView);
        }

        /// <summary>
        /// The main text field of the text editor.
        /// </summary>
        private readonly UITextView _textView;
    }
}
