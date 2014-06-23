using System;
using System.Runtime.InteropServices;
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
        private readonly Scrollbar healthBar;

        public MusicParameters Parameters { get; private set; }

        public MusicSettingsHud(GameState game)
        {
            this.game = game;
            this.lightnessBar = new Scrollbar(new Vector3(-16, 2, 0), 0.6f, this.change,
                KeyboardKeyAction.FromKey(Key.Number1), KeyboardKeyAction.FromKey(Key.Number2));

            this.tensionBar = new Scrollbar(new Vector3(-16, 4.5f, 0), 0.8f, this.change,
                KeyboardKeyAction.FromKey(Key.Number3), KeyboardKeyAction.FromKey(Key.Number4));

            this.healthBar = new Scrollbar(new Vector3(-16, 7f, 0), 0.8f, this.change,
                KeyboardKeyAction.FromKey(Key.Number5), KeyboardKeyAction.FromKey(Key.Number6));

        }

        private void change(float f)
        {
        }

        public void Update(GameUpdateEventArgs e)
        {
            this.lightnessBar.Update(e);
            this.tensionBar.Update(e);
            this.healthBar.Update(e);

            this.lightnessBar.Value = this.game.Player.Tile.Info.Lightness * 2;

            var chasingCount = this.game.ChasingEnemies.Count;
            var nearCount = this.game.MonstersCloseToPlayer;
            var tension = 1 - (float)Math.Pow(1.2, -nearCount) * (float)Math.Pow(1.5, -chasingCount);
            this.tensionBar.Value = tension;

            this.healthBar.Value = this.game.Player.HealthPercentage;

            this.Parameters = new MusicParameters(
                this.lightnessBar.Value,
                this.tensionBar.Value,
                this.game.Player.HealthPercentage,
                this.game.State);
        }

        public void Draw(SpriteManager sprites)
        {
            this.lightnessBar.Draw(sprites);
            this.tensionBar.Draw(sprites);
            this.healthBar.Draw(sprites);

            var font = sprites.ScreenText;

            font.Color = Color.Gray;
            font.Height = 1;
            font.DrawString(new Vector3(-16, 1, 0), "Lightness (1 - 2)");
            font.DrawString(new Vector3(-16, 3.5f, 0), "Tension (3 - 4)");
            font.DrawString(new Vector3(-16, 6f, 0), "Health (5 - 6)");
        }
    }
}
