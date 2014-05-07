using System;
using System.Collections.Generic;
using System.Linq;

namespace yatl.Input
{
    sealed class LambdaAction : IAction
    {
        private readonly Func<IAction> action;

        public LambdaAction(Func<IAction> actionSelector)
        {
            this.action = actionSelector;
        }

        public static LambdaAction From(Func<IAction> actionSelector)
        {
            return new LambdaAction(actionSelector);
        }

        public static IAction Any(IEnumerable<Func<IAction>> actionSelectors)
        {
            return InputAction.AnyOf(actionSelectors.Select(LambdaAction.From));
        }

        public static IAction Any(params Func<IAction>[] actionSelectors)
        {
            return LambdaAction.Any((IEnumerable<Func<IAction>>)actionSelectors);
        }

        public bool Hit { get { return this.action().Hit; } }

        public bool Active { get { return this.action().Active; } }

        public bool Released { get { return this.action().Released; } }

        public bool IsAnalog { get { return this.action().IsAnalog; } }

        public float AnalogAmount { get { return this.action().AnalogAmount; } }

        public string ToUIString()  { return this.action().ToUIString(); }
    }
}
