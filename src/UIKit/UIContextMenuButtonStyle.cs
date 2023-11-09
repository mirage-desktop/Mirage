/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.GraphicsKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// Context menu button style.
    /// </summary>
    public class UIContextMenuButtonStyle : UIButtonStyle
    {
        public override void Render(Canvas target, int x, int y, ushort width, ushort height, bool hovered, bool pressed, bool @checked)
        {
            if (hovered || pressed)
            {
                for (int i = 0; i < width; i++)
                {
                    target.DrawImage(x + i, y, Resources.MenuSelectionGradient, Alpha: false);
                }
            }
        }
    }
}
