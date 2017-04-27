using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MTE.Core;
using System.IO;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;

namespace MTE.CSharp
{
    public abstract class PerFileTemplate : CSharpSyntaxRewriter, ITemplate
    {
        public virtual bool Execute(Config config)
        {
            bool rv = true;
            foreach (ITaskItem item in config.InputItems)
            {
                rv &= ProcessItem(item, config);
                if (!rv) break;
            }
            return rv;
        }

        protected virtual bool ProcessItem(ITaskItem item, Config config)
        {
            string fullPath = item.GetMetadata("FullPath");
            //TODO: ensure this is actually a C# file
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(fullPath));
            SyntaxNode newRoot = Visit(tree.GetRoot());
            newRoot = Formatter.Format(newRoot, newRoot.FullSpan, MSBuildWorkspace.Create());
            WriteFile(newRoot, item, config);
            return true;
        }

        protected virtual void WriteFile(SyntaxNode root, ITaskItem originalItem, Config config)
        {
            string generatedFilePath = config.ReplaceWithGeneratedFile(originalItem);
            using (var sw = new StreamWriter(generatedFilePath))
            {
                root.WriteTo(sw);
            }
        }
    }
}
