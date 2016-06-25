using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Adventure.Maps;

namespace Adventure.Entities.Environment
{
    public class Chest : ActivatingEntity, Interactable, Triggerable
    {
        private const int TREASURE_DISPLAY_TIME = 1000;
        private const string UNOPENED_SPRITE_ID = "unopened_sprite";
        private const string OPENED_SPRITE_ID = "opened_sprite";

        public override bool IsObstacle { get { return true; } }

        public bool CanStartInteraction { get { return !isOpened; } }
        public bool MustBeAllignedWithToInteract { get { return false; } }

        public bool IsOpened { get { return isOpened; } }
        public Entity Treasure { get { return treasure; } }

        private SoundEffect openSound;

        private bool isOpened = false;
        private bool isDisplayingTreasure = false;
        private Entity treasure = null;
        private int treasureDisplayTimer = 0;

        public Chest(GameWorld game, Map map, Area area)
            : base(game, map, area)
        {
            BoundingBox.RelativeX = -15;
            BoundingBox.RelativeY = -12;
            BoundingBox.Width = 30;
            BoundingBox.Height = 24;

            Vector2 origin = new Vector2(15, 12);
            Sprite unopenedSprite = new Sprite("Sprites/Environment/chest_unopened", this, origin);
            spriteHandler.AddSprite(UNOPENED_SPRITE_ID, unopenedSprite);
            Sprite openedSprite = new Sprite("Sprites/Environment/chest_opened", this, origin);
            spriteHandler.AddSprite(OPENED_SPRITE_ID, openedSprite);
            spriteHandler.SetSprite(UNOPENED_SPRITE_ID);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            openSound = gameWorld.Content.Load<SoundEffect>("Audio/chest_open");
        }


        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            treasure = Entity.FromString(dataDict["treasure"], gameWorld, map, area);
            isVisible = bool.Parse(dataDict["isActive"]);
        }

        public override string ToString()
        {
            string treasureTypeName = treasure.GetType().ToString().Replace("Adventure.", "");
            return "(" + base.ToString() + ")(" + treasureTypeName + " " + treasure.ToString() + ")(" + isVisible.ToString() + ")";
        }

        public override void Update(GameTime gameTime)
        {
            spriteHandler.Update(gameTime);

            if (isDisplayingTreasure)
            {
                treasure.CurrentSprite.Update(gameTime);
                treasureDisplayTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (treasureDisplayTimer <= (TREASURE_DISPLAY_TIME / 3))
                    treasure.Position.Y -= 48 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (treasureDisplayTimer >= TREASURE_DISPLAY_TIME)
                    isDisplayingTreasure = false;
            }
        }

        public void StartInteraction()
        {
            if (CanStartInteraction)
                gameWorld.Player.StartOpening(this);
        }

        public void StartOpening()
        {
            isOpened = true;
            spriteHandler.SetSprite(OPENED_SPRITE_ID);
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
                treasure.CurrentSprite.Draw(spriteBatch, treasure.Position);
        }

        public void TriggerOn()
        {
            isVisible = true;
        }

        public void TriggerOff()
        {
            isVisible = false;
        }
    }
}
