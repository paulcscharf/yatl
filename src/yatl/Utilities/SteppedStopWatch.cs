using System;
using System.Diagnostics;

namespace yatl.Utilities
{
    class SteppedStopwatch
    {
        private readonly Stopwatch stopwatch;

        private TimeSpan totalTime = new TimeSpan(0);

        public bool Mute { get; set; }

        private SteppedStopwatch()
        {
            this.stopwatch = Stopwatch.StartNew();
        }

        public static SteppedStopwatch StartNew()
        {
            return new SteppedStopwatch();
        }

        public void WriteStepToConsole(string message)
        {
            var time = this.stopwatch.Elapsed;
            this.writeTime(time, message);
            this.totalTime += time;
            this.stopwatch.Restart();
        }

        public void WriteTotalToConsole(string message)
        {
            this.totalTime += this.stopwatch.Elapsed;
            this.writeTime(this.totalTime, message);
            this.stopwatch.Restart();
        }

        private void writeTime(TimeSpan time, string message)
        {
            if (this.Mute)
                return;

            Console.WriteLine(message, time.TotalSeconds >= 1
                ? (time.TotalSeconds.ToString("#.##") + "s")
                : (time.TotalMilliseconds.ToString("#.##") + "ms")
                );
        }
    }
}
