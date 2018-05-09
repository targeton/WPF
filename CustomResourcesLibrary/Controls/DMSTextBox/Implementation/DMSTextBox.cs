using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeoVis.Plugin.CustomResourcesLibrary
{
    public class DMSTextBox : TextBox
    {
        #region 变量
        internal List<DMSInfo> _dmsInfoList = new List<DMSInfo>();
        internal DMSInfo _selectedDMSInfo;
        internal bool _fireSelectionChangedEvent = true;
        #endregion

        #region 属性
        public static readonly DependencyProperty StrValueProperty =
            DependencyProperty.Register("StrValue", typeof(string), typeof(DMSTextBox), new PropertyMetadata("", OnStrValueChanged, OnCoerceStrValue));

        public string StrValue
        {
            get { return (string)GetValue(StrValueProperty); }
            set { SetValue(StrValueProperty, value); }
        }

        private static void OnStrValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DMSTextBox dmsTextBox = d as DMSTextBox;
            if (dmsTextBox != null)
                dmsTextBox.InitializeDMSInfoList();
        }

        private static object OnCoerceStrValue(DependencyObject d, object baseValue)
        {
            var str = (string)baseValue;
            if (string.IsNullOrEmpty(str))
                return "";
            double outValue;
            if (double.TryParse(str, out outValue))
                str = outValue.ToString("N7");
            else
                return "0.0000000";
            var dmsTextBox = d as DMSTextBox;
            var pattern = @"^-*\d|[1-9]\d|1[0-7]\d\.[0-5]\d{1}[0-5]\d{4}$";
            if (d != null)
                pattern = dmsTextBox.LonLatClass == LonLatType.Longitude ? pattern : @"^-*\d|[1-8]\d\.[0-5]\d{1}[0-5]\d{4}$";
            if (Regex.IsMatch(str, @"^-*\d{1,3}\.[0-5]\d{1}[0-5]\d{4}$"))
                return str;
            else
                return "0.0000000";
        }

        private LonLatType _lonLatClass = LonLatType.Longitude;
        public LonLatType LonLatClass
        {
            get { return _lonLatClass; }
            set { _lonLatClass = value; }
        }

        #endregion

        #region 构造函数
        public DMSTextBox()
            : base()
        {
            InitializeDMSInfoList();
            this.AllowDrop = false;
        }
        #endregion

        #region 方法
        protected void InitializeDMSInfoList()
        {
            _dmsInfoList.Clear();
            _selectedDMSInfo = null;
            string strValue = this.StrValue;
            string interValue = "0";
            string decimalValue = "0000000";
            string[] splitStr = strValue.Split('.');
            if (splitStr.Length == 2)
            {
                interValue = splitStr[0];
                decimalValue = splitStr[1];
            }
            int index = 0;
            _dmsInfoList.Add(new DMSInfo()
            {
                StartPosition = index,
                Length = interValue.Length,
                Content = interValue,
                Type = DMSPart.Degree
            });
            index += interValue.Length;
            _dmsInfoList.Add(new DMSInfo()
            {
                StartPosition = index,
                Length = 1,
                Content = "°",
                Type = DMSPart.Separator
            });
            index += 1;
            _dmsInfoList.Add(new DMSInfo()
            {
                StartPosition = index,
                Length = 2,
                Content = decimalValue.Substring(0, 2),
                Type = DMSPart.Minute
            });
            index += 2;
            _dmsInfoList.Add(new DMSInfo()
            {
                StartPosition = index,
                Length = 1,
                Content = "'",
                Type = DMSPart.Separator
            });
            index += 1;
            _dmsInfoList.Add(new DMSInfo()
            {
                StartPosition = index,
                Length = 6,
                Content = decimalValue.Substring(2, 5).Insert(2, "."),
                Type = DMSPart.Second
            });
            index += 6;
            _dmsInfoList.Add(new DMSInfo()
            {
                StartPosition = index,
                Length = 1,
                Content = "\"",
                Type = DMSPart.Separator
            });
            index += 1;

            SetDMSText();
        }

        private void SetDMSText()
        {
            var text = string.Empty;
            foreach (var info in _dmsInfoList)
            {
                text += info.Content;
            }
            this.Text = string.IsNullOrEmpty(this.StrValue) ? "" : text;
        }

        internal DMSInfo GetDMSInfo(int selectionStart)
        {
            DMSInfo result = null;
            while (result == null)
            {
                result = _dmsInfoList.FirstOrDefault(info => info.Type != DMSPart.Separator && (info.StartPosition <= selectionStart) && (selectionStart < (info.StartPosition + info.Length)));
                selectionStart += 1;
                if (selectionStart >= this.Text.Length)
                {
                    result = _dmsInfoList.LastOrDefault(o => o.Type != DMSPart.Separator);
                    break;
                }
                if (selectionStart <= 0)
                {
                    result = _dmsInfoList.FirstOrDefault(o => o.Type != DMSPart.Separator);
                    break;
                }
            }
            return result;
        }

        internal void Select(DMSInfo info)
        {
            if (info != null)
            {
                _fireSelectionChangedEvent = false;
                this.Select(info.StartPosition, info.Length);
                _fireSelectionChangedEvent = true;
                _selectedDMSInfo = info;
            }
        }

        private void PerformMouseSelection()
        {
            var dmsInfo = this.GetDMSInfo(this.SelectionStart);
            if (_dmsInfoList.Count(info => info.Type != DMSPart.Separator && string.IsNullOrEmpty(info.Content)) > 0)
            {
                ConvertListToStrValue();
                var str = this.StrValue;
                str = double.Parse(str).ToString("N7");
                var strs = str.Split('.');
                var index = _dmsInfoList.FindIndex(info => info.Type == DMSPart.Degree);
                _dmsInfoList[index].Content = strs[0];
                index = _dmsInfoList.FindIndex(info => info.Type == DMSPart.Minute);
                _dmsInfoList[index].Content = strs[1].Substring(0, 2);
                index = _dmsInfoList.FindIndex(info => info.Type == DMSPart.Second);
                _dmsInfoList[index].Content = strs[1].Substring(2, 5).Insert(2, ".");
                SetDMSText();
            }
            dmsInfo = _dmsInfoList.FirstOrDefault(info => info.Type == dmsInfo.Type);
            this.Select(dmsInfo);
        }

        private void PerformKeyboardSelection(int nextSelectionStart)
        {
            this.Focus();
            int selectedDMSStartPosition = _selectedDMSInfo != null ? _selectedDMSInfo.StartPosition : 0;
            int direction = nextSelectionStart - selectedDMSStartPosition;
            if (direction > 0)
                this.Select(this.GetNextDMSInfo(nextSelectionStart));
            else
                this.Select(this.GetPreviousDMSInfo(nextSelectionStart - 1));
        }

        private DMSInfo GetNextDMSInfo(int nextSelectionStart)
        {
            DMSInfo info = this.GetDMSInfo(nextSelectionStart);
            if (info == null)
                info = _dmsInfoList.FirstOrDefault(o => o.Type != DMSPart.Separator);
            return info;
        }

        private DMSInfo GetPreviousDMSInfo(int previousSelectionStart)
        {
            DMSInfo info = this.GetDMSInfo(previousSelectionStart);
            if (info == null)
                info = _dmsInfoList.LastOrDefault(o => o.Type != DMSPart.Separator);
            return info;
        }

        private string ConvertListToStr()
        {
            var strValue = string.Empty;
            var degreeInfo = _dmsInfoList.FirstOrDefault(info => info.Type == DMSPart.Degree);
            if (degreeInfo == null || string.IsNullOrEmpty(degreeInfo.Content))
                strValue += "0";
            else
                strValue += degreeInfo.Content == "-" ? "0" : degreeInfo.Content;
            strValue += ".";
            var minuteInfo = _dmsInfoList.FirstOrDefault(info => info.Type == DMSPart.Minute);
            if (minuteInfo == null || string.IsNullOrEmpty(minuteInfo.Content))
                strValue += "00";
            else
            {
                int outValue = 0;
                if (int.TryParse(minuteInfo.Content, out outValue))
                    strValue += outValue.ToString("00");
                else
                    strValue += "00";
            }
            var secondInfo = _dmsInfoList.FirstOrDefault(info => info.Type == DMSPart.Second);
            if (secondInfo == null || string.IsNullOrEmpty(secondInfo.Content))
                strValue += "00000";
            else
            {
                double outValue = 0;
                if (double.TryParse(secondInfo.Content, out outValue))
                    strValue += outValue.ToString("00.000").Replace(".", "");
                else
                    strValue += "00000";
            }
            return strValue;
        }

        private void ConvertListToStrValue()
        {
            var str = ConvertListToStr();
            this.StrValue = str;
        }

        private void ConvertTextToList(string text)
        {
            string[] str = text.Split('°');
            var index = 0;
            if (str.Length == 2)
            {
                var degreeInfo = _dmsInfoList.FirstOrDefault(info => info.Type == DMSPart.Degree);
                if (degreeInfo != null)
                {
                    degreeInfo.Content = str[0];
                    degreeInfo.StartPosition = index;
                    degreeInfo.Length = str[0].Length;
                    index += str[0].Length;
                }
                var separtorInfo = _dmsInfoList.FirstOrDefault(info => info.Content == "°");
                if (separtorInfo != null)
                {
                    separtorInfo.StartPosition = index;
                    index += 1;
                }
                str = str[1].Split('\'');
                if (str.Length == 2)
                {
                    var minuteInfo = _dmsInfoList.FirstOrDefault(info => info.Type == DMSPart.Minute);
                    if (minuteInfo != null)
                    {
                        minuteInfo.Content = str[0];
                        minuteInfo.StartPosition = index;
                        minuteInfo.Length = str[0].Length;
                        index += str[0].Length;
                    }
                    separtorInfo = _dmsInfoList.FirstOrDefault(info => info.Content == "'");
                    if (separtorInfo != null)
                    {
                        separtorInfo.StartPosition = index;
                        index += 1;
                    }
                    var secondInfo = _dmsInfoList.FirstOrDefault(info => info.Type == DMSPart.Second);
                    if (secondInfo != null)
                    {
                        var secondStr = str[1].Replace("\"", "");
                        secondInfo.Content = secondStr;
                        secondInfo.StartPosition = index;
                        secondInfo.Length = secondStr.Length;
                        index += secondStr.Length;
                    }
                    separtorInfo = _dmsInfoList.FirstOrDefault(info => info.Content == "\"");
                    if (separtorInfo != null)
                    {
                        separtorInfo.StartPosition = index;
                        //index += 1;
                    }
                }
            }
        }
        #endregion

        #region 基类方法重写
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            int selectionStart = _selectedDMSInfo == null ? 0 : _selectedDMSInfo.StartPosition;
            int selectionLength = _selectedDMSInfo == null ? 0 : _selectedDMSInfo.Length;
            switch (e.Key)
            {
                case Key.Enter:
                    ConvertListToStrValue();
                    break;
                case Key.Right:
                    this.PerformKeyboardSelection(selectionStart + selectionLength);
                    e.Handled = true;
                    _fireSelectionChangedEvent = false;
                    break;
                case Key.Left:
                    this.PerformKeyboardSelection(selectionStart > 0 ? selectionStart - 1 : 0);
                    e.Handled = true;
                    _fireSelectionChangedEvent = false;
                    break;
                default:
                    _fireSelectionChangedEvent = false;
                    break;
            }
            if (this.SelectionLength == 0 && (e.Key == Key.Back || e.Key == Key.Delete))
            {
                var pos = e.Key == Key.Back ? this.CaretIndex - 1 : this.CaretIndex;
                if (pos < 0 || pos >= this.Text.Length - 1 || this.Text[pos] == '°' || this.Text[pos] == '\'' || this.Text[pos] == '\"')
                    e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (_selectedDMSInfo == null)
            {
                this.Select(this.GetDMSInfo(0));
            }
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            ConvertListToStrValue();
            base.OnLostFocus(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            if (_fireSelectionChangedEvent)
                this.PerformMouseSelection();
            else
                _fireSelectionChangedEvent = true;
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {

            base.OnTextInput(e);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"[0-9.]"))
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            var text = this.Text;
            var pattern = LonLatClass == LonLatType.Longitude ? "^-*(\\d{0}|\\d|[1-9]\\d|1[0-7]\\d)[°](\\d|[0-5]\\d)['](\\d|[0-5]\\d)(\\.\\d{0,3})?[\"]$" : "^-*(\\d{0}|\\d|[1-8]\\d)[°](\\d|[0-5]\\d)['](\\d|[0-5]\\d)(\\.\\d{0,3})?[\"]$";
            if (Regex.IsMatch(text, pattern))
            {
                ConvertTextToList(text);
            }
            else
            {
                if (Regex.IsMatch(text, "^\\d*[°]\\d*['](\\d*\\.\\d*)?[\"]$"))
                    ConvertTextToList(text);
                TextChange change = e.Changes.ElementAt(0) as TextChange;
                if (change != null)
                {
                    this.Text = text.Remove(change.Offset, change.AddedLength);
                    this.CaretIndex = change.Offset;
                }
            }
            base.OnTextChanged(e);
        }
        #endregion

    }
}
