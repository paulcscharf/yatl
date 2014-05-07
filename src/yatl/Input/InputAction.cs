
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using yatl.Utilities;

namespace yatl.Input
{
    static class InputAction
    {
        public static IAction FromString(string value)
        {
            return value.ToLowerInvariant().Trim() == "unbound"
                ? InputAction.Unbound
                : KeyboardKeyAction.FromString(value) ?? GamePadAction.FromString(value);
        }

        public static IEnumerable<IAction> GetAllAvailable()
        {
            return KeyboardKeyAction.GetAll().Concat(InputManager.GamePads.SelectMany(GamePadAction.GetAll));
        }

        public static bool IsSameAs(this IAction me, IAction other)
        {
            return me == other || me.ToString() == other.ToString();
        }

        private static readonly IAction unbound = new DummyAction("unbound", "unbound");
        public static IAction Unbound { get { return InputAction.unbound; } }

        public static IAction AnyOf(params IAction[] actions)
        {
            return InputAction.AnyOf((IEnumerable<IAction>)actions);
        }

        public static IAction AnyOf(IEnumerable<IAction> actions)
        {
            var actionList = new List<IAction>();
            foreach (var action in actions)
            {
                if (action == null)
                    continue;

                var asOr = action as OrAction;
                if (asOr != null)
                {
                    actionList.Add(asOr.Child1);
                    actionList.Add(asOr.Child2);
                    continue;
                }
                var asAny = action as AnyAction;
                if (asAny != null)
                {
                    actionList.AddRange(asAny.Actions);
                    continue;
                }
                actionList.Add(action);
            }

            if (actionList.Count == 1)
                return actionList[0];

            if (actionList.Count == 2)
                return new OrAction(actionList[0], actionList[1]);

            return new AnyAction(actionList);
        }

        public static IAction Or(this IAction me, IEnumerable<IAction> others)
        {
            if (me == null)
                throw new ArgumentNullException("me");
            if (others == null)
                throw new ArgumentNullException("others");

            return InputAction.AnyOf(others.Append(me));
        }

        public static IAction Or(this IAction me, params IAction[] others)
        {
            return me.Or((IEnumerable<IAction>)others);
        }


        private abstract class BinaryAction : IAction
        {
            protected readonly IAction child1;
            protected readonly IAction child2;

            public IAction Child1 { get { return this.child1; } }
            public IAction Child2 { get { return this.child2; } }

            public BinaryAction(IAction child1, IAction child2)
            {
                this.child1 = child1;
                this.child2 = child2;
            }

            protected abstract bool boolOp(bool one, bool other);
            protected abstract float floatOp(float one, float other);


            public bool Hit
            {
                get { return this.boolOp(this.child1.Hit, this.child2.Hit); }
            }

            public bool Active
            {
                get { return this.boolOp(this.child1.Active, this.child2.Active); }
            }

            public bool Released
            {
                get { return this.boolOp(this.child1.Released, this.child2.Released); }
            }

            public bool IsAnalog
            {
                get { return this.child1.IsAnalog || this.child2.IsAnalog; }
            }

            public float AnalogAmount
            {
                get { return this.floatOp(this.child1.AnalogAmount, this.child2.AnalogAmount); }
            }

            public abstract string ToUIString();
        }

        private class OrAction : BinaryAction
        {
            public OrAction(IAction child1, IAction child2)
                : base(child1, child2)
            {
            }

            protected override bool boolOp(bool one, bool other)
            {
                return one || other;
            }

            protected override float floatOp(float one, float other)
            {
                return Math.Max(one, other);
            }

            public override string ToUIString()
            {
                return this.child1.ToUIString() + "||" + this.child2.ToUIString();
            }
        }

        private class AnyAction : IAction
        {
            private readonly ReadOnlyCollection<IAction> actions;

            public IEnumerable<IAction> Actions { get { return this.actions; } } 

            public AnyAction(IEnumerable<IAction> actions)
            {
                this.actions = actions.ToList().AsReadOnly();
            }

            public AnyAction(params IAction[] actions)
            {
                this.actions = actions.ToList().AsReadOnly();
            }

            public bool Hit
            {
                get { return this.actions.Any(a => a.Hit); }
            }

            public bool Active
            {
                get { return this.actions.Any(a => a.Active); }
            }

            public bool Released
            {
                get { return this.actions.Any(a => a.Released); }
            }

            public bool IsAnalog
            {
                get { return this.actions.Any(a => a.IsAnalog); }
            }

            public float AnalogAmount
            {
                get { return this.actions.Max(a => a.AnalogAmount); }
            }

            public string ToUIString()
            {
                return string.Join("||", this.actions.Select(a => a.ToUIString()));
            }
        }
    }
}
