﻿using System.Windows;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for AddComputerWindow.xaml
    /// </summary>
    public partial class AddComputerPage
    {
        private readonly string path;
        private readonly MainWindow window;

        public AddComputerPage()
        {
            InitializeComponent();
            path = MainWindow.ClusterPath;
            window = (MainWindow)Application.Current.MainWindow!;
        }

        /// <summary>
        /// Adds a computer to the cluster.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text == "" || nbProcessor.Value == null || nbMemory.Value == null)
            {
                window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"),
                    TranslationSource.T("Errors.MissingFields"), ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                return;
            }

            int processor = Convert.ToInt32(nbProcessor.Value);
            int memory = Convert.ToInt32(nbMemory.Value);

            string? error = Computer.AddComputer(path, tbName.Text, processor, memory);
            if (error != null)
            {
                window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"), error, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
            }
            else
            {
                window.RootSnackbarService.Show(TranslationSource.T("AddComputerPage.Success.Title"),
                    $"'{tbName.Text}' {TranslationSource.T("AddComputerPage.Success.Text")}",
                    ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));

                Log.WriteLog([tbName.Text, $"{processor}", $"{memory}"], LogType.AddComputer);

                tbName.Clear();
                nbProcessor.Clear();
                nbMemory.Clear();
            }
        }
    }
}