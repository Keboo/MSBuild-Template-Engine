using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using MTE.Core;
using System.IO;

namespace MTE.CSharp
{
    public abstract class PerFileTemplate : CSharpSyntaxRewriter, ITemplate
    {
        public virtual TemplateResult Execute(Config config)
        {
            var rv = new TemplateResult();
            foreach (ITaskItem item in config.InputItems)
            {
                rv.Log($"Processing: {item}");
                if (!ProcessItem(item, config))
                {
                    rv.Failed($"Failed on {item}");
                    break;
                }
            }
            return rv;
        }

        protected virtual bool ProcessItem(ITaskItem item, Config config)
        {
            string fullPath = item.GetMetadata("FullPath");
            //TODO: ensure this is actually a C# file
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(fullPath));
            SyntaxNode newRoot = Visit(tree.GetRoot());
            newRoot = Formatter.Format(newRoot, newRoot.FullSpan, new AdhocWorkspace());
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
