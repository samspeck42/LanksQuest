using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class MapTransition
    {
        private Point locationCell;
        public Point LocationCell { get { return locationCell; } }

        private int destinationDungeonNumber;
        public int DestinationDungeonNumber { get { return destinationDungeonNumber; } }

        private int destinationAreaIndex;
        public int DestinationAreaIndex { get { return destinationAreaIndex; } }

        private Point destinationCell;
        public Point DestinationCell { get { return destinationCell; } }

        public bool IsDungeonTransition { get { return destinationDungeonNumber >= 0; } }

        public MapTransition(Point locationCell, int destinationDungeonNumber, int destinationAreaIndex, Point destinationCell)
        {
            this.locationCell = locationCell;
            this.destinationDungeonNumber = destinationDungeonNumber;
            this.destinationAreaIndex = destinationAreaIndex;
            this.destinationCell = destinationCell;
        }

        public static MapTransition FromString(String str)
        {
            string[] mapTransitionData = str.Split(' ');
            string[] locCell = mapTransitionData[0].Split(',');
            int destDungeonNumber = int.Parse(mapTransitionData[1]);
            int destAreaIndex = int.Parse(mapTransitionData[2]);
            string[] destCell = mapTransitionData[3].Split(',');
            MapTransition mapTransition = new MapTransition(new Point(int.Parse(locCell[0].Trim()), int.Parse(locCell[1].Trim())),
                destDungeonNumber, destAreaIndex, new Point(int.Parse(destCell[0].Trim()), int.Parse(destCell[1].Trim())));
            return mapTransition;
        }

        public override string ToString()
        {
            string str = "" + locationCell.X + "," + locationCell.Y + " " +
                destinationDungeonNumber + " " +
                destinationAreaIndex + " " +
                destinationCell.X + "," + destinationCell.Y;
            return str;
        }
    }
}
