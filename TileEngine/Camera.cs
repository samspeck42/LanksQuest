using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class Camera
    {
        public Vector2 Position = Vector2.Zero;

        public Matrix TransformMatrix
        {
            get { return Matrix.CreateTranslation(new Vector3(-Position, 0f)); }

        }

        public void LockToTarget(Rectangle target, int screenWidth, int screenHeight)
        {
            Position.X = target.X - (screenWidth / 2 - target.Width / 2);
            Position.Y = target.Y - (screenHeight / 2 - target.Height / 2);
        }

        public void ClampToArea(int width, int height)
        {
            if (Position.X > width)
                Position.X = width;
            if (Position.Y > height)
                Position.Y = height;

            if (Position.X < 0)
                Position.X = 0;
            if (Position.Y < 0)
                Position.Y = 0;
        }
    }
}
