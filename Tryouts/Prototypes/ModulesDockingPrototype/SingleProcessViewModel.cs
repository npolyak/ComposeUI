using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using System;
using System.ComponentModel;

namespace MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype
{
    public class SingleProcessViewModel : ProcessInfo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event Action<SingleProcessViewModel>? StopEvent;

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
        }

        public void ReactToMessage(LifecycleEvent lifecycleEvent)
        {
            if (lifecycleEvent.EventType == LifecycleEventType.StoppingCanceled || 
                lifecycleEvent.EventType == LifecycleEventType.FailedToStart)
            {
                return; // no change whatever it is
            }

            IsRunning =
                lifecycleEvent.EventType == LifecycleEventType.Started;

            if (lifecycleEvent.ProcessInfo.UiHint == null && IsRunning)
            {
                this.UiHint = lifecycleEvent.ProcessInfo.UiHint;

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
