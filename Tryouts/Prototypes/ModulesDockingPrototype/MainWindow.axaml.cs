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
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using MorganStanley.ComposeUI.Tryouts.Visuals.Avalonia.VisualUtils;
using NP.Avalonia.UniDock;
using NP.Avalonia.UniDockService;
using NP.Concepts.Behaviors;
using System;
using System.Collections.ObjectModel;

namespace MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype
{
    public partial class MainWindow : Window
    {
        ProcessesViewModel? _viewModel;

        IUniDockService _uniDockService;

        int _newDockId = 0;

        TabbedDockGroup? _appTabs;
        public MainWindow()
        {
            
        }

        public MainWindow(ProcessesViewModel viewModel)
        {
            InitializeComponent();
            _appTabs = this.FindControl<TabbedDockGroup>("AppTabs");
            _uniDockService = (IUniDockService)
                this.Resources["TheDockManager"]!;

            _uniDockService.DockItemsViewModels =
                new ObservableCollection<DockItemViewModelBase>();

            _viewModel = viewModel;

            DataContext = _viewModel;

            this.Closed += MainWindow_Closed;

            _viewModel.ProcessesWithWindows.AddBehavior(OnProcessAdded);
        }

        private void OnProcessAdded(SingleProcessViewModel processViewModel)
        {
            ++_newDockId;

            Dispatcher.UIThread.Post(
                () =>
                {
                    _appTabs.DockChildren.Add
                    (
                        new DockItem
                        {
                            DockId = _newDockId.ToString(),
                            Header = $"{processViewModel.Name}_{_newDockId}",
                            Content = new EmbeddedWindowBasedNativeControl { WindowHandle = processViewModel.ProcessMainWindowHandle },
                            IsPredefined = false,
                            IsSelected = true,
                            IsActive = true,
                        }
                     );
                });

            //Dispatcher.UIThread.Post(
            //    () =>
            //    {
            //        _uniDockService.DockItemsViewModels.Add
            //        (
            //            new DockItemViewModelBase
            //            {
            //                DockId = _newDockId.ToString(),
            //                Header = $"{processViewModel.Name}_{_newDockId}",
            //                DefaultDockGroupId = "MainProcessesTab",
            //                DefaultDockOrderInGroup = _newDockId,
            //                Content = processViewModel.ProcessMainWindowHandle,
            //                ContentTemplateResourceKey= "EmbeddedWindowTemplate",
            //                IsSelected = true,
            //                IsActive = true,
            //                IsPredefined = false
            //            }
            //        );
            //    }
            //);
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
