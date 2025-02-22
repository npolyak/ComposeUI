﻿/// ********************************************************************************************************
///
/// Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License").
/// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
/// See the NOTICE file distributed with this work for additional information regarding copyright ownership.
/// Unless required by applicable law or agreed to in writing, software distributed under the License
/// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and limitations under the License.
/// 
/// ********************************************************************************************************

using MorganStanley.ComposeUI.Tryouts.Core.Abstractions;
using NP.Utilities.Attributes;
using System.Windows;

namespace AnotherWpfThemeReceiverApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IThemingService _themeService;

        [CompositeConstructor]
        public MainWindow(IThemingService themeService)
        {
            _themeService = themeService;

            InitializeComponent();

            Theme = _themeService.Theme.ToString();

            _themeService.SetTheme();

            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged()
        {
            Theme = _themeService.Theme.ToString();
        }

        public string Theme
        {
            get { return (string)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(string), typeof(MainWindow), new PropertyMetadata(null));
    }
}
