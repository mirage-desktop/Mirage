/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.GraphicsKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// Menu bar button style.
    /// </summary>
    public class UIMenuBarButtonStyle : UIButtonStyle
    {
        public override void Render(Canvas target, int x, int y, ushort width, ushort height, bool hovered, bool pressed, bool @checked)
        {
            if (@checked || pressed)
            {
                for (int i = 0; i < width; i++)
                {
                    target.DrawImage(x + i, y, Resources.MenuSelectionGradient, Alpha: false);
                }
            }
            else if (hovered)
            {
                target.DrawFilledRectangle(x, y, width, height, 0, Color.LightGray);
            }
        }
    }
}
