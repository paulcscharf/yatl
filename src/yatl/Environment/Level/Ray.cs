using OpenTK;

namespace yatl.Environment.Level
{
    struct Ray
    {
        public readonly Vector2 Start;
        public readonly Vector2 Direction;

        public Ray(Vector2 start, Vector2 direction)
        {
            this.Start = start;
            this.Direction = direction;
        }
    }
}
