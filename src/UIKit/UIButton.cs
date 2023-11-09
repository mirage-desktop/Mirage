/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Drawing;
using Mirage.DataKit;
using Mirage.InputKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// A generic button view which can take on various form factors and styles.
    /// </summary>
    public class UIButton : UITextView
    {
        /// <summary>
        /// Initialise a new button from a string.
        /// </summary>
        /// <param name="text">The initial text.</param>
        public UIButton(string text) : base(text)
        {
            OnMouseDown.Bind(HandleMouseDown);
            OnMouseRelease.Bind(HandleMouseRelease);
            OnMouseClick.Bind(HandleMouseClick);
            HorizontalPadding = 16;
        }

        private void HandleMouseDown(MouseArgs args)
        {
            Pressed = true;
            QueuePaint();
        }

        private void HandleMouseRelease(MouseArgs args)
        {
            Pressed = false;
            QueuePaint();
        }

        private void HandleMouseClick(MouseArgs args)
        {
            if (Checkable)
            {
                if (!Checked)
                {
                    Checked = true;
                    QueuePaint();
                }
                else if (AllowUnchecking)
                {
                    Checked = false;
                    QueuePaint();
                }
            }
        }

        /// <summary>
        /// The style of the button.
        /// </summary>
        public UIButtonStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                QueuePaint();
            }
        }

        /// <summary>
        /// If the button is currently pressed.
        /// </summary>
        public bool Pressed { get; private set; } = false;

        /// <summary>
        /// If the button is checkable.
        /// </summary>
        public bool Checkable
        {
            get => _checkable;
            set
            {
                if (_checkable != value)
                {
                    _checkable = value;
                    if (_checkable == false)
                    {
                        Checked = false;
                    }
                }
            }
        }

        /// <summary>
        /// If the button allows being unchecked.
        /// </summary>
        public bool AllowUnchecking { get; set; } = true;

        /// <summary>
        /// If the button is checked.
        /// </summary>
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    OnCheckedChange.Fire(new ToggleArgs(value));
                    QueuePaint();
                }
            }
        }

        public override void PaintSelf()
        {
            if (Window == null)
            {
                return;
            }
            Rectangle rr = RootRectangle;
            _style.Render(Window.Surface.Canvas, rr.X, rr.Y, (ushort)rr.Width, (ushort)rr.Height, false, Pressed, Checked);
            base.PaintSelf();
        }

        protected override Size GetImplicitSize()
        {
            Size baseSize = base.GetImplicitSize();
            return new Size(
                baseSize.Width,
                Math.Max(24, baseSize.Height)
            );
        }

        /// <summary>
        /// Fired when the checked state of the button changes.
        /// </summary>
        public readonly Signal<ToggleArgs> OnCheckedChange = new();

        /// <summary>
        /// Private backing cache for Style.
        /// </summary>
        private UIButtonStyle _style = new UIStandardButtonStyle();

        /// <summary>
        /// Private backing cache for Checkable.
        /// </summary>
        private bool _checkable = false;

        /// <summary>
        /// Private backing cache for Checked.
        /// </summary>
        private bool _checked = false;
    }
}
