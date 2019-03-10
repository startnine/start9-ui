using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Start9.UI.Wpf
{
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PartBreadcrumbsStackPanel, Type = typeof(StackPanel))]
    public partial class BreadcrumbsBar : Control
    {
        const String PartTextBox = "PART_TextBox";
        const String PartBreadcrumbsStackPanel = "PART_BreadcrumbsStackPanel";

        TextBox _textBox;
        StackPanel _breadcrumbsStackPanel;

        public BreadcrumbItem[] BreadcrumbItems
        {
            get => (BreadcrumbItem[]) GetValue(BreadcrumbItemsProperty);
            set => SetValue(BreadcrumbItemsProperty, value);
        }

        public static readonly DependencyProperty BreadcrumbItemsProperty =
            DependencyProperty.Register(nameof(BreadcrumbItems), typeof(BreadcrumbItem[]), typeof(BreadcrumbsBar), new PropertyMetadata(OnItemsPropertyChangedCallback));

        public BreadcrumbsPathToItemsConverter Converter { get; set; }

        static void OnItemsPropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            var sned = sender as BreadcrumbsBar;
            sned.PopulateStackPanel(sned.BreadcrumbItems);
        }

        public event EventHandler<EventArgs> PathUpdated;
        public event EventHandler<EventArgs> NavigationCanceled;

        public BreadcrumbsBar()
        {

        }

        protected void PopulateStackPanel(BreadcrumbItem[] values)
        {
            BreadcrumbItems = values;
            if (_breadcrumbsStackPanel != null)
            {
                _breadcrumbsStackPanel.Children.Clear();
                foreach (var item in values)
                {
                    var breadcrumbButton = new Button()
                    {
                        Content = item.Name
                    };

                    breadcrumbButton.Click += (sender, e) =>
                    {
                        PopulateStackPanel(Converter.Invoke?.Invoke(item.Path));
                        PathUpdated?.Invoke(this, null);
                    };

                    _breadcrumbsStackPanel.Children.Add(breadcrumbButton);

                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild(PartTextBox) as TextBox;
            _textBox.GotFocus += (sneder, args) =>
            {
                //if (sned._textBox.Text.ToLowerInvariant() != e.NewValue.ToString().ToLowerInvariant())
                _textBox.Text = BreadcrumbItems.Last().Path;
                _breadcrumbsStackPanel.Visibility = Visibility.Collapsed;
            };
            _textBox.LostFocus += (sneder, args) =>
            {
                if (!_breadcrumbsStackPanel.IsVisible)
                    NavigationCanceled?.Invoke(this, null);
            };
            _textBox.KeyDown += (sneder, args) =>
            {
                if (args.Key == System.Windows.Input.Key.Enter)
                {
                    PopulateStackPanel(Converter.Invoke?.Invoke(_textBox.Text));
                }

                if (args.Key == System.Windows.Input.Key.Escape)
                    NavigationCanceled?.Invoke(this, null);

                if ((args.Key == System.Windows.Input.Key.Enter) || (args.Key == System.Windows.Input.Key.Escape))
                {
                    _textBox.Text = string.Empty;
                    _breadcrumbsStackPanel.Visibility = Visibility.Visible;
                    //_textBox.MoveFocus(null);
                }
            };

            _breadcrumbsStackPanel = GetTemplateChild(PartBreadcrumbsStackPanel) as StackPanel;
        }
    }

    public struct BreadcrumbItem
    {
        public BreadcrumbItem(String name, String path)
        {
            Name = name;
            Path = path;
        }

        public String Name { get; }
        public String Path { get; }
    }

    public class BreadcrumbsPathToItemsConverter
    {
        public BreadcrumbsPathToItemsConverter(Func<String, BreadcrumbItem[]> converterDelegate)
        {
            Invoke = converterDelegate;
        }

        public Func<String, BreadcrumbItem[]> Invoke { get; }
    }
}
