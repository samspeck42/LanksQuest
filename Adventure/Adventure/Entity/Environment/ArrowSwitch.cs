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
        private AnimatedSprite offSprite;
        private AnimatedSprite onSprite;
        private AnimatedSprite trapOffSprite;
        private AnimatedSprite trapOnSprite;
        private SoundEffect activateSound;

        private Rectangle target;

        private ArrowSwitchType type = ArrowSwitchType.Normal;
        public ArrowSwitchType Type { get { return type; } }

        public ArrowSwitch(GameWorld game, Area area)
            : base(game, area)
        {
            Rectangle bounds = new Rectangle(7, 2, 18, 30);
            offSprite = new AnimatedSprite(bounds);
            onSprite = new AnimatedSprite(bounds);
            trapOffSprite = new AnimatedSprite(bounds);
            trapOnSprite = new AnimatedSprite(bounds);

            CurrentSprite = offSprite;

            target = new Rectangle(0, 0, 18, 18);
        }

        public override void Update()
        {
            Vector2 prevVelocity = new Vector2(Velocity.X, Velocity.Y);

            base.Update();

            if (JustCollidedWithWall)
            {
                Velocity.X = -prevVelocity.X;
                Velocity.Y = -prevVelocity.Y;
            }

            target.X = HitBox.X;
            target.Y = HitBox.Y;
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
            offSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_off");
            onSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_on");
            trapOffSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_trap_off");
            trapOnSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Environment/arrow_switch_trap_on");
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
