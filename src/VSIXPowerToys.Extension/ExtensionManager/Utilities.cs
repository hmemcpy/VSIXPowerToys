using System;

namespace VSIXPowerToys.Extension.ExtensionManager
{
    internal static class Utilities
    {
        public static T GetService<S, T>(IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider?.GetService(typeof(S)) as T;
        }
    }
}