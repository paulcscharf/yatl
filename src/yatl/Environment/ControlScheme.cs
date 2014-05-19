using OpenTK.Input;
using yatl.Input;

namespace yatl.Environment
{
    sealed class ControlScheme
    {
    	public IAction Up { get; private set; }
		public IAction Down { get; private set; }
		public IAction Left { get; private set; }
		public IAction Right { get; private set; }
		public IAction Action { get; private set; }

        public ControlScheme()
        {
            this.Up = ControlScheme.keyboardOrGamepadAction(Key.Up, Key.W, "-y");
            this.Down = ControlScheme.keyboardOrGamepadAction(Key.Down, Key.S, "+y");
            this.Left = ControlScheme.keyboardOrGamepadAction(Key.Left, Key.A, "-x");
            this.Right = ControlScheme.keyboardOrGamepadAction(Key.Right, Key.D, "+x");

            this.Action = ControlScheme.keyboardOrGamepadAction(Key.Space, "a");
        }

        private static IAction keyboardOrGamepadAction(Key key, Key key2, string gamepadControl)
        {
            return ControlScheme.keyboardOrGamepadAction(key, gamepadControl)
                .Or(KeyboardKeyAction.FromKey(key2));
        }

        private static IAction keyboardOrGamepadAction(Key key, string gamepadControl)
        {
            return KeyboardKeyAction.FromKey(key)
                .Or(GamePadAction.FromString("gamepad0:" + gamepadControl));
        }
    }
}
