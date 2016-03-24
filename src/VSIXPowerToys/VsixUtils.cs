using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.ExtensionManager;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace VSIXPowerToys
{
    [ComVisible(true)]
    [DisplayName("VSIX PowerToys")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".vsix")]
    public class VsixUtils : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return SelectedItemPaths.Count() == 1 && 
                   Path.GetExtension(SelectedItemPaths.First()).ToLowerInvariant() == ".vsix";
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menuStrip = new ContextMenuStrip();
            var topLevel = new ToolStripMenuItem("VSIX PowerToys");

            var copyVsix = new ToolStripMenuItem("Copy VSIX ID to Clipboard");
            copyVsix.Click += (sender, args) => CopyVsixIdToClipboard();
            topLevel.DropDownItems.Add(copyVsix);

            menuStrip.Items.Add(topLevel);
            return menuStrip;
        }

        private void CopyVsixIdToClipboard()
        {
            string path = SelectedItemPaths.FirstOrDefault();
            if (string.IsNullOrEmpty(path)) return;

            IInstallableExtension vsix = ExtensionManagerService.CreateInstallableExtension(path);

            Clipboard.SetText(vsix.Header.Identifier);
        }
    }
}