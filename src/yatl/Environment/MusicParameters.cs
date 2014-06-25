namespace yatl.Environment
{
    sealed class MusicParameters
    {
        public readonly static MusicParameters Default =
            new MusicParameters(1f, 0f, 1f, GameState.GameOverState.Undetermined);

        public float Lightness { get; private set; }
        public float Tension { get; private set; }
        public float Health { get; private set; }
        public GameState.GameOverState GameOverState { get; private set; }

        public MusicParameters(float lightness, float tension, float health, GameState.GameOverState gameOverState)
        {
            this.Lightness = lightness;
            this.Tension = tension;
            this.Health = health;
            this.GameOverState = gameOverState;
        }
    }
}
