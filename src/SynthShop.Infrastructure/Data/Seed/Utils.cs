using System.Reflection;

namespace SynthShop.Infrastructure.Data.Seed
{
    internal static class Utils
    {
        public static string GetSeedConfigPath(string fileName)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(assemblyPath, "Data", "Seed", "InitialData", fileName);
            return path;
        }
    }
}
