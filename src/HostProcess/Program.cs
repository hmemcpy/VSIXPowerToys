using System;
using System.Diagnostics;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Settings;

namespace HostProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Log("Starting HostProcess...");

            if (args.Length < 2)
            {
                LogError($"Invalid number of arguments. Expected 3 but was {args.Length}: {string.Join(", ", args)}");
                Environment.Exit(-1);
            }

            string vsixPath = args[0];
            string devenvPath = args[1];
            string hive = args.Length == 3 ? args[2] : null;

            try
            {
                IInstallableExtension vsix = ExtensionManagerService.CreateInstallableExtension(vsixPath);

                if (string.IsNullOrWhiteSpace(hive))
                    hive = "";

                PerformInstallation(vsix, devenvPath, hive);
            }
            catch (Exception)
            {
                Environment.Exit(-1);
            }

            Log("Exiting HostProcess...");
        }

        private static IInstalledExtension PerformInstallation(IInstallableExtension vsix, string devenvPath, string hive)
        {
            IInstalledExtension extension;

            using (var settingsManager = ExternalSettingsManager.CreateForApplication(devenvPath, hive))
            {
                var identifier = vsix.Header.Identifier;
                var name = vsix.Header.Name;

                Log($"Preparing to install '{name}' with identifier '{identifier}' into '{hive}'");

                var extensionManager = new ExtensionManagerService(settingsManager);

                if (extensionManager.TryGetInstalledExtension(identifier, out extension))
                {
                    Log($"Extension '{name}' was already installed. Uninstalling...");
                    try
                    {
                        extensionManager.Uninstall(extension);
                        extensionManager.CommitExternalUninstall(extension);
                    }
                    catch (Exception ex)
                    {
                        LogError($"An error ocurred while trying to uninstall '{name}'. Rolling back...", ex);
                        RevertUninstall(extensionManager, extension);
                        throw;
                    }
                }
                try
                {
                    Log($"Starting installation of '{name}'");
                    extensionManager.Install(vsix, perMachine: false);
                    extension = extensionManager.GetInstalledExtension(identifier);
                    Log($"Installation of '{name}' into '{hive}' completed successfully.");
                }
                catch (Exception ex)
                {
                    LogError($"An error ocurred while trying to install '{name}'. Rolling back...", ex);
                    RevertUninstall(extensionManager, extension);
                    throw;
                }
            }

            return extension;
        }

        private static void Log(string message)
        {
            Console.Out.WriteLine(message);
        }

        private static void LogError(string message, Exception ex = null)
        {
            Console.Error.WriteLine(message);
            if (ex != null)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static void RevertUninstall(ExtensionManagerService extensionManager, IInstalledExtension oldExtension)
        {
            if (oldExtension == null || extensionManager.IsInstalled(oldExtension))
                return;

            Log($"Reverting uninstall of '{oldExtension.Header.Name}'...");
            extensionManager.RevertUninstall(oldExtension);
        }

    }
}
