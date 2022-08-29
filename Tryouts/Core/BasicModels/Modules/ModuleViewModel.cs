using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;

namespace MorganStanley.ComposeUI.Tryouts.Core.BasicModels.Modules
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
