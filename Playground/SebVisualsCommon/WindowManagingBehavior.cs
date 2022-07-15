using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sebastion.Visuals
{
    public class WindowManagingBehavior<THomeWindow, TNewWindow>
        where THomeWindow : Window
        where TNewWindow : Window
    {
        private THomeWindow _homeWindow;

        private Func<THomeWindow, TNewWindow> _createNewWindow;

        private TNewWindow? _newWindow;

        public WindowManagingBehavior(THomeWindow currentWindow, Func<THomeWindow, TNewWindow>? createNewWindow = null)
        {
            _homeWindow = currentWindow;

            _createNewWindow = createNewWindow!;

            if (_createNewWindow == null)
            {
                _createNewWindow = _ => Activator.CreateInstance<TNewWindow>();
            }    
        }

        public void OpenWindow(Action<TNewWindow>? setWindow)
        {
            if (_newWindow == null)
            {
                _newWindow = _createNewWindow?.Invoke(_homeWindow);
                _newWindow!.Show();

                _newWindow.Closed += _newWindow_Closed;
            }
            else
            {
                _newWindow.BringWindowUp();
            }

            setWindow?.Invoke(_newWindow);
        }

        public void OpenWindow()
        {
            OpenWindow(null);
        }

        private void _newWindow_Closed(object? sender, EventArgs e)
        {
            _newWindow = null;
        }
    }
}
