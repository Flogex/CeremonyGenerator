using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax.Structs
{
    /// <summary>
    /// Generates the syntax for implementing object.Equals.
    /// </summary>
    /// <remarks>
    /// The resulting syntax is "public override bool Equals(object obj) => obj is StructName @struct && Equals(@struct);".
    /// </remarks>
    internal static class ObjectEqualsSyntax
    {
        public static MethodDeclarationSyntax Create(SyntaxToken structName)
        {
            // Equals(obj as ClassName)
            var body = BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                IsPatternExpression(
                    IdentifierName("obj"),
                    DeclarationPattern(
                        IdentifierName(structName),
                        SingleVariableDesignation(Identifier("@struct")))),
                InvocationExpression(
                    IdentifierName(nameof(object.Equals)))
                .WithSingleArgument(IdentifierName("@struct")));

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
