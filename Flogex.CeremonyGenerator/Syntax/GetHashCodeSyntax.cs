using System;
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
    /// The resulting syntax is "public override int GetHashCode() => HashCode.Combine(Property1, Property2, ...);" for up to 8 properties.
    /// </remarks>
    internal static class GetHashCodeSyntax
    {
        public static MemberDeclarationSyntax Create(List<PropertyDeclarationSyntax> properties)
        {
            var getHashCodeMethod = MethodDeclaration(
                PredefinedType(Token(SyntaxKind.IntKeyword)),
                Identifier(nameof(GetHashCode)))
                .WithModifiers(TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.OverrideKeyword)));

            return properties.Count <= 8
               ? getHashCodeMethod
                     .WithExpressionBody(ArrowExpressionClause(CreateCombineSyntax(properties)))
                     .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
               : getHashCodeMethod
                     .WithBody(CreateAddSyntax(properties));
        }

        private static InvocationExpressionSyntax CreateCombineSyntax(IEnumerable<PropertyDeclarationSyntax> properties)
        {
            var args = properties.Select(p => Argument(PropertyAccessSyntax.Create(p.Identifier)));

            // HashCode.Combine(this.Property1, this.Property2)
            return InvocationExpression(AccessMemberSyntax.Create("HashCode.Combine"))
                .WithArgumentList(ArgumentList(SeparatedList(args)));
        }

        private static BlockSyntax CreateAddSyntax(List<PropertyDeclarationSyntax> properties)
        {
            // var hash = new HashCode();
            var declaration = VariableDeclaration.Create("hash", nameof(HashCode));

            var addExpressions = properties.Select(CreateAddInvocation).Cast<StatementSyntax>().ToList();

            // return hash.ToHashCode();
            var returnStatement = ReturnStatement(
                InvocationExpression(AccessMemberSyntax.Create("hash.ToHashCode")));

            var statements = new StatementSyntax[addExpressions.Count + 2];
            statements[0] = declaration;
            addExpressions.CopyTo(statements, 1);
            statements[^1] = returnStatement;

            return Block(statements);
        }

        private static ExpressionStatementSyntax CreateAddInvocation(PropertyDeclarationSyntax property)
        {
            // hash.Add(this.[propertyName]);
            return ExpressionStatement(
                InvocationExpression(AccessMemberSyntax.Create("hash.Add"))
                .WithSingleArgument(PropertyAccessSyntax.Create(property.Identifier)));
        }
    }
}
