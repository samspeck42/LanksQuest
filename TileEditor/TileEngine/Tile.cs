using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class Tile
    {
        public const int Height = 32;
        public const int Width = 32;

        public Texture2D Texture;
        public TileCollision Collision;

        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }

        public Tile()
        {
            Texture = null;
            Collision = 0;
        }
    }

    public enum TileCollision
    {
        Passable,
        Impassable,
        Obstacle
    }
}
