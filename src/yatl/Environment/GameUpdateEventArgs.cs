using amulware.Graphics;

namespace yatl.Environment
{
    sealed class GameUpdateEventArgs
    {
        public readonly double ElapsedTime;
        public readonly float ElapsedTimeF;

        public GameUpdateEventArgs(UpdateEventArgs args, float timeScale)
        {
            this.ElapsedTime = args.ElapsedTimeInS * timeScale;
            this.ElapsedTimeF = (float)(this.ElapsedTime);
        }
    }
}