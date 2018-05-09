using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoVis.Plugin.CustomResourcesLibrary
{
    public class RectBorder : Border
    {
        #region 依赖属性

        #region RectCornerBrush
        public Brush RectCornerBrush
        {
            get { return (Brush)GetValue(RectCornerBrushProperty); }
            set { SetValue(RectCornerBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectCornerBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectCornerBrushProperty =
            DependencyProperty.Register("RectCornerBrush", typeof(Brush), typeof(RectBorder), new PropertyMetadata(Brushes.White));
        #endregion

        #region RectCornerThickness
        public double RectCornerThickness
        {
            get { return (double)GetValue(RectCornerThicknessProperty); }
            set { SetValue(RectCornerThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectCornerThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectCornerThicknessProperty =
            DependencyProperty.Register("RectCornerThickness", typeof(double), typeof(RectBorder), new PropertyMetadata(2.0));
        #endregion

        #region RectCornerLength
        public double RectCornerLength
        {
            get { return (double)GetValue(RectCornerLengthProperty); }
            set { SetValue(RectCornerLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectCornerLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectCornerLengthProperty =
            DependencyProperty.Register("RectCornerLength", typeof(double), typeof(RectBorder), new PropertyMetadata(5.0));
        #endregion

        #endregion

        #region 重载函数
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);

            Pen pen = new Pen(this.RectCornerBrush, RectCornerThickness);
            var figures = new List<PathFigure>() 
            { 
                new PathFigure(new Point(0.0, RectCornerLength), new List<LineSegment>() 
                { 
                    new LineSegment(new Point(0.0, 0.0),true),
                    new LineSegment(new Point(RectCornerLength, 0.0), true)
                }, false),
                new PathFigure(new Point(ActualWidth - RectCornerLength, 0.0), new List<LineSegment>() 
                { 
                    new LineSegment(new Point(ActualWidth, 0.0), true),
                    new LineSegment(new Point(ActualWidth, RectCornerLength), true)
                }, false),
                new PathFigure(new Point(ActualWidth, ActualHeight - RectCornerLength), new List<LineSegment>() 
                { 
                    new LineSegment(new Point(ActualWidth, ActualHeight),true),
                    new LineSegment(new Point(ActualWidth - RectCornerLength, ActualHeight), true)
                }, false),
                new PathFigure(new Point(RectCornerLength, ActualHeight), new List<LineSegment>() 
                { 
                    new LineSegment(new Point(0.0, ActualHeight),true),
                    new LineSegment(new Point(0.0, ActualHeight - RectCornerLength),true)
                }, false)
            };
            PathGeometry geometry = new PathGeometry(figures);
            dc.DrawGeometry(Brushes.Transparent, pen, geometry);
        }
        #endregion

    }
}
