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

        public string TextureName;
        public TileCollision Collision;

        public static readonly string[] ImageExtensions = new string[] 
        {
            ".png", ".jpg",
        };

        public Tile(string textureName, TileCollision collision)
        {
            TextureName = textureName;
            Collision = collision;
        }

        public Tile()
        {
            TextureName = "";
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
