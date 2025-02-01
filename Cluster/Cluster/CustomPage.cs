using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Cluster;

public class CustomPage : Page
{
    public static readonly DependencyProperty HeaderControlsProperty =
        DependencyProperty.Register(
            nameof(HeaderControls),
            typeof(ObservableCollection<UIElement>),
            typeof(CustomPage),
            new PropertyMetadata(null));
    
    public CustomPage()
    {
        SetValue(HeaderControlsProperty, new ObservableCollection<UIElement>());
    }
    
    public ObservableCollection<UIElement> HeaderControls
    {
        get => (ObservableCollection<UIElement>)GetValue(HeaderControlsProperty);
        set => SetValue(HeaderControlsProperty, value);
    }
}