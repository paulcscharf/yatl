
using System.Collections.ObjectModel;
using System.Linq;
using OpenTK;
using OpenTK.Input;

namespace yatl.Utilities
{
    /// <summary>
    /// The class for managing input.
    /// </summary>
    static class InputManager
    {
        private static KeyboardState currentKbState, prevKbState;
        private static MouseState currentMouseState, prevMouseState;
        private static MouseDevice mouse;
        private static ReadOnlyCollection<GamePadStateContainer> gamepads;

        public static ReadOnlyCollection<GamePadStateContainer> GamePads { get { return InputManager.gamepads; } }

        public class GamePadStateContainer
        {
            public int Id { get; private set; }
            public GamePadState CurrentState { get; private set; }
            public GamePadState PreviousState { get; private set; }

            public static GamePadStateContainer ForId(int id)
            {
                return new GamePadStateContainer(id);
            }

            private GamePadStateContainer(int id)
            {
                this.Id = id;
                this.CurrentState = GamePad.GetState(id);
                this.PreviousState = this.CurrentState;
            }

            public void Update()
            {
                this.PreviousState = this.CurrentState;
                this.CurrentState = GamePad.GetState(this.Id);
            }
        }

        public static void Initialize(MouseDevice mouse)
        {
            InputManager.mouse = mouse;

            InputManager.gamepads =
                Enumerable.Range(0, int.MaxValue - 1)
                    .TakeWhile(i => GamePad.GetState(i).IsConnected)
                    .Select(GamePadStateContainer.ForId)
                    .ToList().AsReadOnly();
        }

        public static void Update()
        {
            // Move the keyboard state forward
            InputManager.prevKbState = InputManager.currentKbState;
            InputManager.currentKbState = Keyboard.GetState();
            // Move the mouse state forward
            InputManager.prevMouseState = InputManager.currentMouseState;
            InputManager.currentMouseState = Mouse.GetState();
            // Move the gamepad states forward
            foreach (var gamepad in InputManager.gamepads)
            {
                gamepad.Update();
            }
        }

        #region Keyboard methods
        public static bool IsKeyPressed(Key k)
        {
            return InputManager.currentKbState.IsKeyDown(k);
        }
        public static bool IsKeyHit(Key k)
        {
            return InputManager.IsKeyPressed(k) && InputManager.prevKbState.IsKeyUp(k);
        }
        public static bool IsKeyReleased(Key k)
        {
            return !InputManager.IsKeyPressed(k) && InputManager.prevKbState.IsKeyDown(k);
        }
        #endregion

        #region Mouse methods
        public static Vector2 MousePosition()
        {
            return new Vector2(InputManager.mouse.X, InputManager.mouse.Y);
        }
        public static bool MouseMoved()
        {
            return InputManager.mouse.X != InputManager.mouse.X
                || InputManager.mouse.Y != InputManager.mouse.Y;
        }
        public static bool IsLeftMousePressed()
        {
            return InputManager.currentMouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool IsLeftMouseReleased()
        {
            return InputManager.currentMouseState.LeftButton == ButtonState.Released
                && InputManager.prevMouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool IsLeftMouseHit()
        {
            return InputManager.currentMouseState.LeftButton == ButtonState.Pressed
                && InputManager.prevMouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsRightMousePressed()
        {
            return InputManager.currentMouseState.RightButton == ButtonState.Pressed;
        }
        public static bool IsRightMouseReleased()
        {
            return InputManager.currentMouseState.RightButton == ButtonState.Released
                && InputManager.prevMouseState.RightButton == ButtonState.Pressed;
        }
        public static bool IsRightMouseHit()
        {
            return InputManager.currentMouseState.RightButton == ButtonState.Pressed
                && InputManager.prevMouseState.RightButton == ButtonState.Released;
        }
        public static int DeltaScroll()
        {
            return InputManager.currentMouseState.ScrollWheelValue - InputManager.prevMouseState.ScrollWheelValue;
        }
        #endregion Mouse methods
    }
}
