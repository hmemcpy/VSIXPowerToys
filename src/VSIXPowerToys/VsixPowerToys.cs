using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.ExtensionManager;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.SharpContextMenu;

namespace VSIXPowerToys
{
    [ComVisible(true)]
    [DisplayName("VSIX PowerToys")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".vsix")]
    public class VsixPowerToys : SharpContextMenu
    {
        private IInstallableExtension vsix;

        protected override bool CanShowMenu()
        {
            var paths = SelectedItemPaths.ToArray();
            if (paths.Length != 1 ||
                Path.GetExtension(paths[0]).ToLowerInvariant() != ".vsix")
                return false;

            try
            {
                vsix = ExtensionManagerService.CreateInstallableExtension(paths[0]);
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
            //var value = new ToolStripMenuItem("Custom...");
            //root.DropDownItems.Add(value);

            var vsVersions = VsUtils.GetInstalledVisualStudioVersions().Where(IsSupportedByTarget).ToList();
            foreach (var vsVersion in vsVersions)
            {
                if (root.HasDropDownItems && !(root.DropDownItems[root.DropDownItems.Count - 1] is ToolStripSeparator))
                {
                    root.DropDownItems.Add(new ToolStripSeparator());
                }

                var hives = VsUtils.GetAllHives(vsVersion).Where(hive => hive.Exists());

                foreach (var hive in hives)
                {
                    var item = new ToolStripMenuItem(hive.ToString(), null, (sender, args) => InstallVsix(hive));
                    root.DropDownItems.Add(item);
                }
            }

            if (!root.HasDropDownItems)
            {
                root.DropDownItems.Add(new ToolStripMenuItem("(No compatible Visual Studio editions found)") { Enabled = false });
            }
        }

        private void InstallVsix(VsHive hive)
        {
            if (vsix.Header.AllUsers && !hive.IsMainHive)
            {
                if (MessageBox.Show("This extension is configured to be installed for all users, " +
                                    $"and will be installed in the per-machine location (Common7\\IDE\\Extensions) instead of {hive}.\n\n" +
                                    "Proceed with installation?", "VSIX PowerToys", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return;
                }
            }

            SystemCursor.SetSystemCursor(Cursors.WaitCursor);
            bool result = false;
            try
            {
                result = PerformInstallation(hive);
            }
            catch (Exception ex)
            {
                LogError("An error ocurred while installing the extension", ex);
                Debugger.Launch();
            }
            finally
            {
                SystemCursor.RestoreSystemCursor();
            }

            if (result)
            {
                MessageBox.Show($"Installation of '{vsix.Header.Name}' into '{hive}' was successful!",
                    "VSIX PowerToys",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Installation of '{vsix.Header.Name}' into '{hive}' was unsuccessful.\n\n" +
                                "Please check the log file at %TEMP%\\VSIXPowerToys.log",
                    "VSIX PowerToys",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private bool PerformInstallation(VsHive hive)
        {
            var codebase = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var hostProcess = Path.Combine(Path.GetDirectoryName(codebase), "HostProcess.exe");
            var arguments = $"\"{vsix.PackagePath}\" \"{hive.VsVersion.DevEnvPath}\" {hive.RootSuffix}";
            Log($"Executing '{hostProcess} {arguments}'");

            var p = new Process
            {
                StartInfo =
                {
                    FileName = hostProcess,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            p.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data)) Log(args.Data);
            };
            p.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data)) LogError(args.Data);
            };

            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                p.WaitForExit();
                return p.ExitCode == 0;
            }
            catch (Exception ex)
            {
                LogError($"An error ocurred while executing HostProcess with arguments '{p.StartInfo.Arguments}'", ex);
                return false;
            }
            finally
            {
                p.Dispose();
            }
        }

        private bool IsSupportedByTarget(VsVersion vsVersion)
        {
            // todo add support for specific productIds
            return vsix.Targets.DistinctBy(req => req.VersionRange)
                    .Any(target => target.VersionRange.Contains(Version.Parse(vsVersion.Version)));
        }

        private void CopyVsixIdToClipboard()
        {
            Clipboard.SetText(vsix.Header.Identifier);
        }
    }
}