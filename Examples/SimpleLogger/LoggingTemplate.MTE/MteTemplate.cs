using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MTE.CSharp;

public class MteTemplate : PerFileTemplate
{
    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        BlockSyntax blockSyntax = node.Body ?? SyntaxFactory.Block();

        var statement = SyntaxFactory.ParseStatement($"System.Console.WriteLine(\"Entered {node.Identifier.Text}\");\r\n");

        SyntaxList<StatementSyntax> newStatements = blockSyntax.Statements.Insert(0, statement);

        return node.WithBody(blockSyntax.Update(blockSyntax.OpenBraceToken, newStatements, blockSyntax.CloseBraceToken));
    }
}
