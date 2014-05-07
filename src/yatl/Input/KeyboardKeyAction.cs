using System;
using System.Collections.Generic;
using System.Linq;
using yatl.Utilities;
using OpenTK.Input;

namespace yatl.Input
{
    sealed class KeyboardKeyAction : DigitalAction
    {
        private readonly Key key;

        public KeyboardKeyAction(Key key)
        {
            this.key = key;
        }

        public static IEnumerable<KeyboardKeyAction> GetAll()
        {
            return ((Key[])Enum.GetValues(typeof(Key))).Select(k => new KeyboardKeyAction(k));
        }

        public static KeyboardKeyAction FromKey(Key key)
        {
            return new KeyboardKeyAction(key);
        }

        public static KeyboardKeyAction FromString(string name)
        {
            var lower = name.ToLowerInvariant().Trim();
            if (!lower.StartsWith("keyboard:"))
                return null;

            var keyName = name.Substring(9).Trim();

            Key key = (Key)Enum.Parse(typeof (Key), keyName, true);

            if (key == Key.Unknown)
                throw new ArgumentException("Keyboard key name unknown.", "name");

            return new KeyboardKeyAction(key);
        }

        public override bool Hit
        {
            get { return InputManager.IsKeyHit(this.key); }
        }

        public override bool Active
        {
            get { return InputManager.IsKeyPressed(this.key); }
        }

        public override bool Released
        {
            get { return InputManager.IsKeyReleased(this.key); }
        }

        public override string ToUIString()
        {
            return this.key.ToButtonString();
        }

        public override string ToString()
        {
            return "keyboard:" + this.key;
        }
    }
}
