using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    /// <summary>
    /// Generates the syntax for a partial class implementing IEquatable for its sibling.
    /// </summary>
    internal static class EquatableClassSyntax
    {
        public static ClassDeclarationSyntax? Create(ClassDeclarationSyntax @class)
        {
            var properties = @class.Members
                .Where(m => m.IsAccessibleProperty())
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            // Do not generate the interface if the class does not contain any properties.
            if (properties.Count == 0) return null;

            return ClassDeclaration(@class.Identifier)
                .WithModifiers(@class.Modifiers) // Reuse modifiers of sibling class.
                                                 // They must contain "partial".
                .WithSingleBase(EquatableImplementationSyntax.Create(@class.Identifier))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    EquatableEqualsSyntax.Create(@class.Identifier, properties),
                    ObjectEqualsSyntax.Create(@class.Identifier),
                    GetHashCodeSyntax.Create(properties)
                    //TODO Add overloaded operators
                }));
        }
    }
}
