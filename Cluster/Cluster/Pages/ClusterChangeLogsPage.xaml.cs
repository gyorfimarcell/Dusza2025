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
    public partial class ClusterChangeLogsPage : Page
    {
        public ClusterChangeLogsPage()
        {
            InitializeComponent();
            GenerateLogView();
        }

        private void GenerateLogView()
        {
            string directoryPath = Log.GetLogDirectoryPath();

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.log");

                stLogs.Children.Clear();

                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);

                    Expander expander = new Expander
                    {
                        Header = fileName,
                        Margin = new Thickness(5)
                    };

                    StackPanel stackPanel = new StackPanel();

                    string[] lines = File.ReadAllLines(file);

                    foreach (var line in lines)
                    {
                        string[] lineData = line.Replace("\n", "").Split(" - ");
                        LogType type;
                        bool success = Enum.TryParse(lineData[0], true, out type);
                        string headerText = $"{type.ToString()} - {DateTime.ParseExact(lineData[1], "yyyy.MM.dd. HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm")}";

                        if (lineData[2..].Length > 0)
                        {
                            Expander subExpander = new Expander
                            {
                                Header = headerText,
                                Margin = new Thickness(5)
                            };
                            subExpander.SetResourceReference(Control.BorderBrushProperty, "ControlStrokeColorDefaultBrush");
                            stackPanel.Children.Add(subExpander);
                            lineData = lineData[2..];
                            StackPanel subStackPanel = new StackPanel();
                            List<string> cardData = new();
                            for (int i = 0; i < lineData.Length; i++)
                            {
                                    cardData.Add($"{Log.LogDataTypes[type][i]}: {lineData[i]}");
                            }
                            subStackPanel.Children.Add(GetUnexpandableCard(cardData));
                            subExpander.Content = subStackPanel;
                        }
                        else
                        {
                            Border card = GetUnexpandableCard(new() { headerText });
                            stackPanel.Children.Add(card);
                        }
                    }

                    expander.Content = stackPanel;
                    stLogs.Children.Add(expander);
                }
            }
        }

        private Border GetUnexpandableCard(List<string> headerTextList)
        {
            Border cardContainer = new Border()
            {
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(5),
                Margin = new Thickness(5),
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

    }
}
