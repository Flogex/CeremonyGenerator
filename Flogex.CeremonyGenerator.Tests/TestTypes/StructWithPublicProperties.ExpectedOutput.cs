using System;

namespace Flogex.CeremonyGenerator.Tests
{
    partial struct TestStruct : IEquatable<TestStruct>
    {
        public bool Equals(TestStruct other) => this.PropertyA.Equals(other.PropertyA) && this.PropertyB.Equals(other.PropertyB) && this.PropertyC.Equals(other.PropertyC);
        public override bool Equals(object obj) => obj is TestStruct @struct && Equals(@struct);
        public override int GetHashCode() => HashCode.Combine(this.PropertyA, this.PropertyB, this.PropertyC);
    }
}