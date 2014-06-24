
using System.Collections.Generic;
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

        public GameOverState State { get { return this.gameOverState; } }

        public int MonstersCloseToPlayer { get; set; }

        public GameState()
        {
            this.Level = new Level.Level(this, LevelGenerator.NewDefault.Verbose);
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
        }

        public void Update(UpdateEventArgs args)
        {
            var newArgs = new GameUpdateEventArgs(args, 1f);

            // don't use 'args' after this point

            this.time += newArgs.ElapsedTime;
            this.timeF = (float)this.time;

            if (InputManager.IsKeyHit(Key.F3))
                this.DrawDebug = !this.DrawDebug;

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

            if (InputManager.IsKeyHit(Key.ControlLeft))
            {
                this.Camera.Zoom = !this.Camera.Zoom;
            }
            this.Camera.Update(newArgs);

            this.musicSettings.Update(newArgs);
        }

        public void Draw(SpriteManager sprites)
        {
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
            this.Level.DisposeGeometry();
        }

        public void GameOver(bool won)
        {
            this.gameOverState = won ? GameOverState.Won : GameOverState.Lost;
        }
    }
}
