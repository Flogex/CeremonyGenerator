using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax.Structs
{
    /// <summary>
    /// Generates the syntax for a partial struct implementing IEquatable for its sibling.
    /// </summary>
    internal static class EquatableStructSyntax
    {
        public static TypeDeclarationSyntax? Create(StructDeclarationSyntax extendee, out Diagnostic? diagnostic) =>
            EquatableTypeSyntax.Create(extendee, NewStructSyntax, EquatableMembers, out diagnostic);

        private static TypeDeclarationSyntax NewStructSyntax(SyntaxToken identifier) =>
            StructDeclaration(identifier);

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
