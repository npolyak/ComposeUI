using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using NP.Concepts.Behaviors;

namespace MorganStanley.ComposeUI.Tryouts.Core.BasicModels.Modules
{
    public class ProcessesViewModel
    {
        public ModuleViewModel[] Modules { get; }

        IModuleLoader _moduleLoader;

        public IEnumerable<SingleProcessViewModel> Processes { get; } =
            new ObservableCollection<SingleProcessViewModel>();


        public IEnumerable<SingleProcessViewModel> ProcessesWithWindows { get; } =
            new ObservableCollection<SingleProcessViewModel>();

        private IDisposable _modulesBehavior;
        private IDisposable _processesBehavior;
        IModuleLoaderFactory _moduleLoaderFactory;
        public ProcessesViewModel
        (
            IModuleLoaderFactory moduleLoaderFactory,
            ModuleViewModel[] modules)
        {
            _moduleLoaderFactory = moduleLoaderFactory;

            Modules = modules;

            _modulesBehavior = Modules.AddBehavior(OnModuleAdded, OnModuleRemoved);

            _processesBehavior = Processes.AddBehavior(OnProcessesAdded, OnProcessesRemoved);

            ModuleCatalogue moduleCatalogue =
                new ModuleCatalogue(Modules.ToDictionary(m => m.Name, m => (ModuleManifest)m));

            _moduleLoader = _moduleLoaderFactory.Create(moduleCatalogue);

            _moduleLoader.LifecycleEvents.Subscribe(OnLifecycleEventArrived);
        }

        private void OnLifecycleEventArrived(LifecycleEvent lifecycleEvent)
        {
            Guid instanceId = lifecycleEvent.ProcessInfo.instanceId;

            var process = Processes.FirstOrDefault(p => p.InstanceId == instanceId);

            process?.ReactToMessage(lifecycleEvent);
        }

        private void OnModuleAdded(ModuleViewModel module)
        {
            module.LaunchEvent += OnLaunchModule;
        }

        private void OnModuleRemoved(ModuleViewModel module)
        {
            module.LaunchEvent -= OnLaunchModule;
        }

        private void OnProcessesAdded(SingleProcessViewModel process)
        {
            process.StopEvent += OnStopProcess;
            process.StartedEvent += Process_StartedEvent;
        }

        private void Process_StartedEvent(SingleProcessViewModel process)
        {
            ((IList<SingleProcessViewModel>)ProcessesWithWindows).Add(process);
        }

        private void OnProcessesRemoved(SingleProcessViewModel process)
        {
            process.StartedEvent -= Process_StartedEvent;
            process.StopEvent -= OnStopProcess;
        }

        private void OnLaunchModule(ModuleManifest moduleInfo)
        {
            Guid instanceId = Guid.NewGuid();

            LaunchProcess(moduleInfo, instanceId);
        }

        public ModuleManifest? FindManifest(string processName)
        {
            return Modules?.FirstOrDefault(module => module.Name == processName);
        }

        public void LaunchProcess(ModuleManifest moduleInfo, Guid instanceId)
        {
            SingleProcessViewModel processViewModel =
                new SingleProcessViewModel(instanceId, moduleInfo.Name, moduleInfo.UIType);

            ((IList<SingleProcessViewModel>)Processes).Add(processViewModel);

            _moduleLoader.RequestStartProcess
            (
                new LaunchRequest
                {
                    name = moduleInfo.Name,
                    instanceId = instanceId
                });
        }

        public void LaunchProcess(string processName, Guid instanceId)
        {
            ModuleManifest? moduleManifest = FindManifest(processName);

            if (moduleManifest == null)
            {
                throw new Exception($"Module with Name '{processName}' is not found");
            }

            LaunchProcess(moduleManifest, instanceId);
        }

        private void OnStopProcess(SingleProcessViewModel process)
        {
            StopRequest stopRequest = new StopRequest();
            stopRequest.instanceId = process.InstanceId;
            _moduleLoader.RequestStopProcess(stopRequest);
        }
    }
}
