using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    /// <summary>
    /// Generates the syntax for implementing the IEquatable interface.
    /// </summary>
    /// <remarks>
    /// The resulting syntax is ": IEquatable<ClassName>".
    /// </remarks>
    internal static class EquatableImplementationSyntax
    {
        public static SimpleBaseTypeSyntax Create(SyntaxToken className)
        {
            return SimpleBaseType(
                GenericName("IEquatable")
                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(
                        IdentifierName(className)))));
        }
    }
}
