using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cireon.Audio;

namespace yatl
{
    /// <summary>
    /// Abstraction for SoundFile
    /// </summary>
    abstract class Instrument
    {
        public abstract Sound CreateSound(double volume, double frequency);
    }

    class Piano : Instrument
    {
        SoundFile[] samples = new SoundFile[6];
        double[] baseFrequencies = new double[6];

        public Piano()
        {
            this.samples[0] = SoundFile.FromOgg("data/music/piano/Piano.mf.Gb1.ogg");
            this.samples[1] = SoundFile.FromOgg("data/music/piano/Piano.mf.Gb2.ogg");
            this.samples[2] = SoundFile.FromOgg("data/music/piano/Piano.mf.Gb3.ogg");
            this.samples[3] = SoundFile.FromOgg("data/music/piano/Piano.mf.Gb4.ogg");
            this.samples[4] = SoundFile.FromOgg("data/music/piano/Piano.mf.Gb5.ogg");
            this.samples[5] = SoundFile.FromOgg("data/music/piano/Piano.mf.Gb6.ogg");
            this.baseFrequencies[0] = Pitch.NameFrequencyTable["fis1"];
            this.baseFrequencies[1] = Pitch.NameFrequencyTable["fis2"];
            this.baseFrequencies[2] = Pitch.NameFrequencyTable["fis3"];
            this.baseFrequencies[3] = Pitch.NameFrequencyTable["fis4"];
            this.baseFrequencies[4] = Pitch.NameFrequencyTable["fis5"];
            this.baseFrequencies[5] = Pitch.NameFrequencyTable["fis6"];
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            int octave = (int)Math.Round(Math.Log(frequency / this.baseFrequencies[0], 2)) + 1;
            return new SimpleSound(this.samples[octave - 1], this.baseFrequencies[octave - 1], volume, frequency);
        }
    }

    class SimpleInstrument : Instrument
    {
        SoundFile sample;
        double sampleFrequency;

        public SimpleInstrument(string soundfile, double sampleFrequency)
        {
            this.sampleFrequency = sampleFrequency;
            this.sample = SoundFile.FromOgg(soundfile);
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            return new SimpleSound(this.sample, this.sampleFrequency, volume, frequency);
        }
    }

    class SRInstrument : Instrument
    {
        SoundFile sustain, release;
        double sampleFrequency;

        public SRInstrument(string sustain, string release, double sampleFrequency)
        {
            this.sampleFrequency = sampleFrequency;
            this.sustain = SoundFile.FromOgg(sustain);
            this.release = SoundFile.FromOgg(release);
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            return new SRSound(this.sustain, this.release, this.sampleFrequency, volume, frequency);
        }
    }
}
