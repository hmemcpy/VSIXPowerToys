using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace VSIXPowerToys
{
    public static class VsUtils
    {
        private static readonly DirectoryInfo VsLocalDirectory = GetLocalVisualStudioDirectory();

        private static DirectoryInfo GetLocalVisualStudioDirectory()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return new DirectoryInfo(Path.Combine(localAppData, "Microsoft", "VisualStudio"));
        }

        public static IEnumerable<VsHive> GetAllHives(VsVersion vsVersion)
        {
            return VsLocalDirectory.EnumerateDirectories().Where(d => d.Name.StartsWith(vsVersion.Version))
                .Select(d => new VsHive(vsVersion, d.Name.Substring(vsVersion.Version.Length)));
        }

        public static IEnumerable<VsVersion> GetInstalledVisualStudioVersions()
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            using (var vs = hklm.OpenSubKey("Software\\Microsoft\\VisualStudio"))
            {
                if (vs == null)
                    yield break;

                foreach (var s in vs.GetSubKeyNames().Where(IsVsVersion).OrderByDescending(s => s))
                {
                    using (var setup = vs.OpenSubKey(s + "\\" + "Setup\\vs"))
                    {
                        var devenv = (string)setup?.GetValue("EnvironmentPath");
                        if (!string.IsNullOrWhiteSpace(devenv) && File.Exists(devenv))
                            yield return new VsVersion(s, devenv);
                    }
                }
            }
        }

        public static void CheckLastWin32Error()
        {
            var error = Marshal.GetLastWin32Error();
            if (error != 0) throw new Win32Exception(error);
        }

        private static bool IsVsVersion(string arg)
        {
            decimal v;
            return decimal.TryParse(arg, NumberStyles.Number, CultureInfo.InvariantCulture, out v);
        }
    }

    public class VsVersion
    {
        public string Version { get; }
        public string DevEnvPath { get; }

        public VsVersion(string vsVersion, string devenvPath)
        {
            Version = vsVersion;
            DevEnvPath = devenvPath;
        }
    }

    public class VsHive
    {
        public VsVersion VsVersion { get; }
        public string RootSuffix { get; }
        public bool IsMainHive { get; }

        public VsHive(VsVersion vsVersion, string rootSuffix)
        {
            VsVersion = vsVersion;
            RootSuffix = rootSuffix;
            IsMainHive = string.IsNullOrWhiteSpace(rootSuffix);
        }

        public bool Exists()
        {
            using (var hive = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\VisualStudio"))
            {
                if (hive == null)
                    return false;

                return hive.GetSubKeyNames().Contains(ToString());
            }
        }

        public override string ToString() => $"{VsVersion.Version}{RootSuffix}";
    }
}