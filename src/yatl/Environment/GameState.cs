
using System.Collections;
using System.Collections.Generic;
using amulware.Graphics;
using OpenTK;
using OpenTK.Input;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment
{
    sealed class GameState
    {
        private double time;
        private float timeF;
        public float Time { get { return this.timeF; } }

        public bool DrawDebug { get; private set; }

        public Level Level { get; private set; }
        public Wisp Player { get; private set; }

        private readonly List<GameObject> gameObjects = new List<GameObject>();

        public Camera Camera { get; private set; }

        public GameState()
        {
            this.Level = new Level(this);
            this.Player = new Wisp(this, Vector2.Zero);

            this.Camera = new Camera(this.Player);
        }

        public void Update(UpdateEventArgs args)
        {
            var newArgs = new GameUpdateEventArgs(args, 1f);

            // don't use 'args' after this point

            this.time += newArgs.ElapsedTime;
            this.timeF = (float)this.time;

            if (InputManager.IsKeyHit(Key.F3))
                this.DrawDebug = !this.DrawDebug;

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

            this.Camera.Zoom = InputManager.IsKeyPressed(Key.ControlLeft)
                ? Settings.Game.Camera.OverviewZoom
                : Settings.Game.Camera.DefaultZoom;
            this.Camera.Update(newArgs);
        }

        public void Draw(SpriteManager sprites)
        {
            this.Level.Draw(sprites);

            foreach (var gameObject in this.gameObjects)
            {
                gameObject.Draw(sprites);
            }

        }

        public void AddObject(GameObject gameObject)
        {
            this.gameObjects.Add(gameObject);
        }
    }
}
