/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
namespace Mirage.GraphicsKit
{
    /// <summary>
    /// A 9-slice image.
    /// </summary>
    class Slice
    {
        /// <summary>
        /// Initialise a new 9-slice image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="cs">The size of the corners.</param>
        /// <param name="alpha">Whether to apply alpha blending to the slice.</param>
        public Slice(Canvas image, ushort cs, bool alpha = true)
        {
            _cs = cs;
            _alpha = alpha;
            // middle
            _middle = Filters.Sample(cs, cs, (ushort)(image.Width - cs * 2), (ushort)(image.Height - cs * 2), image);
            // corners
            _topLeft = Filters.Sample(0, 0, cs, cs, image);
            _topRight = Filters.Sample(image.Width - cs, 0, cs, cs, image);
            _bottomLeft = Filters.Sample(0, image.Height - cs, cs, cs, image);
            _bottomRight = Filters.Sample(image.Width - cs, image.Height - cs, cs, cs, image);
            // edges
            _left = Filters.Sample(0, cs, cs, (ushort)(image.Height - cs * 2), image);
            _right = Filters.Sample(image.Width - cs, cs, cs, (ushort)(image.Height - cs * 2), image);
            _top = Filters.Sample(cs, 0, (ushort)(image.Width - cs * 2), cs, image);
            _bottom = Filters.Sample(cs, image.Height - cs, (ushort)(image.Width - cs * 2), cs, image);
        }

        /// <summary>
        /// Scale the 9-slice image to the desired size.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <returns>The resized image.</returns>
        public Canvas Scale(ushort width, ushort height)
        {
            Canvas result = new Canvas(width, height);
            result.Clear(Color.Transparent);
            // middle
            result.DrawImage(_cs, _cs, Filters.Scale((ushort)(width - _cs * 2), (ushort)(height - _cs * 2), _middle), Alpha: false);
            // corners
            result.DrawImage(0, 0, _topLeft, Alpha: false);
            result.DrawImage(width - _cs, 0, _topRight, Alpha: false);
            result.DrawImage(0, height - _cs, _bottomLeft, Alpha: false);
            result.DrawImage(width - _cs, height - _cs, _bottomRight, Alpha: false);
            // edges
            result.DrawImage(0, _cs, Filters.Scale(_cs, (ushort)(height - _cs * 2), _left), Alpha: false);
            result.DrawImage(width - _cs, _cs, Filters.Scale(_cs, (ushort)(height - _cs * 2), _right), Alpha: false);
            result.DrawImage(_cs, 0, Filters.Scale((ushort)(width - _cs * 2), _cs, _top), Alpha: false);
            result.DrawImage(_cs, height - _cs, Filters.Scale((ushort)(width - _cs * 2), _cs, _bottom), Alpha: false);
            return result;
        }

        /// <summary>
        /// Render the 9-slice image to the target canvas.
        /// </summary>
        /// <param name="target">The target canvas.</param>
        /// <param name="x">The target X coordinate.</param>
        /// <param name="y">The target Y coordinate.</param>
        /// <param name="width">The width to scale the 9-slice image to.</param>
        /// <param name="height">The height to scale the 9-slice image to.</param>
        public void Render(Canvas target, int x, int y, ushort width, ushort height)
        {
            Canvas image = Scale(width, height);
            target.DrawImage(x, y, image, _alpha);
        }

        /// <summary>
        /// The size of the slice's corners.
        /// </summary>
        private readonly ushort _cs;

        /// <summary>
        /// Whether to apply alpha blending to the slice.
        /// </summary>
        private readonly bool _alpha;

        /// <summary>
        /// Left edge sample.
        /// </summary>
        private readonly Canvas _left;
        /// <summary>
        /// Right edge sample.
        /// </summary>
        private readonly Canvas _right;
        /// <summary>
        /// Top edge sample.
        /// </summary>
        private readonly Canvas _top;
        /// <summary>
        /// Bottom edge sample.
        /// </summary>
        private readonly Canvas _bottom;

        /// <summary>
        /// Top left corner sample.
        /// </summary>
        private readonly Canvas _topLeft;
        /// <summary>
        /// Top right corner sample.
        /// </summary>
        private readonly Canvas _topRight;
        /// <summary>
        /// Bottom left corner sample.
        /// </summary>
        private readonly Canvas _bottomLeft;
        /// <summary>
        /// Bottom right corner sample.
        /// </summary>
        private readonly Canvas _bottomRight;

        /// <summary>
        /// Middle sample.
        /// </summary>
        private readonly Canvas _middle;
    }
}
