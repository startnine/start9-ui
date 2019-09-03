using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Start9.UI.Wpf.Rearranger
{
    [TemplatePart(Name = PartTitlebar, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbLeft, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbTop, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbRight, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbBottom, Type = typeof(Thumb))]
    [TemplatePart(Name = PartContentPresenter, Type = typeof(ContentPresenter))]
    public class RearrangeablePane : ContentControl
    {
        const String PartTitlebar = "PART_Titlebar";
        const String PartThumbLeft = "PART_ThumbLeft";
        const String PartThumbTop = "PART_ThumbTop";
        const String PartThumbRight = "PART_ThumbRight";
        const String PartThumbBottom = "PART_ThumbBottom";
        const String PartContentPresenter = "PART_ContentPresenter";

        Thumb _titlebar;
        Thumb _thumbLeft;
        Thumb _thumbTop;
        Thumb _thumbRight;
        Thumb _thumbBottom;
        ContentPresenter _contentPresenter;

        public bool ComputedCanResize
        {
            get => (bool)GetValue(ComputedCanResizeProperty);
            set => SetValue(ComputedCanResizeProperty, value);
        }

        public static DependencyProperty ComputedCanResizeProperty =
            DependencyProperty.Register(nameof(ComputedCanResize), typeof(bool), typeof(RearrangeablePane), new FrameworkPropertyMetadata(false));

        public bool? CanResize
        {
            get => (bool?)GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
        }

        public static DependencyProperty CanResizeProperty =
            DependencyProperty.Register(nameof(CanResize), typeof(bool?), typeof(RearrangeablePane), new FrameworkPropertyMetadata(null));

        static void OnCanResizePropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is RearrangeablePane pane)
                pane.RefreshCanResize();
        }

        void RefreshCanResize()
        {
            if (CanResize == null)
            {
                if ((Content != null) && (Content is FrameworkElement fElement))
                {
                    if ((DockPanel.GetDock(this) == Dock.Left) || (DockPanel.GetDock(this) == Dock.Right))
                        ComputedCanResize = (!(double.IsInfinity(fElement.Width) || double.IsNaN(fElement.Width)));
                    else
                        ComputedCanResize = (!(double.IsInfinity(fElement.Height) || double.IsNaN(fElement.Height)));
                }
            }
            else
                ComputedCanResize = CanResize.Value;
        }

        double _oldWidth = 0;
        double _oldHeight = 0;
        void UpdateBounds(bool locking)
        {
            if (locking)
            {
                _oldWidth = ActualWidth;
                _oldHeight = ActualHeight;
                Width = _contentPresenter.ActualWidth;
                Height = _contentPresenter.ActualHeight;
            }
            else
            {
                Width = _oldWidth;
                Height = _oldHeight;
                _oldWidth = 0;
                _oldHeight = 0;
            }
        }

        public RearrangeablePane() : base()
        {
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (oldContent != null)
            {

            }

            if ((newContent != null) && (newContent is DependencyObject obj))
            {
                Binding paneTitleBinding = new Binding()
                {
                    Source = obj,
                    Path = new PropertyPath("(0)", Rearranger.PaneTitleProperty),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                SetBinding(Rearranger.PaneTitleProperty, paneTitleBinding);

                Binding dockBinding = new Binding()
                {
                    Source = obj,
                    Path = new PropertyPath("(0)", DockPanel.DockProperty),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                SetBinding(DockPanel.DockProperty, dockBinding);

                Binding visibilityBinding = new Binding()
                {
                    Source = obj,
                    Path = new PropertyPath(FrameworkElement.VisibilityProperty),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                SetBinding(FrameworkElement.VisibilityProperty, visibilityBinding);
                RefreshCanResize();
            }
        }

        Rearranger owner
        {
            get
            {
                /*FrameworkElement element = (FrameworkElement)this;
                while ((element != null) && (!(element is Rearranger)))
                    element = (FrameworkElement)element.Parent;

                if ((element != null) && (element is Rearranger rerr))
                    return rerr;
                else
                    return null;*/
                if (Parent is DockPanel d)
                {
                    if (d.TemplatedParent is Rearranger rerr)
                        return rerr;
                }
                return null;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _titlebar = GetTemplateChild(PartTitlebar) as Thumb;
            if (_titlebar != null)
                _titlebar.PreviewMouseLeftButtonDown += Titlebar_PreviewMouseLeftButtonDown;


            _thumbLeft = GetTemplateChild(PartThumbLeft) as Thumb;
            if (_thumbLeft != null)
                _thumbLeft.DragDelta += ThumbLeft_DragDelta;

            _thumbTop = GetTemplateChild(PartThumbTop) as Thumb;
            if (_thumbTop != null)
                _thumbTop.DragDelta += ThumbTop_DragDelta;

            _thumbRight = GetTemplateChild(PartThumbRight) as Thumb;
            if (_thumbRight != null)
                _thumbRight.DragDelta += ThumbRight_DragDelta;

            _thumbBottom = GetTemplateChild(PartThumbBottom) as Thumb;
            if (_thumbBottom != null)
                _thumbBottom.DragDelta += ThumbBottom_DragDelta;

            _contentPresenter = GetTemplateChild(PartContentPresenter) as ContentPresenter;
        }

        private void Titlebar_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //owner.MovePane(this);
            TitlebarMouseLeftButtonDown?.Invoke(this, null);
        }

        public static event EventHandler TitlebarMouseLeftButtonDown;

        void ThumbLeft_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (DockPanel.GetDock(this) == Dock.Right)
                ValidSetWidth(_contentPresenter.ActualWidth - e.HorizontalChange);
        }

        void ThumbTop_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (DockPanel.GetDock(this) == Dock.Bottom)
                ValidSetHeight(_contentPresenter.ActualHeight - e.VerticalChange);
        }

        void ThumbRight_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (DockPanel.GetDock(this) == Dock.Left)
                ValidSetWidth(_contentPresenter.ActualWidth + e.HorizontalChange);
        }

        void ThumbBottom_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (DockPanel.GetDock(this) == Dock.Top)
                ValidSetHeight(_contentPresenter.ActualHeight + e.VerticalChange);
        }

        void ValidSetWidth(double newWidth)
        {
            double minWidth = //0;
            //if ((Content != null) && (Content is FrameworkElement fElement))
                /*minWidth = */_contentPresenter.ActualWidth + _contentPresenter.DesiredSize.Width;

            if (/*(newWidth >= minWidth)
                && */(newWidth >= MinWidth)
                && (newWidth <= MaxWidth))
                _contentPresenter.Width = newWidth;
        }

        void ValidSetHeight(double newHeight)
        {
            double minHeight = //0;
            //if ((Content != null) && (Content is FrameworkElement fElement))
                /*minHeight = */_contentPresenter.ActualHeight + _contentPresenter.DesiredSize.Height;

            if (/*(newHeight >= minHeight)
                && */(newHeight >= MinHeight)
                && (newHeight <= MaxHeight))
                _contentPresenter.Height = newHeight;
        }
    }
}
