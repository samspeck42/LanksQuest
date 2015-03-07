using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class ArrowSwitch : Switch, Triggerable
    {
        private const string OFF_SPRITE_ID = "off_sprite";
        private const string ON_SPRITE_ID = "on_sprite";
        private const string TRAP_OFF_SPRITE_ID = "trap_off_sprite";
        private const string TRAP_ON_SPRITE_ID = "trap_on_sprite";

        private SoundEffect activateSound;

        private ArrowSwitchType type = ArrowSwitchType.Normal;
        public ArrowSwitchType Type { get { return type; } }

        public ArrowSwitch(GameWorld game, Area area)
            : base(game, area)
        {
            BoundingBox.RelativeX = -9;
            BoundingBox.RelativeY = -28;
            BoundingBox.Width = 18;
            BoundingBox.Height = 17;

            Vector2 origin = new Vector2(16, 30);
            Sprite sprite = new Sprite("Sprites/Environment/arrow_switch_off", this, origin);
            spriteHandler.AddSprite(OFF_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Environment/arrow_switch_on", this, origin);
            spriteHandler.AddSprite(ON_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Environment/arrow_switch_trap_off", this, origin);
            spriteHandler.AddSprite(TRAP_OFF_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Environment/arrow_switch_trap_on", this, origin);
            spriteHandler.AddSprite(TRAP_ON_SPRITE_ID, sprite);

            spriteHandler.SetSprite(OFF_SPRITE_ID);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            activateSound = game.Content.Load<SoundEffect>("Audio/arrow_switch_activate");
        }

        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            //if (dataDict.ContainsKey("velocity"))
            //{
            //    string[] pos = dataDict["velocity"].Split(',');
            //    Velocity = new Vector2(float.Parse(pos[0].Trim()), float.Parse(pos[1].Trim()));
            //}
            if (dataDict.ContainsKey("type"))
            {
                type = (ArrowSwitchType)int.Parse(dataDict["type"]);

                if (type == ArrowSwitchType.Normal)
                    spriteHandler.SetSprite(OFF_SPRITE_ID);
                else if (type == ArrowSwitchType.Trap)
                    spriteHandler.SetSprite(TRAP_OFF_SPRITE_ID);
            }
        }

        public override void Update(GameTime gameTime)
        {
            spriteHandler.Update(gameTime);
        }

        public override bool IsActivatedByArrow(HitBox thisHitBox)
        {
            return true;
        }

        public override void Activate()
        {
            if (!hasTriggeredActivations)
            {
                if (type == ArrowSwitchType.Normal)
                    spriteHandler.SetSprite(ON_SPRITE_ID);
                else if (type == ArrowSwitchType.Trap)
                    spriteHandler.SetSprite(OFF_SPRITE_ID);
                tryToTriggerActivations();
                activateSound.Play(0.75f, 0, 0);
            }
        }

        //public override void Update(GameTime gameTime)
        //{
        //    Vector2 prevVelocity = new Vector2(Velocity.X, Velocity.Y);

        //    base.Update(gameTime);

        //    if (JustCollidedWithWall)
        //    {
        //        Velocity.X = -prevVelocity.X;
        //        Velocity.Y = -prevVelocity.Y;
        //    }

        //    target.X = HitBox2.X;
        //    target.Y = HitBox2.Y;
        //}

        //public override void OnEntityCollision(Entity other)
        //{
        //    if (other is Arrow)
        //    {
        //        Arrow arrow = (Arrow)other;
        //        if (arrow.IsFired && target.Contains(new Point((int)Math.Round(arrow.TipPosition.X), (int)Math.Round(arrow.TipPosition.Y))))
        //        {
        //            arrow.HitEntity(this);
        //            if (!hasTriggeredActivations)
        //            {
        //                if (type == ArrowSwitchType.Normal)
        //                    CurrentSprite = onSprite;
        //                else if (type == ArrowSwitchType.Trap)
        //                    CurrentSprite = trapOnSprite;
        //                tryToTriggerActivations();
        //                activateSound.Play(0.75f, 0, 0);
        //            }
        //        }
        //    }
        //}

        public void TriggerOn()
        {
            isVisible = true;
        }

        public void TriggerOff()
        {
            isVisible = false;
        }
    }

    public enum ArrowSwitchType
    {
        Normal,
        Trap,
    }
}
