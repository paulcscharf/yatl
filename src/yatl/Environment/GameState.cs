
using amulware.Graphics;
using OpenTK;
using yatl.Rendering;

namespace yatl.Environment
{
    sealed class GameState
    {
        private double time;
        private float timeF;
        public float Time { get { return this.timeF; } }

        public GameState()
        {
            
        }

        public void Update(UpdateEventArgs args)
        {
            var newArgs = new GameUpdateEventArgs(args, 1f);

            // don't use 'args' after this point

            this.time += newArgs.ElapsedTime;
            this.timeF = (float)this.time;

            
        }

        public void Draw(SpriteManager sprites)
        {
            sprites.ScreenText.Height = 3;
            sprites.ScreenText.DrawString(new Vector2(0, 9), "Hi! :)", 0.5f, 0.5f);
        }
    }
}
