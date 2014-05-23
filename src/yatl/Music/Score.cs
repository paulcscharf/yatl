using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace yatl
{
    class ValueException : Exception
    {
        public ValueException(string message): base(message)
        {
        }
    }
    abstract class MusicObject
    {
        public abstract int Duration
        {
            get;
        }
    }

    /// <summary>
    /// Atomic MusicObject
    /// </summary>
    class Note : MusicObject
    {
        Pitch pitch;
        int duration;

        public override int Duration
        {
            get
            {
                return this.duration;
            }
        }

        public Note(int duration, Pitch pitch)
        {
            this.duration = duration;
            this.pitch = pitch;
        }

        public override string ToString()
        {
            return this.Duration.ToString() + " " + this.pitch.ToString();
        }
    }

    /// <summary>
    /// Set of MusicObjects that sound simultaneously
    /// </summary>
    class Parallel : MusicObject
    {
        // NOTE: All MusicObjects should have the same duration
        // Throw Exception if not
        MusicObject[] content;
        int durationMultiplier;
        int innerDuration;

        public override int Duration
        {
            get
            {
                return this.innerDuration * this.durationMultiplier;
            }
        }

        public Parallel(MusicObject[] content, int durationMultiplier = 1)
        {
            if (content.Length == 0)
                throw new ValueException("No empty content allowed.");

            int duration = content[0].Duration;
            Console.WriteLine(content[0].ToString());
            Console.WriteLine(duration.ToString());
            bool allSameDuration = content.All(o => o.Duration == duration);
            if (!allSameDuration)
                throw new ValueException("Not every musicobject has the same duration.");

            this.durationMultiplier = durationMultiplier;
            this.innerDuration = duration;
            this.content = content;
        }

        public override string ToString()
        {
            return this.durationMultiplier.ToString() + "{" + string.Join(",", this.content.Select(obj => obj.ToString())) + "}";
        }
    }

/// <summary>
/// Set of MusicObjects that sound subsequently
/// </summary>
    class Serial : MusicObject
    {
        MusicObject[] content;
        public override int Duration
        {
            get
            {
                return this.content.Sum(o => o.Duration);
            }
        }

        public Serial(MusicObject[] content)
        {
            this.content = content;
        }

        public override string ToString()
        {
            return string.Join(" ", this.content.Select(obj => obj.ToString()));
        }
    }
}
