using System;

namespace Flogex.CeremonyGenerator.Tests.Samples
{
    public partial class TestClass : IEquatable<TestClass>
    {
        public bool Equals(TestClass other) => other is object && this.A.Equals(other.A) && this.B.Equals(other.B) && this.C.Equals(other.C);
        public override bool Equals(object obj) => Equals(obj as TestClass);
        public override int GetHashCode() => HashCode.Combine(this.A, this.B, this.C);
    }
}