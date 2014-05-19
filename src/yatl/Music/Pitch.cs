using System;
using System.Collections.Generic;

namespace yatl
{
    class Pitch
    {
        public readonly string Name;
        public readonly double Frequency;
        static Dictionary<string, Pitch> byName = new Dictionary<string, Pitch>
        {
            {"c4", new Pitch("c4", 261.63)},
            {"cis4", new Pitch("cis4", 277.18)},
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
            return byName[name];
        }
    }
}
