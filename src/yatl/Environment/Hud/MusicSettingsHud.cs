using amulware.Graphics;
using OpenTK;
using OpenTK.Input;
using yatl.Input;
using yatl.Rendering;

namespace yatl.Environment.Hud
{
    sealed class MusicSettingsHud
    {
        private readonly GameState game;
        private readonly Scrollbar lightnessBar;
        private readonly Scrollbar tensionBar;

        private bool changed;

        public MusicParameters Parameters { get; private set; }

        public MusicSettingsHud(GameState game)
        {
            this.game = game;
            this.lightnessBar = new Scrollbar(new Vector3(-16, 2, 0), 0.6f, this.change,
                KeyboardKeyAction.FromKey(Key.Number1), KeyboardKeyAction.FromKey(Key.Number2));

            this.tensionBar = new Scrollbar(new Vector3(-16, 4.5f, 0), 0.8f, this.change,
                KeyboardKeyAction.FromKey(Key.Number3), KeyboardKeyAction.FromKey(Key.Number4));

            this.changed = true;
        }

        private void change(float f)
        {
            this.changed = true;
        }

        public void Update(GameUpdateEventArgs e)
        {
            this.lightnessBar.Update(e);
            this.tensionBar.Update(e);

            var light = this.game.Player.Tile.Info.Lightness * 2;
            if (light != this.lightnessBar.Value)
                this.changed = true;
            this.lightnessBar.Value = light;

            if (this.changed)
            {
                this.Parameters = new MusicParameters(
                    this.lightnessBar.Value,
                    this.tensionBar.Value);
                this.changed = false;
            }
        }

        public void Draw(SpriteManager sprites)
        {
            this.lightnessBar.Draw(sprites);
            this.tensionBar.Draw(sprites);

            var font = sprites.ScreenText;

            font.Color = Color.Gray;
            font.Height = 1;
            font.DrawString(new Vector3(-16, 1, 0), "Lightness (1 - 2)");
            font.DrawString(new Vector3(-16, 3.5f, 0), "Tension (3 - 4)");
        }
    }
}
