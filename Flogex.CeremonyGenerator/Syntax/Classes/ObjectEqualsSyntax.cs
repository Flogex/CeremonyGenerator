using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax.Classes
{
    /// <summary>
    /// Generates the syntax for implementing object.Equals.
    /// </summary>
    /// <remarks>
    /// The resulting syntax is "public override bool Equals(object obj) => Equals(obj as ClassName);".
    /// </remarks>
    internal static class ObjectEqualsSyntax
    {
        public static MethodDeclarationSyntax Create(SyntaxToken className)
        {
            // Equals(obj as ClassName)
            var body = InvocationExpression(
                IdentifierName("Equals"))
                .WithSingleArgument(
                    BinaryExpression(
                        SyntaxKind.AsExpression,
                        IdentifierName("obj"),
                        IdentifierName(className)));

            return MethodDeclaration(PredefinedType(
                Token(SyntaxKind.BoolKeyword)),
                Identifier("Equals"))
                .WithModifiers(TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.OverrideKeyword)))
                .WithSingleParameter(Parameter(Identifier("obj")).WithType(PredefinedType(Token(SyntaxKind.ObjectKeyword))))
                .WithExpressionBody(ArrowExpressionClause(body))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }
    }
}
