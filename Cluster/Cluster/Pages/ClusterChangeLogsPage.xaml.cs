using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ClusterChangeLogsPage.xaml
    /// </summary>
    public partial class ClusterChangeLogsPage : CustomPage
    {
        List<string> logTypeOptions = new[] { "All log types" }.Concat(Enum.GetNames(typeof(LogType))).ToList();
        List<string> logDetailOptions = new[] { "All log details" }.Concat(Log.LogDataTypes.Values.SelectMany(v => v).Distinct()).ToList();

        private Dictionary<string, List<string>> logEntries = new();

        public ClusterChangeLogsPage()
        {
            InitializeComponent();
            cbLogTypes.ItemsSource = logTypeOptions;
            cbLogDetails.ItemsSource = logDetailOptions;
            cbLogTypes.SelectedIndex = 0;
            cbLogDetails.SelectedIndex = 0;

            LoadLogFiles();
            GenerateLogView();
        }

        private void LoadLogFiles()
        {
            string directoryPath = Log.GetLogDirectoryPath();
            if (!Directory.Exists(directoryPath)) return;

            logEntries.Clear();
            string[] files = Directory.GetFiles(directoryPath, "*.log");

            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                logEntries[fileName] = File.ReadAllLines(file).ToList();
            }
        }

        private void expandAllItem(Expander mainExpander)
        {
            bool isAllSubItemsExpanded = true;
            foreach (var child in ((StackPanel)mainExpander.Content).Children)
            {
                if (child is Expander subExpander && !subExpander.IsExpanded)
                {
                    isAllSubItemsExpanded = false;
                    break;
                }
            }

            mainExpander.IsExpanded = !mainExpander.IsExpanded;

            if(!isAllSubItemsExpanded)
            {
                mainExpander.IsExpanded = true;
            }
            foreach (var child in ((StackPanel)mainExpander.Content).Children)
            {
                if (child is Expander subExpander)
                {
                    subExpander.IsExpanded = mainExpander.IsExpanded;
                }
            }
        }

        private void expandAll_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Wpf.Ui.Controls.Button btn)
            {
                if(btn.Parent is Grid grid)
                {
                    expandAllItem(grid.Parent as Expander);
                }
            }
        }

        private void GenerateLogView()
        {
            stLogs.Children.Clear();

            foreach (var entry in logEntries)
            {
                string fileName = entry.Key;
                List<string> lines = entry.Value;

                Expander expander = new Expander
                {
                    Margin = new Thickness(5)
                };

                Grid headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

                Wpf.Ui.Controls.TextBlock headerTextBlock = new Wpf.Ui.Controls.TextBlock
                {
                    Text = fileName,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(headerTextBlock, 0);

                Wpf.Ui.Controls.Button headerButton = new Wpf.Ui.Controls.Button
                {
                    Content = "Expand All",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                headerButton.Click += expandAll_Click;
                Grid.SetColumn(headerButton, 1);

                headerGrid.Children.Add(headerTextBlock);
                headerGrid.Children.Add(headerButton);

                expander.Header = headerGrid;

                expander.Content = new Wpf.Ui.Controls.TextBlock
                {
                    Text = "Ez a kiterjesztett tartalom.",
                    Margin = new Thickness(10)
                };



                StackPanel stackPanel = new StackPanel();

                for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
                {
                    string? line = lines[lineIndex];
                    bool last = lineIndex == lines.Count - 1;

                    string[] lineData = line.Replace("\n", "").Split(" - ");
                    if (lineData.Length < 2) continue;

                    LogType type;
                    bool success = Enum.TryParse(lineData[0], true, out type);
                    if (!success) continue;

                    if (cbLogTypes.SelectedIndex != 0 && type.ToString() != cbLogTypes.SelectedValue.ToString())
                        continue;

                    if (cbLogDetails.SelectedIndex == 0 && !string.IsNullOrEmpty(tbFilter.Text) && !lineData[2..].Any(x => x.Contains(tbFilter.Text)))
                        continue;

                    if (cbLogDetails.SelectedIndex != 0 && !string.IsNullOrEmpty(tbFilter.Text) && !lineData[2..].Any(x =>
                        Log.LogDataTypes[type].Contains(cbLogDetails.SelectedValue.ToString()) && x.Contains(tbFilter.Text)))
                        continue;

                    string headerText = $"{type} - {DateTime.ParseExact(lineData[1], "yyyy.MM.dd. HH:mm:ss", CultureInfo.InvariantCulture):HH:mm}";

                    if (lineData.Length > 2)
                    {
                        Expander subExpander = new Expander
                        {
                            Header = headerText,
                            Margin = last ? new Thickness(0) : new Thickness(0, 0, 0, 8),
                        };
                        subExpander.SetResourceReference(Control.BorderBrushProperty, "ControlStrokeColorDefaultBrush");
                        stackPanel.Children.Add(subExpander);

                        StackPanel subStackPanel = new StackPanel();
                        List<string> cardData = new();

                        for (int i = 0; i < lineData[2..].Length; i++)
                        {
                            cardData.Add($"{Log.LogDataTypes[type][i]}: {lineData[i + 2]}");
                        }

                        subStackPanel.Children.Add(GetUnexpandableCard(cardData, true));
                        subExpander.Content = subStackPanel;
                    }
                    else
                    {
                        Border card = GetUnexpandableCard(new() { headerText }, last);
                        stackPanel.Children.Add(card);
                    }
                }

                if(stackPanel.Children.Count != 0) 
                {
                    expander.Content = stackPanel;
                    stLogs.Children.Add(expander);
                }
            }
        }

        private Border GetUnexpandableCard(List<string> headerTextList, bool last)
        {
            Border cardContainer = new Border()
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
                Wpf.Ui.Controls.TextBlock headerTextBlock = new Wpf.Ui.Controls.TextBlock()
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

        private void cbLogTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = cbLogTypes.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(selectedValue) && cbLogTypes.SelectedIndex != 0) 
            {
                cbLogDetails.ItemsSource = new[] { "All log details" }.Concat(logDetailOptions.Where(x => Log.LogDataTypes[Enum.Parse<LogType>(selectedValue)].Contains(x))).ToList();
                cbLogDetails.SelectedIndex = 0;
            } else
            {
                cbLogDetails.ItemsSource = logDetailOptions;
            }
            GenerateLogView();
        }

        private void cbLogDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateLogView();
        }

        private void tbFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            GenerateLogView();
        }
    }
}
