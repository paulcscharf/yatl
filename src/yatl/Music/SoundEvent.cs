using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cireon.Audio;

namespace yatl
{
    /// <summary>
    /// Abstract class for all kinds of sound events
    /// </summary>
    abstract class SoundEvent
    {
        public double StartTime;

        public SoundEvent(double startTime)
        {
            this.StartTime = startTime;
        }

        public void AddOffset(double offsetTime)
        {
            this.StartTime += offsetTime;
        }

        public void MultiplyOffset(double multiplier)
        {
            this.StartTime *= multiplier;
        }

        public abstract void Execute();
    }

    class NoteOn : SoundEvent
    {
        public Note Note;
        public Instrument Instrument;
        public Sound Sound = null;
        public double Volume;

        public NoteOn(double startTime, Note note, Instrument instrument, double volume)
            : base(startTime)
        {
            this.Note = note;
            this.Volume = volume;
            this.Instrument = instrument;
        }

        public override void Execute()
        {
            this.Sound = this.Instrument.CreateSound(this.Volume, this.Note.Frequency);
            this.Sound.Play();
        }
    }

    class NoteOff : SoundEvent
    {
        NoteOn noteOn;

        public NoteOff(double startTime, NoteOn noteOn)
            : base(startTime)
        {
            this.noteOn = noteOn;
        }

        public override void Execute()
        {
            this.noteOn.Sound.Stop();
        }
    }
}