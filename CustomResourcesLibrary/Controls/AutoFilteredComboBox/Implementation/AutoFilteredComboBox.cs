using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GeoVis.Plugin.CustomResourcesLibrary
{
    public class AutoFilteredComboBox : ComboBox
    {
        #region 字段
        private bool _ignoreTextChanged;
        private string _currentText;
        #endregion

        #region 构造函数
        public AutoFilteredComboBox()
        {
            IsEditable = true;
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
        }
        #endregion

        #region 依赖属性
        public static readonly DependencyProperty FilterDelegateProperty =
            DependencyProperty.Register("FilterDelegate", typeof(Func<object, string, bool>), typeof(AutoFilteredComboBox), new PropertyMetadata(null));

        [Description("The way the combo Box for treat the Match Pattern")]
        public Func<object, string, bool> FilterDelegate
        {
            get { return (Func<object, string, bool>)GetValue(FilterDelegateProperty); }
            set { SetValue(FilterDelegateProperty, value); }
        }

        public static readonly DependencyProperty IsCaseSensitiveProperty =
            DependencyProperty.Register("IsCaseSensitiveProperty", typeof(bool), typeof(AutoFilteredComboBox), new UIPropertyMetadata(false));

        [Description("The way the combo Box treats the case sensitivity of typed text.")]
        [Category("IsCaseSensitive")]
        [DefaultValue(true)]
        public bool IsCaseSensitive
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (bool)GetValue(IsCaseSensitiveProperty); }
            [System.Diagnostics.DebuggerStepThrough]
            set { SetValue(IsCaseSensitiveProperty, value); }
        }

        public static readonly DependencyProperty DropDownOnFocusProperty =
            DependencyProperty.Register("DropDownOnFocus", typeof(bool), typeof(AutoFilteredComboBox), new UIPropertyMetadata(false));

        [Description("The way the combo Box hehave when it receives focus")]
        [Category("DropDownOnFocus")]
        [DefaultValue(false)]
        public bool DropDownOnFocus
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (bool)GetValue(DropDownOnFocusProperty);
            }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                SetValue(DropDownOnFocusProperty, value);
            }
        }
        #endregion

        #region 路由事件
        public delegate void FilterConfirmedEventHandler(object sender, FilterConfirmedEventArgs e);
        /// <summary>
        /// 自定义路由事件（用户按下回车键或输入筛选条件选中筛选项时触发该事件，该事件会对外传出当前筛选选中的结果，存于FilterConfirmedEventArgs类中的FilterConfirmedItem属性）
        /// </summary>
        public static readonly RoutedEvent FilterConfirmedEvent = EventManager.RegisterRoutedEvent("FilterConfirmed", RoutingStrategy.Direct, typeof(FilterConfirmedEventHandler), typeof(AutoFilteredComboBox));

        public event FilterConfirmedEventHandler FilterConfirmed
        {
            add { AddHandler(FilterConfirmedEvent, value); }
            remove { RemoveHandler(FilterConfirmedEvent, value); }
        }

        private void RaiseFilterConfirmedEvent(object filterConfitmedItem)
        {
            FilterConfirmedEventArgs newEventArgs = new FilterConfirmedEventArgs(AutoFilteredComboBox.FilterConfirmedEvent, this, filterConfitmedItem);
            RaiseEvent(newEventArgs);
        }
        #endregion

        #region 方法

        #region 处理焦点
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (ItemsSource != null && DropDownOnFocus)
                IsDropDownOpen = true;
        }
        #endregion

        #region 处理输入
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnTextChanged));
            KeyUp += AutoFilteredComboxBox_KeyUp;
            IsTextSearchEnabled = false;
        }
        private void AutoFilteredComboxBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (IsDropDownOpen)
                {
                    if (Keyboard.FocusedElement is TextBox)
                    {
                        Keyboard.Focus(this);
                        if (Items.Count > 0)
                        {
                            if (SelectedIndex == -1 || SelectedIndex == 0)
                                SelectedIndex = 0;
                        }
                    }
                }
            }
            if (e.Key == Key.Enter)
            {
                if (SelectedIndex >= 0)
                    RaiseFilterConfirmedEvent(SelectedItem);
            }
            if (Keyboard.FocusedElement is TextBox)
            {
                if (e.OriginalSource is TextBox)
                {
                    TextBox textBox = e.OriginalSource as TextBox;
                    if (textBox.Text.Length == 1 && textBox.SelectionLength == 1 && textBox!= null)
                    {
                        textBox.SelectionLength = 0;
                        textBox.SelectionStart = 1;
                    }
                }
            }
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_ignoreTextChanged)
            {
                RaiseFilterConfirmedEvent(SelectedItem);
                return;
            }
            _currentText = Text;
            if (!IsTextSearchEnabled)
            {
                RefreshFilter();
            }
        }
        #endregion

        #region 处理筛选
        private void RefreshFilter()
        {
            if (ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemsSource);
                view.Refresh();
                SelectedIndex = -1;
                IsDropDownOpen = true;
            }
        }

        private bool FilterPredicate(object value)
        {
            if (value == null)
                return false;
            if (string.IsNullOrEmpty(_currentText))
                return true;
            Func<object, string, bool> filterItem = FilterDelegate;
            if (filterItem != null)
                return filterItem(value, _currentText);
            return IsCaseSensitive ?
                value.ToString().Contains(_currentText) :
                value.ToString().ToUpper().Contains(_currentText.ToUpper());
        }
        #endregion

        #region 处理选择变更、源变化
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            try
            {
                _ignoreTextChanged = true;
                base.OnSelectionChanged(e);
            }
            finally
            {
                _ignoreTextChanged = false;
            }

        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(newValue);
                view.Filter += FilterPredicate;
            }
            if (oldValue != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(oldValue);
                if (view.Filter != null) view.Filter -= FilterPredicate;
            }
            base.OnItemsSourceChanged(oldValue, newValue);
        }
        #endregion

        #endregion
    }
}
