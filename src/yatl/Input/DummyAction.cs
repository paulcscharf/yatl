namespace yatl.Input
{
    sealed class DummyAction : IAction
    {
        private readonly string name;
        private readonly string uiString;

        public DummyAction(string name, string uiString = null)
        {
            this.name = name;
            this.uiString = uiString ?? name;
        }

        public bool Hit
        {
            get { return false; }
        }

        public bool Active
        {
            get { return false; }
        }

        public bool Released
        {
            get { return false; }
        }

        public bool IsAnalog
        {
            get { return false; }
        }

        public float AnalogAmount
        {
            get { return 0; }
        }

        public string ToUIString()
        {
            return this.uiString;
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
