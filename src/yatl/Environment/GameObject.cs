using yatl.Rendering;

namespace yatl.Environment
{
    abstract class GameObject
    {
        protected readonly GameState game;

        protected GameObject(GameState game)
        {
            this.game = game;
            game.AddObject(this);
        }

        public abstract void Update(GameUpdateEventArgs e);
        public abstract void Draw(SpriteManager sprites);

        public void Delete()
        {
            this.Deleted = true;
        }

        public virtual void Dispose()
        {

        }

        public bool Deleted { get; private set; }
    }
}
