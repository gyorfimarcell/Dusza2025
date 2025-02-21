using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Wpf.Ui.Controls;

namespace Cluster.Controls;

[ContentProperty(nameof(Content))]
public class CustomCard : Control
{
    static CustomCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomCard), new FrameworkPropertyMetadata(typeof(CustomCard)));
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
        typeof(IconElement), typeof(CustomCard), new PropertyMetadata(null));

    public IconElement? Icon
    {
        get => (IconElement?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
        typeof(CustomCard), new PropertyMetadata(default(string)));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description),
        typeof(string), typeof(CustomCard), new PropertyMetadata(default(string)));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(CustomCard));

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
}