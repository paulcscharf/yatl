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
            this.source.Volume = (float) volume; // No jitter
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
            if (!this.source.Disposed)
                if (!this.source.FinishedPlaying)
                    source.Stop();
        }
    }

    class SRSound : Sound
    {
        Source sustain, release;

        public SRSound(SoundFile sustain, SoundFile release, double sampleFrequency, double volume, double frequency)
            : base(sampleFrequency, volume, frequency)
        {
            this.sustain = sustain.GenerateSource();
            this.release = release.GenerateSource();

            foreach (var source in new Source[] { this.sustain, this.release }) {
                //source.Volume = (float)(volume * 0.4); // Fix jitter
                source.Volume = (float)volume; // No jitter with wav files
                source.Pitch = (float)(frequency / this.sampleFrequency);
            }
        }

        public override void Play()
        {
            this.sustain.Repeating = true;
            this.sustain.Play();
        }

        public override void Stop()
        {
            this.release.Play();
            this.sustain.Stop();
        }
    }
}
