using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    internal static class SyntaxNodeExtensions
    {
        public static bool IsAccessibleProperty(this MemberDeclarationSyntax member)
        {
            var isPublicProperty =
                member.IsKind(SyntaxKind.PropertyDeclaration) &&
                member.Modifiers.Contains(m => m.IsKind(SyntaxKind.PublicKeyword));

            var getter = member.DescendantNodes()
                .Where(a => a.IsKind(SyntaxKind.GetAccessorDeclaration))
                .Cast<AccessorDeclarationSyntax>()
                .Where(g => g.HasModifiers() == false)
                .SingleOrDefault();

            return isPublicProperty && getter != default;
        }

        public static bool IsPartial(this ClassDeclarationSyntax @class)
            => @class.Modifiers.Contains(m => m.IsKind(SyntaxKind.PartialKeyword));

        public static SyntaxNode GetRoot(this SyntaxNode syntaxNode)
        {
            var root = syntaxNode;
            while (root.Parent != null) root = root.Parent;
            return root;
        }

        public static NamespaceDeclarationSyntax? GetContainingNamespace(this SyntaxNode syntaxNode)
        {
            var root = syntaxNode.GetRoot();

            return root.ChildNodes()
                .Where(c => c.IsKind(SyntaxKind.NamespaceDeclaration))
                .SingleOrDefault() as NamespaceDeclarationSyntax;
        }

        public static CompilationUnitSyntax WithSingleUsing(this CompilationUnitSyntax compilationUnit, UsingDirectiveSyntax @using)
            => compilationUnit.WithUsings(SingletonList(@using));

        public static CompilationUnitSyntax WithSingleMember(this CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
            => compilationUnit.WithMembers(SingletonList(member));

        public static NamespaceDeclarationSyntax WithSingleMember(this NamespaceDeclarationSyntax @namespace, MemberDeclarationSyntax member)
            => @namespace.WithMembers(SingletonList(member));

        public static ClassDeclarationSyntax WithSingleBase(this ClassDeclarationSyntax @class, BaseTypeSyntax @base)
            => @class.WithBaseList(BaseList(SingletonSeparatedList(@base)));

        public static MethodDeclarationSyntax WithPublicModifier(this MethodDeclarationSyntax method)
            => method.AddModifiers(Token(SyntaxKind.PublicKeyword));

        public static MethodDeclarationSyntax WithSingleParameter(this MethodDeclarationSyntax method, ParameterSyntax parameter)
            => method.WithParameterList(ParameterList(SingletonSeparatedList(parameter)));

        public static bool HasModifiers(this AccessorDeclarationSyntax accessor)
            => accessor.Modifiers.Any();

        public static InvocationExpressionSyntax WithSingleArgument(this InvocationExpressionSyntax invocation, ExpressionSyntax argument)
            => invocation.WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(argument))));
    }
}
