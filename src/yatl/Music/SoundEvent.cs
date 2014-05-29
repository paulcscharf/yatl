using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cireon.Audio;

namespace yatl
{
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

        public abstract void Execute(MusicManager manager);
    }

    class NoteOn : SoundEvent
    {
        public Note Note;
        public Source Source = null;

        public NoteOn(double startTime, Note note) : base(startTime)
        {
            this.Note = note;
        }

        public override void Execute(MusicManager manager)
        {
            this.Source = manager.PianoSound.GenerateSource();
            this.Source.Pitch = (float) (this.Note.Frequency / 130.8);
            this.Source.Play();
        }
    }

    class NoteOff : SoundEvent
    {
        NoteOn noteOn;

        public NoteOff(double startTime, NoteOn noteOn) : base(startTime)
        {
            this.noteOn = noteOn;
        }

        public override void Execute(MusicManager manager)
        {
            if (this.noteOn.Source == null)
                throw new Exception("NoteOn must be executed before NoteOff.");
            this.noteOn.Source.Stop();
        }
    }
}
