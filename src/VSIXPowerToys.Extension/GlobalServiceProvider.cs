using System;
using Microsoft.VisualStudio.Shell;

namespace VSIXPowerToys.Extension
{
    /// <summary>
    /// Locates global services inside Visual Studio, in a thread-safe way, unlike 
    /// the VS Shell version.
    /// </summary>
    public static class GlobalServiceProvider
    {
        private static readonly IServiceProvider dteProvider = new DteServiceProvider();
        private static readonly VsServiceProvider vsProvider = new VsServiceProvider();

        /// <summary>
        /// Gets the global service provider.
        /// </summary>
        public static IServiceProvider Instance { get; } = new FallbackServiceProvider(
            // DTE, /*ComponentModel*/, Global Package
            dteProvider, vsProvider);

        private class FallbackServiceProvider : IServiceProvider
        {
            private readonly IServiceProvider primary;
            private readonly IServiceProvider fallback;

            public FallbackServiceProvider(IServiceProvider primary, IServiceProvider fallback)
            {
                this.primary = primary;
                this.fallback = fallback;
            }

            public object GetService(Type serviceType)
            {
                return primary.GetService(serviceType) ?? fallback.GetService(serviceType);
            }
        }

        private class DteServiceProvider : IServiceProvider
        {
            private static readonly Lazy<IServiceProvider> globalProvider = new Lazy<IServiceProvider>(GetGlobalProvider);

            public object GetService(Type serviceType)
            {
                return globalProvider.Value.GetService(serviceType);
            }

            private static IServiceProvider GetGlobalProvider()
            {
                var dte = Package.GetGlobalService(typeof(EnvDTE.DTE));
                var ole = dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
                return new Microsoft.VisualStudio.Shell.ServiceProvider(ole);
            }
        }

        private class VsServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return Package.GetGlobalService(serviceType);
            }
        }
    }
}