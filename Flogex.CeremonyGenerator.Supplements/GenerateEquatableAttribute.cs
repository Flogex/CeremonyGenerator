using System;
using static System.AttributeTargets;

namespace Flogex.CeremonyGenerator
{
    [AttributeUsage(Class | Struct, AllowMultiple = false, Inherited = false)]
    public class GenerateEquatableAttribute : Attribute
    {
        internal const string Name = "GenerateEquatable";
    }
}
