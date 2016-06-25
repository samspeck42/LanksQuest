using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure.Maps
{
    public class MapTransition
    {
        public Point LocationCell { get { return locationCell; } }
        //public int DestinationDungeonNumber { get { return destinationDungeonNumber; } }
        public String DestinationMapName { get { return destinationMapName; } }
        public int DestinationAreaIndex { get { return destinationAreaIndex; } }
        public Point DestinationCell { get { return destinationCell; } }
        public Directions4 Direction { get { return direction; } }
        //public bool IsDungeonTransition { get { return destinationDungeonNumber >= 0; } }

        private Point locationCell;
        private String destinationMapName;
        private int destinationAreaIndex;
        private Point destinationCell;
        private Directions4 direction;
        
        public MapTransition(Point locationCell, String destinationMapName, int destinationAreaIndex, Point destinationCell, Directions4 direction)
        {
            this.locationCell = locationCell;
            this.destinationMapName = destinationMapName;
            this.destinationAreaIndex = destinationAreaIndex;
            this.destinationCell = destinationCell;
            this.direction = direction;
        }

        public static MapTransition FromString(String str)
        {
            string[] mapTransitionData = str.Split(' ');
            string[] locCell = mapTransitionData[0].Split(',');
            String destMapName = mapTransitionData[1].Trim();
            int destAreaIndex = int.Parse(mapTransitionData[2]);
            string[] destCell = mapTransitionData[3].Split(',');
            Directions4 direction = (Directions4)Enum.Parse(typeof(Directions4), mapTransitionData[4]);
            MapTransition mapTransition = new MapTransition(new Point(int.Parse(locCell[0].Trim()), int.Parse(locCell[1].Trim())),
                destMapName, destAreaIndex, new Point(int.Parse(destCell[0].Trim()), int.Parse(destCell[1].Trim())), direction);
            return mapTransition;
        }

        public override string ToString()
        {
            string str = "" + locationCell.X + "," + locationCell.Y + " " +
                destinationMapName + " " +
                destinationAreaIndex + " " +
                destinationCell.X + "," + destinationCell.Y + " " + 
                direction; 
            return str;
        }
    }
}
