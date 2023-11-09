/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.GraphicsKit;

namespace Mirage.UIKit
{
    /// <summary>
    /// Radio button style.
    /// </summary>
    public class UIRadioButtonStyle : UIButtonStyle
    {
        public override void Render(Canvas target, int x, int y, ushort width, ushort height, bool hovered, bool pressed, bool @checked)
        {
            Canvas image = @checked ? Resources.RadioButtonChecked : Resources.RadioButton;
            if (pressed)
            {
                image = Filters.Brightness(0.75f, image);
            }
            target.DrawImage(x, y + ((height - image.Height) / 2), image, Alpha: true);
        }
    }
}
