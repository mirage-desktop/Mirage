/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Drawing;
using Mirage.DataKit;
using Mirage.InputKit;
using Mirage.SurfaceKit;
using Mirage.TextKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// A view that displays rich text.
    /// </summary>
    public class UITextView : UIView
    {
        /// <summary>
        /// Initialise a new empty text view with the default style.
        /// </summary>
        public UITextView()
        {
            Content = new TextBlock();
            _renderer = new TextRenderer();
            Content.OnChanged.Bind(HandleTextBlockChange);
            OnMouseDown.Bind(HandleMouseDown);
            OnMouseDrag.Bind(HandleMouseDrag);
            OnKeyTyped.Bind(HandleKeyTyped);
            _textLayout = GetTextLayout();
        }

        /// <summary>
        /// Initialise a new empty text view with a set text style.
        /// </summary>
        /// <param name="style">The text style.</param>
        public UITextView(TextStyle style)
        {
            Content = new TextBlock(style);
            _renderer = new TextRenderer();
            Content.OnChanged.Bind(HandleTextBlockChange);
            OnMouseDown.Bind(HandleMouseDown);
            OnMouseDrag.Bind(HandleMouseDrag);
            OnKeyTyped.Bind(HandleKeyTyped);
            _textLayout = GetTextLayout();
        }

        /// <summary>
        /// Initialise a new text view with the default style from a string.
        /// </summary>
        /// <param name="text">The initial text.</param>
        public UITextView(string text)
        {
            Content = new TextBlock(text);
            _renderer = new TextRenderer();
            Content.OnChanged.Bind(HandleTextBlockChange);
            OnMouseDown.Bind(HandleMouseDown);
            OnMouseDrag.Bind(HandleMouseDrag);
            OnKeyTyped.Bind(HandleKeyTyped);
            _textLayout = GetTextLayout();
        }

        /// <summary>
        /// Initialise a new text view from an existing text block.
        /// </summary>
        /// <param name="textBlock">The text block.</param>
        public UITextView(TextBlock textBlock)
        {
            Content = textBlock;
            _renderer = new TextRenderer();
            Content.OnChanged.Bind(HandleTextBlockChange);
            OnMouseDown.Bind(HandleMouseDown);
            OnMouseDrag.Bind(HandleMouseDrag);
            OnKeyTyped.Bind(HandleKeyTyped);
            _textLayout = GetTextLayout();
        }

        /// <summary>
        /// Handle the text block being changed.
        /// </summary>
        /// <param name="args">Signal args.</param>
        private void HandleTextBlockChange(SignalArgs args)
        {
            _textLayout = GetTextLayout();
            QueuePaint();
        }

        /// <summary>
        /// Handle the mouse being pressed on the text view.
        /// </summary>
        /// <param name="args">Signal args.</param>
        private void HandleMouseDown(MouseArgs args)
        {
            if (!Editable)
            {
                return;
            }
            int idx = _textLayout.GetIndexAtPoint(args.Point);
            SelectionStart = idx;
            SelectionEnd = idx;
        }
        
        /// <summary>
        /// Handle the mouse being dragged on the text view.
        /// </summary>
        /// <param name="args">Signal args.</param>
        private void HandleMouseDrag(MouseArgs args)
        {
            if (!Editable)
            {
                return;
            }
            int idx = _textLayout.GetIndexAtPoint(args.Point);
            SelectionEnd = idx;
        }

        private void ShiftCaret(int amount)
        {
            int contentLength = Content.Length;
            if (_selectionStart == _selectionEnd)
            {
                _selectionStart = Math.Clamp(_selectionStart + amount, 0, contentLength);
                _selectionEnd = Math.Clamp(_selectionEnd + amount, 0, contentLength);
            }
            else
            {
                int caret = amount > 0 ? _selectionEnd : _selectionStart;
                _selectionStart = caret;
                _selectionEnd = caret;
            }
            QueuePaint();
        }

        /// <summary>
        /// Handle a key being typed in the text view.
        /// </summary>
        /// <param name="args">Signal args.</param>
        private void HandleKeyTyped(KeyboardArgs args)
        {
            if (ReadOnly || !Editable)
            {
                return;
            }
            if (_selectionStart > _selectionEnd)
            {
                (_selectionStart, _selectionEnd) = (_selectionEnd, _selectionStart);
            }
            switch (args.Key)
            {
                case Cosmos.System.ConsoleKeyEx.LeftArrow:
                    ShiftCaret(-1);
                    break;
                case Cosmos.System.ConsoleKeyEx.RightArrow:
                    ShiftCaret(1);
                    break;
                case Cosmos.System.ConsoleKeyEx.Backspace:
                    if (SelectionLength > 0)
                    {
                        Content.Text = Content.Text.Remove(SelectionStart, SelectionLength);
                    }
                    else
                    {
                        if (SelectionStart == 0)
                        {
                            return;
                        }
                        Content.Text = Content.Text.Remove(SelectionStart - 1, 1);
                    }
                    ShiftCaret(-1);
                    break;
                case Cosmos.System.ConsoleKeyEx.Enter:
                    Content.Text = Content.Text.Remove(SelectionStart, SelectionLength).Insert(SelectionStart, "\n");
                    ShiftCaret(1);
                    break;
                default:
                    if (args.Key == Cosmos.System.ConsoleKeyEx.A && args.Modifiers == ConsoleModifiers.Control)
                    {
                        SelectionStart = 0;
                        SelectionEnd = Content.Length;
                        return;
                    }
                    if (!char.IsControl(args.Character))
                    {
                        Content.Text = Content.Text.Remove(SelectionStart, SelectionLength).Insert(SelectionStart, args.Character.ToString());
                        ShiftCaret(1);
                    }
                    break;
            }
            int contentLength = Content.Length;
            _selectionStart = Math.Clamp(_selectionStart, 0, contentLength);
            _selectionEnd = Math.Clamp(_selectionEnd, 0, contentLength);
        }

        /// <summary>
        /// Lay out the text.
        /// </summary>
        /// <returns>The new text layout.</returns>
        private TextLayout GetTextLayout()
        {
            return TextLayout.LayOut(Content, Size.Width - (HorizontalPadding * 2), _wrapping, _horizontalAlignment);
        }

        public override void PaintSelf()
        {
            if (Window == null)
            {
                return;
            }
            base.PaintSelf();
            Point rootLocation = RootLocation;
            if (Editable)
            {
                _renderer.Render(_textLayout, Window.Surface.Canvas, rootLocation.X, rootLocation.Y, highlightStart: _selectionStart, highlightEnd: _selectionEnd);
                if (!ReadOnly && _selectionStart == _selectionEnd)
                {
                    (Point Point, int Height) caretInfo = _textLayout.GetCaretInfoAtIndex(_selectionStart);
                    int caretX = caretInfo.Point.X;
                    int caretBaselineY = caretInfo.Point.Y;
                    int caretHeight = caretInfo.Height;
                    Window.Surface.Canvas.DrawLine(
                        caretX,
                        caretBaselineY,
                        caretX,
                        caretBaselineY - caretHeight,
                        GraphicsKit.Color.Black
                    );
                }
            }
            else
            {
                _renderer.Render(_textLayout, Window.Surface.Canvas, rootLocation.X + _horizontalPadding, rootLocation.Y);
            }
        }

        protected override Size GetImplicitSize()
        {
            return new Size(
                _textLayout.GetWidth() + (HorizontalPadding * 2),
                _textLayout.GetHeight()
            );
        }

        public override PointerType GetPointerType()
        {
            return Editable ? PointerType.IBeam : PointerType.Default;
        }

        /// <summary>
        /// Whether to perform text wrapping. The default is false.
        /// </summary>
        public bool Wrapping
        {
            get => _wrapping;
            set
            {
                if (_wrapping != value)
                {
                    _wrapping = value;
                    _textLayout = GetTextLayout();
                    QueuePaint();
                }
            }
        }

        public TextAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                if (_horizontalAlignment != value)
                {
                    _horizontalAlignment = value;
                    _textLayout = GetTextLayout();
                    QueuePaint();
                }
            }
        }

        /// <summary>
        /// The content of the text block.
        /// </summary>
        public TextBlock Content { get; init; }

        /// <summary>
        /// The <see cref="TextRenderer"/> that renders the content of the text view.
        /// </summary>
        private readonly TextRenderer _renderer;

        /// <summary>
        /// The current layout of the text.
        /// </summary>
        protected TextLayout _textLayout;

        /// <summary>
        /// If the text view is editable.
        /// </summary>
        public bool Editable { get; set; } = false;

        /// <summary>
        /// If the text view is read-only.
        /// </summary>
        public bool ReadOnly { get; set; } = false;
        
        /// <summary>
        /// The start of the text selection.
        /// </summary>
        public int SelectionStart {
            get => _selectionStart;
            set {
                if (_selectionStart != value)
                {
                    value = Math.Clamp(value, 0, Content.Length);
                    _selectionStart = value;
                    QueuePaint();
                }
            }
        }
    
        /// <summary>
        /// The end of the text selection.
        /// </summary>
        public int SelectionEnd {
            get => _selectionEnd;
            set {
                if (_selectionEnd != value)
                {
                    value = Math.Clamp(value, 0, Content.Length);
                    _selectionEnd = value;
                    QueuePaint();
                }
            }
        }

        /// <summary>
        /// The length of the text selection.
        /// </summary>
        public int SelectionLength => _selectionEnd - _selectionStart;

        /// <summary>
        /// Private backing cache for SelectionStart.
        /// </summary>
        private int _selectionStart = 0;
        /// <summary>
        /// Private backing cache for SelectionEnd.
        /// </summary>
        private int _selectionEnd = 0;

        /// <summary>
        /// Private backing cache for Wrapping.
        /// </summary>
        private bool _wrapping = false;

        /// <summary>
        /// Private backing cache for HorizontalAlignment.
        /// </summary>
        private TextAlignment _horizontalAlignment = TextAlignment.Left;

        /// <summary>
        /// The horizontal padding on each side of the text view.
        /// </summary>
        public int HorizontalPadding
        {
            get => _horizontalPadding;
            set
            {
                if (_horizontalPadding != value)
                {
                    _horizontalPadding = value;
                    QueuePaint();
                }
            }
        }

        /// <summary>
        /// Private backing cache for HorizontalPadding.
        /// </summary>
        private int _horizontalPadding = 0;
    }
}
