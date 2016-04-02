using System.ComponentModel;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.ExtensionsExplorer;

namespace VSIXPowerToys.Extension.ExtensionManager
{
    public class InstalledExtensionItem : IVsExtension, INotifyPropertyChanged
    {
        public InstalledExtensionItem(IInstalledExtension installedExtension)
        {
            
        }

        public string Name { get; }
        public string Id { get; }
        public string Description { get; }
        public float Priority { get; }
        public BitmapSource MediumThumbnailImage { get; }
        public BitmapSource SmallThumbnailImage { get; }
        public BitmapSource PreviewImage { get; }
        public bool IsSelected { get; set; }

        public bool IsExtensionSdk => false;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}