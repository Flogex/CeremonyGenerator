using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Flogex.CeremonyGenerator.DiagnosticDescriptors;

namespace Flogex.CeremonyGenerator.Syntax
{
    /// <summary>
    /// Generates the syntax for a partial class implementing IEquatable for its sibling.
    /// </summary>
    internal static class EquatableClassSyntax
    {
        public static ClassDeclarationSyntax? Create(ClassDeclarationSyntax extendee, out Diagnostic? diagnostic)
        {
            if (!extendee.IsPartial())
            {
                diagnostic = ClassMustBePartial(extendee);
                return null;
            }

            var properties = extendee.Members
                .Where(m => m.IsAccessibleProperty())
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            // Do not generate the interface if the class does not contain any properties.
            if (properties.Count == 0)
            {
                diagnostic = ClassMustHavePublicProperties(extendee);
                return null;
            }

            var generatedClass = ClassDeclaration(extendee.Identifier)
                .WithModifiers(extendee.Modifiers) // Reuse modifiers
                .WithSingleBase(EquatableImplementationSyntax.Create(extendee.Identifier))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    EquatableEqualsSyntax.Create(extendee.Identifier, properties),
                    ObjectEqualsSyntax.Create(extendee.Identifier),
                    GetHashCodeSyntax.Create(properties)
                    //TODO Add overloaded operators
                }));

            diagnostic = null;
            return generatedClass;
        }
    }
}
