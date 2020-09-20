using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    internal static class VariableDeclaration
    {
        public static LocalDeclarationStatementSyntax Create(string variableName, string typeName)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(variableName)
                        .WithInitializer(
                            EqualsValueClause(
                                ObjectCreationExpression(
                                    IdentifierName(typeName))
                                .WithArgumentList(ArgumentList()))))));
        }
    }
}
