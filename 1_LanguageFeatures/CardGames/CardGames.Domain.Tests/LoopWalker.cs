using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CardGames.Domain.Tests;

internal class LoopWalker : CSharpSyntaxWalker
{
    public bool ContainsLoops { get; private set; }

    public override void VisitForStatement(ForStatementSyntax node)
    {
        ContainsLoops = true;
        base.VisitForStatement(node);
    }

    public override void VisitForEachStatement(ForEachStatementSyntax node)
    {
        ContainsLoops = true;
        base.VisitForEachStatement(node);
    }

    public override void VisitWhileStatement(WhileStatementSyntax node)
    {
        ContainsLoops = true;
        base.VisitWhileStatement(node);
    }

    public override void VisitDoStatement(DoStatementSyntax node)
    {
        ContainsLoops = true;
        base.VisitDoStatement(node);
    }
}