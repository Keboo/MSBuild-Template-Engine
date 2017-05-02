using System.IO;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MTE.Core;
using MTE.CSharp;

public class MteTemplate : PerFileTemplate
{
    protected override bool ProcessItem(ITaskItem item, Config config, TemplateResult result)
    {
        if (Path.GetFileName(item.GetMetadata("FullPath"))?.StartsWith("TemporaryGeneratedFile") == true)
            return true;
        return base.ProcessItem(item, config, result);
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        BlockSyntax blockSyntax = node.Body ?? SyntaxFactory.Block();
    
        var statement = SyntaxFactory.ParseStatement($"System.Console.WriteLine(\"Entered {node.Identifier.Text}\");\r\n");
    
        SyntaxList<StatementSyntax> newStatements = blockSyntax.Statements.Insert(0, statement);
    
        return node.WithBody(blockSyntax.Update(blockSyntax.OpenBraceToken, newStatements, blockSyntax.CloseBraceToken));
    }
}
