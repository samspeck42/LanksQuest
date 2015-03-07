using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class MapTransition
    {
        public Point LocationCell { get { return locationCell; } }
        public int DestinationDungeonNumber { get { return destinationDungeonNumber; } }
        public int DestinationAreaIndex { get { return destinationAreaIndex; } }
        public Point DestinationCell { get { return destinationCell; } }
        public Directions4 Direction { get { return direction; } }
        public bool IsDungeonTransition { get { return destinationDungeonNumber >= 0; } }

        private Point locationCell;
        private int destinationDungeonNumber;
        private int destinationAreaIndex;
        private Point destinationCell;
        private Directions4 direction;
        
        public MapTransition(Point locationCell, int destinationDungeonNumber, int destinationAreaIndex, Point destinationCell, Directions4 direction)
        {
            this.locationCell = locationCell;
            this.destinationDungeonNumber = destinationDungeonNumber;
            this.destinationAreaIndex = destinationAreaIndex;
            this.destinationCell = destinationCell;
            this.direction = direction;
        }

        public static MapTransition CreateFromString(String str)
        {
            string[] mapTransitionData = str.Split(' ');
            string[] locCell = mapTransitionData[0].Split(',');
            int destDungeonNumber = int.Parse(mapTransitionData[1]);
            int destAreaIndex = int.Parse(mapTransitionData[2]);
            string[] destCell = mapTransitionData[3].Split(',');
            Directions4 direction = (Directions4)Enum.Parse(typeof(Directions4), mapTransitionData[4]);
            MapTransition mapTransition = new MapTransition(new Point(int.Parse(locCell[0].Trim()), int.Parse(locCell[1].Trim())),
                destDungeonNumber, destAreaIndex, new Point(int.Parse(destCell[0].Trim()), int.Parse(destCell[1].Trim())), direction);
            return mapTransition;
        }

        public override string ToString()
        {
            string str = "" + locationCell.X + "," + locationCell.Y + " " +
                destinationDungeonNumber + " " +
                destinationAreaIndex + " " +
                destinationCell.X + "," + destinationCell.Y + " " + 
                direction; 
            return str;
        }
    }
}
