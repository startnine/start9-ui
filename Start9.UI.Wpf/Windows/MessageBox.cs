using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Start9.UI.Wpf.Windows
{
    public static class MessageBoxActionSets
    {
        public enum OkButton
        {
            OK
        }

        public enum OkCancelButtons
        {
            OK,
            Cancel
        }

        public enum IgnoreRetryAbortButtons
        {
            Ignore,
            Retry,
            Abort
        }

        public enum YesNoButtons
        {
            Yes,
            No
        }

        public class OkActionSet : IMessageBoxActionSet
        {
            public string GetDisplayName(object value)
            {
                if (value is string valueString)
                    return valueString;
                else
                    return value.ToString();
            }

            public object[] GetValues()
            {
                object[] objects = new object[Enum.GetNames(typeof(OkButton)).Count()];
                Enum.GetValues(typeof(OkButton)).CopyTo(objects, 0);
                return objects;
            }
        }

        public class OkCancelActionSet : IMessageBoxActionSet
        {
            public string GetDisplayName(object value)
            {
                if (value is string valueString)
                    return valueString;
                else
                    return value.ToString();
            }

            public object[] GetValues()
            {
                object[] objects = new object[Enum.GetNames(typeof(OkCancelButtons)).Count()];
                Enum.GetValues(typeof(OkCancelButtons)).CopyTo(objects, 0);
                return objects;
            }
        }

        public class IgnoreRetryAbortActionSet : IMessageBoxActionSet
        {
            public string GetDisplayName(object value)
            {
                if (value is string valueString)
                    return valueString;
                else
                    return value.ToString();
            }

            public object[] GetValues()
            {
                object[] objects = new object[Enum.GetNames(typeof(IgnoreRetryAbortButtons)).Count()];
                Enum.GetValues(typeof(IgnoreRetryAbortButtons)).CopyTo(objects, 0);
                return objects;
            }
        }

        public class YesNoActionSet : IMessageBoxActionSet
        {
            public string GetDisplayName(object value)
            {
                if (value is string valueString)
                    return valueString;
                else
                    return value.ToString();
            }

            public object[] GetValues()
            {
                object[] objects = new object[Enum.GetNames(typeof(YesNoButtons)).Count()];
                Enum.GetValues(typeof(YesNoButtons)).CopyTo(objects, 0);
                return objects;
            }
        }
    }

    public static class MessageBox
    {
        public static MessageBoxActionSets.OkButton Show(string text, string caption)
        {
            return (MessageBoxActionSets.OkButton)(MessageBox<MessageBoxActionSets.OkActionSet>.Show(text, caption));
        }
    }

    public interface IMessageBoxActionSet
    {
        object[] GetValues();

        string GetDisplayName(object value);
    }

    public class MessageBoxAction : DependencyObject
    {
        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.RegisterAttached(nameof(DisplayName), typeof(string), typeof(MessageBoxAction), new PropertyMetadata(string.Empty));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(object), typeof(MessageBoxAction), new PropertyMetadata(null));

        public MessageBoxAction(object value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public static class MessageBox<T>
    {
        public static object Show(string text, string caption)
        {
            return Show(text, caption, null);
        }

        public static object Show(string text, string caption, FrameworkElement icon)
        {
            if (typeof(T).GetInterface(nameof(IMessageBoxActionSet)) == null)
                throw new InvalidOperationException("MessageBoxActions<T>: T must implement IMessageBoxActionSet.");

            MessageBoxContent content = new MessageBoxContent((IMessageBoxActionSet)Activator.CreateInstance(typeof(T)), text, icon);
            DecoratableWindow window = new DecoratableWindow()
            {
                Title = caption,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            object value = null;

            content.ResultButtonClicked += (sneder, args) =>
            {
                var arg = args as MessageBoxEventArgs;
                value = arg.Result;
            };

            window.Content = content;

            window.ShowDialog();

            return value;
        }
    }
}
