using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Platform;
using NP.Utilities;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MorganStanley.ComposeUI.Playground.VisualUtils
{
    using static WindowStyles;
    using static WindowLongFlags;
    using static Win32Exports;

    public class EmbeddedNativeControl : ContentPresenter
    {
        #region ProcessExePath Styled Avalonia Property
        public string ProcessExePath
        {
            get { return GetValue(ProcessExePathProperty); }
            set { SetValue(ProcessExePathProperty, value); }
        }

        public static readonly StyledProperty<string> ProcessExePathProperty =
            AvaloniaProperty.Register<EmbeddedNativeControl, string>
            (
                nameof(ProcessExePath)
            );
        #endregion ProcessExePath Styled Avalonia Property

        IDisposable _subscription;
        public EmbeddedNativeControl()
        {
            _subscription =
                this.GetObservable(ProcessExePathProperty).Subscribe(OnProcessExePathChanged);
        }

        private async void OnProcessExePathChanged(string newPath)
        {
            await LoadChildWindow();
        }

        public async Task LoadChildWindow()
        {
            NativeHost oldHost = this.Content as NativeHost;
            if (oldHost != null)
            {
                oldHost.DestroyProcess();
            }

            if (ProcessExePath != null)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = ProcessExePath
                };

                Process p = Process.Start(processStartInfo);

                await Task.Delay(2000);

                this.Content = new NativeHost(p);
            }
        }

        private class NativeHost : NativeControlHost
        {
            private Process _process;

            public IntPtr WindowHandle => _process.MainWindowHandle;

            public NativeHost(Process process)
            {
                _process = process;
            }

            protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
            {
                Window rootWindow = e.Root as Window;

                if (rootWindow != null)
                {
                    SetParent(WindowHandle, rootWindow.PlatformImpl.Handle.Handle);

                    long style = (long)GetWindowLongPtr(WindowHandle, (int)GWL_STYLE);

                    style = (style & ~((uint)WS_POPUP | (uint)WS_CAPTION | (uint)WS_THICKFRAME | (uint)WS_MINIMIZEBOX | (uint)WS_MAXIMIZEBOX | (uint)WS_SYSMENU)) | (uint)WS_CHILD;

                    SetWindowLongPtr(new HandleRef(null, WindowHandle), (int)GWL_STYLE, (IntPtr)style);
                }

                // force refreshing the handle
                MethodInfo methodInfo = typeof(NativeControlHost).GetMethod("DestroyNativeControl", BindingFlags.Instance | BindingFlags.NonPublic);

                methodInfo.Invoke(this, null);
                //(this as NativeControlHost).CallMethodExtras("DestroyNativeControl", true, false);
                base.OnAttachedToVisualTree(e);
            }

            protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
            {
                base.OnDetachedFromVisualTree(e);
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
                    //DestroyProcess();
                }
                else
                {
                    base.DestroyNativeControlCore(control);
                }
            }

            public void DestroyProcess()
            {
                _process?.Kill(true);

                _process?.WaitForExit();

                _process?.Dispose();

                _process = null;
            }
        }
    }
}
