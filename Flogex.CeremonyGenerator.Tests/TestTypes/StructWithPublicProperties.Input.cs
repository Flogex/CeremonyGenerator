using System;
using System.Collections.Generic;

namespace Flogex.CeremonyGenerator.Tests
{
    [GenerateEquatable]
    partial struct TestStruct
    {
        public int PropertyA { get; set; }

        public string PropertyB { get; set; }

        public object PropertyC { get; set; }
    }
}
