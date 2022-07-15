using Avalonia.Controls;
using MorganStanley.ComposeUI.Common.VisualUtils;
using NP.NLogAdapter;
using NP.Utilities.PluginUtils;
using Sebastion.Core;

namespace MorganStanley.ComposeUI.Prototypes.SebAnaylicsPluginPrototype
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Opened += MainWindow_Opened;
        }

        private async void MainWindow_Opened(object? sender, System.EventArgs e)
        {            
           
            IAppInfoContainer appInfoContainer =
                (IAppInfoContainer)App.TheContainer.Resolve<IPlugin>("SebMainViewModel");

            appInfoContainer.IsActive = true;
            await appInfoContainer.RefreshData();
        }
    }
}
