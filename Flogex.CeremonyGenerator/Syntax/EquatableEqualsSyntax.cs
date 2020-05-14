using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    /// <summary>
    /// Generates the syntax for implementing IEquatable.Equals.
    /// </summary>
    /// <remarks>
    /// The resulting syntax is "public bool Equals(ClassName other) => other is object && this.Property1.Equals(other.Property1) && ...;".
    /// </remarks>
    internal static class EquatableEqualsSyntax
    {
        public static MethodDeclarationSyntax Create(SyntaxToken className, IList<PropertyDeclarationSyntax> properties)
        {
            //TODO Avoid recursive calls, if somehow it stores an instance to self or so

            /* I show some sample code here for a class with Property1 and Property2. */

            // this.Property2.Equals(other.Property2)
            ExpressionSyntax rightExpression = ComparePropertySyntax.Create(properties[^1].Identifier);

            // Start with the last property because the expression is built from right to left.
            for (var i = properties.Count - 2; i >= 0; i--)
            {
                // this.Property1.Equals(other.Property1) && this.Property2.Equals(other.Property2)
                rightExpression = BinaryExpression(
                    SyntaxKind.LogicalAndExpression,
                    ComparePropertySyntax.Create(properties[i].Identifier),
                    rightExpression);
            }

            // other is object && this.Property1.Equals(other.Property1) && this.Property2.Equals(other.Property2)
            // 'other is object' is checking for not-null.
            var rootExpression = BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                BinaryExpression(
                    SyntaxKind.IsExpression,
                    IdentifierName("other"),
                    PredefinedType(Token(SyntaxKind.ObjectKeyword))),
                rightExpression);

            return MethodDeclaration(
                PredefinedType(Token(SyntaxKind.BoolKeyword)),
                Identifier("Equals"))
            .WithPublicModifier()
            .WithSingleParameter(Parameter(Identifier("other")).WithType(IdentifierName(className)))
            .WithExpressionBody(ArrowExpressionClause(rootExpression))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }
    }
}
