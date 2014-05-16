
using System.Collections;
using System.Collections.Generic;
using amulware.Graphics;
using OpenTK;
using yatl.Rendering;

namespace yatl.Environment
{
    sealed class GameState
    {
        private double time;
        private float timeF;
        public float Time { get { return this.timeF; } }

        private readonly List<GameObject> gameObjects = new List<GameObject>();

        public GameState()
        {
            new Wisp(this, Vector2.Zero);
        }

        public void Update(UpdateEventArgs args)
        {
            var newArgs = new GameUpdateEventArgs(args, 1f);

            // don't use 'args' after this point

            this.time += newArgs.ElapsedTime;
            this.timeF = (float)this.time;

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
        }

        public void Draw(SpriteManager sprites)
        {
            foreach (var gameObject in this.gameObjects)
            {
                gameObject.Draw(sprites);
            }

            sprites.ScreenText.Height = 3;
            sprites.ScreenText.DrawString(new Vector2(0, 9), "Hi! :)", 0.5f, 0.5f);
        }

        public void AddObject(GameObject gameObject)
        {
            this.gameObjects.Add(gameObject);
        }
    }
}
