using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace yatl
{
    class ValueException : Exception
    {
        public ValueException(string message) : base(message) { }
    }

    class SoundEvent
    {
        public double StartTime;
        public Note Note;

        public SoundEvent(double startTime, Note note)
        {
            this.Note = note;
            this.StartTime = startTime;
        }

        public void AddOffset(double offsetTime)
        {
            this.StartTime += offsetTime;
        }
    }

    abstract class Audible
    {
        public abstract double Duration
        {
            get;
        }

        public abstract IEnumerable<SoundEvent> Render();
    }

    /// <summary>
    /// Atomic MusicObject
    /// </summary>
    class Note : Audible
    {
        Pitch pitch;
        double duration;

        public override double Duration { get { return this.duration; } }
        public double Frequency { get { return this.pitch.Frequency; } }

        public Note(int duration, Pitch pitch)
        {
            this.duration = duration;
            this.pitch = pitch;
        }

        public override IEnumerable<SoundEvent> Render()
        {
            throw new NotImplementedException();
            //return new SoundEvent[] { new SoundEvent(0, this) };
        }

        public override string ToString()
        {
            return this.Duration.ToString() + " " + this.pitch.ToString();
        }
    }

    /// <summary>
    /// Set of MusicObjects that sound subsequently
    /// </summary>
    class Serial : Audible
    {
        Audible[] content;
        public override double Duration { get { return this.content.Sum(o => o.Duration); } }

        public Serial(Audible[] content)
        {
            this.content = content;
        }

        public override IEnumerable<SoundEvent> Render()
        {
            double time = 0;

            return this.content.Select(o => {
                time += o.Duration;
                return new SoundEvent(time, (Note)o);
            });
        }

        public override string ToString()
        {
            return string.Join(" ", this.content.Select(obj => obj.ToString()));
        }
    }

    /// <summary>
    /// Set of MusicObjects that sound simultaneously
    /// </summary>
    class Parallel : Audible
    {
        // NOTE: All MusicObjects should have the same duration
        // Throw Exception if not
        Audible[] content;
        double durationMultiplier;
        double innerDuration;

        public override double Duration
        {
            get
            {
                return this.innerDuration * this.durationMultiplier;
            }
        }

        public Parallel(Audible[] content, int durationMultiplier = 1)
        {
            if (content.Length == 0)
                throw new ValueException("No empty content allowed.");

            double duration = content[0].Duration;
            Console.WriteLine(content[0].ToString());
            Console.WriteLine(duration.ToString());
            bool allSameDuration = content.All(o => o.Duration == duration);
            if (!allSameDuration)
                throw new ValueException("Not every musicobject has the same duration.");

            this.durationMultiplier = durationMultiplier;
            this.innerDuration = duration;
            this.content = content;
        }

        public override IEnumerable<SoundEvent> Render()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.durationMultiplier.ToString() + "{" + string.Join(",", this.content.Select(obj => obj.ToString())) + "}";
        }
    }
}
