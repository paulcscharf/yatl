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
        public int Duration
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Atomic MusicObject
    /// </summary>
    class Note : MusicObject
    {
        Pitch pitch;

        public Note(int duration, Pitch pitch)
        {
            this.Duration = duration;
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

        public Parallel(MusicObject[] content)
        {
            if (content.Length == 0)
                throw new ValueException("No empty content allowed.");

            int duration = content[0].Duration;
            bool allSameDuration = content.All(o => o.Duration == duration);
            if (!allSameDuration)
                throw new ValueException("Not every musicobject has the same duration.");

            this.Duration = Duration;
            this.content = content;
        }

        public override string ToString()
        {
            return "{" + string.Join(",", this.content.Select(obj => obj.ToString())) + "}";
        }
    }

    /// <summary>
    /// Set of MusicObjects that sound subsequently
    /// </summary>
    class Serial : MusicObject
    {
        MusicObject[] content;
        public int Duration
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
