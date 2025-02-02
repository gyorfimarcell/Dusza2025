using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Cluster;

public class CustomPage : Page
{
    private readonly MainWindow _window;
    
    public static readonly DependencyProperty HeaderControlsProperty =
        DependencyProperty.Register(
            nameof(HeaderControls),
            typeof(ObservableCollection<UIElement>),
            typeof(CustomPage),
            new PropertyMetadata(null));
    
    public CustomPage()
    {
        _window = (MainWindow)Application.Current.MainWindow!;
        SetValue(HeaderControlsProperty, new ObservableCollection<UIElement>());
    }
    
    public ObservableCollection<UIElement> HeaderControls
    {
        get => (ObservableCollection<UIElement>)GetValue(HeaderControlsProperty);
        set => SetValue(HeaderControlsProperty, value);
    }

    public void ChangeTitle(string title)
    {
        ObservableCollection<object> newItemSource = new((IEnumerable<object>)_window.BreadcrumbBar.ItemsSource);
        newItemSource.RemoveAt(newItemSource.Count - 1);
        newItemSource.Add(new CustomBreadcrumbItem(title));
        _window.BreadcrumbBar.ItemsSource = newItemSource;
    }
}

internal class CustomBreadcrumbItem(string title)
{
    public object Content { get; set; } = title;
}
