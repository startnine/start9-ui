using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Start9.UI.Wpf.Windows
{
    public static class MessageBoxEnums
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
    }

    public static class MessageBox<T>
    {
        /*public enum TargetContainer
        {
            Client,
            Title,
            FullWindow
        }*/

        /*public static T Show()
        {
            return Show(string.Empty, string.Empty, TargetContainer.Client, null);
        }

        public static T Show(ResourceDictionary skin)
        {
            return Show(string.Empty, string.Empty, TargetContainer.Client, skin);
        }

        public static T Show(TargetContainer container)
        {
            return Show(string.Empty, string.Empty, container, null);
        }

        public static T Show(TargetContainer container, ResourceDictionary skin)
        {
            return Show(string.Empty, string.Empty, container, skin);
        }*/

        public static T Show(string text)
        {
            return Show(text, string.Empty, null);
        }

        public static T Show(string text, ResourceDictionary skin)
        {
            return Show(text, string.Empty, skin);
        }

        public static T Show(string text, string caption)
        {
            return Show(text, caption, null);
        }

        public static T Show(string text, string caption, ResourceDictionary skin)
        {
            MessageBoxContent content = new MessageBoxContent(typeof(T), text);
            DecoratableWindow window = new DecoratableWindow()
            {
                Title = caption,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            if (skin != null)
                window.Resources.MergedDictionaries.Add(skin);

            T value = default(T);

            content.ResultButtonClicked += (sneder, args) =>
            {
                var arg = (args as MessageBoxEventArgs);
                value = (T)(arg.Result);
                //window.Close();
            };

            window.Content = content;

            window.ShowDialog();

            return value;
        }
    }
}
