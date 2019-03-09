using System;
using System.Collections.Generic;
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

        public string BreadcrumbPath
        {
            get => (string)GetValue(BreadcrumbPathProperty);
            set => SetValue(BreadcrumbPathProperty, value);
        }

        public static readonly DependencyProperty BreadcrumbPathProperty =
            DependencyProperty.Register(nameof(BreadcrumbPath), typeof(string), typeof(BreadcrumbsBar), new PropertyMetadata(string.Empty, OnPathPropertyChangedCallback));

        static void OnPathPropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue.ToString().Contains(@"\"))
            {
                var sned = (sender as BreadcrumbsBar);

                sned.PopulateStackPanel();
                sned.PathUpdated?.Invoke(sned, null);
            }
        }

        public event EventHandler<EventArgs> PathUpdated;
        public event EventHandler<EventArgs> NavigationCancelled;

        public BreadcrumbsBar()
        {

        }

        protected void PopulateStackPanel()
        {
            if (_breadcrumbsStackPanel != null)
            {
                _breadcrumbsStackPanel.Children.Clear();
                if (BreadcrumbPath != null)
                {
                    if (BreadcrumbPath.ToString().Contains("\\") && (BreadcrumbPath.Split('\\').Length > 1))
                    {
                        string[] pathSegments = BreadcrumbPath.Split('\\');
                        int index = 0;
                        foreach (string s in pathSegments)
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                index++;
                                Button breadcrumbButton = new Button()
                                {
                                    Content = s
                                };

                                string breadcrumbPath = "";
                                for (int i = 0; i < index; i++)
                                {
                                    breadcrumbPath += (pathSegments[i] + @"\");
                                }

                                breadcrumbButton.Click += (sneder, args) =>
                                {
                                    BreadcrumbPath = breadcrumbPath;
                                };

                                _breadcrumbsStackPanel.Children.Add(breadcrumbButton);
                            }
                        }
                    }
                    else
                    {
                        Button breadcrumbButton = new Button()
                        {
                            Content = BreadcrumbPath
                        };

                        breadcrumbButton.Click += (sneder, args) =>
                        {
                            BreadcrumbPath = BreadcrumbPath;
                        };

                        _breadcrumbsStackPanel.Children.Add(breadcrumbButton);
                    }
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
                _textBox.Text = BreadcrumbPath;
                _breadcrumbsStackPanel.Visibility = Visibility.Collapsed;
            };
            _textBox.LostFocus += (sneder, args) =>
            {
                if (!_breadcrumbsStackPanel.IsVisible)
                    NavigationCancelled?.Invoke(this, null);
            };
            _textBox.KeyDown += (sneder, args) =>
            {
                if (args.Key == System.Windows.Input.Key.Enter)
                    BreadcrumbPath = _textBox.Text;

                if (args.Key == System.Windows.Input.Key.Escape)
                    NavigationCancelled?.Invoke(this, null);

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
}
