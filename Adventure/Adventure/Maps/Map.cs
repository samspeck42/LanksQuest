using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Adventure.Maps
{
    public class Map
    {
        public const int MAP_CELL_WIDTH = 128;
        public const int MAP_CELL_HEIGHT = 96;

        public Area CurrentArea
        {
            get { return areas[currentAreaIndex]; }
        }

        protected int[,] areaLayout;
        private GameWorld gameWorld;
        private List<Area> areas;
        private int currentAreaIndex = 0;

        public Map(GameWorld gameWorld)
        {
            this.gameWorld = gameWorld;
            areaLayout = new int[0,0];
            areas = new List<Area>();
        }

        public virtual void LoadContent()
        {
            
        }

        public static Map FromFile(string mapName, GameWorld gameWorld)
        {
            Map map = processFile(mapName, gameWorld, "Content/Maps/" + mapName + ".gamemap");

            map.LoadContent();

            return map;
        }

        private static Map processFile(string mapName, GameWorld gameWorld, string fileName)
        {
            List<List<int>> tempLayout = new List<List<int>>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                bool readingLayout = false;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                        continue;

                    else if (line.Contains("[Layout]"))
                    {
                        readingLayout = true;
                    }

                    else if (readingLayout)
                    {
                        string[] cells = line.Split(' ');

                        List<int> row = new List<int>();
                        foreach (string c in cells)
                        {
                            if (!string.IsNullOrEmpty(c))
                                row.Add(int.Parse(c));
                        }
                        tempLayout.Add(row);
                    }
                }
            }

            int width = tempLayout[0].Count;
            int height = tempLayout.Count;

            Map map = new Map(gameWorld);

            map.areaLayout = new int[height, width];

            for (int r = 0; r < map.areaLayout.GetLength(0); r++)
                for (int c = 0; c < map.areaLayout.GetLength(1); c++)
                    map.SetAreaIndexAtCell(new Point(c, r), tempLayout[r][c]);

            // load areas
            Dictionary<Area, Point> areaSizeDict = new Dictionary<Area, Point>();
            Dictionary<int, Area> areaDict = new Dictionary<int, Area>();
            for (int y = 0; y < map.areaLayout.GetLength(0); y++)
            {
                for (int x = 0; x < map.areaLayout.GetLength(1); x++)
                {
                    int index = map.areaLayout[y, x];
                    if (index != -1)
                    {
                        if (!areaDict.ContainsKey(index))
                        {
                            Area area = Area.FromFile(mapName + "_" + index, gameWorld, map);
                            area.MapCellCoordinates = new Point(x, y);
                            areaDict.Add(index, area);
                            areaSizeDict.Add(area, new Point(1, 1));
                        }
                        else
                        {
                            Area area = areaDict[index];
                            Point areaSize = areaSizeDict[area];
                            if (x == 0 || map.areaLayout[y, x - 1] != index)
                                areaSizeDict[area] = new Point(areaSize.X, areaSize.Y + 1);
                            else if (y == 0 || map.areaLayout[y - 1, x] != index)
                                areaSizeDict[area] = new Point(areaSize.X + 1, areaSize.Y);
                        }
                    }
                }
            }
            for (int i = 0; i < areaDict.Count; i++)
            {
                if (areaDict.ContainsKey(i))
                {
                    Area area = areaDict[i];
                    int expectedWidth = areaSizeDict[area].X * Map.MAP_CELL_WIDTH;
                    int expectedHeight = areaSizeDict[area].Y * Map.MAP_CELL_HEIGHT;

                    if (expectedWidth != area.WidthInPixels || expectedHeight != area.HeightInPixels)
                    {
                        throw new Exception("Area not expected size: " + area.ToString());
                    }

                    map.areas.Add(area);
                }
                else
                {
                    map.areas.Add(null);
                }
            }

            return map;
        }

        public void Update()
        { }

        public void SetAreaIndexAtCell(Point cell, int index)
        {
            if (MapCellInBounds(cell))
                areaLayout[cell.Y, cell.X] = index;
        }

        public bool MapCellInBounds(Point cell)
        {
            return (cell.X >= 0 && cell.Y >= 0 && cell.X < areaLayout.GetLength(1) && cell.Y < areaLayout.GetLength(0));
        }

        public Point GetPlayerMapCell()
        {
            Vector2 playerPosition = gameWorld.Player.Position;
            return new Point(CurrentArea.MapCellCoordinates.X + (int)Math.Floor(playerPosition.X / MAP_CELL_WIDTH),
                CurrentArea.MapCellCoordinates.Y + (int)Math.Floor(playerPosition.Y / MAP_CELL_HEIGHT));
        }

        public int GetPlayerAreaIndex()
        {
            Point playerCell = GetPlayerMapCell();

            if (playerCell.X < areaLayout.GetLength(1) && playerCell.Y < areaLayout.GetLength(0) &&
                playerCell.X >= 0 && playerCell.Y >= 0)
            {
                int index = areaLayout[playerCell.Y, playerCell.X];
                if (index >= 0 && index < areas.Count)
                    return index;
            }
            return -1;
        }

        public Area GetAreaByIndex(int index)
        {
            if (index >= 0 && index < areas.Count)
                return areas[index];
            return null;
        }


        public void SetCurrentAreaIndex(int index)
        {
            currentAreaIndex = index;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }
    }
}
