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
        protected double sampleFrequency;

        public Instrument(double sampleFrequency)
        {
            this.sampleFrequency = sampleFrequency;
        }

        public abstract Sound CreateSound(double volume, double frequency);
    }

    class SimpleInstrument : Instrument
    {
        SoundFile sample;

        public SimpleInstrument(string soundfile, double sampleFrequency)
            : base(sampleFrequency)
        {
            this.sample = SoundFile.FromOgg(soundfile);
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            return new SimpleSound(this.sample, this.sampleFrequency, volume, frequency);
        }
    }

    class ASRInstrument : Instrument
    {
        SoundFile attack, sustain, release;

        public ASRInstrument(string attack, string sustain, string release, double sampleFrequency)
            : base(sampleFrequency)
        {
            this.attack = SoundFile.FromOgg(attack);
            this.sustain = SoundFile.FromOgg(sustain);
            this.release = SoundFile.FromOgg(release);
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            return new ASRSound(this.attack, this.sustain, this.release, this.sampleFrequency, volume, frequency);
        }
    }
}
