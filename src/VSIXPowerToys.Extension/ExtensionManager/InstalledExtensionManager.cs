using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.ExtensionsExplorer;

namespace VSIXPowerToys.Extension.ExtensionManager
{
    [Export(typeof(IVsExtensionManagerDialogProvider))]
    public class InstalledExtensionManager : IVsExtensionManagerDialogProvider
    {
        private ObservableCollection<InstalledExtensionItem> installedExtensions;
        private IVsExtensionManager extensionManager;
        private object _detailViewDataTemplate;

        internal ObservableCollection<InstalledExtensionItem> InstalledExtensions
        {
            get
            {
                return installedExtensions ??
                       (installedExtensions = new ObservableCollection<InstalledExtensionItem>(
                           ExtensionManager.GetInstalledExtensions()
                               .Where(x => !x.Header.SystemComponent &&
                                           !x.IsPackComponent && !
                                               IsProductUpdate(x))
                               .Select(x => new InstalledExtensionItem(x)).ToList()));
            }
        }

        private bool IsProductUpdate(IInstalledExtension installedExtension)
        {
            return true;
        }

        private IVsExtensionManager ExtensionManager => 
            extensionManager ?? (extensionManager = Utilities.GetService<SVsExtensionManager, IVsExtensionManager>(GlobalServiceProvider.Instance));
        
        public IVsExtensionsTreeNode Search(string searchTerms)
        {
            return null;
        }

        public string Name { get; } = "VSIX PowerToys";
        public float SortOrder { get; } = 0x0600;
        public object SmallIconDataTemplate { get; }
        public object MediumIconDataTemplate { get; }
        public object LargeIconDataTemplate { get; }

        public object DetailViewDataTemplate
            => _detailViewDataTemplate ?? (_detailViewDataTemplate = new ResourceDictionary
            {
                Source = new Uri("VSIXPowerToys.Extension;component/UI/InstalledProviderTemplates.xaml", UriKind.Relative)
            }["InstalledDetailTemplate"]);

        public object HeaderContent { get; }
        public object View { get; }
        public object ItemContainerStyle { get; }
        public IVsExtensionsTreeNode ExtensionsTree { get; }
        public bool ListVisibility { get; }
        public bool ListMultiSelect { get; }
    }
}