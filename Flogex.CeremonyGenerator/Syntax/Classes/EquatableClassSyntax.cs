using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax.Classes
{
    /// <summary>
    /// Generates the syntax for a partial class implementing IEquatable for its sibling.
    /// </summary>
    internal static class EquatableClassSyntax
    {
        public static TypeDeclarationSyntax? Create(ClassDeclarationSyntax extendee, out Diagnostic? diagnostic) =>
            EquatableTypeSyntax.Create(extendee, NewClassSyntax, EquatableMembers, out diagnostic);

        private static TypeDeclarationSyntax NewClassSyntax(SyntaxToken identifier) =>
            ClassDeclaration(identifier);

        private static MemberDeclarationSyntax[] EquatableMembers(TypeDeclarationSyntax extendee, IList<PropertyDeclarationSyntax> properties)
        {
            return new MemberDeclarationSyntax[]
            {
                EquatableEqualsSyntax.Create(extendee.Identifier, properties),
                ObjectEqualsSyntax.Create(extendee.Identifier),
                GetHashCodeSyntax.Create(properties)
                //TODO Add overloaded operators
            };
        }
    }
}
