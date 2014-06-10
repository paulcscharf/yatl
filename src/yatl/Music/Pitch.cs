using System;
using System.Collections.Generic;
// C       C#      D       Eb      E       F       F#      G       G#      A       Bb      B
// 16.35   17.32   18.35   19.45   20.60   21.83   23.12   24.50   25.96   27.50   29.14   30.87
// 32.70   34.65   36.71   38.89   41.20   43.65   46.25   49.00   51.91   55.00   58.27   61.74
// 65.41   69.30   73.42   77.78   82.41   87.31   92.50   98.00   103.8   110.0   116.5   123.5
// 130.8   138.6   146.8   155.6   164.8   174.6   185.0   196.0   207.7   220.0   233.1   246.9
// 261.6   277.2   293.7   311.1   329.6   349.2   370.0   392.0   415.3   440.0   466.2   493.9
// 523.3   554.4   587.3   622.3   659.3   698.5   740.0   784.0   830.6   880.0   932.3   987.8
// 1047    1109    1175    1245    1319    1397    1480    1568    1661    1760    1865    1976
// 2093    2217    2349    2489    2637    2794    2960    3136    3322    3520    3729    3951
// 4186    4435    4699    4978    5274    5588    5920    6272    6645    7040    7459    7902

namespace yatl
{
    class Pitch : IComparable
    {
        public readonly string Name;
        public readonly double Frequency;
        public static Dictionary<string, double> NameFrequencyTable = new Dictionary<string, double>
        {
            {"_", 0},

            {"c0", 16.35},
            {"cis0", 17.32},
            {"d0", 18.35},
            {"dis0", 19.45},
            {"e0", 20.60},
            {"f0", 21.83},
            {"fis0", 23.12},
            {"g0", 24.50},
            {"gis0", 25.96},
            {"a0", 27.50},
            {"ais0", 29.14},
            {"b0", 30.87},

            {"c1", 32.70},
            {"cis1", 34.65},
            {"d1", 36.71},
            {"dis1", 38.89},
            {"e1", 41.20},
            {"f1", 43.65},
            {"fis1", 46.25},
            {"g1", 49.00},
            {"gis1", 51.91},
            {"a1", 55.00},
            {"ais1", 58.27},
            {"b1", 61.74},

            {"c2", 65.41},
            {"cis2", 69.30},
            {"d2", 73.42},
            {"dis2", 77.78},
            {"e2", 82.41},
            {"f2", 87.31},
            {"fis2", 92.50},
            {"g2", 98.00},
            {"gis2", 103.8},
            {"a2", 110.0},
            {"ais2", 116.5},
            {"b2", 123.5},

            {"c3", 130.8},
            {"cis3", 138.6},
            {"d3", 146.8},
            {"dis3", 155.6},
            {"e3", 164.8},
            {"f3", 174.6},
            {"fis3", 185.0},
            {"g3", 196.0},
            {"gis3", 207.7},
            {"a3", 220.0},
            {"ais3", 233.1},
            {"b3", 246.9},

            {"c4", 261.6},
            {"cis4", 277.2},
            {"d4", 293.7},
            {"dis4", 311.1},
            {"e4", 329.6},
            {"f4", 349.2},
            {"fis4", 370.0},
            {"g4", 392.0},
            {"gis4", 415.3},
            {"a4", 440.0},
            {"ais4", 466.2},
            {"b4", 493.9},

            {"c5", 523.3},
            {"cis5", 554.4},
            {"d5", 587.3},
            {"dis5", 622.3},
            {"e5", 659.3},
            {"f5", 698.5},
            {"fis5", 740.0},
            {"g5", 784.0},
            {"gis5", 830.6},
            {"a5", 880.0},
            {"ais5", 932.3},
            {"b5", 987.8},

            {"fis6", 1480.0},
        };

        public Pitch(string name, double frequency)
        {
            this.Name = name;
            this.Frequency = frequency;
        }

        public Pitch NextOctave()
        {
            return new Pitch("exact frequency: " + this.Frequency.ToString(), this.Frequency * 2.0);
        }

        public Pitch PreviousOctave()
        {
            return new Pitch("exact frequency: " + this.Frequency.ToString(), this.Frequency / 2.0);
        }

        public int CompareTo(object o)
        {
            return ((Pitch)o).Frequency <= this.Frequency ? 1 : -1;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static Pitch FromString(string name)
        {
            return new Pitch(name, NameFrequencyTable[name]);
        }
    }
}