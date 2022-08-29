using System;

namespace MorganStanley.ComposeUI.Tryouts.Visuals.Avalonia.VisualUtils
{
    internal class WindowBasedNativeHost : NativeHostBase
    {
        internal override IntPtr WindowHandle { get; }

        internal WindowBasedNativeHost(long windowHandle)
        {
            WindowHandle = (IntPtr)windowHandle;
        }
    }
}
