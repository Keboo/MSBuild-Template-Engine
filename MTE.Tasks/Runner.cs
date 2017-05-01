using MTE.Core;
using System;
using System.IO;
using System.Reflection;

namespace MTE.Tasks
{
    [Serializable]
    internal class Runner : MarshalByRefObject
    {
        public TemplateResult Run(Config config, string assemblyPath)
        {
            var rv = new TemplateResult();

            AssemblyResolver.Create(Path.GetDirectoryName(assemblyPath));

            Assembly assembly = Assembly.LoadFile(assemblyPath);
            rv.Log($"Loaded {assemblyPath}");
            ITemplate template = (ITemplate)assembly.CreateInstance("MteTemplate");
            try
            {
                if (template != null)
                {
                    rv.Log($"Found template {template.GetType().FullName}");
                    return template.Execute(config);
                }
                throw new Exception($"Could not find 'MteTemplate' in {assembly.FullName} @ '{assembly.Location}'");
            }
            catch (Exception e)
            {
                rv.Log($"Exception in Template {assemblyPath}\r\n{e}");
                throw;
            }
        }
    }
}