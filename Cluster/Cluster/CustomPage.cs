using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Cluster;

public class CustomPage : Page
{
    protected readonly MainWindow _window;

    public static readonly DependencyProperty HeaderControlsProperty =
        DependencyProperty.Register(
            nameof(HeaderControls),
            typeof(ObservableCollection<UIElement>),
            typeof(CustomPage),
            new PropertyMetadata(null));

    protected CustomPage()
    {
        _window = (MainWindow)Application.Current.MainWindow!;
        SetValue(HeaderControlsProperty, new ObservableCollection<UIElement>());
    }

    public ObservableCollection<UIElement> HeaderControls
    {
        get => (ObservableCollection<UIElement>)GetValue(HeaderControlsProperty);
        set => SetValue(HeaderControlsProperty, value);
    }

    protected void ChangeTitle(string title)
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