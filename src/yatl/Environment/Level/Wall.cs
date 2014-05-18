using OpenTK;

namespace yatl.Environment.Level
{
    class Wall
    {
        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }

        public Wall(Vector2 startPoint, Vector2 endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

    }
}
