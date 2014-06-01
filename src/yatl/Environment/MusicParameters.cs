namespace yatl.Environment
{
    sealed class MusicParameters
    {
        public float Lightness { get; private set; }
        public float Tension { get; private set; }

        public MusicParameters(float lightness, float tension)
        {
            this.Lightness = lightness;
            this.Tension = tension;
        }
    }
}
