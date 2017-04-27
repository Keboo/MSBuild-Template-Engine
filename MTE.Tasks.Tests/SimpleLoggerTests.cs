using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MTE.Tasks.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CanRunSimpleLoggingTemplate()
        {
            ProjectCollection pc = new ProjectCollection();
            Dictionary<string, string> globalProperty =
                new Dictionary<string, string>
                {
                    {"Configuration", "Debug"},
                    { "Platform", "AnyCPU"}
                };

            string projectPath =
                Path.GetFullPath(@"..\..\..\Examples\SimpleLogger\AssemblyToProcess\AssemblyToProcess.csproj");

            BuildRequestData buildRequest = new BuildRequestData(projectPath, globalProperty, null, new[] { "DoMteTemplate" }, null);
            
            pc.Loggers.Add(new ConsoleLogger(LoggerVerbosity.Detailed));
            
            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(new BuildParameters(pc), buildRequest);

            Assert.IsTrue(buildResult.OverallResult == BuildResultCode.Success);
        }
    }
}
