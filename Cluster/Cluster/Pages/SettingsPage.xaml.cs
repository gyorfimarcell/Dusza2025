using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : CustomPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            int index = cbLanguage.Items.Cast<ComboBoxItem>().ToList().FindIndex(x => x.Tag.ToString() == TranslationSource.Instance.CurrentCulture.Name);
            cbLanguage.SelectedIndex = index == -1 ? 0 : index;
            cbLanguage.SelectionChanged += cbLanguage_SelectionChanged;

            tsDarkmode.IsChecked = _window.DarkMode;
        }

        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string newLanguage = (string)((ComboBoxItem)cbLanguage.SelectedItem).Tag;
            TranslationSource.Instance.CurrentCulture = new CultureInfo(newLanguage);

            Registry.SetValue(MainWindow.SETTINGS_KEY, "language", newLanguage);

            ChangeTitle(TranslationSource.T("Settings.Title"));
            _window.RefreshLblPath();
        }

        private void tsDarkmode_Click(object sender, RoutedEventArgs e)
        {
            _window.DarkMode = tsDarkmode.IsChecked == true;
        }
    }
}
