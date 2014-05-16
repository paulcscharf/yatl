using OpenTK;
using yatl.Rendering;
using amulware.Graphics;

namespace yatl.Environment
{
    abstract class Unit : GameObject, IPositionable
    {
        protected Vector2 position;
        public Vector2 Position { get { return this.position; } }
        protected Vector2 velocity;
        public Vector2 Velocity { get { return this.velocity; } }

        private readonly float frictionCoefficient;

        public Unit(GameState game, Vector2 position, float frictionCoefficient = 10)
            : base(game)
        {
            this.frictionCoefficient = frictionCoefficient;
            this.position = position;
        }

        public override void Update(GameUpdateEventArgs e)
        {
            this.position += this.velocity * e.ElapsedTimeF;

            float slowDownFactor = 1 - this.frictionCoefficient * e.ElapsedTimeF;
            this.velocity *= slowDownFactor < 0 ? 0 : slowDownFactor;
        }

        public override void Draw(SpriteManager sprites)
        {
            sprites.Bloob.Color = Color.DeepPink;
            sprites.Bloob.DrawSprite(this.position, 0, 1);
        }
    }

    abstract class GameObject
    {
        protected readonly GameState game;

        public GameObject(GameState game)
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
