using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MTE.Tasks")]
namespace MTE.Core
{
    [Serializable]
    internal class Runner : MarshalByRefObject
    {
        public bool Run(Config config, string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            ITemplate template = (ITemplate)assembly.CreateInstance("MteTemplate");
            return template?.Execute(config) ?? false;
        }
    }
}