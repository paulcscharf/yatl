
namespace yatl.Input
{
    abstract class DigitalAction : IAction
    {
        public abstract bool Hit { get; }
        public abstract bool Active { get; }
        public abstract bool Released { get; }

        public bool IsAnalog { get { return false; } }
        public float AnalogAmount { get { return this.Active ? 1 : 0; } }

        public abstract string ToUIString();
    }
}
