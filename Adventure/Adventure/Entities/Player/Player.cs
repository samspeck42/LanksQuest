using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using TileEngine;

namespace Adventure
{
    public class Player : Entity
    {
        public const string SWORD_HITBOX_ID = "sword_hitbox";

        const float WALK_SPEED = 3.0f;
        const float AIMING_WALK_SPEED = 1.6f;
        //public const int ATTACK_TIME = 8;
        const int INVINCIBILITY_TIME = 60;
        const int KNOCKBACK_TIME = 10;
        const float KNOCKBACK_SPEED = 8.0f;
        const int BLINK_DELAY = 2;
        // public const int SWORD_LENGTH = 38;
        // const int SWORD_HITBOX_LENGTH = 6;
        const int MAX_HEALTH = 8;
        const int DAMAGE = 1;
        public const int START_TO_PUSH_TIME = 30;
        const int ENTER_DISTANCE = 80;
        public const float PUSH_SPEED = 1.0f;
        public const float MAP_TRANSITION_WALK_SPEED = 1.0f;

        public override bool IsBlockedByObstacleEntities { get { return true; } }

        public SoundEffect SwordSwingSound;
        private SoundEffect swordTink;
        public SoundEffect HitSound;

        //public Rectangle SwordHitBox
        //{
        //    get
        //    {
        //        return new Rectangle((int)Math.Round(SwordHitPoint.X) - (SWORD_HITBOX_LENGTH / 2),
        //            (int)Math.Round(SwordHitPoint.Y) - (SWORD_HITBOX_LENGTH / 2), SWORD_HITBOX_LENGTH, SWORD_HITBOX_LENGTH);
        //    }
        //}

        public bool CanWalk { get { return stateHandler.CanWalk && !isKnockedBack; } }

        public bool CanGetHurt { get { return stateHandler.CanGetHurt && !isInvincible; } }

        private Inventory inventory;
        public Inventory Inventory { get { return inventory; } }
        private bool isInvincible = false;
        public bool IsInvincible { get { return isInvincible; } }
        public bool IsReadyToPush = false;
        private bool isTryingToInteract = false;
        public bool IsTryingToInteract { get { return isTryingToInteract; } }
        private PlayerStateHandler stateHandler;
        public PlayerStateHandler StateHandler { get { return stateHandler; } }
        public PlayerState State { get { return stateHandler.State; } }
        private PlayerSpriteHandler spriteHandler;
        public PlayerSpriteHandler SpriteHandler { get { return spriteHandler; } }

        private PlayerInput input;
        public PlayerInput Input { get { return input; } }

        //public Vector2 SwordHitPoint = Vector2.Zero;
        //public Vector2 ForwardSwordOrigin = new Vector2(20, 18);
        //public Vector2 BackwardSwordOrigin = new Vector2(1, 13);
        //public Vector2 LeftSwordOrigin = new Vector2(10, 17);
        //public Vector2 RightSwordOrigin = new Vector2(11, 17);
        private bool isKnockedBack = false;
        public bool IsKnockedBack = false;
        public bool IsAiming = false;
        private int invincibilityTimer = 0;
        private int knockBackTimer = 0;
        public int StartToPushTimer = 0;
        private EquippableItem equippableItemBeingUsed = null;
        private bool isEntering = false;
        private Vector2 enterPosition = Vector2.Zero;
        public Vector2 PreviousVelocity= Vector2.Zero;

        public Player(GameWorld game)
            : base(game, null)
        {
            BoundingBox.RelativeX = -11;
            BoundingBox.RelativeY = -23;
            BoundingBox.Width = 22;
            BoundingBox.Height = 25;

            HitBox swordHitBox = new HitBox(this, SWORD_HITBOX_ID);
            swordHitBox.IsActive = false;
            HitBoxes.Add(swordHitBox);

            input = new PlayerInput(this);
            stateHandler = new NormalStateHandler(this);
            FaceDirection = Directions4.Down;
            spriteHandler = new PlayerSpriteHandler(this);
            spriteHandler.SetSpriteStill();

            MaxHealth = MAX_HEALTH;
            Health = MaxHealth;
            Damage = DAMAGE;

            inventory = new Inventory();

            Bow bow = new Bow(game, null);
            bow.LoadContent();
            inventory.CollectEquippableItem(bow);
            inventory.EquipItem(bow, 0);
        }

        public override void LoadContent()
        {
            spriteHandler.Load(game.Content);

            SwordSwingSound = game.Content.Load<SoundEffect>("Audio/Player/sword_swing");
            swordTink = game.Content.Load<SoundEffect>("Audio/sword_tink");
            HitSound = game.Content.Load<SoundEffect>("Audio/player_hit");
        }

        public override void Update(GameTime gameTime)
        {
            if (game.IsMapTransitioning || isEntering)
            {
                spriteHandler.Update(gameTime);
                Position += Velocity;

                if (isEntering)
                {
                    if ((Velocity.X != 0 && (Math.Abs(Position.X - enterPosition.X) > ENTER_DISTANCE)) ||
                        (Velocity.Y != 0 && (Math.Abs(Position.Y - enterPosition.Y) > ENTER_DISTANCE)))
                    {
                        isEntering = false;
                        Velocity = Vector2.Zero;
                    }
                }

                return;
            }

            if (IsInvincible)
            {
                invincibilityTimer++;
                if (invincibilityTimer >= INVINCIBILITY_TIME)
                {
                    isInvincible = false;
                    invincibilityTimer = 0;
                }
            }
            if (isKnockedBack)
            {
                knockBackTimer++;
                if (isKnockedBack && knockBackTimer >= KNOCKBACK_TIME)
                {
                    isKnockedBack = false;
                    Velocity = Vector2.Zero;
                    knockBackTimer = 0;
                }
            }

            input.Update();

            if (CanWalk)
            {
                updateMovement();

                pushAroundWalls();
            }

            spriteHandler.Update(gameTime);

            input.Handle();

            //CanLeaveArea = State == PlayerState.Normal;

            stateHandler.Update(gameTime);

            
            base.Update(gameTime);
        }

        private void pushAroundWalls()
        {
            // push player around impassable tiles
            Vector2 pushVel1 = Vector2.Zero;
            Vector2 pushVel2 = Vector2.Zero;
            Vector2 expectedPosition1 = Vector2.Zero;
            Vector2 expectedPosition2 = Vector2.Zero;
            Vector2 expectedPosition3 = Vector2.Zero;
            bool expectedPosition1Collides = false;
            bool expectedPosition2Collides = false;
            bool expectedPosition3Collides = false;
            List<Entity> entitiesToCheck = new List<Entity>();
            bool needToCheck = false;

            if (Velocity.X > 0 && Velocity.Y == 0)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX + Width + 1, BoundingBox.ActualY);
                expectedPosition2 = new Vector2(BoundingBox.ActualX + Width + 1, Center.Y);
                expectedPosition3 = new Vector2(BoundingBox.ActualX + Width + 1, BoundingBox.ActualY + Height - 1);
                pushVel1 = new Vector2(0, 2);
                pushVel2 = new Vector2(0, -2);
                entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathX(this));
                needToCheck = true;
            }
            else if (Velocity.X < 0 && Velocity.Y == 0)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX - 1, BoundingBox.ActualY);
                expectedPosition2 = new Vector2(BoundingBox.ActualX - 1, Center.Y);
                expectedPosition3 = new Vector2(BoundingBox.ActualX - 1, BoundingBox.ActualY + Height - 1);
                pushVel1 = new Vector2(0, 2);
                pushVel2 = new Vector2(0, -2);
                entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathX(this));
                needToCheck = true;
            }
            else if (Velocity.X == 0 && Velocity.Y > 0)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX, BoundingBox.ActualY + Height + 1);
                expectedPosition2 = new Vector2(Center.X, BoundingBox.ActualY + Height + 1);
                expectedPosition3 = new Vector2(BoundingBox.ActualX + Width - 1, BoundingBox.ActualY + Height + 1);
                pushVel1 = new Vector2(2, 0);
                pushVel2 = new Vector2(-2, 0);
                entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathY(this));
                needToCheck = true;
            }
            else if (Velocity.X == 0 && Velocity.Y < 0)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX, BoundingBox.ActualY - 1);
                expectedPosition2 = new Vector2(Center.X, BoundingBox.ActualY - 1);
                expectedPosition3 = new Vector2(BoundingBox.ActualX + Width - 1, BoundingBox.ActualY - 1);
                pushVel1 = new Vector2(2, 0);
                pushVel2 = new Vector2(-2, 0);
                entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathY(this));
                needToCheck = true;
            }
            if (needToCheck)
            {
                expectedPosition1Collides = game.CurrentArea.GetCollisionAtCell(Area.ConvertPositionToCell(expectedPosition1)) == TileCollision.Obstacle;
                expectedPosition2Collides = game.CurrentArea.GetCollisionAtCell(Area.ConvertPositionToCell(expectedPosition2)) == TileCollision.Obstacle;
                expectedPosition3Collides = game.CurrentArea.GetCollisionAtCell(Area.ConvertPositionToCell(expectedPosition3)) == TileCollision.Obstacle;

                foreach (Entity entity in entitiesToCheck)
                {
                    expectedPosition1Collides = expectedPosition1Collides || entity.Contains(expectedPosition1);
                    expectedPosition2Collides = expectedPosition2Collides || entity.Contains(expectedPosition2);
                    expectedPosition3Collides = expectedPosition3Collides || entity.Contains(expectedPosition3);
                }

                if (expectedPosition1Collides && !expectedPosition2Collides && !expectedPosition3Collides)
                    Velocity += pushVel1;
                else if (!expectedPosition1Collides && !expectedPosition2Collides && expectedPosition3Collides)
                    Velocity += pushVel2;
            }
        }

        private void updateMovement()
        {
            Vector2 motion = DirectionsHelper.GetDirectionVector(input.InputDirection);

            Velocity.X = motion.X * (IsAiming ? AIMING_WALK_SPEED : WALK_SPEED);
            Velocity.Y = motion.Y * (IsAiming ? AIMING_WALK_SPEED : WALK_SPEED);

            if (stateHandler.CanChangeFaceDirection)
            {
                updateFaceDirection();
            }
        }

        private void updateFaceDirection()
        {
            if (input.InputDirection == Directions.Up && FaceDirection != Directions4.Up)
                FaceDirection = Directions4.Up;
            else if (input.InputDirection == Directions.Down && FaceDirection != Directions4.Down)
                FaceDirection = Directions4.Down;
            else if (input.InputDirection == Directions.Left && FaceDirection != Directions4.Left)
                FaceDirection = Directions4.Left;
            else if (input.InputDirection == Directions.Right && FaceDirection != Directions4.Right)
                FaceDirection = Directions4.Right;
            else if (input.InputDirection == Directions.UpLeft &&
                !(FaceDirection == Directions4.Up || FaceDirection == Directions4.Left))
                FaceDirection = Directions4.Left;
            else if (input.InputDirection == Directions.UpRight &&
                !(FaceDirection == Directions4.Up || FaceDirection == Directions4.Right))
                FaceDirection = Directions4.Right;
            else if (input.InputDirection == Directions.DownLeft &&
                !(FaceDirection == Directions4.Down || FaceDirection == Directions4.Left))
                FaceDirection = Directions4.Left;
            else if (input.InputDirection == Directions.DownRight &&
                !(FaceDirection == Directions4.Down || FaceDirection == Directions4.Right))
                FaceDirection = Directions4.Right;
        }

        private void startUsingEquippableItem(EquippableItem equippableItem)
        {
            if (equippableItem is Bow)
            {
                EnterState(new UsingBowStateHandler(this), (Bow)equippableItem);
            }
        }

        public void StartLifting(CarriableEntity entity)
        {
            EnterState(new CarryingStateHandler(this), entity);
        }

        //public void StartPushing(MovableBlock block)
        //{
        //    EnterState(new PushingStateHandler(this), block);
        //}

        //public void StartOpening(Chest chest)
        //{
        //    EnterState(new OpeningChestStateHandler(this), chest);
        //}

        private void startAttacking()
        {
            EnterState(new AttackingStateHandler(this));
        }

        public void StartEnteringArea(Directions direction)
        {
            isEntering = true;
            enterPosition = new Vector2(Position.X, Position.Y);

            Velocity = Vector2.Zero;
            if (direction == Directions.Up)
                Velocity.Y = -WALK_SPEED;
            else if (direction == Directions.Down)
                Velocity.Y = WALK_SPEED;
            else if (direction == Directions.Left)
                Velocity.X = -WALK_SPEED;
            else if (direction == Directions.Right)
                Velocity.X = WALK_SPEED;
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            bool shouldDraw = true;
            if (IsInvincible)
            {
                int n = invincibilityTimer % (BLINK_DELAY * 2);
                if (n < BLINK_DELAY)
                    shouldDraw = false;
            }

            Entity equippableItemBeingUsedEntity = (Entity)equippableItemBeingUsed;

            //if (State == PlayerState.UsingBow && FaceDirection != Directions.Down)
            //    equippableItemBeingUsedEntity.Draw(spriteBatch, changeColorsEffect);
            if (shouldDraw)
                spriteHandler.Draw(spriteBatch);
            //    base.Draw(spriteBatch, changeColorsEffect);
            //if (State == PlayerState.UsingBow && FaceDirection == Directions.Down)
            //    equippableItemBeingUsedEntity.Draw(spriteBatch, changeColorsEffect);
            //if (State == PlayerState.Carrying && entityBeingCarried != null)
            //    entityBeingCarried.Draw(spriteBatch, changeColorsEffect);


            spriteBatch.DrawString(game.Font, State.ToString(), new Vector2(Position.X - 30, Position.Y - 70), Color.White);

            foreach (HitBox hitBox in GetActiveHitBoxes())
            {
                spriteBatch.Draw(game.SquareTexture,
                    hitBox.ToRectangle(),
                    Color.Red);
                
            }
        }


        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            if (thisHitBox.IsId(SWORD_HITBOX_ID))
            {
                if (otherHitBox.TakesDamageFromPlayerSword)
                {
                    // TODO
                }
                if (other is Breakable && otherHitBox.IsId(HitBox.BOUNDING_BOX_ID))
                {
                    Breakable breakable = (Breakable)other;
                    breakable.StartBreaking();
                }
            }

            //if (other is Enemy)
            //{
            //    Enemy enemy = (Enemy)other;
            //    if (CanGetHurt && enemy.DoesDamageOnContact)
            //    {
            //        takeDamageFrom(other);
            //    }
            //}

            //if (other is Spikes)
            //{
            //    Spikes spikes = (Spikes)other;
            //    if (CanGetHurt && spikes.AreActivated)
            //    {
            //        takeDamageFrom(other);
            //    }
            //}

            //if (other is Pickup)
            //{
            //    Pickup pickup = (Pickup)other;
            //    if (pickup.Type == PickupType.Heart)
            //    {
            //        Health += pickup.Value;
            //        if (Health > MaxHealth)
            //            Health = MaxHealth;
            //    }
            //    else
            //    {
            //        inventory.CollectPickup(pickup);
            //    }

            //    pickup.PlayCollectionSound();
            //}

            //if (other is KeyDoor)
            //{
            //    if (IsInFrontOf(other) && inventory.NumKeys > 0)
            //    {
            //        inventory.UseKey();
            //        KeyDoor keyDoor = (KeyDoor)other;
            //        keyDoor.StartOpening();
            //    }
            //}

            //if (other is EnemyProjectile)
            //{
            //    if (CanGetHurt)
            //        takeDamageFrom(other);
            //}
        }

        private void takeDamageFrom(Entity other)
        {
            EnterState(new HurtStateHandler(this), other);
        }

        public void StartInvincibility()
        {
            isInvincible = true;
            invincibilityTimer = 0;
        }

        public void KnockBack(Entity entity)
        {
            if (!isKnockedBack)
            {
                isKnockedBack = true;
                knockBackTimer = 0;

                Vector2 direction;
                //if (entity is Spikes)
                //    direction = ((Spikes)entity).GetKnockBackDirection();
                //else
                    direction = this.Center - entity.Center;

                float angle = (float)Math.Atan2(direction.Y, direction.X);
                Velocity.X = (float)Math.Cos(angle) * KNOCKBACK_SPEED;
                Velocity.Y = (float)Math.Sin(angle) * KNOCKBACK_SPEED;

                //if (entity is RobotEnemy && State == PlayerState.Attacking)
                //{
                //    swordTink.Play(0.75f, 0, 0);
                //}
                //else
                //{
                //    spriteHandler.SetSpriteStill();
                //}
                spriteHandler.SetSpriteStill();
            }
        }

        public bool IsAllignedWith(Entity entity)
        {
            Vector2 expectedPosition1 = Vector2.Zero;
            Vector2 expectedPosition2 = Vector2.Zero;

            if (FaceDirection == Directions4.Left)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX - 1, BoundingBox.ActualY);
                expectedPosition2 = new Vector2(BoundingBox.ActualX - 1, BoundingBox.ActualY + Height - 1);
            }
            else if (FaceDirection == Directions4.Right)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX + Width, BoundingBox.ActualY);
                expectedPosition2 = new Vector2(BoundingBox.ActualX + Width, BoundingBox.ActualY + Height - 1);
            }
            else if (FaceDirection == Directions4.Up)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX, BoundingBox.ActualY - 1);
                expectedPosition2 = new Vector2(BoundingBox.ActualX + Width - 1, BoundingBox.ActualY - 1);
            }
            else if (FaceDirection == Directions4.Down)
            {
                expectedPosition1 = new Vector2(BoundingBox.ActualX, BoundingBox.ActualY + Height);
                expectedPosition2 = new Vector2(BoundingBox.ActualX + Width - 1, BoundingBox.ActualY + Height);
            }

            return entity.BoundingBox.Contains(expectedPosition1) && entity.BoundingBox.Contains(expectedPosition2);
        }

        public bool IsInFrontOf(Entity entity)
        {
            //if (entity is Chest && FaceDirection != Directions4.Up)
            //    return false;

            Vector2 inFrontPosition = Vector2.Zero;
            inFrontPosition.X = this.Center.X + (DirectionsHelper.GetDirectionVector(this.FaceDirection).X * (this.Width / 2 + 1));
            inFrontPosition.Y = this.Center.Y + (DirectionsHelper.GetDirectionVector(this.FaceDirection).Y * (this.Height / 2 + 1));

            return entity.Contains(inFrontPosition);
        }

        public bool CanEnterState(PlayerState state)
        {
            if (state == PlayerState.Attacking)
                return this.State == PlayerState.Normal || this.State == PlayerState.Attacking;
            else if (state == PlayerState.Pushing)
                return this.State == PlayerState.Normal;
            else if (state == PlayerState.Carrying)
                return this.State == PlayerState.Normal;
            else if (state == PlayerState.UsingBow)
                return this.State == PlayerState.Normal;
            else if (state == PlayerState.OpeningChest)
                return this.State == PlayerState.Normal;
            return true;
        }

        public void EnterState(PlayerStateHandler newStateHandler)
        {
            EnterState(newStateHandler, null);
        }

        public void EnterState(PlayerStateHandler newStateHandler, Entity associatedEntity)
        {
            this.stateHandler.End(newStateHandler.State);
            this.stateHandler = newStateHandler;
            this.stateHandler.Start(associatedEntity);
        }

        public void OnButtonPressed(PlayerButtons button)
        {
            stateHandler.OnButtonPressed(button);

            if (button == PlayerButtons.Interact && stateHandler.CanInteract)
            {
                // TODO
            }
            if (button == PlayerButtons.Attack && stateHandler.CanStartAttacking)
                startAttacking();
        }

        public void OnButtonReleased(PlayerButtons button)
        {
            stateHandler.OnButtonReleased(button);
        }

    }
}
