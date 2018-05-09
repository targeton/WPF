using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeoVis.Plugin.CustomResourcesLibrary
{
    public sealed class FilterConfirmedEventArgs : RoutedEventArgs
    {
        #region 属性
        public object FilterConfirmedItem { get; set; }
        #endregion
        #region 构造函数
        public FilterConfirmedEventArgs(object filterItem)
            : base()
        {
            FilterConfirmedItem = filterItem;
        }

        public FilterConfirmedEventArgs(RoutedEvent routedEvent, object filterItem)
            : base(routedEvent)
        {
            FilterConfirmedItem = filterItem;
        }

        public FilterConfirmedEventArgs(RoutedEvent routedEvent, object source, object filterItem)
            : base(routedEvent, source)
        {
            FilterConfirmedItem = filterItem;
        }
        #endregion
    }
}
