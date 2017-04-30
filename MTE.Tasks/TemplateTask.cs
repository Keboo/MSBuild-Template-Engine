using Microsoft.Build.Framework;
using MTE.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MTE.Tasks
{
    public class TemplateTask : Microsoft.Build.Utilities.Task, ICancelableTask
    {
        public string SolutionDir { get; set; }

        public string ProjectDir { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public ITaskItem[] InputFiles { get; set; }

        [Output]
        public ITaskItem[] RemoveItems { get; set; }

        [Output]
        public ITaskItem[] NewItems { get; set; }

        public override bool Execute()
        {
            bool rv = true;
            var config = new Config(InputFiles, ProjectPath);
            AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
            {
                return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == e.Name);
            };
            //TODO: Configure list of templates
            string assemblyPath = Path.GetFullPath(
                @"..\..\..\Examples\SimpleLogger\LoggingTemplate.MTE\bin\Debug\net462\LoggingTemplate.MTE.dll");
            rv &= RunTemplate(assemblyPath, config);

            foreach (string message in config.LogMessages)
            {
                Log.LogMessage(message);
            }

            if (rv)
            {
                RemoveItems = config.RemoveItems.ToArray();
                NewItems = config.AddItems.ToArray();
            }
            return rv;
        }

        private bool RunTemplate(string assemblyPath, Config config)
        {
            var setup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(assemblyPath),
                ConfigurationFile = $"{assemblyPath}.config",
                TargetFrameworkName = AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName
            };
            Log.LogMessage($"Creating app domain for: {assemblyPath}");
            var domain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(assemblyPath) + "MteDomain", null, setup);
            
            var runner = (Runner)domain.CreateInstanceAndUnwrap(typeof(Runner).Assembly.FullName, typeof(Runner).FullName);
            bool rv = runner.Run(config, assemblyPath);
            Log.LogMessage($"Success? {rv} {config.LogMessages.Count}");
            AppDomain.Unload(domain);
            return rv;
        }

        public void Cancel()
        {

        }
    }
}
