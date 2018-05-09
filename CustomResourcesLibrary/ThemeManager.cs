using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GeoVis.Plugin.CustomResourcesLibrary
{
    public class ThemeManager
    {
        #region 附加属性

        #region ThemeName
        public static ThemeNames GetThemeName(DependencyObject obj)
        {
            return (ThemeNames)obj.GetValue(ThemeNameProperty);
        }

        public static void SetThemeName(DependencyObject obj, ThemeNames value)
        {
            obj.SetValue(ThemeNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for ThemeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeNameProperty =
            DependencyProperty.RegisterAttached("ThemeName", typeof(ThemeNames), typeof(FrameworkElement), new PropertyMetadata(ThemeNames.None, new PropertyChangedCallback(OnThemeNameChanged)));

        private static void OnThemeNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element == null) return;
            element.Resources.MergedDictionaries.Clear();
            var themeName = (ThemeNames)e.NewValue;
            ResourceDictionary resourceDictionary = null;
            switch (themeName)
            {
                case ThemeNames.Theme901:
                    resourceDictionary = new ResourceDictionary();
                    resourceDictionary.Source = new Uri("pack://application:,,,/CustomResourcesLibrary;component/Themes/Generic.xaml");
                    break;
                case ThemeNames.None:
                    break;
                default:
                    break;
            }
            if (resourceDictionary == null) return;
            element.Resources.MergedDictionaries.Add(resourceDictionary);
        }
        #endregion

        #endregion

    }

    public enum ThemeNames
    {
        Theme901,
        None
    }
}
