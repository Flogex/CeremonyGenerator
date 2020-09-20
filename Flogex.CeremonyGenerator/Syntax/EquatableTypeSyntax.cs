using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Flogex.CeremonyGenerator.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    internal class EquatableTypeSyntax
    {
        public static TypeDeclarationSyntax? Create(
            TypeDeclarationSyntax extendee,
            Func<SyntaxToken, TypeDeclarationSyntax> declarationStrategy,
            Func<TypeDeclarationSyntax, IList<PropertyDeclarationSyntax>, MemberDeclarationSyntax[]> membersStrategy,
            out Diagnostic? diagnostic)
        {
            if (!extendee.IsPartial())
            {
                diagnostic = TypeMustBePartial(extendee);
                return null;
            }

            var properties = extendee.Members
                .Where(m => m.IsAccessibleProperty())
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            // Do not generate the interface if the class does not contain any properties.
            if (properties.Count == 0)
            {
                diagnostic = TypeMustHavePublicProperties(extendee);
                return null;
            }

            var generatedClass = declarationStrategy.Invoke(extendee.Identifier)
                .WithModifiers(extendee.Modifiers) // Reuse modifiers
                .WithSingleBase(EquatableImplementationSyntax.Create(extendee.Identifier))
                .WithMembers(List(membersStrategy.Invoke(extendee, properties)));

            diagnostic = null;
            return generatedClass;
        }
    }
}
