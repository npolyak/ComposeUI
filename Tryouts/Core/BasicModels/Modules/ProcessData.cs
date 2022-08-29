using MorganStanley.ComposeUI.Tryouts.Core.Utilities;
using System;
using System.Xml.Serialization;

namespace MorganStanley.ComposeUI.Tryouts.Core.BasicModels.Modules
{
    public class ProcessData : ViewModelBase
    {
        [XmlAttribute]
        public Guid InstanceId { get; set; }

        [XmlAttribute]
        public string? ProcessName { get; set; }

        [XmlAttribute]
        public int WindowNumber { get; set; }

        long? _mainWindowHandle;
        [XmlIgnore]
        public long? ProcessMainWindowHandle 
        {
            get => _mainWindowHandle; 
            set
            {
                if (_mainWindowHandle == value)
                {
                    return;
                }

                _mainWindowHandle = value;

                OnPropertyChanged(nameof(ProcessMainWindowHandle));
            }
        }
    }
}
