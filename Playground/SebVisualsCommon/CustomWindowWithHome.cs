using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using NP.Avalonia.Visuals.Controls;
using System;

namespace Sebastion.Visuals
{
    public class CustomWindowWithHome : CustomWindow
    {
        #region HomeWindow Styled Avalonia Property
        public Window HomeWindow
        {
            get { return GetValue(HomeWindowProperty); }
            set { SetValue(HomeWindowProperty, value); }
        }

        public static readonly StyledProperty<Window> HomeWindowProperty =
            AvaloniaProperty.Register<CustomWindowWithHome, Window>
            (
                nameof(HomeWindow)
            );
        #endregion HomeWindow Styled Avalonia Property


        public void SwitchHome()
        {
            HomeWindow.BringWindowUp();
        }
    }
}
