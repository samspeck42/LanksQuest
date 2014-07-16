using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class Spikes : Entity, Activatable
    {
        const int DAMAGE = 1;

        private AnimatedSprite activatedSprite;
        private AnimatedSprite deactivatedSprite;
        private bool areActivated;
        public bool AreActivated { get { return areActivated; } }

        public Spikes(GameWorld game, Area area)
            : base(game, area)
        {
            Rectangle bounds = new Rectangle(0, 0, 32, 32);
            activatedSprite = new AnimatedSprite(bounds);
            deactivatedSprite = new AnimatedSprite(bounds);

            CurrentSprite = activatedSprite;
            areActivated = true;
            Damage = DAMAGE;
            IsOnGround = true;
            IsPassable = false;
        }

        public override void LoadContent()
        {
            activatedSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Environment/spikes_activated");
            deactivatedSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Environment/spikes_deactivated");
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            if (dataDict.ContainsKey("areActivated"))
                areActivated = bool.Parse(dataDict["areActivated"]);

            if (!areActivated)
            {
                areActivated = false;
                IsPassable = true;
                CurrentSprite = deactivatedSprite;
            }
        }

        public Vector2 GetKnockBackDirection()
        {
            Vector2 direction = Vector2.Zero;
            bool spikesAbove = false, spikesBelow = false, spikesLeft = false, spikesRight = false;

            foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X, SpawnCell.Y - 1)))
            {
                if (entity is Spikes)
                    spikesAbove = true;
            }
            foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X, SpawnCell.Y + 1)))
            {
                if (entity is Spikes)
                    spikesBelow = true;
            }
            foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X - 1, SpawnCell.Y)))
            {
                if (entity is Spikes)
                    spikesLeft = true;
            }
            foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X + 1, SpawnCell.Y)))
            {
                if (entity is Spikes)
                    spikesRight = true;
            }

            if (spikesAbove && spikesBelow)
                direction = new Vector2(game.Player.Center.X - this.Center.X, 0);
            else if (spikesLeft && spikesRight)
                direction = new Vector2(0, game.Player.Center.Y - this.Center.Y);
            else
                direction = game.Player.Center - this.Center;

            direction.Normalize();
            return direction;
        }

        public override void OnEntityCollision(Entity other)
        {
        }

        public void Activate()
        {
            areActivated = true;
            IsPassable = false;
            CurrentSprite = activatedSprite;
        }

        public void Deactivate()
        {
            areActivated = false;
            IsPassable = true;
            CurrentSprite = deactivatedSprite;
        }
    }
}
