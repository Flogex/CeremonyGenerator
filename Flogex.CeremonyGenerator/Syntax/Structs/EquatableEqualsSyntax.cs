using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax.Structs
{
    /// <summary>
    /// Generates the syntax for implementing IEquatable.Equals.
    /// </summary>
    /// <remarks>
    /// The resulting syntax is "public bool Equals(StructName other) => this.Property1.Equals(other.Property1) && this.Property2.Equals(other.Property2);".
    /// </remarks>
    internal static class EquatableEqualsSyntax
    {
        public static MethodDeclarationSyntax Create(SyntaxToken className, IList<PropertyDeclarationSyntax> properties)
        {
            //TODO Avoid recursive calls, if somehow it stores an instance to self

            /* I show some sample code here for a class with Property1 and Property2. */

            // this.Property2.Equals(other.Property2)
            ExpressionSyntax compExpression = ComparePropertySyntax.Create(properties[^1].Identifier);

            // Start with the last property because the expression is built from right to left.
            for (var i = properties.Count - 2; i >= 0; i--)
            {
                // this.Property1.Equals(other.Property1) && this.Property2.Equals(other.Property2)
                compExpression = BinaryExpression(
                    SyntaxKind.LogicalAndExpression,
                    ComparePropertySyntax.Create(properties[i].Identifier),
                    compExpression);
            }

            return MethodDeclaration(
                PredefinedType(Token(SyntaxKind.BoolKeyword)),
                Identifier(nameof(object.Equals)))
            .WithPublicModifier()
            .WithSingleParameter(Parameter(Identifier("other")).WithType(IdentifierName(className)))
            .WithExpressionBody(ArrowExpressionClause(compExpression))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }
    }
}
