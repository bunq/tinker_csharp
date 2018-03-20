using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tinker.Utils
{
    public class TinkerRunner
    {
        public static void Main(string[] args)
        {
            if (args.Length <= 0) return;

            var tinkerClassName = Path.GetFileNameWithoutExtension(args[0]);
            var tinkerInstance = (ITinker) MagicallyCreateInstance(tinkerClassName);
            tinkerInstance.Run(args);
        }

        private static object MagicallyCreateInstance(string className)
        {
            var assembly = Assembly.GetEntryAssembly();
            var type = assembly.GetTypes().First(t => t.Name == className);

            return Activator.CreateInstance(type);
        }
    }
}