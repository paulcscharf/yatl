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
            this.sample = new SoundFile(soundfile);
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            return new SimpleSound(this.sample, this.sampleFrequency, volume, frequency);
        }
    }

    class ASRInstrument : Instrument
    {
        SoundFile attack, sustain, release;

        public ASRInstrument(SoundFile attack, SoundFile sustain, SoundFile release, double sampleFrequency)
            : base(sampleFrequency)
        {
            this.attack = attack;
            this.sustain = sustain;
            this.release = release;
        }

        public override Sound CreateSound(double volume, double frequency)
        {
            throw new NotImplementedException();
        }
    }
}
