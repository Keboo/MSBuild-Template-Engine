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
            config.LogMessage($"Loaded {assemblyPath}");
            ITemplate template = (ITemplate)assembly.CreateInstance("MteTemplate");
            try
            {
                if (template != null)
                {
                    config.LogMessage($"Found template {template.GetType().FullName}");
                    return template.Execute(config);
                }
                config.LogMessage($"Could not find 'MteTemplate' in {assemblyPath}");
                return false;
            }
            catch (Exception e)
            {
                config.LogMessage($"Exception in Template {assemblyPath}\r\n{e}");
                return false;
            }
        }
    }
}