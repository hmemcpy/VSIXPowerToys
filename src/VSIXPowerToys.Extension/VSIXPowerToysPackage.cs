using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSIXPowerToys.Extension
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(Vsix.Id)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    public sealed class VSIXPowerToysPackage : Package
    {
    }
}
