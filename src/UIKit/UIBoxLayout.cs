/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Drawing;

namespace Mirage.UIKit
{
    /// <summary>
    /// A view that lays its children out in a line.
    /// </summary>
    class UIBoxLayout : UIView
    {
        public UIBoxLayout(UIBoxOrientation orientation)
        {
            _orientation = orientation;
        }

        /// <summary>
        /// Lay out the view's children.
        /// </summary>
        public void LayOut()
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < _children.Count; i++)
            {
                UIView view = _children[i];
                view.Location = new Point(x, y);
                if (_orientation == UIBoxOrientation.Vertical) 
                {
                    y += view.Size.Height;
                    if (i < _children.Count - 1)
                    {
                        y += Spacing;
                    }
                }
                if (_orientation == UIBoxOrientation.Horizontal)
                {
                    x += view.Size.Width;
                    if (i < _children.Count - 1)
                    {
                        x += Spacing;
                    }
                }
            }
            Size oldSize = Size;
            CalculateImplicitLayoutSize();
            if (oldSize != Size)
            {
                OnSizeChanged.Fire(new());
            }
        }

        private void CalculateImplicitLayoutSize()
        {
            int width = 0;
            int height = 0;
            int x = 0;
            int y = 0;
            for (int i = 0; i < _children.Count; i++)
            {
                UIView view = _children[i];
                width = Math.Max(width, x + view.Size.Width);
                height = Math.Max(height, y + view.Size.Height);
                if (_orientation == UIBoxOrientation.Vertical)
                {
                    y += view.Size.Height;
                    if (i < _children.Count - 1)
                    {
                        y += Spacing;
                    }
                }
                if (_orientation == UIBoxOrientation.Horizontal)
                {
                    x += view.Size.Width;
                    if (i < _children.Count - 1)
                    {
                        x += Spacing;
                    }
                }
            }
            _implicitLayoutSize = new Size(width, height);
        }

        protected override Size GetImplicitSize()
        {
            return _implicitLayoutSize;
        }

        /// <summary>
        /// The spacing between items in the box layout. By default, there is no spacing.
        /// </summary>
        public int Spacing {
            get => _spacing;
            set
            {
                if (_spacing != value)
                {
                    _spacing = value;
                    LayOut();
                }
            }
        }

        /// <summary>
        /// The orientation of the layout.
        /// </summary>
        private readonly UIBoxOrientation _orientation;

        /// <summary>
        /// The implicit size of the layout view.
        /// </summary>
        private Size _implicitLayoutSize = Size.Empty;

        /// <summary>
        /// Private backing cache for Spacing.
        /// </summary>
        private int _spacing = 0;
    }
}
