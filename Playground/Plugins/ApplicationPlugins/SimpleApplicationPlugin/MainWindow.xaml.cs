using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleApplicationPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

            Left = -9999;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;

            Console.WriteLine("Initialized");

            //await Task.Delay(30000);

            //while (true)
            //{
            //    Thread.Sleep(10000);

            //    await Task.Delay(1);
            //}
        }
    }
}
