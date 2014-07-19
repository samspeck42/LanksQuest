using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class Chest : ActivatingEntity, Activatable
    {
        private const int TREASURE_DISPLAY_TIME = 60;

        private Sprite unopenedSprite;
        private Sprite openedSprite;
        private SoundEffect openSound;

        private bool isOpened = false;
        private bool isDisplayingTreasure = false;
        private Entity treasure = null;
        private int treasureDisplayTimer = 0;

        public bool IsOpened { get { return isOpened; } }
        public Entity Treasure { get { return treasure; } }

        public Chest(GameWorld game, Area area)
            : base(game, area)
        {
            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 30;
            hitBoxHeight = 24;

            Vector2 origin = new Vector2(1, 4);
            unopenedSprite = new Sprite(origin);
            openedSprite = new Sprite(origin);

            CurrentSprite = unopenedSprite;

            IsPassable = false;
        }

        public override void OnEntityCollision(Entity other)
        {
        }

        public override void LoadContent()
        {
            unopenedSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/chest_unopened");
            openedSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/chest_opened");
            openSound = game.Content.Load<SoundEffect>("Audio/chest_open");
        }


        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            treasure = Entity.CreateEntityFromString(dataDict["treasure"], game, area);
            isActive = bool.Parse(dataDict["isActive"]);
        }

        public override string ToString()
        {
            string treasureTypeName = treasure.GetType().ToString().Replace("Adventure.", "");
            return "(" + base.ToString() + ")(" + treasureTypeName + " " + treasure.ToString() + ")(" + isActive.ToString() + ")";
        }

        public override void Update()
        {
            base.Update();

            if (isDisplayingTreasure)
            {
                treasure.CurrentSprite.UpdateAnimation();
                treasureDisplayTimer++;

                if (treasureDisplayTimer <= (TREASURE_DISPLAY_TIME / 3))
                    treasure.Origin.Y -= 0.8f;
                if (treasureDisplayTimer >= TREASURE_DISPLAY_TIME)
                    isDisplayingTreasure = false;
            }
        }

        public void StartOpening()
        {
            isOpened = true;
            CurrentSprite = openedSprite;
            startDisplayingTreasure();

            tryToTriggerActivations();
            openSound.Play(0.75f, 0, 0);
        }

        private void startDisplayingTreasure()
        {
            isDisplayingTreasure = true;
            treasureDisplayTimer = 0;
            treasure.Center = new Vector2(this.Center.X, this.Center.Y - 20);
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            base.Draw(spriteBatch, changeColorsEffect);

            if (isDisplayingTreasure)
                treasure.CurrentSprite.Draw(spriteBatch, treasure.Origin);
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
        }
    }
}
