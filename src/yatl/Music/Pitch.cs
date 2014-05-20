using System;
using System.Collections.Generic;

namespace yatl
{
    class Pitch
    {
        public readonly string Name;
        public readonly double Frequency;
        static Dictionary<string, double> nameFrequencyTable = new Dictionary<string, double>
        {
            {"c4", 261.63},
            {"cis4", 277.18},
        };

        public Pitch(string name, double frequency)
        {
            this.Name = name;
            this.Frequency = frequency;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static Pitch FromString(string name)
        {
            return new Pitch(name, nameFrequencyTable[name]);
        }
    }
}
