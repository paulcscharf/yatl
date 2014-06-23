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
        SoundFile[] samples = new SoundFile[7];
        double[] baseFrequencies = new double[7];

        public Piano()
        {
            this.samples[0] = SoundFile.FromWav("data/music/piano/Piano.mf.Gb1.wav");
            this.samples[1] = SoundFile.FromWav("data/music/piano/Piano.mf.Gb2.wav");
            this.samples[2] = SoundFile.FromWav("data/music/piano/Piano.mf.Gb3.wav");
            this.samples[3] = SoundFile.FromWav("data/music/piano/Piano.mf.Gb4.wav");
            this.samples[4] = SoundFile.FromWav("data/music/piano/Piano.mf.Gb5.wav");
            this.samples[5] = SoundFile.FromWav("data/music/piano/Piano.mf.Gb6.wav");
            this.samples[6] = SoundFile.FromWav("data/music/piano/Piano.mf.Gb7.wav");
            this.baseFrequencies[0] = Pitch.NameFrequencyTable["fis1"];
            this.baseFrequencies[1] = Pitch.NameFrequencyTable["fis2"];
            this.baseFrequencies[2] = Pitch.NameFrequencyTable["fis3"];
            this.baseFrequencies[3] = Pitch.NameFrequencyTable["fis4"];
            this.baseFrequencies[4] = Pitch.NameFrequencyTable["fis5"];
            this.baseFrequencies[5] = Pitch.NameFrequencyTable["fis6"];
            this.baseFrequencies[6] = Pitch.NameFrequencyTable["fis7"];
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            if (frequency > 0) {
                int octave = (int)Math.Round(Math.Log(frequency / this.baseFrequencies[0], 2)) + 1;
                return new SimpleSound(this.samples[octave - 1], this.baseFrequencies[octave - 1], volume, frequency);
            }
            else
                return new SimpleSound(this.samples[0], this.baseFrequencies[0], volume, frequency);
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
