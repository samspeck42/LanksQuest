using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class Dungeon : Map
    {
        private List<Area> areas;
        private SoundEffect normalBackgroundMusic;
        private SoundEffect bossBackgroundMusic;
        private SoundEffect victoryBackgroundMusic;
        private SoundEffectInstance backgroundMusicInstance;
        private Area previousArea;
        private int number;

        public Dungeon(GameWorld game, int number)
            : base(game)
        {
            this.number = number;
            filePath = "Content/Dungeons/dungeon" + number + ".dungeon";
            areas = new List<Area>();
            previousArea = null;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            processLayout();
            normalBackgroundMusic = game.Content.Load<SoundEffect>("Audio/DST-CrystalCavern");
            bossBackgroundMusic = game.Content.Load<SoundEffect>("Audio/DST-BossRide");
            victoryBackgroundMusic = game.Content.Load<SoundEffect>("Audio/DST-Orpheus-I");
        }

        private void processLayout()
        {
            Dictionary<Area, Point> areaSizeDict = new Dictionary<Area, Point>();
            Dictionary<int, Area> areaDict = new Dictionary<int, Area>();
            for (int y = 0; y < areaLayout.GetLength(0); y++)
            {
                for (int x = 0; x < areaLayout.GetLength(1); x++)
                {
                    int index = areaLayout[y, x];
                    if (index != -1)
                    {
                        if (!areaDict.ContainsKey(index))
                        {
                            Area area = Area.FromFile(game, this, "dungeon" + number + "_" + index);
                            area.CellCoordinates = new Point(x, y);
                            areaDict.Add(index, area);
                            areaSizeDict.Add(area, new Point(1, 1));
                        }
                        else
                        {
                            Area area = areaDict[index];
                            Point areaSize = areaSizeDict[area];
                            if (x == 0 || areaLayout[y, x - 1] != index)
                                areaSizeDict[area] = new Point(areaSize.X, areaSize.Y + 1);
                            else if (y == 0 || areaLayout[y - 1, x] != index)
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

                    areas.Add(area);
                }
                else
                {
                    areas.Add(null);
                }
            }
        }

        public override void Update()
        {
            if (previousArea != null && previousArea != game.CurrentArea)
            {
                if (game.CurrentArea.IsBossArea && !previousArea.IsBossArea)
                {
                    if (backgroundMusicInstance.State == SoundState.Playing)
                        backgroundMusicInstance.Stop();
                    backgroundMusicInstance = bossBackgroundMusic.CreateInstance();
                    backgroundMusicInstance.IsLooped = true;
                    backgroundMusicInstance.Volume = 0.5f;
                    backgroundMusicInstance.Play();
                }
                if (game.CurrentArea.IsRewardArea && !previousArea.IsRewardArea)
                {
                    if (backgroundMusicInstance.State == SoundState.Playing)
                        backgroundMusicInstance.Stop();
                    backgroundMusicInstance = victoryBackgroundMusic.CreateInstance();
                    backgroundMusicInstance.IsLooped = true;
                    backgroundMusicInstance.Volume = 0.5f;
                    backgroundMusicInstance.Play();
                }
                //else if (!game.CurrentArea.IsBossArea && previousArea.IsBossArea)
                //{
                //    if (backgroundMusicInstance.State == SoundState.Playing)
                //        backgroundMusicInstance.Stop();
                //    backgroundMusicInstance = normalBackgroundMusic.CreateInstance();
                //    backgroundMusicInstance.IsLooped = true;
                //    backgroundMusicInstance.Volume = 0.5f;
                //    backgroundMusicInstance.Play();
                //}
            }

            if (areas.Contains(game.CurrentArea))
                previousArea = game.CurrentArea;
        }

        public override Area GetPlayerArea()
        {
            Point playerCell = GetPlayerCell();

            if (playerCell.X < areaLayout.GetLength(1) && playerCell.Y < areaLayout.GetLength(0) &&
                playerCell.X >= 0 && playerCell.Y >= 0)
            {
                int index = areaLayout[playerCell.Y, playerCell.X];
                if (index >= 0 && index < areas.Count)
                    return areas[index];
            }
            return null;
        }

        public override Area GetAreaByIndex(int index)
        {
            if (index < areas.Count)
                return areas[index];
            return null;
        }

        public override void Enter()
        {
            backgroundMusicInstance = normalBackgroundMusic.CreateInstance();
            backgroundMusicInstance.IsLooped = true;
            backgroundMusicInstance.Volume = 0.5f;
            backgroundMusicInstance.Play();
        }

        public override void Exit()
        {
            if (backgroundMusicInstance.State == SoundState.Playing)
                backgroundMusicInstance.Stop();
        }

        public void OnBossDeath()
        {
            if (game.CurrentArea.IsBossArea)
            {
                if (backgroundMusicInstance.State == SoundState.Playing)
                    backgroundMusicInstance.Stop();
            }
        }
    }
}
