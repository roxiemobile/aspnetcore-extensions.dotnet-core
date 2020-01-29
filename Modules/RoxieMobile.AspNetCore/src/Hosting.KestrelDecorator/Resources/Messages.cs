using System.Globalization;
using System.Resources;
using RoxieMobile.CSharpCommons.Diagnostics;

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator.Resources
{
    // ASP.NET Core Localization with help of SharedResources
    // @link https://stackoverflow.com/a/42649361

    internal static class Messages
    {
// MARK: - Properties

        public static string ServerIsNotStarted =>
            GetString("ServerIsNotStarted");

// MARK: - Private Properties

        private static ResourceManager ResourceManager =>
            new ResourceManager(typeof(Messages).FullName!, typeof(Messages).Assembly);

        private static string GetString(string name)
        {
            var value = ResourceManager.GetString(name, CultureInfo.InvariantCulture);
            Guard.NotNull(value, Funcs.Null(nameof(value)));
            return value!;
        }
    }
}
