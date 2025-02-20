using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Cluster.Controls;

public partial class EmptyStatus : UserControl
{
    public EmptyStatus()
    {
        InitializeComponent();
    }
    
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(EmptyStatus), new PropertyMetadata("Title"));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(EmptyStatus), new PropertyMetadata(""));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
        typeof(string), typeof(EmptyStatus), new PropertyMetadata(""));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public delegate void StatusClickHandler(object sender, EventArgs e);
    public event StatusClickHandler? Click;

    private void EmptyStatus_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        Click?.Invoke(this, EventArgs.Empty);
    }
}