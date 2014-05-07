using System;
using System.Collections.Generic;
using System.Linq;
using yatl.Utilities;
using OpenTK.Input;

namespace yatl.Input
{
    using ButtonSelector = Func<GamePadState, ButtonState>;
    using AxisSelector = Func<GamePadState, float>;
    using StickSettings = Settings.Input.Gamepad.Sticks;

    static class GamePadAction
    {
        public static IAction FromString(string name)
        {
            var lower = name.ToLowerInvariant().Trim();
            if (!lower.StartsWith("gamepad"))
                return null;
            var split = lower.Substring(7).Split(':');
            if (split.Length != 2)
                throw new ArgumentException("Gamepad button name must have exactly one ':'.", "name");
            int id;
            if (!int.TryParse(split[0].Trim(), out id))
                throw new ArgumentException("Gamepad button name must include gamepad id.", "name");

            var buttonName = split[1].Trim();

            if (id >= InputManager.GamePads.Count)
            {
                // this may not be the best solution
                // but it prevents crashing and settings from being overridden, if a gamepad is not connected
                // TODO: Add ui string so this is displayed properly
                return new DummyAction(name, buttonName);
            }


            var buttonSelector = GamePadAction.GamePadButtonAction.GetButtonSelector(buttonName);

            if (buttonSelector != null)
                return new GamePadButtonAction(id, buttonName, buttonSelector);

            var axisSelector = GamePadAction.GamePadAxisAction.GetAxisSelector(buttonName);

            if(axisSelector != null)
                return new GamePadAxisAction(id, buttonName, axisSelector);

            throw new ArgumentException("Gamepad button name unknown.", "name");
        }

        public static IEnumerable<IAction> GetAll(InputManager.GamePadStateContainer pad)
        {
            return GamePadAction.GetAll(pad.Id);
        }

        public static IEnumerable<IAction> GetAll(int id)
        {
            return GamePadAxisAction.GetAll(id).Cast<IAction>().Concat(GamePadButtonAction.GetAll(id));
        }

        sealed private class GamePadButtonAction : DigitalAction
        {
            private static Dictionary<string, ButtonSelector> buttonSelectors = new Dictionary<string, ButtonSelector>()
            {
                { "a", b => b.Buttons.A },
                { "b", b => b.Buttons.B },
                { "x", b => b.Buttons.X },
                { "y", b => b.Buttons.Y },

                { "start", b => b.Buttons.Start },
                { "back", b => b.Buttons.Back },
                { "bigbutton", b => b.Buttons.BigButton },

                { "leftshoulder", b => b.Buttons.LeftShoulder },
                { "leftstick", b => b.Buttons.LeftStick },
                { "rightshoulder", b => b.Buttons.RightShoulder },
                { "rightstick", b => b.Buttons.RightStick },

                { "dpadright", b => b.DPad.Right },
                { "dpadleft", b => b.DPad.Left },
                { "dpadup", b => b.DPad.Up },
                { "dpaddown", b => b.DPad.Down }
            };

            private readonly InputManager.GamePadStateContainer pad;
            private readonly string buttonName;
            private readonly ButtonSelector buttonSelector;

            public GamePadButtonAction(int id, string buttonName, ButtonSelector buttonSelector)
            {
                this.pad = InputManager.GamePads[id];
                this.buttonName = buttonName;
                this.buttonSelector = buttonSelector;
            }

            public static IEnumerable<GamePadButtonAction> GetAll(int id)
            {
                return GamePadButtonAction.buttonSelectors.Select(
                    pair => new GamePadButtonAction(id, pair.Key, pair.Value)
                    );
            }


            public static ButtonSelector GetButtonSelector(string buttonName)
            {
                ButtonSelector selector;

                GamePadButtonAction.buttonSelectors.TryGetValue(buttonName, out selector);

                return selector;
            }

            private bool downNow
            {
                get { return this.buttonSelector(this.pad.CurrentState) == ButtonState.Pressed; }
            }

            private bool downBefore
            {
                get { return this.buttonSelector(this.pad.PreviousState) == ButtonState.Pressed; }
            }

            public override bool Hit
            {
                get { return this.downNow && !this.downBefore; }
            }

            public override bool Active
            {
                get { return this.downNow; }
            }

            public override bool Released
            {
                get { return this.downBefore && !this.downNow; }
            }

            public override string ToUIString()
            {
                return this.buttonName;
            }

            public override string ToString()
            {
                return "gamepad" + this.pad.Id + ":" + this.buttonName;
            }

        }

        sealed private class GamePadAxisAction : IAction
        {
            private static Dictionary<string, AxisSelector> axisSelectors = new Dictionary<string, AxisSelector>()
            {
                { "+x", s => s.ThumbSticks.Left.X },
                { "-x", s => -s.ThumbSticks.Left.X },

                { "+y", s => s.ThumbSticks.Left.Y },
                { "-y", s => -s.ThumbSticks.Left.Y },

                { "+z", s => s.ThumbSticks.Right.X },
                { "-z", s => -s.ThumbSticks.Right.X },

                { "+w", s => s.ThumbSticks.Right.Y },
                { "-w", s => -s.ThumbSticks.Right.Y },

                { "triggerleft", s => s.Triggers.Left },
                { "triggerright", s => s.Triggers.Right }
            };

            private readonly InputManager.GamePadStateContainer pad;
            private readonly string axisName;
            private readonly AxisSelector axisSelector;
            private bool digitalDownBefore;
            private bool digitalDown;

            private float digitalValueBefore;
            private float digitalValueNow;

            public GamePadAxisAction(int id, string axisName, AxisSelector axisSelector)
            {
                this.pad = InputManager.GamePads[id];
                this.axisName = axisName;
                this.axisSelector = axisSelector;
            }

            public static IEnumerable<GamePadAxisAction> GetAll(int id)
            {
                return GamePadAxisAction.axisSelectors.Select(
                    pair => new GamePadAxisAction(id, pair.Key, pair.Value)
                    );
            }

            public static AxisSelector GetAxisSelector(string axisName)
            {
                AxisSelector selector;

                GamePadAxisAction.axisSelectors.TryGetValue(axisName, out selector);

                return selector;
            }

            private void updateDigital()
            {
                float v = this.axisSelector(this.pad.CurrentState);
                float vb = this.axisSelector(this.pad.PreviousState);

                if (v == this.digitalValueNow && vb == this.digitalValueBefore)
                    return;

                this.digitalValueNow = v;
                this.digitalValueBefore = vb;

                if (v >= StickSettings.HitValue)
                {
                    this.digitalDownBefore = this.digitalDown && vb >= StickSettings.HitValue;
                    this.digitalDown = true;
                }
                else if (v <= StickSettings.ReleaseValue)
                {
                    this.digitalDownBefore = this.digitalDown && vb > StickSettings.ReleaseValue;
                    this.digitalDown = false;
                }
            }

            private float adjustedAnalog
            {
                get
                {
                    float v = this.axisSelector(this.pad.CurrentState);
                    if (v < StickSettings.DeadZone)
                        return 0;
                    if (v > StickSettings.MaxValue)
                        return 1;
                    return (v - StickSettings.DeadZone) / StickSettings.DeadToMaxRange;
                }
            }

            public bool Hit
            {
                get
                {
                    this.updateDigital();
                    return this.digitalDown && !this.digitalDownBefore;
                }
            }

            public bool Active
            {
                get
                {
                    this.updateDigital();
                    return this.digitalDown;
                }
            }

            public bool Released
            {
                get
                {
                    this.updateDigital();
                    return this.digitalDownBefore && !this.digitalDown;
                }
            }

            public bool IsAnalog
            {
                get { return true; }
            }

            public float AnalogAmount
            {
                get { return this.adjustedAnalog; }
            }

            public string ToUIString()
            {
                return this.axisName;
            }

            public override string ToString()
            {
                return "gamepad" + this.pad.Id + ":" + this.axisName;
            }
        }
    }

}
