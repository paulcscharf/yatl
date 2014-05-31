using System;
using System.Collections.Generic;
using System.Linq;
using Cireon.Audio;

namespace yatl
{
    /// <summary>
    /// Abstraction for Source
    /// </summary>
    abstract class Sound
    {
        protected double sampleFrequency, volume, frequency;

        public Sound(double sampleFrequency, double volume, double frequency)
        {
            this.sampleFrequency = sampleFrequency;
            this.volume = volume;
            this.frequency = frequency;
        }

        public abstract void Play();
        public abstract void Stop();
    }

    class SimpleSound : Sound
    {
        Source source;

        public SimpleSound(SoundFile sample, double sampleFrequency, double volume, double frequency)
            : base(sampleFrequency, volume, frequency)
        {
            this.source = sample.GenerateSource();
            this.source.Volume = (float) (volume * 0.4); // Fix jitter
            this.source.Pitch = (float)(frequency / this.sampleFrequency);
        }

        public override void Play()
        {
            this.source.Play();
        }

        public override void Stop()
        {
            if (this.source == null)
                throw new Exception("Can't stop sound that has not started.");
            if (!this.source.FinishedPlaying && !this.source.Disposed)
                source.Stop();
        }
    }

    class ASRSound : Sound
    {
        Source attack, sustain, decay;

        public ASRSound(SoundFile attackSample, double sampleFrequency, double volume, double frequency)
            : base(sampleFrequency, volume, frequency)
        {
            this.attack = attackSample.GenerateSource();
            this.attack.Volume = (float) (volume * 0.4); // Fix jitter
            this.attack.Pitch = (float)(frequency / this.sampleFrequency);
        }

        public override void Play()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
