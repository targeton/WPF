using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeoVis.Plugin.CustomResourcesLibrary
{
    [TemplatePart(Name = PART_WindowRoot, Type = typeof(Panel))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MinMaxButton, Type = typeof(Button))]
    public class CustomWindow : Window
    {
        private const string PART_WindowRoot = "PART_WindowRoot";
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_MinMaxButton = "PART_MinMaxButton";
        #region 构造函数
        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
        }
        #endregion

        #region 依赖属性

        #region HeaderBlank
        /// <summary>
        /// 窗口标题和关闭按钮间的空白区域
        /// </summary>
        public object HeaderBlank
        {
            get { return (object)GetValue(HeaderBlankProperty); }
            set { SetValue(HeaderBlankProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBlank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBlankProperty =
            DependencyProperty.Register("HeaderBlank", typeof(object), typeof(CustomWindow), new PropertyMetadata(null, OnHeaderBlankChanged));

        private static void OnHeaderBlankChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomWindow win = d as CustomWindow;
            win.RemoveLogicalChild(e.OldValue);
            win.AddLogicalChild(e.NewValue);
        }
        #endregion

        #region HeaderBlankTemplate
        /// <summary>
        /// 窗口标题和关闭按钮间的空白区域的模板
        /// </summary>
        public DataTemplate HeaderBlankTemplate
        {
            get { return (DataTemplate)GetValue(HeaderBlankTemplateProperty); }
            set { SetValue(HeaderBlankTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBlankTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBlankTemplateProperty =
            DependencyProperty.Register("HeaderBlankTemplate", typeof(DataTemplate), typeof(CustomWindow), new PropertyMetadata(null));
        #endregion

        #region HeaderBlankTemplateSelector
        public DataTemplateSelector HeaderBlankTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderBlankTemplateSelectorProperty); }
            set { SetValue(HeaderBlankTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBlankTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBlankTemplateSelectorProperty =
            DependencyProperty.Register("HeaderBlankTemplateSelector", typeof(DataTemplateSelector), typeof(CustomWindow), new PropertyMetadata(null));
        #endregion


        #endregion

        #region 方法
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Panel rootPanel = GetTemplateChild(PART_WindowRoot) as Panel;
            if (rootPanel != null)
                rootPanel.MouseLeftButtonDown += rootPanel_MouseLeftButtonDown;
            Button closeButton = GetTemplateChild(PART_CloseButton) as Button;
            if (closeButton != null)
                closeButton.Click += closeButton_Click;
            Button minMaxButton = GetTemplateChild(PART_MinMaxButton) as Button;
            if (minMaxButton != null)
                minMaxButton.Click += minMaxButton_Click;
        }

        private void rootPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minMaxButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if (this.WindowState != WindowState.Minimized)
                this.WindowState = WindowState.Minimized;
            else
                this.WindowState = WindowState.Maximized;
        }
        #endregion

    }
}
