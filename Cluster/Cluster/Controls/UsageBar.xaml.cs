using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Cluster.Controls;

public partial class UsageBar : UserControl
{
    public UsageBar()
    {
        InitializeComponent();
    }

    private void UsageBar_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(UsageBar), new PropertyMetadata("Usage"));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty CurrentProperty =
        DependencyProperty.Register(nameof(Current), typeof(int), typeof(UsageBar), new PropertyMetadata(0, new PropertyChangedCallback(OnNumChanged)));

    public int Current
    {
        get => (int)GetValue(CurrentProperty);
        set => SetValue(CurrentProperty, value);
    }

    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register(nameof(Max), typeof(int), typeof(UsageBar), new PropertyMetadata(100, new PropertyChangedCallback(OnNumChanged)));

    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    private static void OnNumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var testControl = d as UsageBar;
        if (testControl != null)
        {
            testControl.Percent = $"({testControl.Current / (double)testControl.Max * 100:f0}%)";
        }

    }

    public static readonly DependencyProperty PercentProperty =
        DependencyProperty.Register(nameof(Percent), typeof(string), typeof(UsageBar), new PropertyMetadata(""));

    public string Percent
    {
        get => (string)GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }
}