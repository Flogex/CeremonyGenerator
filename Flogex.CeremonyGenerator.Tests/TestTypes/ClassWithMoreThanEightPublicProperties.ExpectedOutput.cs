using System;

namespace Flogex.CeremonyGenerator.Tests.Samples
{
    internal partial class TestClass : IEquatable<TestClass>
    {
        public bool Equals(TestClass other) => other is object && this.PropertyA.Equals(other.PropertyA) && this.PropertyB.Equals(other.PropertyB) && this.PropertyC.Equals(other.PropertyC) && this.PropertyD.Equals(other.PropertyD) && this.PropertyE.Equals(other.PropertyE) && this.PropertyF.Equals(other.PropertyF) && this.PropertyG.Equals(other.PropertyG) && this.PropertyH.Equals(other.PropertyH) && this.PropertyI.Equals(other.PropertyI);
        public override bool Equals(object obj) => Equals(obj as TestClass);
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.PropertyA);
            hash.Add(this.PropertyB);
            hash.Add(this.PropertyC);
            hash.Add(this.PropertyD);
            hash.Add(this.PropertyE);
            hash.Add(this.PropertyF);
            hash.Add(this.PropertyG);
            hash.Add(this.PropertyH);
            hash.Add(this.PropertyI);
            return hash.ToHashCode();
        }
    }
}