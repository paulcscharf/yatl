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
    abstract class Playable
    {
        public abstract int Duration
        {
            get;
        }

        public abstract void Play();
    }

    /// <summary>
    /// Atomic MusicObject
    /// </summary>
    class Note : Playable
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

        public override void Play()
        {
        }

        public override string ToString()
        {
            return this.Duration.ToString() + " " + this.pitch.ToString();
        }
    }

    /// <summary>
    /// Set of MusicObjects that sound simultaneously
    /// </summary>
    class Parallel : Playable
    {
        // NOTE: All MusicObjects should have the same duration
        // Throw Exception if not
        Playable[] content;
        int durationMultiplier;
        int innerDuration;

        public override int Duration
        {
            get
            {
                return this.innerDuration * this.durationMultiplier;
            }
        }

        public Parallel(Playable[] content, int durationMultiplier = 1)
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

        public override void Play()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.durationMultiplier.ToString() + "{" + string.Join(",", this.content.Select(obj => obj.ToString())) + "}";
        }
    }

/// <summary>
/// Set of MusicObjects that sound subsequently
/// </summary>
    class Serial : Playable
    {
        Playable[] content;
        public override int Duration
        {
            get
            {
                return this.content.Sum(o => o.Duration);
            }
        }

        public Serial(Playable[] content)
        {
            this.content = content;
        }

        public override void Play()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Join(" ", this.content.Select(obj => obj.ToString()));
        }
    }
}
