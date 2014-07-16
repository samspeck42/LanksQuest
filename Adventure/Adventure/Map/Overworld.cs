using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class Overworld : Map
    {
        private List<Area> areas;
        private SoundEffect backgroundMusic;
        private SoundEffectInstance backgroundMusicInstance;

        public Overworld(GameWorld game)
            : base(game)
        {
            areas = new List<Area>();
            filePath = "Content/Maps/overworld.gamemap";
        }

        public override void Load()
        {
            base.Load();

            processLayout();
            backgroundMusic = game.Content.Load<SoundEffect>("Audio/DST-OverlandTheme");
        }

        private void processLayout()
        {
            Dictionary<int, Area> areaDict = new Dictionary<int, Area>();
            for (int y = 0; y < layout.GetLength(0); y++)
            {
                for (int x = 0; x < layout.GetLength(1); x++)
                {
                    int index = layout[y, x];
                    if (!areaDict.ContainsKey(index))
                    {
                        Area area = Area.FromFile(game, this, "overworld_" + index);
                        area.CellCoordinates = new Point(x, y);
                        areaDict.Add(index, area);
                    }
                }
            }
            for (int i = 0; i < areaDict.Count; i++)
            {
                if (areaDict.ContainsKey(i))
                    areas.Add(areaDict[i]);
                else
                    areas.Add(null);
            }
        }

        public override void Update()
        {
            
        }

        public override Area GetPlayerArea()
        {
            Point playerCell = GetCurrentCell();

            if (playerCell.X < layout.GetLength(1) && playerCell.Y < layout.GetLength(0) &&
                playerCell.X >= 0 && playerCell.Y >= 0)
            {
                int index = layout[playerCell.Y, playerCell.X];
                return GetAreaByIndex(index);
            }
            return null;
        }

        public override Area GetAreaByIndex(int index)
        {
            if (index < areas.Count)
            {
                //if (!areas[index].IsLoaded)
                //    areas[index].Load();
                return areas[index];
            }
            return null;
        }

        public override void Enter()
        {
            backgroundMusicInstance = backgroundMusic.CreateInstance();
            backgroundMusicInstance.IsLooped = true;
            backgroundMusicInstance.Volume = 0.5f;
            backgroundMusicInstance.Play();
        }

        public override void Exit()
        {
            backgroundMusicInstance.Stop();
        }
    }
}
