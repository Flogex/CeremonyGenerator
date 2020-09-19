using System.Text;
using Flogex.CeremonyGenerator.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Flogex.CeremonyGenerator.DiagnosticDescriptors;

namespace Flogex.CeremonyGenerator
{
    /// <summary>
    /// Generates the IEquatable implementation for classes or structs
    /// marked with <see cref="GenerateEquatableAttribute"/>.
    /// </summary>
    [Generator]
    public class EquatableGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new TypeDeclarationsReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (TypeDeclarationsReceiver)context.SyntaxReceiver!;

            foreach (var @class in receiver.ReceivedClasses)
            {
                if (@class.IsPartial() == false)
                    context.ReportDiagnostic(ClassMustBePartial(@class));

                MemberDeclarationSyntax? partialClass = EquatableClassSyntax.Create(@class);

                if (partialClass is null)
                {
                    context.ReportDiagnostic(ClassMustHavePublicProperties(@class));
                    continue;
                }

                var @namespace = @class.GetContainingNamespace();

                var compilationUnitMember = @namespace != null
                    ? NamespaceDeclaration(@namespace.Name).WithSingleMember(partialClass)
                    : partialClass;

                var compilationUnit = CompilationUnit()
                    .WithSingleUsing(UsingDirective(IdentifierName("System"))) // Needed for IEquatable and HashCode
                    .WithSingleMember(compilationUnitMember);

                var source = SourceText.From(compilationUnit.NormalizeWhitespace().ToFullString(), Encoding.UTF8);
                context.AddSource(@class.Identifier.ToString() + ".EquatableGenerated.cs", source);
            }

            //TODO Process structs

            // GetGeneratedFileName(string path) => $"{Path.GetFileNameWithoutExtension(path.Replace('\\', Path.DirectorySeparatorChar))}.generated";
        }
    }
}