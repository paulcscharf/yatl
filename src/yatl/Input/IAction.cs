namespace yatl.Input
{
    interface IAction
    {
        bool Hit { get; }
        bool Active { get; }
        bool Released { get; }

        bool IsAnalog { get; }
        float AnalogAmount { get; }

        string ToUIString();
    }
}
