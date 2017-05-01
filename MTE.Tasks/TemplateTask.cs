using Microsoft.Build.Framework;
using MTE.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

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

        public TemplateTask()
        {
            
        }

        public override bool Execute()
        {
            bool rv = true;
            AppDomain.CurrentDomain.AssemblyLoad += (_, e) =>
            {
                Log.LogMessage("Main Load " + e.LoadedAssembly.FullName);
            };
            AssemblyResolver.Create(Path.GetDirectoryName(typeof(Runner).Assembly.Location));

            //AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
            //{
            //    var found = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == e.Name);
            //    Log.LogMessage($"Main Resolve {e.Name} Found? {found != null}");
            //    //return null;
            //    return found;
            //};
            var config = new Config(InputFiles, ProjectPath);
            //TODO: Configure list of templates
            string assemblyPath = Path.GetFullPath(
                @"..\..\..\Examples\SimpleLogger\LoggingTemplate.MTE\bin\Debug\net462\LoggingTemplate.MTE.dll");
            rv &= RunTemplate(assemblyPath, config);
            
            if (rv)
            {
                //RemoveItems = config.RemoveItems.ToArray();
                //NewItems = config.AddItems.ToArray();
            }
            return rv;
        }

        private bool RunTemplate(string assemblyPath, Config config)
        {
            AppDomain domain = null;
            try
            {
                var setup = new AppDomainSetup
                {
                    ApplicationBase = Path.GetDirectoryName(typeof(Runner).Assembly.Location),
                    ConfigurationFile = $"{assemblyPath}.config",
                    LoaderOptimization = LoaderOptimization.MultiDomain,
                };
                Log.LogMessage($"Creating app domain for: {assemblyPath}");
                domain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(assemblyPath) + "MteDomain", null,
                    setup);
                //
                ////domain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(assemblyPath) + "MteDomain");
                //
                //Log.LogMessage($"MTE.Internal: {typeof(Runner).Assembly.Location}");
                //
                var runner =
                    (Runner)domain.CreateInstanceFromAndUnwrap(typeof(Runner).Assembly.Location, typeof(Runner).FullName);
                
                Log.LogMessage("Created runner");
                TemplateResult result = runner.Run(config, assemblyPath);
                Log.LogMessage($"Got TemplateResult {result.Success}");
                foreach (var message in result.Messages ?? Enumerable.Empty<string>())
                {
                    Log.LogMessage($" ==> {message}");
                }
                return result.Success;
            }
            finally
            {
                if (domain != null)
                    AppDomain.Unload(domain);
            }
        }

        //private bool RunTemplate(string assemblyPath, Config config)
        //{
        //    var setup = new AppDomainSetup
        //    {
        //        ApplicationBase = Path.GetDirectoryName(assemblyPath),
        //        ConfigurationFile = $"{assemblyPath}.config",
        //        LoaderOptimization = LoaderOptimization.MultiDomain,
        //        TargetFrameworkName = AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName
        //    };
        //    Log.LogMessage($"Creating app domain for: {assemblyPath}");
        //    var domain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(assemblyPath) + "MteDomain", null, setup);
        //    //domain.AssemblyLoad += (sender, args) =>
        //    //{
        //    //    config.LogMessage("Loaded " + args.LoadedAssembly.FullName);
        //    //};
        //    //domain.AssemblyResolve += (sender, args) =>
        //    //{
        //    //    config.LogMessage("Resolving " + args.Name);
        //    //    return null;
        //    //};
        //    try
        //    {
        //        Log.LogMessage("1");
        //        var bytes = File.ReadAllBytes(typeof(Runner).Assembly.Location);
        //        var a = domain.Load(bytes);
        //        Log.LogMessage("5");
        //        object foo = domain.CreateInstanceFromAndUnwrap(typeof(Runner).Assembly.FullName, typeof(Runner).FullName);
        //        Log.LogMessage("2");
        //        var cast = (Runner) foo;
        //        Log.LogMessage("3");
        //        cast.Run(config, assemblyPath);
        //        Log.LogMessage("4");
        //        //var runner = (Runner)domain.CreateInstanceAndUnwrap(typeof(Runner).Assembly.FullName, typeof(Runner).FullName);
        //        //bool rv = runner.Run(config, assemblyPath);
        //        //Log.LogMessage($"Success? {rv}");
        //        //config.LogMessage("My message");
        //        AppDomain.Unload(domain);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Log.LogErrorFromException(e);
        //        return false;
        //    }
        //}

        public void Cancel()
        {

        }
    }
}
