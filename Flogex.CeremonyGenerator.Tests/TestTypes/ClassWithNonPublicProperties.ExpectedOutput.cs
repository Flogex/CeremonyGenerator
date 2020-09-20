using System;

namespace Flogex.CeremonyGenerator.Tests.Samples
{
    internal partial class TestClass : IEquatable<TestClass>
    {
        public bool Equals(TestClass other) => other is object && this.C.Equals(other.C);
        public override bool Equals(object obj) => Equals(obj as TestClass);
        public override int GetHashCode() => HashCode.Combine(this.C);
    }
}