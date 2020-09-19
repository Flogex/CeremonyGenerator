﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flogex.CeremonyGenerator
{
    /// <summary>
    /// The <see cref="ISyntaxReceiver"/> that collects the classes and structs for which the
    /// IEquatable interface is generated by <see cref="EquatableGenerator"/>.
    /// </summary>
    internal class TypeDeclarationsReceiver : ISyntaxReceiver
    {
        private readonly List<ClassDeclarationSyntax> _classDeclarations = new List<ClassDeclarationSyntax>();
        private readonly List<StructDeclarationSyntax> _structDeclarations = new List<StructDeclarationSyntax>();

        /// <summary>
        /// The classes that are marked with <see cref="GenerateEquatableAttribute"/> and for which
        /// therefore the IEquatable interface is generated.
        /// </summary>
        public IReadOnlyCollection<ClassDeclarationSyntax> ReceivedClasses => _classDeclarations.AsReadOnly();

        /// <summary>
        /// The structs that are marked with <see cref="GenerateEquatableAttribute"/> and for which
        /// therefore the IEquatable interface is generated.
        /// </summary>
        public IReadOnlyCollection<StructDeclarationSyntax> ReceivedStructs => _structDeclarations.AsReadOnly();

        /// <summary>
        /// Checks whether <paramref name="syntaxNode"/> is the <see cref="GenerateEquatableAttribute"/>. If yes, it
        /// adds the class or struct marked with this attribute to either <see cref="ReceivedClasses"/> or
        /// <see cref="ReceivedStructs"/>, respectively.
        /// </summary>
        /// <param name="syntaxNode"></param>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (IsGenerateEquatableAttribute(syntaxNode))
            {
                // Parent of Attribute is AttributeList
                // Parent of AttributeList is TypeDeclaration
                var type = syntaxNode.Parent?.Parent; //TODO syntaxNode.Anchestors()?

                if (type is ClassDeclarationSyntax @class)
                    _classDeclarations.Add(@class);
                else if (type is StructDeclarationSyntax @struct)
                    _structDeclarations.Add(@struct);
            }
        }

        private static bool IsGenerateEquatableAttribute(SyntaxNode syntaxNode)
        {
            return syntaxNode.IsKind(SyntaxKind.Attribute) &&
                (syntaxNode as AttributeSyntax)?.Name.ToFullString() == GenerateEquatableAttribute.Name;
        }
    }
}