using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    /// <summary>
    /// Generates the syntax for accessing a property.
    /// </summary>
    internal static class PropertyAccessSyntax
    {
        public static MemberAccessExpressionSyntax Create(SyntaxToken propertyName)
        {
            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                ThisExpression(),
                IdentifierName(propertyName));
        }
    }
}
