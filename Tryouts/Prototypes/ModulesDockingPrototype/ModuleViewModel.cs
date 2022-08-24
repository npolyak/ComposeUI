using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using System;

namespace MorganStanley.ComposeUI.Prototypes.ModulesDockingPrototype
{
    public class ModuleViewModel : ModuleManifest
    {
        public event Action<ModuleManifest>? LaunchEvent;

        public void Launch()
        { 
            LaunchEvent?.Invoke(this);
        }
    }
}
