using System;
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
    }

    /// <summary>
    /// MusicObjects that sound simultaneously
    /// </summary>
    class Parallel : MusicObject
    {
        // NOTE: All MusicObjects should have the same duration
        // Throw Exception if not
        MusicObject[] content;
    }

    /// <summary>
    /// MusicObjects that sound subsequently
    /// </summary>
    class Serial : MusicObject
    {
        MusicObject[] content;
    }
}
