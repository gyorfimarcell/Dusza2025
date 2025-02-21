using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ClusterChangeLogsPage.xaml
    /// </summary>
    public partial class ClusterChangeLogsPage
    {
        private readonly List<KeyValuePair<string, string>> logTypeOptions =
        [
            new(TranslationSource.T("Logs.AllTypes"), TranslationSource.T("Logs.AllTypes")),
            ..(Enum.GetNames(typeof(LogType))
                .Select(x => new KeyValuePair<string, string>(x, TranslationSource.T("LogType." + x))))
        ];

        private readonly List<KeyValuePair<string, string>> logDetailOptions =
        [
            new(TranslationSource.T("Logs.AllTypes"), TranslationSource.T("Logs.AllTypes")),
            ..(Log.LogDataTypes.Values.SelectMany(v => v).Distinct().Select(x =>
                new KeyValuePair<string, string>(x, TranslationSource.T("LogDetail." + x))))
        ];

        private readonly Dictionary<string, List<string>> logEntries = new();

        public ClusterChangeLogsPage()
        {
            InitializeComponent();
            cbLogTypes.ItemsSource = logTypeOptions;
            cbLogTypes.SelectedValuePath = "Key";
            cbLogTypes.DisplayMemberPath = "Value";

            cbLogDetails.ItemsSource = logDetailOptions;
            cbLogDetails.SelectedValuePath = "Key";
            cbLogDetails.DisplayMemberPath = "Value";

            cbLogTypes.SelectedIndex = 0;
            cbLogDetails.SelectedIndex = 0;

            LoadLogFiles();
            GenerateLogView();
        }

        /// <summary>
        /// Load all log files from the log directory.
        /// </summary>
        private void LoadLogFiles()
        {
            string directoryPath = Log.GetLogDirectoryPath();
            if (!Directory.Exists(directoryPath)) return;

            logEntries.Clear();
            string[] files = Directory.GetFiles(directoryPath, "*.log");

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                logEntries[fileName] = File.ReadAllLines(file).ToList();
            }
        }

        /// <summary>
        /// Expand or collapse all sub items of the main expander.
        /// </summary>
        /// <param name="mainExpander">The main expander</param>
        private static void expandAllItem(Expander mainExpander)
        {
            bool isAllSubItemsExpanded = true;
            foreach (object? child in ((StackPanel)mainExpander.Content).Children)
            {
                if (child is Expander { IsExpanded: false })
                {
                    isAllSubItemsExpanded = false;
                    break;
                }
            }
            if (mainExpander.Content is StackPanel sp)
            {
                Wpf.Ui.Controls.Button btn = sp.Children.OfType<Wpf.Ui.Controls.Button>().FirstOrDefault();
                if (btn != null)
                {
                    btn.Content = isAllSubItemsExpanded ? TranslationSource.T("Logs.CollapseAll") : TranslationSource.T("Logs.ExpandAll");
                }
            }

            mainExpander.IsExpanded = !mainExpander.IsExpanded || !isAllSubItemsExpanded;

            foreach (object? child in ((StackPanel)mainExpander.Content).Children)
            {
                if (child is Expander subExpander)
                {
                    subExpander.IsExpanded = mainExpander.IsExpanded;
                }
            }
        }

        /// <summary>
        /// Expand all sub items of the main expander.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void expandAll_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button { Parent: Grid grid })
            {
                expandAllItem((grid.Parent as Expander)!);
            }
        }

        /// <summary>
        /// Generate the log view based on the log entries.
        /// </summary>
        private void GenerateLogView()
        {
            stLogs.Children.Clear();

            foreach (KeyValuePair<string, List<string>> entry in logEntries)
            {
                string fileName = entry.Key;
                List<string> lines = entry.Value;

                var expander = new Expander
                {
                    Margin = new Thickness(5)
                };

                var headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });

                Wpf.Ui.Controls.TextBlock headerTextBlock = new Wpf.Ui.Controls.TextBlock
                {
                    Text = fileName,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(headerTextBlock, 0);

                var headerButton = new Wpf.Ui.Controls.Button
                {
                    Content = TranslationSource.T("Logs.ExpandAll"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                headerButton.Click += expandAll_Click;
                Grid.SetColumn(headerButton, 1);

                headerGrid.Children.Add(headerTextBlock);
                headerGrid.Children.Add(headerButton);

                expander.Header = headerGrid;

                var stackPanel = new StackPanel();

                for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
                {
                    string line = lines[lineIndex];
                    bool last = lineIndex == lines.Count - 1;

                    string[] lineData = line.Replace("\n", "").Split(" - ");
                    if (lineData.Length < 2) continue;

                    bool success = Enum.TryParse(lineData[0], true, out LogType type);
                    if (!success) continue;

                    if (cbLogTypes.SelectedIndex != 0 && type.ToString() != cbLogTypes.SelectedValue.ToString())
                        continue;

                    if (cbLogDetails.SelectedIndex == 0 && !string.IsNullOrEmpty(tbFilter.Text) &&
                        !lineData[2..].Any(x => x.Contains(tbFilter.Text)))
                        continue;

                    if (cbLogDetails.SelectedIndex != 0 && !string.IsNullOrEmpty(tbFilter.Text) && !lineData[2..].Any(
                            x =>
                                Log.LogDataTypes[type].Contains(cbLogDetails.SelectedValue.ToString()!) &&
                                x.Contains(tbFilter.Text)))
                        continue;

                    var headerText =
                        $"{TranslationSource.T("LogType." + type)} - {DateTime.ParseExact(lineData[1], "yyyy.MM.dd. HH:mm:ss", CultureInfo.InvariantCulture):HH:mm}";

                    if (lineData.Length > 2)
                    {
                        Expander subExpander = new Expander
                        {
                            Header = headerText,
                            Margin = last ? new Thickness(0) : new Thickness(0, 0, 0, 8),
                        };
                        subExpander.SetResourceReference(Control.BorderBrushProperty, "ControlStrokeColorDefaultBrush");
                        stackPanel.Children.Add(subExpander);

                        var subStackPanel = new StackPanel();
                        List<string> cardData = new();

                        for (int i = 0; i < lineData[2..].Length; i++)
                        {
                            cardData.Add(
                                $"{TranslationSource.T("LogDetail." + Log.LogDataTypes[type][i])}: {lineData[i + 2]}");
                        }

                        subStackPanel.Children.Add(GetUnexpandableCard(cardData, true));
                        subExpander.Content = subStackPanel;
                    }
                    else
                    {
                        Border card = GetUnexpandableCard([headerText], last);
                        stackPanel.Children.Add(card);
                    }
                }

                if (stackPanel.Children.Count != 0)
                {
                    expander.Content = stackPanel;
                    stLogs.Children.Add(expander);
                }

                expander.Collapsed += (sender, e) =>
                {
                    headerButton.Content = TranslationSource.T("Logs.ExpandAll");
                };
                expander.Expanded += (sender, e) =>
                {
                    bool isAllSubItemsExpanded = true;
                    foreach (object? child in ((StackPanel)expander.Content).Children)
                    {
                        if (child is Expander { IsExpanded: false })
                        {
                            isAllSubItemsExpanded = false;
                            break;
                        }
                    }
                    headerButton.Content = isAllSubItemsExpanded ? TranslationSource.T("Logs.CollapseAll") : TranslationSource.T("Logs.ExpandAll");
                };

                showStatus.Visibility = stLogs.Children.Count > 0 ? Visibility.Hidden : Visibility.Visible;
            }
        }

        /// <summary>
        /// Get a card that is not expandable.
        /// </summary>
        /// <param name="headerTextList">The title of the card</param>
        /// <param name="last">Is it the last element of the list</param>
        /// <returns>The card which contains the title</returns>
        private Border GetUnexpandableCard(List<string> headerTextList, bool last)
        {
            var cardContainer = new Border()
            {
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(5),
                Margin = last ? new Thickness(0) : new Thickness(0, 0, 0, 8),
            };

            cardContainer.SetResourceReference(Control.BackgroundProperty, "ControlFillColorDefaultBrush");
            cardContainer.SetResourceReference(Control.BorderBrushProperty, "ControlStrokeColorDefaultBrush");

            StackPanel mainPanel = new StackPanel();

            foreach (string headerText in headerTextList)
            {
                var headerTextBlock = new Wpf.Ui.Controls.TextBlock()
                {
                    Text = headerText,
                    Padding = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center
                };

                mainPanel.Children.Add(headerTextBlock);
            }

            cardContainer.Child = mainPanel;

            return cardContainer;
        }

        /// <summary>
        /// Log type selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLogTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = cbLogTypes.SelectedValue.ToString()!;
            if (!string.IsNullOrEmpty(selectedValue) && cbLogTypes.SelectedIndex != 0)
            {
                List<KeyValuePair<string, string>> filtered =
                [
                    new(TranslationSource.T("Logs.AllTypes"), TranslationSource.T("Logs.AllTypes")),
                    ..(logDetailOptions.Where(x =>
                        Log.LogDataTypes[Enum.Parse<LogType>(selectedValue)].Contains(x.Key)))
                ];
                cbLogDetails.ItemsSource = filtered;
                cbLogDetails.SelectedIndex = 0;
            }
            else
            {
                cbLogDetails.ItemsSource = logDetailOptions;
            }

            GenerateLogView();
        }

        /// <summary>
        /// Log detail selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLogDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateLogView();
        }

        /// <summary>
        /// Filter text changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            GenerateLogView();
        }
    }
}