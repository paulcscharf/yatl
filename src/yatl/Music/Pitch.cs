using System;
using System.Collections.Generic;

namespace yatl
{
    class Pitch
    {
        public readonly string Name;
        public readonly double Frequency;

        public Pitch(string name, double frequency)
        {
            this.Name = name;
            this.Frequency = frequency;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    static class Pitches
    {
        public static Pitch C = new Pitch("C", 261.6);
    }
}
