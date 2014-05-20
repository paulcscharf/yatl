using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace yatl
{
    abstract class MusicObject
    {
    }

    /// <summary>
    /// Atomic MusicObject
    /// </summary>
    class Note : MusicObject
    {
        int duration;
        Pitch pitch;

        public Note(int duration, Pitch pitch)
        {
            this.duration = duration;
            this.pitch = pitch;
        }

        public override string ToString()
        {
            return this.duration.ToString() + " " + this.pitch.ToString();
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
    }

    /// <summary>
    /// Set of MusicObjects that sound subsequently
    /// </summary>
    class Serial : MusicObject
    {
        MusicObject[] content;

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
