using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    /// <summary>
    /// Generates the syntax for comparing this.Property with other.Property for equality.
    /// </summary>
    /// <remarks>
    /// The resulting syntax is "this.PropertyName.Equals(other.PropertyName)".
    /// </remarks>
    internal static class ComparePropertySyntax
    {
        public static InvocationExpressionSyntax Create(SyntaxToken propertyName)
        {
            return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName(propertyName)),
                    IdentifierName("Equals")))
                .WithSingleArgument(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("other"),
                        IdentifierName(propertyName)));
        }
    }
}
