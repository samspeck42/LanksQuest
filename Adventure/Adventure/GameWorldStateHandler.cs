using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public abstract class GameWorldStateHandler
    {
        public abstract GameWorldState State { get; }

        protected GameWorld gameWorld;

        public GameWorldStateHandler(GameWorld gameWorld)
        {
            this.gameWorld = gameWorld;
        }

        public virtual void Start() { }
        public abstract void Update(GameTime gameTime);
        public virtual void End() { }
    }

    public enum GameWorldState
    {
        Playing,
        AreaTransition,
        MapTransition
    }
}
