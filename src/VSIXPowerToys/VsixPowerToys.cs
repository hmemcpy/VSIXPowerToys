using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace VSIXPowerToys
{
    [ComVisible(true)]
    [DisplayName("VSIX PowerToys")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".vsix")]
    public class VsixPowerToys : SharpContextMenu
    {
        private IInstallableExtension vsix;
        private string identifier;

        protected override bool CanShowMenu()
        {
            var paths = SelectedItemPaths.ToArray();
            if (paths.Length != 1 ||
                Path.GetExtension(paths[0]).ToLowerInvariant() != ".vsix")
                return false;

            try
            {
                vsix = ExtensionManagerService.CreateInstallableExtension(paths[0]);
                identifier = vsix.Header.Identifier;
            }
            catch (Exception ex)
            {
                LogError($"Unable to load the extension '{paths[0]}'", ex);
                return false;
            }

            return vsix != null;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menuStrip = new ContextMenuStrip();
            var install = new ToolStripMenuItem("Install into");
            CreateInstallItems(install);
            menuStrip.Items.Add(install);


            var topLevel = new ToolStripMenuItem("VSIX PowerToys");

            var copyVsix = new ToolStripMenuItem("Copy VSIX ID to Clipboard");
            copyVsix.Click += (sender, args) => CopyVsixIdToClipboard();
            topLevel.DropDownItems.Add(copyVsix);

            menuStrip.Items.Add(topLevel);
            return menuStrip;
        }

        private void CreateInstallItems(ToolStripMenuItem root)
        {
            Debug.Assert(vsix != null);

            root.DropDownItems.Add(new ToolStripMenuItem("Custom..."));

            var vsVersions = VsUtils.GetInstalledVisualStudioVersions().Where(IsSupportedByTarget).ToList();
            foreach (var vsVersion in vsVersions)
            {
                if (!(root.DropDownItems[root.DropDownItems.Count - 1] is ToolStripSeparator))
                {
                    root.DropDownItems.Add(new ToolStripSeparator());
                }

                var hives = VsUtils.GetAllHives(vsVersion.Version).Where(hive => hive.Exists());

                foreach (var hive in hives)
                {
                    var item = new ToolStripMenuItem(hive.ToString(), null, (sender, args) => InstallVsix(vsVersion, hive));
                    root.DropDownItems.Add(item);
                }
            }
        }

        private void InstallVsix(VsVersion vsVersion, VsHive hive)
        {
            var cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var extension = PerformInstallation(vsVersion, hive);
                if (extension != null)
                {
                    MessageBox.Show($"Installation of '{extension.Header.Name}' into '{hive}' was successful!",
                        "VSIX PowerToys",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogError("An error ocurred while installing the extension", ex);
                Debugger.Launch();
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }

        private bool IsSupportedByTarget(VsVersion vsVersion)
        {
            // todo add support for specific productIds
            return vsix.Targets.DistinctBy(req => req.VersionRange)
                    .Any(target => target.VersionRange.Contains(Version.Parse(vsVersion.Version)));
        }

        private IInstalledExtension PerformInstallation(VsVersion vsVersion, VsHive hive)
        {
            IInstalledExtension extension;

            using (var settingsManager = ExternalSettingsManager.CreateForApplication(vsVersion.DevEnvPath, hive.RootSuffix))
            {
                Log($"Preparing to install '{vsix.Header.Name}' with identifier '{identifier}' into '{hive}'");

                var extensionManager = new ExtensionManagerService(settingsManager);
                
                if (extensionManager.TryGetInstalledExtension(identifier, out extension))
                {
                    Log($"Extension '{extension.Header.Name}' was already installed. Uninstalling...");
                    try
                    {
                        extensionManager.Uninstall(extension);
                    }
                    catch (Exception ex)
                    {
                        LogError($"An error ocurred while trying to uninstall '{extension.Header.Name}'. Rolling back...", ex);
                        RevertUninstall(extensionManager, extension);
                        throw;
                    }
                }
                try
                {
                    Log($"Starting installation of '{vsix.Header.Name}'");
                    extensionManager.Install(vsix, perMachine: false);
                    extension = extensionManager.GetInstalledExtension(vsix.Header.Identifier);
                    Log($"Installation of '{extension.Header.Name}' into '{hive}' completed successfully.");
                }
                catch (Exception ex)
                {
                    LogError($"An error ocurred while trying to install '{vsix.Header.Name}'. Rolling back...", ex);
                    RevertUninstall(extensionManager, extension);
                    throw;
                }
            }

            return extension;
        }

        private void RevertUninstall(ExtensionManagerService extensionManager, IInstalledExtension oldExtension)
        {
            if (oldExtension == null || extensionManager.IsInstalled((IExtension)oldExtension))
                return;
            Log($"Reverting uninstall of '{oldExtension.Header.Name}'...");
            extensionManager.RevertUninstall(oldExtension);
        }

        private void CopyVsixIdToClipboard()
        {
            Debug.Assert(vsix != null);

            Clipboard.SetText(identifier);
        }
    }
}