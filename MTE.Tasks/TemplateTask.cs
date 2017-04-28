using Microsoft.Build.Framework;
using MTE.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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
            Log.LogMessage($"Creating app domain in: {setup.ApplicationBase}");
            Log.LogMessage(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            Log.LogMessage(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            var domain = AppDomain.CreateDomain("OtherDomain", null, setup);

            var runner = (Runner)domain.CreateInstanceAndUnwrap(typeof(Runner).Assembly.FullName, typeof(Runner).FullName);
            bool rv = runner.Run(config, assemblyPath);

            AppDomain.Unload(domain);
            return rv;
        }

        public void Cancel()
        {

        }
    }
}
