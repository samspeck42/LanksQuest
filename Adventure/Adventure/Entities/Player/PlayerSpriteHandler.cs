using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class PlayerSpriteHandler : SpriteHandler
    {
        public const string STILL_SPRITES_ID = "still_sprites";
        public const string WALKING_SPRITES_ID = "walking_sprites";
        public const string ATTACKING_SPRITES_ID = "attacking_sprites";
        public const string GRABBING_SPRITES_ID = "grabbing_sprites";
        public const string PUSHING_SPRITES_ID = "pushing_sprites";
        public const string BOW_STILL_SPRITES = "bow_still_sprites";
        public const string BOW_WALKING_SPRITES = "bow_walking_sprites";

        const int WALK_ANIMATION_DELAY = 100;
        const int ATTACK_ANIMATION_DELAY = 24;

        private Player player;

        public PlayerSpriteHandler(Player player)
            : base(player)
        {
            this.currentSpriteId = STILL_SPRITES_ID;
            this.player = player;

            SpriteSet spriteSet = new SpriteSet();
            Vector2 origin = new Vector2(16, 38);
            spriteSet.SetSprite(Directions4.Up, new Sprite("Sprites/Player/player_backward_still2", player, origin));
            spriteSet.SetSprite(Directions4.Down, new Sprite("Sprites/Player/player_forward_still2", player, origin));
            spriteSet.SetSprite(Directions4.Left, new Sprite("Sprites/Player/player_left_still", player, origin));
            spriteSet.SetSprite(Directions4.Right, new Sprite("Sprites/Player/player_right_still", player, origin));
            AddSpriteSet(STILL_SPRITES_ID, spriteSet);


            spriteSet = new SpriteSet();
            spriteSet.SetSprite(Directions4.Up, new Sprite("Sprites/Player/player_backward_walking2", player, origin, 4, WALK_ANIMATION_DELAY));
            spriteSet.SetSprite(Directions4.Down, new Sprite("Sprites/Player/player_forward_walking2", player, origin, 4, WALK_ANIMATION_DELAY));
            spriteSet.SetSprite(Directions4.Left, new Sprite("Sprites/Player/player_left_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            spriteSet.SetSprite(Directions4.Right, new Sprite("Sprites/Player/player_right_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            AddSpriteSet(WALKING_SPRITES_ID, spriteSet);


            spriteSet = new SpriteSet();
            origin = new Vector2(16, 58);
            Sprite sprite = new Sprite("Sprites/Player/player_backward_sword2", player, origin, 3, ATTACK_ANIMATION_DELAY, 1);
            sprite.AddHitBoxTexture("Sprites/Player/player_backward_sword_hitbox", Player.SWORD_HITBOX_ID);
            spriteSet.SetSprite(Directions4.Up, sprite);

            origin = new Vector2(36, 38);
            sprite = new Sprite("Sprites/Player/player_forward_sword2", player, origin, 3, ATTACK_ANIMATION_DELAY, 1);
            sprite.AddHitBoxTexture("Sprites/Player/player_forward_sword_hitbox", Player.SWORD_HITBOX_ID);
            spriteSet.SetSprite(Directions4.Down, sprite);

            origin = new Vector2(44, 58);
            sprite = new Sprite("Sprites/Player/player_left_sword", player, origin, 3, ATTACK_ANIMATION_DELAY, 1);
            sprite.AddHitBoxTexture("Sprites/Player/player_left_sword_hitbox", Player.SWORD_HITBOX_ID);
            spriteSet.SetSprite(Directions4.Left, sprite);

            origin = new Vector2(16, 58);
            sprite = new Sprite("Sprites/Player/player_right_sword", player, origin, 3, ATTACK_ANIMATION_DELAY, 1);
            sprite.AddHitBoxTexture("Sprites/Player/player_right_sword_hitbox", Player.SWORD_HITBOX_ID);
            spriteSet.SetSprite(Directions4.Right, sprite);
            AddSpriteSet(ATTACKING_SPRITES_ID, spriteSet);


            spriteSet = new SpriteSet();
            origin = new Vector2(16, 38);
            spriteSet.SetSprite(Directions4.Up, new Sprite("Sprites/Player/player_backward_grabbing_still", player, origin));
            spriteSet.SetSprite(Directions4.Down, new Sprite("Sprites/Player/player_forward_grabbing_still", player, origin));
            spriteSet.SetSprite(Directions4.Left, new Sprite("Sprites/Player/player_left_grabbing_still", player, origin));
            spriteSet.SetSprite(Directions4.Right, new Sprite("Sprites/Player/player_right_grabbing_still", player, origin));
            AddSpriteSet(GRABBING_SPRITES_ID, spriteSet);


            spriteSet = new SpriteSet();
            spriteSet.SetSprite(Directions4.Up, new Sprite("Sprites/Player/player_backward_grabbing_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            spriteSet.SetSprite(Directions4.Down, new Sprite("Sprites/Player/player_forward_grabbing_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            spriteSet.SetSprite(Directions4.Left, new Sprite("Sprites/Player/player_left_grabbing_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            spriteSet.SetSprite(Directions4.Right, new Sprite("Sprites/Player/player_right_grabbing_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            AddSpriteSet(PUSHING_SPRITES_ID, spriteSet);


            spriteSet = new SpriteSet();
            origin = new Vector2(16, 38);
            spriteSet.SetSprite(Directions4.Up, new Sprite("Sprites/Player/player_backward_bow_still", player, origin));
            spriteSet.SetSprite(Directions4.Down, new Sprite("Sprites/Player/player_forward_bow_still", player, origin));
            origin = new Vector2(24, 38);
            spriteSet.SetSprite(Directions4.Left, new Sprite("Sprites/Player/player_left_bow_still", player, origin));
            origin = new Vector2(16, 38);
            spriteSet.SetSprite(Directions4.Right, new Sprite("Sprites/Player/player_right_bow_still", player, origin));
            AddSpriteSet(BOW_STILL_SPRITES, spriteSet);


            spriteSet = new SpriteSet();
            origin = new Vector2(16, 38);
            spriteSet.SetSprite(Directions4.Up, new Sprite("Sprites/Player/player_backward_bow_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            spriteSet.SetSprite(Directions4.Down, new Sprite("Sprites/Player/player_forward_bow_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            origin = new Vector2(24, 38);
            spriteSet.SetSprite(Directions4.Left, new Sprite("Sprites/Player/player_left_bow_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            origin = new Vector2(16, 38);
            spriteSet.SetSprite(Directions4.Right, new Sprite("Sprites/Player/player_right_bow_walking", player, origin, 4, WALK_ANIMATION_DELAY));
            AddSpriteSet(BOW_WALKING_SPRITES, spriteSet);
        }

        public void SetSpriteStill()
        {
            SetSprite(player.StateHandler.StillSpritesId);
        }

        public void SetSpriteWalking()
        {
            SetSprite(player.StateHandler.WalkingSpritesId);
        }

        public void SetSpriteAttacking()
        {
            SetSprite(ATTACKING_SPRITES_ID);
        }

        public bool IsCurrentSpriteStill()
        {
            return IsCurrentSprite(player.StateHandler.StillSpritesId);
        }

        public bool IsCurrentSpriteWalking()
        {
            return IsCurrentSprite(player.StateHandler.WalkingSpritesId);
        }

        public bool IsCurrentSpriteAttacking()
        {
            return IsCurrentSprite(ATTACKING_SPRITES_ID);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (player.CanWalk)
            {
                // player is providing motion direction input, so set walking sprite if necessary
                if (player.Input.InputDirection != Directions.None && !IsCurrentSpriteWalking())
                    SetSpriteWalking();

                // player is providing no motion direction input, so set still sprite if necessary
                if (player.Input.InputDirection == Directions.None && !IsCurrentSpriteStill())
                    SetSpriteStill();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (player.State == PlayerState.Carrying)
            {
                CarriableEntity carriableEntity = ((CarryingStateHandler)player.StateHandler).CarriableEntity;
                carriableEntity.CurrentSprite.Draw(spriteBatch, new Vector2(
                    player.Center.X,
                    player.BoundingBox.ActualY - (carriableEntity.Height / 2)));
            }
        }
    }
}
