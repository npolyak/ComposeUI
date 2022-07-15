using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace Sebastion.Visuals
{
    public static class VisualsHelper
    {
        public static void BringWindowUp(this Window win)
        {
            if (win.WindowState == WindowState.Minimized)
            {
                win.WindowState = WindowState.Normal;
            }
            win.Activate();
        }

        public static async void CopyDataContextToClipboard(this IControl control)
        {
            string? result = control?.DataContext?.ToString();

            if (result != null)
            {
                await Application.Current.Clipboard.SetTextAsync(result);
            }
        }
    }
}
