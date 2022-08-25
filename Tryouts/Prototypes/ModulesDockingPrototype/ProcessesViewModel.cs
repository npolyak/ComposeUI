using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using NP.Concepts.Behaviors;
using System;

namespace MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype
{
    public class ProcessesViewModel
    {
        public ObservableCollection<ModuleViewModel> Modules { get; } =
            new ObservableCollection<ModuleViewModel>();

        IModuleLoader _moduleLoader;

        public ObservableCollection<SingleProcessViewModel> Processes { get; } =
            new ObservableCollection<SingleProcessViewModel>();


        public ObservableCollection<SingleProcessViewModel> ProcessesWithWindows { get; } =
            new ObservableCollection<SingleProcessViewModel>();

        private IDisposable _modulesBehavior;
        private IDisposable _processesBehavior;
        public ProcessesViewModel
        (
            IModuleLoaderFactory moduleLoaderFactory, 
            IModuleHostFactory moduleHostFactory)
        {
            Modules
                .Add
                (
                    new ModuleViewModel
                    {
                        StartupType = StartupType.Executable,
                        UIType = UIType.Window,
                        Name = "SimpleWpfApp",
                        Path = @"Plugins\ApplicationPlugins\SimpleWpfApp\SimpleWpfApp.exe"
                    }
                );

            Modules
                .Add
                (
                    new ModuleViewModel
                    {
                        StartupType = StartupType.Executable,
                        UIType = UIType.Window,
                        Name = "AnotherWpfApp",
                        Path = @"Plugins\ApplicationPlugins\AnotherWpfApp\AnotherWpfApp.exe"
                    }
                );

            _modulesBehavior = Modules.AddBehavior(OnModuleAdded, OnModuleRemoved);

            _processesBehavior = Processes.AddBehavior(OnProcessesAdded, OnProcessesRemoved);

            ModuleCatalogue moduleCatalogue = 
                new ModuleCatalogue(Modules.ToDictionary(m => m.Name, m => (ModuleManifest) m));

            _moduleLoader = moduleLoaderFactory.Create(moduleCatalogue, moduleHostFactory);

            _moduleLoader.LifecycleEvents.Subscribe(OnLifecycleEventArrived);
        }

        private void OnLifecycleEventArrived(LifecycleEvent lifecycleEvent)
        {
            Guid instanceId = lifecycleEvent.InstanceId;

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
            ProcessesWithWindows.Add(process);
        }

        private void OnProcessesRemoved(SingleProcessViewModel process)
        {
            process.StartedEvent -= Process_StartedEvent;
            process.StopEvent -= OnStopProcess;
        }

        private void OnLaunchModule(ModuleManifest moduleInfo)
        {
            Guid instanceId = Guid.NewGuid();

            ProcessInfo processInfo =
                new ProcessInfo(moduleInfo.Name, moduleInfo.UIType);

            SingleProcessViewModel processViewModel = 
                new SingleProcessViewModel(processInfo, instanceId);

            Processes.Add(processViewModel);

            _moduleLoader.RequestStartProcess
            (
                new LaunchRequest 
                { 
                    name = moduleInfo.Name, 
                    instanceId = instanceId });
        }

        private void OnStopProcess(SingleProcessViewModel process)
        {
            StopRequest stopRequest = new StopRequest();
            stopRequest.instanceId = process.InstanceId;
            _moduleLoader.RequestStopProcess(stopRequest);
        }
    }
}
