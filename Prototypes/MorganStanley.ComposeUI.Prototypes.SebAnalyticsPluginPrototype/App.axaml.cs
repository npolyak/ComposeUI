using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MorganStanley.ComposeUI.Common.VisualUtils;
using NP.Avalonia.Gidon;
using NP.IoCy;
using NP.NLogAdapter;
using NP.Utilities.PluginUtils;
using Sebastion.Core;
using System.Threading.Tasks;

namespace MorganStanley.ComposeUI.Prototypes.SebAnaylicsPluginPrototype
{
    public class App : Application
    {
        /// defined the Gidon plugin manager
        /// use the following paths (relative to the SebAnaylicsPluginPrototype.exe executable)
        /// to dynamically load the plugins and services:
        /// "Plugins/Services" - to load the services (non-visual singletons)
        /// "Plugins/ViewModelPlugins" - to load view model plugins
        /// "Plugins/ViewPlugins" - to load view plugins
        public static PluginManager ThePluginManager { get; } =
            new PluginManager
            (
                "Plugins/Services",
                "Plugins/ViewModelPlugins",
                "Plugins/ViewPlugins");

        // the IoC container
        public static IoCContainer TheContainer => ThePluginManager.TheContainer;

        public App()
        {
            // inject a type from a statically loaded project NLogAdapter
            App.ThePluginManager.InjectType(typeof(NLogWrapper));

            App.ThePluginManager.InjectType(typeof(DispatcherAdapter));

            // inject all dynamically loaded assemblies
            App.ThePluginManager.CompleteConfiguration();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
