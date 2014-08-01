using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public static class DirectionsHelper
    {
        private static Dictionary<Directions, Vector2> directionVectors = new Dictionary<Directions,Vector2>        
        { 
            {Directions.None, new Vector2(0, 0)},
            {Directions.Up, new Vector2(0, -1)}, 
            {Directions.Down, new Vector2(0, 1)}, 
            {Directions.Left, new Vector2(-1, 0)}, 
            {Directions.Right, new Vector2(1, 0)} 
        };

        /// <summary>
        /// Returns the corresponding unit vector for a direction.
        /// </summary>
        /// <param name="direction">The direction whose unit vector will be returned.</param>
        /// <returns></returns>
        public static Vector2 GetDirectionVector(Directions direction)
        {
            Vector2 directionVector = Vector2.Zero;

            foreach (Directions dir in directionVectors.Keys)
            {
                if ((direction & dir) == dir)
                    directionVector += directionVectors[dir];
            }

            if (directionVector != Vector2.Zero)
                directionVector.Normalize();
            return directionVector;
        }

        /// <summary>
        /// Returns the corresponding unit vector for a direction.
        /// </summary>
        /// <param name="direction">The direction whose unit vector will be returned.</param>
        /// <returns></returns>
        public static Vector2 GetDirectionVector(Directions4 direction)
        {
            return GetDirectionVector(ToDirections(direction));
        }

        /// <summary>
        /// Converts a Directions4 to a Directions.
        /// </summary>
        /// <param name="direction4">The Directions4 to be converted.</param>
        /// <returns></returns>
        public static Directions ToDirections(Directions4 direction4)
        {
            int directionNum = (int)direction4;

            if (directionNum < 0)
                return Directions.None;

            return (Directions)((int)Math.Pow(2, directionNum));
        }
    }

    [Flags]
    public enum Directions
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        UpLeft = Up | Left,
        UpRight = Up | Right,
        DownLeft = Down | Left,
        DownRight = Down | Right
    }

    public enum Directions4
    {
        None = -1,
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }
}
