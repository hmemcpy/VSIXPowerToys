using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.ExtensionManager;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace VSIXPowerToys
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".vsix")]
    public class CopyVsixGuid : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menuStrip = new ContextMenuStrip();
            var item = new ToolStripMenuItem("Copy VSIX ID to Clipboard");
            item.Click += (sender, args) =>
            {
                string path = SelectedItemPaths.FirstOrDefault();
                if (string.IsNullOrEmpty(path)) return;

                IInstallableExtension vsix = ExtensionManagerService.CreateInstallableExtension(path);

                Clipboard.SetText(vsix.Header.Identifier);
            };

            menuStrip.Items.Add(item);
            return menuStrip;
        }
    }
}