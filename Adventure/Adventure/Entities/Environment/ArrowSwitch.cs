using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class ArrowSwitch : ActivatingEntity, Activatable
    {
        private Sprite offSprite;
        private Sprite onSprite;
        private Sprite trapOffSprite;
        private Sprite trapOnSprite;
        private SoundEffect activateSound;

        private Rectangle target;

        private ArrowSwitchType type = ArrowSwitchType.Normal;
        public ArrowSwitchType Type { get { return type; } }

        public ArrowSwitch(GameWorld game, Area area)
            : base(game, area)
        {
            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 18;
            hitBoxHeight = 30;

            Vector2 origin = new Vector2(7, 2);
            offSprite = new Sprite(origin);
            onSprite = new Sprite(origin);
            trapOffSprite = new Sprite(origin);
            trapOnSprite = new Sprite(origin);

            CurrentSprite = offSprite;

            target = new Rectangle(0, 0, 18, 18);
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 prevVelocity = new Vector2(Velocity.X, Velocity.Y);

            base.Update(gameTime);

            if (JustCollidedWithWall)
            {
                Velocity.X = -prevVelocity.X;
                Velocity.Y = -prevVelocity.Y;
            }

            target.X = HitBox2.X;
            target.Y = HitBox2.Y;
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Arrow)
            {
                Arrow arrow = (Arrow)other;
                if (arrow.IsFired && target.Contains(new Point((int)Math.Round(arrow.TipPosition.X), (int)Math.Round(arrow.TipPosition.Y))))
                {
                    arrow.HitEntity(this);
                    if (!hasTriggeredActivations)
                    {
                        if (type == ArrowSwitchType.Normal)
                            CurrentSprite = onSprite;
                        else if (type == ArrowSwitchType.Trap)
                            CurrentSprite = trapOnSprite;
                        tryToTriggerActivations();
                        activateSound.Play(0.75f, 0, 0);
                    }
                }
            }
        }


        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            if (dataDict.ContainsKey("velocity"))
            {
                string[] pos = dataDict["velocity"].Split(',');
                Velocity = new Vector2(float.Parse(pos[0].Trim()), float.Parse(pos[1].Trim()));
            }
            if (dataDict.ContainsKey("type"))
            {
                type = (ArrowSwitchType)int.Parse(dataDict["type"]);

                if (type == ArrowSwitchType.Normal)
                    CurrentSprite = offSprite;
                else if (type == ArrowSwitchType.Trap)
                    CurrentSprite = trapOffSprite;
            }
        }

        public override void LoadContent()
        {
            offSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_off");
            onSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_on");
            trapOffSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_trap_off");
            trapOnSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_trap_on");
            activateSound = game.Content.Load<SoundEffect>("Audio/arrow_switch_activate");
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

    public enum ArrowSwitchType
    {
        Normal,
        Trap,
    }
}
