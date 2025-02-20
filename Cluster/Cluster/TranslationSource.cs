using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Cluster
{
    public class TranslationSource : INotifyPropertyChanged
    {
        private static readonly TranslationSource _instance = new TranslationSource();
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        public static TranslationSource Instance => _instance;

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture != value)
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
                foreach (string var in vars)
                {
                    text = text.Replace("{" + i + "}", vars[i]);
                }
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

    public class TranslateExtension : MarkupExtension
    {
        private string _key;

        public TranslateExtension(string key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding($"[{_key}]")
            {
                Source = TranslationSource.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            return binding.ProvideValue(serviceProvider);
        }
    }

    public class TranslateStaticExtension : MarkupExtension
    {
        private string _key;

        public TranslateStaticExtension(string key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return TranslationSource.Instance[_key];
        }
    }
}
