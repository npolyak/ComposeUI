using NP.Avalonia.UniDockService;
using NP.Concepts.Behaviors;
using System;
using System.Reactive.Linq;

namespace MorganStanley.ComposeUI.Tryouts.Core.BasicModels.Modules
{
    public class SaveRestoreProcessDockLayoutBehavior
    {
        IUniDockService _uniDockService;
        ProcessesViewModel _processesViewModel;
        string _dockSerializationFileName;
        string _vmSerializationFileName;
        string _mainGroupDockId;
        string _contentTemplateResourceKey;
        string _headerContentTemplateResourceKey;

        int _newDockId = 0;

        private event Action<SingleProcessViewModel>? OnProcessAdded;

        public SaveRestoreProcessDockLayoutBehavior
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
            _processesViewModel.ProcessesWithWindows.AddBehavior(OnProcessWithWindowAdded);
#pragma warning restore CS8974 // Converting method group to non-delegate type

            Observable.FromEvent<SingleProcessViewModel>
            (
                h => this.OnProcessAdded += h,
                h => this.OnProcessAdded -= h)

            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(SetWindowHandleOrCreateProcessViewModel);
        }

        private void SetWindowHandleOrCreateProcessViewModel(SingleProcessViewModel processViewModel)
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
            this.OnProcessAdded?.Invoke(processViewModel);
        }

    }
}
