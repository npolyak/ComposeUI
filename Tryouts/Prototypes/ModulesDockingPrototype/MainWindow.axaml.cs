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

using Avalonia.Controls;
using Avalonia.Threading;
using MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype;
using NP.Avalonia.UniDock;
using NP.Avalonia.UniDockService;
using NP.Concepts.Behaviors;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype
{
    public partial class MainWindow : Window
    {
        ProcessesViewModel? _viewModel;

        IUniDockService _uniDockService;

        int _newDockId = 0;

        TabbedDockGroup? _appTabs;

        private const string DockSerializationFileName = "DockSerialization.xml";
        private const string VMSerializationFileName = "DockVMSerialization.xml";

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

            SaveButton.Click += SaveButton_Click;

            LoadButton.Click += LoadButton_Click;
        }

        private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _uniDockService.SaveToFile(DockSerializationFileName);

            _uniDockService.SaveViewModelsToFile(VMSerializationFileName);
        }

        private void LoadButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _uniDockService.RestoreFromFile(DockSerializationFileName, false);
            _uniDockService.RestoreViewModelsFromFile(VMSerializationFileName, typeof(DockItemViewModel<ProcessData>));

            foreach (DockItemViewModel<ProcessData> vm in _uniDockService.DockItemsViewModels)
            {
                string processName = vm.TheVM.ProcessName;

                Guid instanceId = vm.TheVM.InstanceId;

                _viewModel.LaunchProcess(processName, instanceId);
            }
        }

        private void OnProcessAdded(SingleProcessViewModel processViewModel)
        {
            Guid instanceId = processViewModel.InstanceId;

            Dispatcher.UIThread.Post(
                () =>
                {
                    var existingVm = 
                        _uniDockService
                            .DockItemsViewModels
                            .Cast<DockItemViewModel<ProcessData>>()
                            .FirstOrDefault(dockItemVm => dockItemVm.TheVM.InstanceId == instanceId);

                    if (existingVm != null)
                    {
                        existingVm.TheVM.ProcessMainWindowHandle = processViewModel.ProcessMainWindowHandle;
                    }
                    else
                    {
                        var dockItemViewModel = new DockItemViewModel<ProcessData>
                        {
                            DockId = _newDockId.ToString(),
                            Header = $"{processViewModel.ProcessName}_{_newDockId}",
                            DefaultDockGroupId = "MainProcessesTab",
                            DefaultDockOrderInGroup = _newDockId,
                            ContentTemplateResourceKey = "EmbeddedWindowTemplate",
                            HeaderContentTemplateResourceKey = "EmbeddedWindowHeaderTemplate",
                            IsSelected = true,
                            IsActive = true,
                            IsPredefined = false,
                        };

                        dockItemViewModel.TheVM = new ProcessData
                        {
                            InstanceId = processViewModel.InstanceId,
                            ProcessName = processViewModel.ProcessName,
                            WindowNumber = _newDockId,
                            ProcessMainWindowHandle = processViewModel.ProcessMainWindowHandle
                        };
                        _uniDockService!.DockItemsViewModels.Add(dockItemViewModel);
                    }
                }
            );
        }


        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
