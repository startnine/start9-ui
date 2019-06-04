#if NETCOREAPP3_0
using Microsoft.Xaml.Behaviors;
#else
using System.Windows.Interactivity;
#endif
using System.Windows;
using System.Windows.Controls;

namespace Start9.Wpf.Styles.EDNA
{
    public class SelectedTabHeaderBehavior : Behavior<TextBlock>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject.TemplatedParent is TabControl tabControl)
            {
                tabControl.SelectionChanged += TabControl_SelectionChanged;
                TabControl_SelectionChanged(tabControl, null);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            if ((tabControl.SelectedItem is TabItem item) && (item.Header is string headerText))
            {
                AssociatedObject.Visibility = Visibility.Visible;
                AssociatedObject.Text = headerText;
            }
            else if (tabControl.SelectedItem is string tabText)
            {
                AssociatedObject.Visibility = Visibility.Visible;
                AssociatedObject.Text = tabText;
            }
            else
            {
                AssociatedObject.Text = string.Empty;
                AssociatedObject.Visibility = Visibility.Collapsed;
            }
        }
    }
}
