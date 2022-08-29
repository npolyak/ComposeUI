using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MorganStanley.ComposeUI.Tryouts.Visuals.Avalonia.VisualUtils
{
    using static Win32Exports;

    internal class ProcessControllingNativeHost : NativeHostBase
    {
        private Process _process;

        internal override IntPtr WindowHandle => _process.MainWindowHandle;

        public ProcessControllingNativeHost(Process process)
        {
            _process = process;
            _process.EnableRaisingEvents = true;

            _process.Exited += OnProcessExited;
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            SetParent(WindowHandle, IntPtr.Zero);

        }

        protected override void OnRootWindowClosed()
        {
            this.DestroyProcess();
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
