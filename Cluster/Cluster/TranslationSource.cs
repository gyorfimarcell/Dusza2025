using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using System.Windows.Markup;

namespace Cluster
{
    public class TranslationSource : INotifyPropertyChanged
    {
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        public static TranslationSource Instance { get; } = new();

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (!Equals(_currentCulture, value))
                {
                    _currentCulture = value;
                    CultureInfo.CurrentUICulture = value;
                    OnCurrentCultureChanged();
                }
            }
        }

        private TranslationSource()
        {
            _resourceManager = new ResourceManager("Cluster.Resources.Strings", typeof(TranslationSource).Assembly);
            _currentCulture = CultureInfo.CurrentUICulture;
        }

        public string this[string key]
            => _resourceManager.GetString(key, _currentCulture) ?? $"#{key}#";

        public string WithParam(string key, params string[] vars)
        {
            string text = this[key];
            for (int i = 0; i < vars.Length; i++)
            {
                text = text.Replace("{" + i + "}", vars[i]);
            }

            return text;
        }

        public static string T(string key) => Instance[key];

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnCurrentCultureChanged()
        {
            CultureInfo.CurrentCulture.ClearCachedData();
            CultureInfo.CurrentUICulture.ClearCachedData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }

    public class TranslateExtension(string key) : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding($"[{key}]")
            {
                Source = TranslationSource.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            return binding.ProvideValue(serviceProvider);
        }
    }

    public class TranslateStaticExtension(string key) : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return TranslationSource.Instance[key];
        }
    }
}