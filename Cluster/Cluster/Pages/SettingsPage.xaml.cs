using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            int index = cbLanguage.Items.Cast<ComboBoxItem>().ToList()
                .FindIndex(x => x.Tag.ToString() == TranslationSource.Instance.CurrentCulture.Name);
            cbLanguage.SelectedIndex = index == -1 ? 0 : index;
            cbLanguage.SelectionChanged += cbLanguage_SelectionChanged;

            tsDarkmode.IsChecked = _window.DarkMode;
        }

        /// <summary>
        /// Changes the language of the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string newLanguage = (string)((ComboBoxItem)cbLanguage.SelectedItem).Tag;
            TranslationSource.Instance.CurrentCulture = new CultureInfo(newLanguage);

            Registry.SetValue(MainWindow.SETTINGS_KEY, "language", newLanguage);

            ChangeTitle(TranslationSource.T("Settings.Title"));
            _window.RefreshLblPath();
        }

        /// <summary>
        /// Changes the darkmode of the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsDarkmode_Click(object sender, RoutedEventArgs e)
        {
            _window.DarkMode = tsDarkmode.IsChecked == true;
        }
    }
}