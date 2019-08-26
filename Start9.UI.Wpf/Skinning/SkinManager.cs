using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Start9.UI.Wpf.Skinning
{
    public class SkinManager : INotifyPropertyChanged
    {
        ObservableCollection<ISkinInfo> _skins = new ObservableCollection<ISkinInfo>();
        public ObservableCollection<ISkinInfo> Skins
        {
            get => _skins;
            set
            {
                _skins = value;
                NotifyPropertyChanged(nameof(Skins));
            }
        }   

        public string SkinsFolderPath = null;
        
        public ISkinInfo DefaultSkin = null;

        public ISkinInfo _activeSkin = null;

        public ISkinInfo ActiveSkin
        {
            get
            {
                if (_activeSkin == null)
                    return DefaultSkin;
                else
                    return _activeSkin;
            }
            set
            {
                if ((value != null) && Skins.Contains(value))
                {
                    _activeSkin = value;
                    ApplySkin(_activeSkin);
                    NotifyPropertyChanged(nameof(ActiveSkin));
                }
            }
        }

        void ApplySkin(ISkinInfo targetSkin)
        {
            if (Application.Current.Resources.MergedDictionaries.Count == 0)
            {
                if (targetSkin != DefaultSkin)
                    Application.Current.Resources.MergedDictionaries.Add(DefaultSkin.OnApplySkinSettings());

                Application.Current.Resources.MergedDictionaries.Add(targetSkin.OnApplySkinSettings());
            }
            else if (Application.Current.Resources.MergedDictionaries.Count == 1)
            {
                if (targetSkin == DefaultSkin)
                    Application.Current.Resources.MergedDictionaries[0] = DefaultSkin.OnApplySkinSettings();
                else
                {
                    Application.Current.Resources.MergedDictionaries[0] = DefaultSkin.OnApplySkinSettings();
                    Application.Current.Resources.MergedDictionaries.Add(targetSkin.OnApplySkinSettings());
                }
            }
            else
            {
                if (targetSkin == DefaultSkin)
                {
                    Application.Current.Resources.MergedDictionaries[0] = DefaultSkin.OnApplySkinSettings();
                    
                    for (int i = 1; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                        Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries[0] = DefaultSkin.OnApplySkinSettings();
                    Application.Current.Resources.MergedDictionaries[1] = targetSkin.OnApplySkinSettings();

                    for (int i = 2; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                        Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                }
            }
        }

        public SkinManager(string skinsFolderPath, ISkinInfo defaultSkin)
        {
            SkinsFolderPath = skinsFolderPath;
            DefaultSkin = defaultSkin;

            SkinsCollection();

            ApplySkin(DefaultSkin);
        }

        void SkinsCollection()
        {
            Skins.Clear();
            Skins.Add(DefaultSkin);
            foreach (string s in Directory.EnumerateDirectories(SkinsFolderPath))
            {
                string path = Path.Combine(s, "Skin.dll");
                if (File.Exists(path))
                {
                    Assembly skinAssembly = Assembly.LoadFrom(path);
                    var attributes = skinAssembly.GetCustomAttributes(true); //typeof(SkinAssemblyAttribute)
                    foreach (Attribute attr in attributes)
                    {
                        if (attr is SkinAssemblyAttribute skinAttr)
                        {
                            ISkinInfo skinInfo = (ISkinInfo)Activator.CreateInstance(skinAttr.InterfaceImplType);
                            Skins.Add(skinInfo);
                        }
                    }
                }
            }
        }


        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
