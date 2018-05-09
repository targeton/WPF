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
    public class ToolBoxWindow : Window
    {
        private const string PART_WindowRoot = "PART_WindowRoot";
        private const string PART_CloseButton = "PART_CloseButton";

        #region 构造函数
        static ToolBoxWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBoxWindow), new FrameworkPropertyMetadata(typeof(ToolBoxWindow)));
        }

        public ToolBoxWindow()
        {
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
        }
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
        }

        private void rootPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

    }
}
