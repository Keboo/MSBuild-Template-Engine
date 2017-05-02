using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using MTE.Core;
using System.IO;
using Microsoft.Build.Utilities;

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
                if (!ProcessItem(item, config, rv))
                {
                    rv.Failed($"Failed on {item}");
                    break;
                }
            }
            return rv;
        }

        protected virtual bool ProcessItem(ITaskItem item, Config config, TemplateResult result)
        {
            string fullPath = item.GetMetadata("FullPath");
            //TODO: ensure this is actually a C# file
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(fullPath));
            SyntaxNode newRoot = Visit(tree.GetRoot());
            newRoot = Formatter.Format(newRoot, newRoot.FullSpan, new AdhocWorkspace());
            WriteFile(newRoot, item, config, result);
            return true;
        }

        protected virtual void WriteFile(SyntaxNode root, ITaskItem originalItem, Config config, TemplateResult result)
        {
            result.RemoveItem(originalItem);
            string filePath = originalItem.GetMetadata("FullPath");
            string generatedFile = Path.Combine(Path.GetDirectoryName(filePath),
                        $"{Path.GetFileNameWithoutExtension(filePath)}.g{Path.GetExtension(filePath)}");
            result.AddItem(new TaskItem(generatedFile));
            using (var sw = new StreamWriter(generatedFile))
            {
                root.WriteTo(sw);
            }
        }
    }
}
