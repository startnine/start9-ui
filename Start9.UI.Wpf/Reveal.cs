using Start9.UI.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Start9.UI.Wpf
{
    public class Reveal : Canvas
    {
        //from https://stackoverflow.com/questions/1517743/in-wpf-how-can-i-determine-whether-a-control-is-visible-to-the-user#1517794
        /*private bool isElementClickable<T>(UIElement container, UIElement element, out bool isPartiallyClickable)
        {
            isPartiallyClickable = false;
            Rect pos = GetAbsolutePlacement((FrameworkElement)container, (FrameworkElement)element);
            bool isTopLeftClickable = GetIsPointClickable<T>(container, element, new Point(pos.TopLeft.X + 1, pos.TopLeft.Y + 1));
            bool isBottomLeftClickable = GetIsPointClickable<T>(container, element, new Point(pos.BottomLeft.X + 1, pos.BottomLeft.Y - 1));
            bool isTopRightClickable = GetIsPointClickable<T>(container, element, new Point(pos.TopRight.X - 1, pos.TopRight.Y + 1));
            bool isBottomRightClickable = GetIsPointClickable<T>(container, element, new Point(pos.BottomRight.X - 1, pos.BottomRight.Y - 1));

            if (isTopLeftClickable | isBottomLeftClickable | isTopRightClickable | isBottomRightClickable)
            {
                isPartiallyClickable = true;
            }

            return isTopLeftClickable && isBottomLeftClickable && isTopRightClickable && isBottomRightClickable; // return if element is fully clickable
        }

        private bool GetIsPointClickable<T>(UIElement container, UIElement element, Point p)
        {
            DependencyObject hitTestResult = HitTest<T>(p, container);
            if (null != hitTestResult)
            {
                return isElementChildOfElement(element, hitTestResult);
            }
            return false;
        }

        private DependencyObject HitTest<T>(Point p, UIElement container)
        {
            PointHitTestParameters parameter = new PointHitTestParameters(p);
            DependencyObject hitTestResult = null;

            HitTestResultCallback resultCallback = (result) =>
            {
                UIElement elemCandidateResult = result.VisualHit as UIElement;
                // result can be collapsed! Even though documentation indicates otherwise
                if (null != elemCandidateResult && elemCandidateResult.Visibility == Visibility.Visible)
                {
                    hitTestResult = result.VisualHit;
                    return HitTestResultBehavior.Stop;
                }

                return HitTestResultBehavior.Continue;
            };

            HitTestFilterCallback filterCallBack = (potentialHitTestTarget) =>
            {
                if (potentialHitTestTarget is T)
                {
                    hitTestResult = potentialHitTestTarget;
                    return HitTestFilterBehavior.Stop;
                }

                return HitTestFilterBehavior.Continue;
            };

            VisualTreeHelper.HitTest(container, filterCallBack, resultCallback, parameter);
            return hitTestResult;
        }

        private bool isElementChildOfElement(DependencyObject child, DependencyObject parent)
        {
            if (child.GetHashCode() == parent.GetHashCode())
                return true;
            IEnumerable<DependencyObject> elemList = FindVisualChildren<DependencyObject>((DependencyObject)parent);
            foreach (DependencyObject obj in elemList)
            {
                if (obj.GetHashCode() == child.GetHashCode())
                    return true;
            }
            return false;
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private Rect GetAbsolutePlacement(FrameworkElement container, FrameworkElement element, bool relativeToScreen = false)
        {
            //Visual
            //element.present
            var absolutePos = element.PointToScreen(new System.Windows.Point(0, 0));
            if (relativeToScreen)
            {
                return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
            }
            var posMW = container.PointToScreen(new System.Windows.Point(0, 0));
            absolutePos = new System.Windows.Point(absolutePos.X - posMW.X, absolutePos.Y - posMW.Y);
            return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
        }

        Timer RevealGlowTimer = new Timer(1);*/

        public Brush Hover
        {
            get => (Brush)GetValue(HoverProperty);
            set => SetValue(HoverProperty, (value));
        }

        internal static readonly DependencyProperty HoverProperty =
            DependencyProperty.Register("Hover", typeof(Brush), typeof(Reveal), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public Double HoverWidth
        {
            get => (Double)GetValue(HoverWidthProperty);
            set => SetValue(HoverWidthProperty, (value));
        }

        internal static readonly DependencyProperty HoverWidthProperty =
            DependencyProperty.Register("HoverWidth", typeof(Double), typeof(Reveal), new PropertyMetadata((Double)1));

        public Double HoverHeight
        {
            get => (Double)GetValue(HoverHeightProperty);
            set => SetValue(HoverHeightProperty, (value));
        }

        internal static readonly DependencyProperty HoverHeightProperty =
            DependencyProperty.Register("HoverHeight", typeof(Double), typeof(Reveal), new PropertyMetadata((Double)1));

        Canvas _vis = null;

        public Reveal()
        {
            //IsVisibleChanged += Reveal_IsVisibleChanged;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Loaded += Reveal_Loaded;
            Unloaded += Reveal_Unloaded;
        }/*

        private void Reveal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                RevealGlowTimer.Start();
            else
                RevealGlowTimer.Stop();
        }*/
        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            if (IsVisible && (_vis != null))
            {
                var c = SystemScaling.CursorPosition;
                var p = PointToScreen(new Point(0, 0));
                var t = new Point((c.X - p.X) - (_vis.Width / 2), (c.Y - p.Y) - (_vis.Height / 2));
                (Background as VisualBrush).Viewbox = new Rect(t.X * -1, t.Y * -1, ActualWidth, ActualHeight);
            }
        }

        private void Reveal_Loaded(Object sender, RoutedEventArgs e)
        {
            SetReveal();
        }

        private void Reveal_Unloaded(Object sender, RoutedEventArgs e)
        {
            _vis = null;
        }

        public void SetReveal()
        {
            if (!(Background is VisualBrush))
            {
                Background = new VisualBrush();
            }
            /*Children.Clear();
            Children.Add(Hover);

            Grid RevealGlowGrid = new Grid()
            {
                Background = new ImageBrush(RevealGlowBitmapImage),
                Width = RevealGlowBitmapImage.PixelWidth,
                Height = RevealGlowBitmapImage.PixelHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 0, 0),
                Visibility = Visibility.Visible,
                Focusable = false
            };*/

            var bg = (Background as VisualBrush);

            if (bg.Visual == null)
            {
                bg.Visual = new Grid()
                {
                    Background = new SolidColorBrush(Color.FromArgb(0x01, 0x0, 0x0, 0x0))
                };
                var gr = (bg.Visual as Grid);
                Binding widthBinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath("ActualWidth"),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(gr, Grid.WidthProperty, widthBinding);

                Binding heightBinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath("ActualHeight"),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(gr, Grid.HeightProperty, heightBinding);

                gr.Children.Add(new Canvas()
                {
                    Background = Hover,
                    Width = HoverWidth,
                    Height = HoverHeight,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                });
            }

            _vis = (bg.Visual as Grid).Children[0] as Canvas;
            /*if (Hover is ImageBrush)
            {
                var br = Hover as ImageBrush;
                vis.Width = br.ImageSource.Width;
                vis.Height = br.ImageSource.Height;
            }*/

            bg.ViewboxUnits = BrushMappingMode.Absolute;

            /*RevealGlowTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (IsVisible)
                    {
                        bool isPartiallyClickable = true;
                        bool isFullyClickable = isElementClickable<Reveal>((Parent as UIElement), this, out isPartiallyClickable);
                        if (isPartiallyClickable)
                        {
                            //Debug.WriteLine("Reveal is Visible");
                            var c = SystemScaling.CursorPosition;
                            var p = PointToScreen(new Point(0, 0));
                            var t = new Point((c.X - p.X) - (_vis.Width / 2), (c.Y - p.Y) - (_vis.Height / 2));
                            bg.Viewbox = new Rect(
                                ((c.X - p.X) - (ActualWidth / 2)) * -1,
                                ((c.Y - p.Y) - (ActualHeight / 2)) * -1,
                                ActualWidth,
                                ActualHeight);
                            bg.Viewbox = new Rect(t.X * -1, t.Y * -1, ActualWidth, ActualHeight);
                            //vis.Margin = new Thickness(t.X, t.Y, t.X * -1, t.Y * -1);
                        //}
                    }
            else
            {
                Debug.WriteLine("Reveal is not visible");
                RevealGlowTimer.Stop();
            }
        }));
    };*/

            //RevealGlowTimer.Start();
        }
    }
}