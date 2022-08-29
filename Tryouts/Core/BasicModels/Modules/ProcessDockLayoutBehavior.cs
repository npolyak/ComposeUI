using NP.Avalonia.UniDockService;
using NP.Concepts.Behaviors;
using System;
using System.Reactive.Linq;

namespace MorganStanley.ComposeUI.Tryouts.Core.BasicModels.Modules
{
    public class ProcessDockLayoutBehavior
    {
        IUniDockService _uniDockService;
        ProcessesViewModel _processesViewModel;
        string _dockSerializationFileName;
        string _vmSerializationFileName;
        string _mainGroupDockId;
        string _contentTemplateResourceKey;
        string _headerContentTemplateResourceKey;

        int _newDockId = 0;

        SynchronizationContext _synchronizationContext;

        public ProcessDockLayoutBehavior
        (
            IUniDockService uniDockService,
            ProcessesViewModel processesViewModel,
            string dockSerializationFileName,
            string vmSerializationFileName,
            string mainGroupDockId,
            string contentTemplateResourceKey,
            string headerContentTemplateResourceKey
        )
        {
            _uniDockService = uniDockService;
            _processesViewModel = processesViewModel;
            _dockSerializationFileName = dockSerializationFileName;
            _vmSerializationFileName = vmSerializationFileName;
            _mainGroupDockId = mainGroupDockId;
            _contentTemplateResourceKey = contentTemplateResourceKey;
            _headerContentTemplateResourceKey = headerContentTemplateResourceKey;

#pragma warning disable CS8974 // Converting method group to non-delegate type
            _processesViewModel.ProcessesWithWindows.AddBehavior(OnProcessWithWindowAdded, OnProcessWithWindowRemoved);
#pragma warning restore CS8974 // Converting method group to non-delegate type

            _synchronizationContext = SynchronizationContext.Current!;
        }


        public void Save()
        {
            _uniDockService.SaveToFile(_dockSerializationFileName);

            _uniDockService.SaveViewModelsToFile(_vmSerializationFileName);
        }

        public void Load()
        {
            _uniDockService.RestoreFromFile(_dockSerializationFileName, false);
            _uniDockService.RestoreViewModelsFromFile(_vmSerializationFileName, typeof(DockItemViewModel<ProcessData>));

            _newDockId =
                _uniDockService
                    .DockItemsViewModels
                    .Cast<DockItemViewModel<ProcessData>>()
                    .Max(dockItemVm => dockItemVm.TheVM!.WindowNumber);

            foreach (DockItemViewModel<ProcessData> vm in _uniDockService.DockItemsViewModels)
            {
                string processName = vm.TheVM!.ProcessName!;

                Guid instanceId = vm.TheVM.InstanceId;

                _processesViewModel!.LaunchProcess(processName, instanceId);
            }
        }


        private void OnProcessWithWindowAdded(SingleProcessViewModel processViewModel)
        {
            _synchronizationContext.Send
            (
                (_) =>
                {
                    Guid instanceId = processViewModel.InstanceId;

                    var existingVm =
                        _uniDockService
                            .DockItemsViewModels
                            .Cast<DockItemViewModel<ProcessData>>()
                            .FirstOrDefault(dockItemVm => dockItemVm.TheVM!.InstanceId == instanceId);

                    if (existingVm != null)
                    {
                        existingVm.TheVM!.ProcessMainWindowHandle = processViewModel.ProcessMainWindowHandle;
                    }
                    else
                    {
                        _newDockId++;
                        var dockItemViewModel = new DockItemViewModel<ProcessData>
                        {
                            DockId = _newDockId.ToString(),
                            DefaultDockGroupId = _mainGroupDockId,
                            DefaultDockOrderInGroup = _newDockId,
                            ContentTemplateResourceKey = _contentTemplateResourceKey,
                            HeaderContentTemplateResourceKey = _headerContentTemplateResourceKey,
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
                },
                null
            );

        }

        private void OnProcessWithWindowRemoved(SingleProcessViewModel processViewModel)
        {
            _synchronizationContext.Send((_) =>
            {
                var dockVm = 
                    _uniDockService
                    .DockItemsViewModels
                    .Cast<DockItemViewModel<ProcessData>>()
                    .FirstOrDefault(vm => vm.TheVM!.InstanceId == processViewModel.InstanceId);

                var dockId = dockVm!.DockId;

                var group = _uniDockService.GetGroupByDockId(dockId);

                _uniDockService.DockItemsViewModels.Remove(dockVm);
            },
            null);
        }
    }
}
