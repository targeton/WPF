using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GeoVis.Plugin.CustomResourcesLibrary
{
    public class RegexTextBox : TextBox
    {
        //#region 变量
        //internal string _preText;
        //internal string _inputText;
        //#endregion

        #region 依赖属性

        public static string GetRegexPattern(DependencyObject obj)
        {
            return (string)obj.GetValue(RegexPatternProperty);
        }

        public static void SetRegexPattern(DependencyObject obj, string value)
        {
            obj.SetValue(RegexPatternProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegexPatternProperty =
            DependencyProperty.Register("RegexPattern", typeof(string), typeof(RegexTextBox), new PropertyMetadata(""));

        public static RegexOption GetFilterOption(DependencyObject obj)
        {
            return (RegexOption)obj.GetValue(FilterOptionProperty);
        }

        public static void SetFilterOption(DependencyObject obj, RegexOption value)
        {
            obj.SetValue(FilterOptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterOptionProperty =
            DependencyProperty.Register("FilterOption", typeof(RegexOption), typeof(RegexTextBox), new PropertyMetadata(RegexOption.None, OnFilterOptionChanged));

        private static void OnFilterOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((RegexOption)e.NewValue == (RegexOption)e.OldValue)
                return;
            switch ((RegexOption)e.NewValue)
            {
                case RegexOption.None:
                    SetRegexPattern(d, "");
                    break;
                case RegexOption.PosInt:
                    SetRegexPattern(d, @"^(0|[1-9]\d*)$");
                    break;
                case RegexOption.NegInt:
                    SetRegexPattern(d, @"^-(0|[1-9]\d*)?$");
                    break;
                case RegexOption.Int:
                    SetRegexPattern(d, @"^-?(0|[1-9]\d*)?$");
                    break;
                case RegexOption.PosDecimal:
                    SetRegexPattern(d, @"^(0|[1-9]\d*)(\.\d*)?$");
                    break;
                case RegexOption.NegDecimal:
                    SetRegexPattern(d, @"^-((0|[1-9]\d*)(\.\d*)?)?$");
                    break;
                case RegexOption.Decimal:
                    SetRegexPattern(d, @"^-?((0|[1-9]\d*)(\.\d*)?)?$");
                    break;
                case RegexOption.Alphabet:
                    SetRegexPattern(d, @"^[a-zA-Z]+$");
                    break;
                case RegexOption.Custom:
                    break;
                default:
                    break;
            }

        }

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(RegexTextBox), new PropertyMetadata(double.MinValue));




        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(RegexTextBox), new PropertyMetadata(double.MaxValue));

        #endregion

        #region 构造函数
        public RegexTextBox()
        {

        }
        #endregion

        #region MyRegion
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            var pattern = GetRegexPattern(this);
            if (string.IsNullOrEmpty(pattern))
                base.OnPreviewTextInput(e);
            else
            {
                var text = this.Text;
                var input = e.Text;
                var selectStart = this.SelectionStart;
                var selectLength = this.SelectionLength;
                if (selectLength > 0)
                {
                    text = text.Remove(selectStart, selectLength);
                }
                var index = this.CaretIndex;
                text = text.Insert(index, input);
                var re = new Regex(pattern);
                if (!re.IsMatch(text))
                {
                    e.Handled = true;
                    return;
                }
                bool numNotMatch = false;
                var filterOption = GetFilterOption(this);
                if ((int)filterOption >= 2 && (int)filterOption <= 7)
                {
                    double convResult;
                    if (!double.TryParse(text, out convResult))
                        numNotMatch = true;
                    else
                        numNotMatch = !(convResult >= ((double)this.GetValue(MinValueProperty)) && convResult <= ((double)this.GetValue(MaxValueProperty)));
                }
                e.Handled = numNotMatch;
            }
        }

        #endregion
    }

    public enum RegexOption
    {
        None,
        PosInt,
        NegInt,
        Int,
        PosDecimal,
        NegDecimal,
        Decimal,
        Alphabet,
        Custom
    }
}
