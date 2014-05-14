
using OpenTK;
using yatl.Rendering;

namespace yatl.Environment
{
    sealed class GameState
    {
        public GameState()
        {
            
        }

        public void Draw(SpriteManager sprites)
        {
            sprites.ScreenText.Height = 3;
            sprites.ScreenText.DrawString(new Vector2(0, 9), "Hi! :)", 0.5f, 0.5f);
        }
    }
}
