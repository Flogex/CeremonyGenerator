using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flogex.CeremonyGenerator.Tests
{
    internal static class SourceGeneratorContextExtensions
    {
        public static SourceGeneratorContext CreateInstance(TypeDeclarationsReceiver syntaxReceiver)
        {
            var args = new object[]
            {
                CSharpCompilation.Create(Assembly.GetExecutingAssembly().FullName),
                ImmutableArray.Create<AdditionalText>(),
                syntaxReceiver,
                null,
                CancellationToken.None
            };

            var instance = Activator.CreateInstance(
                typeof(SourceGeneratorContext),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                args,
                CultureInfo.CurrentCulture);

            return (SourceGeneratorContext)instance;
        }

        public static ImmutableArray<GeneratedSourceText> GetAddedSources(
            this SourceGeneratorContext sourceGeneratorContext)
        {
            var additionalSources = sourceGeneratorContext
                .GetType()
                .GetProperty("AdditionalSources", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sourceGeneratorContext);

            return (ImmutableArray<GeneratedSourceText>)additionalSources
                .GetType()
                .GetMethod("ToImmutableAndFree", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(additionalSources, null);
        }
    }
}
