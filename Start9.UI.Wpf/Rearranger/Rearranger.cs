using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Start9.UI.Wpf.Rearranger
{
    [DefaultEvent("OnItemsChanged"), DefaultProperty("Items")]
    [ContentProperty("Items")]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(Rearranger))]
    [TemplatePart(Name = PartItemsDockPanel, Type = typeof(DockPanel))]
    [TemplatePart(Name = PartDragMovementCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PartDragMovementGhost, Type = typeof(Control))]
    [TemplatePart(Name = PartDragMovementGuide, Type = typeof(Control))]
    public class Rearranger : ItemsControl
    {
        const String PartItemsDockPanel = "PART_ItemsDockPanel";
        const String PartDragMovementCanvas = "PART_DragMovementCanvas";
        const String PartDragMovementGhost = "PART_DragMovementGhost";
        const String PartDragMovementGuide = "PART_DragMovementGuide";

        DockPanel _itemsDockPanel;
        Canvas _dragMovementCanvas;
        Control _dragMovementGhost;
        Control _dragMovementGuide;

        private object _currentItem = null;

        public bool IsLocked
        {
            get => (bool)GetValue(IsLockedProperty);
            set => SetValue(IsLockedProperty, value);
        }

        public static DependencyProperty IsLockedProperty =
            DependencyProperty.Register(nameof(IsLocked), typeof(bool), typeof(Rearranger), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits, OnIsLockedPropertyChangedCallback));

        static void OnIsLockedPropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Rearranger rerr)
                rerr.IsLockedChanged?.Invoke(sender, null);
        }

        public event EventHandler IsLockedChanged;

        public static readonly DependencyProperty PaneTitleProperty = DependencyProperty.RegisterAttached("PaneTitle", typeof(string), typeof(Rearranger), new FrameworkPropertyMetadata(string.Empty));

        public static string GetPaneTitle(DependencyObject element)
        {
            return (string)element.GetValue(PaneTitleProperty);
        }

        public static void SetPaneTitle(DependencyObject element, string value)
        {
            element.SetValue(PaneTitleProperty, value);
        }

        /*public bool CanResize
        {
            get => (bool)GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
        }*/

        public static DependencyProperty CanResizeProperty =
            DependencyProperty.RegisterAttached("CanResize", typeof(bool), typeof(Rearranger), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetCanResize(DependencyObject element)
        {
            return (bool)element.GetValue(CanResizeProperty);
        }

        public static void SetCanResize(DependencyObject element, bool value)
        {
            element.SetValue(CanResizeProperty, value);
        }

        public static DependencyProperty HideFrameWhenLockedProperty =
            DependencyProperty.RegisterAttached("HideFrameWhenLocked", typeof(bool), typeof(Rearranger), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetHideFrameWhenLocked(DependencyObject element)
        {
            return (bool)element.GetValue(HideFrameWhenLockedProperty);
        }

        public static void SetHideFrameWhenLocked(DependencyObject element, bool value)
        {
            element.SetValue(HideFrameWhenLockedProperty, value);
        }

        public static DependencyProperty HideTitlebarWhenLockedProperty =
            DependencyProperty.RegisterAttached("HideTitlebarWhenLocked", typeof(bool), typeof(Rearranger), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetHideTitlebarWhenLocked(DependencyObject element)
        {
            return (bool)element.GetValue(HideTitlebarWhenLockedProperty);
        }

        public static void SetHideTitlebarWhenLocked(DependencyObject element, bool value)
        {
            element.SetValue(HideTitlebarWhenLockedProperty, value);
        }

        /*public static readonly DependencyProperty PaneDockSideProperty = DependencyProperty.RegisterAttached("PaneDockSide", typeof(Dock), typeof(Rearranger), new FrameworkPropertyMetadata(Dock.Left));

        public static Dock GetPaneDockSide(DependencyObject element)
        {
            return (Dock)element.GetValue(PaneDockSideProperty);
        }

        public static void SetPaneDockSide(DependencyObject element, Dock value)
        {
            element.SetValue(PaneDockSideProperty, value);
        }*/


        public Rearranger() : base()
        {
            RearrangeablePane.TitlebarMouseLeftButtonDown += Pane_TitlebarMouseLeftButtonDown;
        }


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            bool val = item is RearrangeablePane;
            if (!val)
                _currentItem = item;

            return val;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (_currentItem is RearrangeablePane pane)
            {
                _currentItem = null;
                return pane;
            }
            else
            {
                _currentItem = null;

                return new RearrangeablePane()/*
            {
                Owner = this
            }*/;
            }
        }

        /*protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded != null)
            {
                if (visualRemoved is RearrangeablePane pane)
                    pane.TitlebarMouseLeftButtonDown += Pane_TitlebarMouseLeftButtonDown; //pane.Owner = this;
                else if (ContainerFromElement(this, visualAdded) is RearrangeablePane pane2)
                    pane2.TitlebarMouseLeftButtonDown -= Pane_TitlebarMouseLeftButtonDown; //pane2.Owner = this;
            }

            if (visualRemoved != null)
            {
                if (visualRemoved is RearrangeablePane pane)
                    pane.TitlebarMouseLeftButtonDown += Pane_TitlebarMouseLeftButtonDown; //pane.Owner = null;
                else if (ContainerFromElement(this, visualRemoved) is RearrangeablePane pane2)
                    pane2.TitlebarMouseLeftButtonDown -= Pane_TitlebarMouseLeftButtonDown; //pane2.Owner = null;
            }
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }*/

        private void Pane_TitlebarMouseLeftButtonDown(object sender, EventArgs e)
        {
            if ((sender != null) && (sender is RearrangeablePane pane))
                MovePane(pane);
        }

        private static bool IsContainerForItemsControl(DependencyObject element, ItemsControl itemsControl)
        {
            return (element != null) && (element is RearrangeablePane);
        }

        public override void OnApplyTemplate()
        {
            /*foreach (object item in Items)
            {
                if ((item.GetType().FullName == "MS.Internal.NamedObject"))// || ((item as RearrangeablePane).Content.GetType().FullName == "MS.Internal.NamedObject"))
                    Items.Remove(item);
            }*/


            base.OnApplyTemplate();

            _itemsDockPanel = GetTemplateChild(PartItemsDockPanel) as DockPanel;

            _dragMovementCanvas = GetTemplateChild(PartDragMovementCanvas) as Canvas;

            _dragMovementGhost = GetTemplateChild(PartDragMovementGhost) as Control;

            _dragMovementGuide = GetTemplateChild(PartDragMovementGuide) as Control;
        }

        int EnsureValidIndex(int baseIndex)
        {
            int newIndex = baseIndex;
            if (newIndex >= _itemsDockPanel.Children.Count)
                newIndex = _itemsDockPanel.Children.Count - 1;
            if (newIndex < 0)
                newIndex = 0;
            return newIndex;
        }

        public void MovePane(RearrangeablePane pane)
        {
            if (pane == null)
                throw new ArgumentNullException("The value of argument \"pane\" cannot be null.");
            else if (!_itemsDockPanel.Children.Contains(pane))
                throw new ArgumentException("The pane passed as argument \"pane\" does not belong to this Rearranger.");
            else
            {
                Point paneInitialPoint = pane.PointToScreen(new Point(0, 0));
                Vector paneCursorOffset = paneInitialPoint - SystemScaling.CursorPosition;

                pane.Visibility = Visibility.Hidden;
                _dragMovementCanvas.Visibility = Visibility.Visible;
                Dock initialDock = DockPanel.GetDock(pane);
                int initialIndex = Items.IndexOf(pane);

                bool toNewLocation = false;
                Dock newDock = Dock.Left;
                int newIndex = 0;

                Timer timer = new Timer(10);
                timer.Elapsed += (sneder, args) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        bool done = Mouse.LeftButton == MouseButtonState.Released;
                        bool cancel = Keyboard.GetKeyStates(Key.Escape) == KeyStates.Down;
                        if (done || cancel)
                        {
                            timer.Stop();
                            _dragMovementCanvas.Visibility = Visibility.Collapsed;

                            Debug.WriteLine("ITEMS BEFORE SHUNFFLE: ");
                            for (int i = 0; i < Items.Count; i++)
                                Debug.WriteLine("Item at " + i + " is " + Items[i].GetType().FullName);

                            if (cancel)
                            {

                            }
                            else
                            {
                                UIElement elem = pane;
                                if (/*(!IsItemItsOwnContainerOverride(pane)) && */(pane.Content != null) && (pane.Content is UIElement uiel))
                                    elem = uiel;

                                Debug.WriteLine("\nelem is RearrangeablePane: " + (elem is RearrangeablePane).ToString() + "\n");
                                Items.Remove(elem);
                                DockPanel.SetDock(elem, newDock);
                                if (toNewLocation)
                                    Items.Insert(EnsureValidIndex(newIndex), elem);
                                else
                                    Items.Insert(EnsureValidIndex(initialIndex), elem);
                            }
                            pane.Visibility = Visibility.Visible;

                            Debug.WriteLine("ITEMS AFTER SHUNFFLE: ");
                            for (int i = 0; i < Items.Count; i++)
                                Debug.WriteLine("Item at " + i + " is " + Items[i].GetType().FullName);
                        }
                        else
                        {
                            paneCursorOffset = paneInitialPoint - SystemScaling.CursorPosition;
                            Canvas.SetLeft(_dragMovementGhost, Canvas.GetLeft(_dragMovementGhost) + paneCursorOffset.X);
                            Canvas.SetTop(_dragMovementGhost, Canvas.GetTop(_dragMovementGhost) + paneCursorOffset.Y);

                            Point rearrangerCursorPoint = PointFromScreen(SystemScaling.CursorPosition);
                            Point rearrangerPoint = PointToScreen(new Point(0, 0));
                            rearrangerPoint.X = SystemScaling.RealPixelsToWpfUnits(rearrangerPoint.X);
                            rearrangerPoint.Y = SystemScaling.RealPixelsToWpfUnits(rearrangerPoint.Y);
                            bool currentToNewLocation = false;

                            if (rearrangerCursorPoint.X < 50)
                            {
                                newIndex = 0;
                                newDock = Dock.Left;

                                Canvas.SetLeft(_dragMovementGuide, 0);
                                Canvas.SetTop(_dragMovementGuide, 0);
                                _dragMovementGuide.Width = 50; /*pane.ActualWidth;
                                if (_dragMovementGuide.ActualWidth < 50)
                                    _dragMovementGuide.Width = 50;*/
                                _dragMovementGuide.Height = ActualHeight;

                                currentToNewLocation = true;
                            }
                            else if (rearrangerCursorPoint.Y < 50)
                            {
                                newIndex = 0;
                                newDock = Dock.Top;

                                Canvas.SetLeft(_dragMovementGuide, 0);
                                Canvas.SetTop(_dragMovementGuide, 0);
                                _dragMovementGuide.Width = ActualWidth;
                                _dragMovementGuide.Height = 50; /*pane.ActualHeight;
                                if (_dragMovementGuide.ActualHeight < 50)
                                    _dragMovementGuide.Height = 50;*/

                                currentToNewLocation = true;
                            }
                            else if (rearrangerCursorPoint.X > (ActualWidth - 50))
                            {
                                newIndex = 0;
                                newDock = Dock.Right;

                                Canvas.SetTop(_dragMovementGuide, 0);
                                _dragMovementGuide.Width = 50; /*pane.ActualWidth;
                                if (_dragMovementGuide.ActualWidth < 50)
                                    _dragMovementGuide.Width = 50;*/
                                _dragMovementGuide.Height = ActualHeight;
                                Canvas.SetLeft(_dragMovementGuide, ActualWidth - _dragMovementGuide.ActualWidth);

                                currentToNewLocation = true;
                            }
                            else if (rearrangerCursorPoint.Y > (ActualHeight - 50))
                            {
                                newIndex = 0;
                                newDock = Dock.Bottom;

                                Canvas.SetLeft(_dragMovementGuide, 0);
                                _dragMovementGuide.Width = ActualWidth;
                                _dragMovementGuide.Height = 50; /*pane.ActualHeight;
                                if (_dragMovementGuide.ActualHeight < 50)
                                    _dragMovementGuide.Height = 50;*/
                                Canvas.SetTop(_dragMovementGuide, ActualHeight - _dragMovementGuide.ActualHeight);

                                currentToNewLocation = true;
                            }
                            else if (SystemScaling.IsMouseWithin((FrameworkElement)_itemsDockPanel.Children[_itemsDockPanel.Children.Count - 1]))
                            {
                                newIndex = _itemsDockPanel.Children.Count - 2;
                                newDock = Dock.Bottom;

                                RearrangeablePane lastPane = (RearrangeablePane)_itemsDockPanel.Children[_itemsDockPanel.Children.Count - 1];
                                Point lastPaneCurPos = lastPane.PointFromScreen(SystemScaling.CursorPosition);
                                double horizontal = lastPaneCurPos.X / lastPane.ActualWidth;
                                double vertical = lastPaneCurPos.Y / lastPane.ActualHeight;

                                if (horizontal > 0.5)
                                {
                                    if (vertical > 0.5)
                                    {
                                        if (horizontal > vertical)
                                            newDock = Dock.Right;
                                        else
                                            newDock = Dock.Bottom;
                                    }
                                    else
                                    {
                                        if (vertical > horizontal)
                                            newDock = Dock.Right;
                                        else
                                            newDock = Dock.Top;
                                    }
                                }
                                else
                                {
                                    if (vertical > 0.5)
                                    {
                                        if (horizontal > vertical)
                                            newDock = Dock.Left;
                                        else
                                            newDock = Dock.Bottom;
                                    }
                                    else
                                    {
                                        if (vertical > horizontal)
                                            newDock = Dock.Left;
                                        else
                                            newDock = Dock.Top;
                                    }
                                }

                                Point lastPaneRearrangerOffset = PointFromScreen(lastPane.PointToScreen(new Point(0, 0)));
                                if (newDock == Dock.Left)
                                {
                                    Canvas.SetLeft(_dragMovementGuide, lastPaneRearrangerOffset.X);
                                    Canvas.SetTop(_dragMovementGuide, lastPaneRearrangerOffset.Y);
                                    _dragMovementGuide.Width = 50;
                                    _dragMovementGuide.Height = lastPane.ActualHeight;
                                }
                                else if (newDock == Dock.Top)
                                {
                                    Canvas.SetLeft(_dragMovementGuide, lastPaneRearrangerOffset.X);
                                    Canvas.SetTop(_dragMovementGuide, lastPaneRearrangerOffset.Y);
                                    _dragMovementGuide.Width = lastPane.ActualWidth;
                                    _dragMovementGuide.Height = 50;
                                }
                                else if (newDock == Dock.Right)
                                {
                                    Canvas.SetLeft(_dragMovementGuide, (lastPaneRearrangerOffset.X + lastPane.ActualWidth) - 50);
                                    Canvas.SetTop(_dragMovementGuide, lastPaneRearrangerOffset.Y);
                                    _dragMovementGuide.Width = 50;
                                    _dragMovementGuide.Height = lastPane.ActualHeight;
                                }
                                else if (newDock == Dock.Bottom)
                                {
                                    Canvas.SetLeft(_dragMovementGuide, lastPaneRearrangerOffset.X);
                                    Canvas.SetTop(_dragMovementGuide, (lastPaneRearrangerOffset.Y + lastPane.ActualHeight) - 50);
                                    _dragMovementGuide.Width = lastPane.ActualWidth;
                                    _dragMovementGuide.Height = 50;
                                }

                                currentToNewLocation = true;
                            }
                            else
                            {
                                foreach (RearrangeablePane pn in _itemsDockPanel.Children)
                                {
                                    if ((pane != pn) && SystemScaling.IsMouseWithin(pn) && (_itemsDockPanel.Children.IndexOf(pn) != (_itemsDockPanel.Children.Count - 1)))
                                    {
                                        if (pn == pane)
                                            currentToNewLocation = false;
                                        else
                                        {
                                            currentToNewLocation = true;
                                            Point pnCurPoint = pn.PointFromScreen(SystemScaling.CursorPosition);
                                            if (DockPanel.GetDock(pn) == Dock.Top)
                                            {
                                                if (pnCurPoint.Y > (pn.ActualHeight / 2))
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn) + 1;
                                                else
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn);
                                            }
                                            else if (DockPanel.GetDock(pn) == Dock.Bottom)
                                            {
                                                if (pnCurPoint.Y <= (pn.ActualHeight / 2))
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn) - 1;
                                                else
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn);
                                            }
                                            else if (DockPanel.GetDock(pn) == Dock.Left)
                                            {
                                                if (pnCurPoint.X > (pn.ActualWidth / 2))
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn) + 1;
                                                else
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn);
                                            }
                                            else if (DockPanel.GetDock(pn) == Dock.Right)
                                            {
                                                if (pnCurPoint.X <= (pn.ActualWidth / 2))
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn) - 1;
                                                else
                                                    newIndex = _itemsDockPanel.Children.IndexOf(pn);
                                            }
                                            newDock = DockPanel.GetDock(pn);

                                            Point pnPoint = _dragMovementCanvas.PointFromScreen(pn.PointToScreen(new Point(0, 0)));
                                            Canvas.SetLeft(_dragMovementGuide, pnPoint.X);
                                            Canvas.SetTop(_dragMovementGuide, pnPoint.Y);
                                            _dragMovementGuide.Width = pn.ActualWidth;
                                            _dragMovementGuide.Height = pn.ActualHeight;
                                        }
                                        break;
                                    }
                                }
                            }
                            toNewLocation = currentToNewLocation;
                        }
                    }));
                };
                timer.Start();
            }
        }
    }
}
