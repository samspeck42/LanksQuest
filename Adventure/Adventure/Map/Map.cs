using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public abstract class Map
    {
        public const int MAP_CELL_WIDTH = 128;
        public const int MAP_CELL_HEIGHT = 96;

        protected int[,] layout;
        protected GameWorld game;
        protected String filePath;

        public Map(GameWorld game)
        {
            this.game = game;
            this.filePath = "";
        }

        public virtual void Load()
        {
            List<List<int>> tempLayout = new List<List<int>>();

            using (StreamReader reader = new StreamReader(filePath))
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

            layout = new int[tempLayout.Count, tempLayout[0].Count];

            for (int r = 0; r < layout.GetLength(0); r++)
                for (int c = 0; c < layout.GetLength(1); c++)
                    layout[r, c] = tempLayout[r][c];
        }

        public Point GetCurrentCell()
        {
            Vector2 playerPosition = game.Player.Center;
            return new Point(game.CurrentArea.CellCoordinates.X + (int)Math.Floor(playerPosition.X / MAP_CELL_WIDTH),
                game.CurrentArea.CellCoordinates.Y + (int)Math.Floor(playerPosition.Y / MAP_CELL_HEIGHT));
        }

        public abstract Area GetPlayerArea();

        public abstract Area GetAreaByIndex(int index);

        public abstract void Enter();

        public abstract void Exit();

        public abstract void Update();
    }
}
