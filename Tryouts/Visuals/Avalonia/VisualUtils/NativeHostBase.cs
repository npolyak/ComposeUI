using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MorganStanley.ComposeUI.Tryouts.Visuals.Avalonia.VisualUtils
{
    using static WindowStyles;
    using static WindowLongFlags;
    using static Win32Exports;

    internal abstract class NativeHostBase : NativeControlHost
    {
        internal abstract IntPtr WindowHandle { get; }

        private Window _rootWindow;

        public NativeHostBase()
        {
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (_rootWindow != null)
            {
                _rootWindow.Closed -= _rootWindow_Closed;
            }

            _rootWindow = e.Root as Window;

            if (_rootWindow != null)
            {
                _rootWindow.Closed += _rootWindow_Closed;

                SetParent(WindowHandle, _rootWindow.PlatformImpl.Handle.Handle);

                long style = (long)GetWindowLongPtr(WindowHandle, (int)GWL_STYLE);

                style = (style & ~((uint)WS_POPUP | (uint)WS_CAPTION | (uint)WS_THICKFRAME | (uint)WS_MINIMIZEBOX | (uint)WS_MAXIMIZEBOX | (uint)WS_SYSMENU)) | (uint)WS_CHILD;

                SetWindowLongPtr(new HandleRef(null, WindowHandle), (int)GWL_STYLE, (IntPtr)style);
            }

            // force refreshing the handle
            MethodInfo methodInfo =
                typeof(NativeControlHost)
                    .GetMethod("DestroyNativeControl", BindingFlags.Instance | BindingFlags.NonPublic);

            methodInfo.Invoke(this, null);
            //(this as NativeControlHost).CallMethodExtras("DestroyNativeControl", true, false);
            base.OnAttachedToVisualTree(e);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (_rootWindow != null)
            {
                _rootWindow.Closed -= _rootWindow_Closed;
            }
            base.OnDetachedFromVisualTree(e);
        }

        private void _rootWindow_Closed(object sender, EventArgs e)
        {
            this.OnRootWindowClosed();
        }

        protected virtual void OnRootWindowClosed()
        {

        }


        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new PlatformHandle(WindowHandle, "CTRL");
            }
            else
            {
                return base.CreateNativeControlCore(parent);
            }

        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

            }
            else
            {
                base.DestroyNativeControlCore(control);
            }
        }
    }
}
