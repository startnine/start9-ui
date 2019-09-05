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
                if ((value != null) && Skins.Contains(value) && ((_activeSkin != value) || (value.GetHaveSettingsChanged())))
                {
                    _activeSkin = value;
                    ApplySkin(_activeSkin);
                    NotifyPropertyChanged(nameof(ActiveSkin));
                }
            }
        }

        void ApplySkin(ISkinInfo targetSkin)
        {
            ResourceDictionary defl = DefaultSkin.OnApplySkinSettings();
            ResourceDictionary targ = targetSkin.OnApplySkinSettings();
            if (Application.Current.Resources.MergedDictionaries.Count == 0)
            {
                if (targetSkin == DefaultSkin)
                    Application.Current.Resources.MergedDictionaries.Add(defl);
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(defl);
                    Application.Current.Resources.MergedDictionaries.Add(targ);
                }
            }
            else if (Application.Current.Resources.MergedDictionaries.Count == 1)
            {
                if (targetSkin == DefaultSkin)
                    Application.Current.Resources.MergedDictionaries[0] = defl;
                else
                {
                    Application.Current.Resources.MergedDictionaries[0] = defl;
                    Application.Current.Resources.MergedDictionaries.Add(targ);
                }
            }
            else
            {
                if (targetSkin == DefaultSkin)
                {
                    Application.Current.Resources.MergedDictionaries[0] = defl;
                    
                    for (int i = 1; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                        Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries[0] = defl;
                    Application.Current.Resources.MergedDictionaries[1] = targ;

                    for (int i = 2; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                        Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                }
            }
        }

        public SkinManager(ISkinInfo defaultSkin)
        {
            DefaultSkin = defaultSkin;

            Skins.Add(DefaultSkin);

            ApplySkin(DefaultSkin);
        }

        public void LoadSkinFromFolder(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException();

            string path = Path.Combine(dirPath, "Skin.dll");
            if (File.Exists(path))
            {
                Assembly skinAssembly = Assembly.LoadFile(path);
                var attributes = skinAssembly.GetCustomAttributes(true); //typeof(SkinAssemblyAttribute)
                foreach (Attribute attr in attributes)
                {
                    if (attr is SkinAssemblyAttribute skinAttr)
                    {
                        ISkinInfo skinInfo = (ISkinInfo)Activator.CreateInstance(skinAttr.InterfaceImplType);
                        Skins.Add(skinInfo);
                        return;
                    }
                }
                throw new Exception("The \"Skin.dll\" assembly within the provided folder does not possess the " + typeof(SkinAssemblyAttribute).FullName.ToString() + " attribute.");
            }
            else
                throw new FileNotFoundException("Assembly \"Skin.dll\" was not found within the folder specified.");
        }


        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
