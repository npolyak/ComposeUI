/// ********************************************************************************************************
///
/// Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License").
/// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
/// See the NOTICE file distributed with this work for additional information regarding copyright ownership.
/// Unless required by applicable law or agreed to in writing, software distributed under the License
/// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and limitations under the License.
/// 
/// ********************************************************************************************************

using Avalonia;
using Avalonia.Controls.Presenters;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MorganStanley.ComposeUI.Tryouts.Visuals.Avalonia.VisualUtils
{
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
            ProcessControllingNativeHost oldHost = this.Content as ProcessControllingNativeHost;
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

                this.Content = new ProcessControllingNativeHost(p);
            }
        }
    }
}
