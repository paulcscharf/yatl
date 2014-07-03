
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using amulware.Graphics;
using OpenTK;
using OpenTK.Input;
using yatl.Environment.Hud;
using yatl.Environment.Level.Generation;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment
{
    sealed class GameState
    {
        public enum GameOverState
        {
            Undetermined = 0,
            Lost = 1,
            Won = 2,
        }

        private double time;
        private float timeF;
        public float Time { get { return this.timeF; } }

        public bool DrawDebug { get; private set; }

        public Level.Level Level { get; private set; }
        public Wisp Player { get; private set; }

        private readonly List<GameObject> gameObjects = new List<GameObject>();

        public HashSet<Monster> ChasingEnemies { get; private set; } 

        private readonly MusicSettingsHud musicSettings;
        public MusicParameters MusicParameters { get { return this.musicSettings.Parameters; } }

        public Camera Camera { get; private set; }

        private GameOverState gameOverState;
        private float resetTime;

        public bool WaitingForReset { get { return this.resetTime != 0 && this.Time >= this.resetTime; } }

        public GameOverState State { get { return this.gameOverState; } }

        public int MonstersCloseToPlayer { get; set; }

        private static float overlayBrightness = 1;
        private static float overlayPercentage = 1;

        private float titleEndTime;

        private bool tutorialVisible;
        private float tutorialAlpha;
        private bool hideTitle;

        public GameStatistics Statistics { get; private set; }

        public GameState(bool isFirst = false)
        {
            this.Statistics = new GameStatistics();

            this.Level = new Level.Level(this, LevelGenerator.NewDefault.Silent);
            this.Level.GrowCrystals();

            this.Player = new Wisp(this, Vector2.Zero);

            this.Camera = new Camera(this.Player);

            this.musicSettings = new MusicSettingsHud(this);

            this.ChasingEnemies = new HashSet<Monster>();

            foreach (var tile in this.Level.Tilemap)
            {
                if (tile.Radius > 2 && tile.Info.Lightness < Settings.Game.Level.LightnessThreshold && GlobalRandom.NextBool(0.15))
                {
                    foreach(var i in Enumerable.Range(0, GlobalRandom.Next(1, 3)))
                        new Monster(this, this.Level.GetPosition(tile)
                            + new Vector2(GlobalRandom.NextFloat(), GlobalRandom.NextFloat()) * 0.1f);
                }
            }

            this.titleEndTime = isFirst ? 3 : 0;
        }

        public void Update(UpdateEventArgs args)
        {
            var newArgs = new GameUpdateEventArgs(args, 1f);

            // don't use 'args' after this point

            this.time += newArgs.ElapsedTime;
            this.timeF = (float)this.time;

            if (InputManager.IsKeyHit(Key.F3))
                this.DrawDebug = !this.DrawDebug;

            if (InputManager.IsKeyHit(Key.F1))
                this.tutorialVisible = !tutorialVisible;

            if (this.tutorialVisible)
            {
                this.tutorialAlpha = Math.Min(1, this.tutorialAlpha + newArgs.ElapsedTimeF * 2);

                if (InputManager.IsKeyHit(Key.F8))
                    Process.Start("http://bit.ly/youarethelight");
            }
            else
                this.tutorialAlpha = Math.Max(0, this.tutorialAlpha - newArgs.ElapsedTimeF * 2);

            if (this.tutorialAlpha == 1)
                this.hideTitle = true;

            this.MonstersCloseToPlayer = 0;

            #region Update Game Objects

            bool deletedObjects = false;
            for (int i = 0; i < this.gameObjects.Count; i++)
            {
                var gameObject = this.gameObjects[i];
                gameObject.Update(newArgs);
                if (gameObject.Deleted)
                {
                    // dispose
                    gameObject.Dispose();
                    this.gameObjects[i] = null;
                    deletedObjects = true;
                }
            }

            if (deletedObjects)
                this.gameObjects.RemoveAll(obj => obj == null);

            #endregion

            if (InputManager.IsKeyHit(Key.F4))
            {
                this.Camera.Zoom = !this.Camera.Zoom;
            }
            this.Camera.Update(newArgs);

            this.musicSettings.Update(newArgs);

            this.updateOverlay(newArgs);

            this.updateStats(newArgs);
        }

        private void updateStats(GameUpdateEventArgs e)
        {
            if (this.Player.Tile.Info.Lightness >= Settings.Game.Level.LightnessThreshold)
                this.Statistics.TimeInLight += e.ElapsedTime;
            else
                this.Statistics.TimeInDarkness += e.ElapsedTime;
        }

        private void updateOverlay(GameUpdateEventArgs e)
        {
            var fadeSpeed = (this.State == GameState.GameOverState.Undetermined ? 2 : 0.3f) * e.ElapsedTimeF;

            var bGoal = this.State == GameState.GameOverState.Won ? 1 : 0;

            GameState.overlayBrightness += (bGoal - GameState.overlayBrightness) * fadeSpeed;

            var pGoal = this.State == GameState.GameOverState.Undetermined
                ? 1 - this.Player.HealthPercentage
                : 1;

            pGoal *= pGoal;

            GameState.overlayPercentage += (pGoal - GameState.overlayPercentage) * fadeSpeed;
        }

        public void Draw(SpriteManager sprites)
        {
            var surfaceMan = SurfaceManager.Instance;

            var overlayColor = new Vector4(GameState.overlayBrightness,
                GameState.overlayBrightness, GameState.overlayBrightness, 1);
            var overlayPercentage = GameState.overlayPercentage;

            var font = sprites.ScreenText;

            if (!this.hideTitle && this.titleEndTime > this.time)
            {
                var alpha = (float)Math.Min(1, this.titleEndTime - this.time) * (1 - this.tutorialAlpha);

                var black = 1 - alpha;

                overlayColor.Xyz *= black;
                overlayPercentage = 1 - (1 - overlayPercentage) * black;

                var textAlpha = alpha * GameMath.Clamp(this.timeF * 0.5f - 0.2f, 0, 1);

                font.Color = Color.White * textAlpha;
                font.Height = 2.4f;
                font.DrawString(new Vector2(0, 9), "You Are The Light", 0.5f, 0.5f);
            }

            if (!this.hideTitle && this.time > 1 && this.titleEndTime > 0)
            {
                var time = (this.timeF - 1) * 0.15f;

                if (time < 1)
                {
                    var a = (float)(0.1 - 0.1 * Math.Cos(2 * Math.PI * time) + 0.4 - 0.4 * Math.Cos(8 * Math.PI * time))
                        * (1 - this.tutorialAlpha);

                    font.Height = 1.2f;
                    font.Color = Color.Black * a;
                    font.DrawString(new Vector2(0.02f, 18.02f), "F1: Help", 0.5f, 1);
                    font.Color = Color.White * a;
                    font.DrawString(new Vector2(0, 18), "F1: Help", 0.5f, 1);
                }
            }

            if (this.tutorialAlpha > 0)
            {
                var black = 1 - this.tutorialAlpha;

                overlayColor.Xyz *= black;
                overlayPercentage = 1 - (1 - overlayPercentage) * black;

                var a = this.tutorialAlpha * this.tutorialAlpha;

                var argb = Color.White * a;

                font.Height = 2f;
                font.Color = argb;
                font.DrawString(new Vector2(0, 0.2f), "You Are The Light", 0.5f);

                font.Height = 1.3f;
                font.DrawString(new Vector2(-8, 3f), "This is you", 0.5f);
                font.DrawString(new Vector2(8, 3f), "This is a monster", 0.5f);

                sprites.TutorialWisp.Color = argb;
                sprites.TutorialMonster.Color = argb;
                sprites.TutorialWisp.DrawSprite(new Vector2(-8, 6));
                sprites.TutorialMonster.DrawSprite(new Vector2(8, 6));

                font.Height = 1f;
                font.DrawString(new Vector2(-8, 8f), "You are lost in a maze.", 0.5f);
                font.DrawString(new Vector2(-8, 9f), "Your goal is to get out.", 0.5f);

                font.DrawString(new Vector2(8, 8f), "They will hunt you, but", 0.5f);
                font.DrawString(new Vector2(8, 9f), "they are scared of light.", 0.5f);

                font.Height = 1.3f;
                font.DrawString(new Vector2(0, 11f), "Controls", 0.5f);

                font.Height = 1f;
                font.DrawString(new Vector2(0, 12.5f), "Move with arrow keys/WASD", 0.5f);
                font.DrawString(new Vector2(0, 13.5f), "Show/hide this screen with F1", 0.5f);
                font.DrawString(new Vector2(0, 14.5f), "Toggle fullscreen with F11", 0.5f);
                font.DrawString(new Vector2(0, 15.5f), "Exit with ESC", 0.5f);

                font.Height = 0.5f;
                font.Color = Color.Gray * a;
                font.DrawString(new Vector2(0, 17.5f), "Made by Chiel ten Brinke & Paul Scharf", 0.5f, 1f);
                font.DrawString(new Vector2(0, 18f), "More info at: paulscharf.com/stuff/games/youarethelight (press F8 to open)", 0.5f, 1f);
            }

            surfaceMan.OverlayColor = overlayColor;
            surfaceMan.OverlayFadePercentage = overlayPercentage;

            this.Level.Draw(sprites);

            foreach (var gameObject in this.gameObjects)
            {
                gameObject.Draw(sprites);
            }

            this.musicSettings.Draw(sprites);

            if (this.gameOverState != GameOverState.Undetermined)
            {
                bool won = this.gameOverState == GameOverState.Won;
                var text = won
                    ? "You found the light!"
                    : "Your light was extinguished...";
                sprites.ScreenText.Height = 1.5f;
                sprites.ScreenText.Color = won ? Color.Black : Color.White;
                sprites.ScreenText.DrawString(new Vector2(0, 9), text, 0.5f, 0.5f);
            }
        }

        public void AddObject(GameObject gameObject)
        {
            this.gameObjects.Add(gameObject);
        }

        public void Dispose()
        {
            if(Settings.Game.SaveStatistics)
                this.Statistics.Save();
            this.Level.DisposeGeometry();
        }

        public void GameOver(bool won)
        {
            if (this.State != GameOverState.Undetermined)
                return;

            this.gameOverState = won ? GameOverState.Won : GameOverState.Lost;

            this.Statistics.GameOverState = this.gameOverState;

            this.resetTime = this.Time + Settings.Game.ResetDelay;
        }
    }
}
