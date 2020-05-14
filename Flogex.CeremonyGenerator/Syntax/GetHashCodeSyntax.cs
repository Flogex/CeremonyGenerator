using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    /// <summary>
    /// Generates the syntax for implementing object.GetHashCode.
    /// </summary>
    /// <remarks>
    /// The resulting syntax is "public oberride int GetHashCode() => HashCode.Combine(Property1, Property2, ...);".
    /// </remarks>
    internal static class GetHashCodeSyntax
    {
        public static MemberDeclarationSyntax Create(List<PropertyDeclarationSyntax> properties)
        {
            var args = properties.Select(p => Argument(PropertyAccessSyntax.Create(p.Identifier)));

            // HashCode.Combine(Property1, Property2)
            //TODO Handle more than eight properties
            //TODO What about platforms not supporting System.HashCode?
            var body = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("HashCode"),
                    IdentifierName("Combine")))
                .WithArgumentList(ArgumentList(SeparatedList(args)));

            return MethodDeclaration(
                PredefinedType(Token(SyntaxKind.IntKeyword)),
                Identifier("GetHashCode"))
                .WithModifiers(TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(ArrowExpressionClause(body))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }
    }
}
