using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using MorganStanley.ComposeUI.Tryouts.Core.Utilities;
using System;
using System.ComponentModel;

namespace MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype
{
    public class SingleProcessViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event Action<SingleProcessViewModel>? StopEvent;

        public event Action<SingleProcessViewModel> StartedEvent;

        public Guid InstanceId { get; }

        public string ProcessName { get; }

        public string UiType { get; }

        public long ProcessMainWindowHandle { get; set; }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (_isRunning == value)
                {
                    return;
                }

                _isRunning = value;

                OnPropChanged(nameof(IsRunning));
                OnPropChanged(nameof(CanStop));
                OnPropChanged(nameof(Status));
            }
        }

        public string Status => IsRunning ? "Running" : "Stopped";

        protected void OnPropChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public SingleProcessViewModel
        (
            Guid instanceId,
            string name, 
            string uiType
        ) 
        {
            InstanceId = instanceId;
            ProcessName = name;
            UiType = uiType;
        }

        public void ReactToMessage(LifecycleEvent lifecycleEvent)
        {
            if (lifecycleEvent.EventType == LifecycleEventType.StoppingCanceled || 
                lifecycleEvent.EventType == LifecycleEventType.FailedToStart)
            {
                return; // no change whatever it is
            }

            bool isStartedEvent = lifecycleEvent.EventType == LifecycleEventType.Started;

            IsRunning = isStartedEvent;

            if (isStartedEvent)
            {
                this.ProcessMainWindowHandle = long.Parse(lifecycleEvent.ProcessInfo.uiHint!);

                StartedEvent?.Invoke(this);

                OnPropChanged(nameof(ProcessMainWindowHandle));
            }
        }

        public void Stop()
        {
            StopEvent?.Invoke(this);
        }

        public bool CanStop => IsRunning;
    }
}
