namespace yatl.Environment.Tilemap.Hexagon
{
    struct Step
    {
        public readonly sbyte X;
        public readonly sbyte Y;

        public Step(sbyte x, sbyte y)
        {
            this.X = x;
            this.Y = y;
        }

        public Step(Direction direction)
        {
            this = direction.Step();
        }
    }
}