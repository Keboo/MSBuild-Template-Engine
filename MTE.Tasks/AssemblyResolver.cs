using System;
using System.IO;
using System.Reflection;

namespace MTE.Tasks
{
    public static class AssemblyResolver
    {
        public static void Create(string probePath, AppDomain domain = null)
        {
            if (domain == null) domain = AppDomain.CurrentDomain;
            domain.AssemblyResolve += (_, e) =>
            {
                var assemblyName = new AssemblyName(e.Name);
                foreach (var file in Directory.EnumerateFiles(probePath, $"{assemblyName.Name}*"))
                {
                    try
                    {
                        var a = Assembly.LoadFrom(file);
                        if (a.FullName.Equals(e.Name))
                        {
                            return a;
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    { }
                }
                return null;
            };
        }
    }
}