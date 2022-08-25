using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using System;
using System.ComponentModel;

namespace MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype
{
    public class SingleProcessViewModel : ProcessInfo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event Action<SingleProcessViewModel>? StopEvent;

        public event Action<SingleProcessViewModel> StartedEvent;

        public Guid InstanceId { get; }

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
            ProcessInfo processInfo,
            Guid instanceId
        ) 
            : 
            base
            (
                processInfo.Name, 
                processInfo.UiType, 
                processInfo.UiHint)
        {
            InstanceId = instanceId;
            ProcessMainWindowHandle = processInfo.ProcessMainWindowHandle;
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
                this.UiHint = lifecycleEvent.ProcessInfo.UiHint;
                this.ProcessMainWindowHandle = lifecycleEvent.ProcessInfo.ProcessMainWindowHandle;

                StartedEvent?.Invoke(this);

                OnPropChanged(nameof(UiHint));
            }
        }

        public void Stop()
        {
            StopEvent?.Invoke(this);
        }

        public bool CanStop => IsRunning;
    }
}
