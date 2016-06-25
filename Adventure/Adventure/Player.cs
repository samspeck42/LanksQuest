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
using Adventure.Entities.Items;
using Adventure.Entities.MovementHandlers;
using Adventure.Entities;
using Adventure.PlayerStateHandlers;
using Adventure.Entities.Environment;
using Adventure.Entities.Enemies;
using Adventure.Maps;

namespace Adventure
{
    public class Player : Mob
    {
        public const string SWORD_HITBOX_ID = "sword_hitbox";
        public const string DAMAGE_HITBOX_ID = "damage_hitbox";

        public const float NORMAL_WALK_SPEED = 160f;
        const int INVINCIBILITY_TIME = 1000;
        //const int KNOCKBACK_TIME = 10;
        //const float KNOCKBACK_SPEED = 8.0f;
        public const float MAP_TRANSITION_WALK_SPEED = 1.0f;

        public override TileCollision ObstacleTileCollisions
        {
            get
            {
                if (stateHandler.CanLeaveArea)
                    return TileCollision.Wall;
                else
                    return TileCollision.Wall | TileCollision.Doorway;
            }
        }
        public override bool IsBlockedByObstacleEntities { get { return true; } }
        public override int Damage { get { return 1; } }
        public override int MaxHealth { get { return 8; } }
        public override bool CanLeaveArea { get { return stateHandler.CanLeaveArea; } }
        public bool CanWalk { get { return stateHandler.CanWalk; } }
        public bool CanGetHurt { get { return stateHandler.CanGetHurt && !isInvincible; } }
        public float WalkSpeed { get { return stateHandler.WalkSpeed; } }
        public PlayerState State { get { return stateHandler.State; } }
        public PlayerStateHandler StateHandler { get { return stateHandler; } }
        public PlayerSpriteHandler SpriteHandler { get { return (PlayerSpriteHandler)spriteHandler; } }
        public MovementHandler MovementHandler { get { return movementHandler; } }
        public Inventory Inventory { get { return inventory; } }
        public bool IsInvincible { get { return isInvincible; } }
        public PlayerInput Input { get { return input; } }

        private SoundEffect swordSwingSound;
        private SoundEffect swordTink;
        private SoundEffect hitSound;
        private SoundEffect arrowFireSound;

        private Inventory inventory;
        private bool isInvincible = false;
        //public bool IsReadyToPush = false;
        //private bool isTryingToInteract = false;
        //public bool IsTryingToInteract { get { return isTryingToInteract; } }
        private PlayerStateHandler stateHandler;
        private MovementHandler movementHandler;

        private PlayerInput input;

        //private bool isKnockedBack = false;
        //public bool IsKnockedBack = false;
        //public bool IsAiming = false;
        private int invincibilityTimer = 0;
        //private int knockBackTimer = 0;
        //public int StartToPushTimer = 0;
        private EquippableItem equippableItemBeingUsed = null;
        private bool isEntering = false;
        private Vector2 enterPosition = Vector2.Zero;
        //public Vector2 PreviousVelocity= Vector2.Zero;

        public Player(GameWorld game)
            : base(game, null, null)
        {
            BoundingBox.RelativeX = -11;
            BoundingBox.RelativeY = -20;
            BoundingBox.Width = 22;
            BoundingBox.Height = 22;

            HitBox swordHitBox = new HitBox(this, SWORD_HITBOX_ID);
            swordHitBox.IsActive = false;
            HitBoxes.Add(swordHitBox);

            HitBox damageHitBox = new HitBox(this, DAMAGE_HITBOX_ID);
            damageHitBox.RelativeX = -11;
            damageHitBox.RelativeY = -32;
            damageHitBox.Width = 22;
            damageHitBox.Height = 34;
            damageHitBox.IsActive = true;
            HitBoxes.Add(damageHitBox);

            input = new PlayerInput(this);
            stateHandler = new NormalStateHandler(this);
            FaceDirection = Directions4.Down;
            spriteHandler = new PlayerSpriteHandler(this);
            SpriteHandler.SetSpriteStill();
            movementHandler = new PlayerMovementHandler(this);
            movementHandler.Start();

            setHealth(MaxHealth);

            inventory = new Inventory();

            Bow bow = new Bow(this);
            inventory.CollectEquippableItem(bow);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            swordSwingSound = gameWorld.Content.Load<SoundEffect>("Audio/Player/sword_swing");
            swordTink = gameWorld.Content.Load<SoundEffect>("Audio/sword_tink");
            hitSound = gameWorld.Content.Load<SoundEffect>("Audio/player_hit");
            arrowFireSound = gameWorld.Content.Load<SoundEffect>("Audio/arrow_fire");
        }

        public override bool ActivatesPressureSwitch()
        {
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            //if (game.IsMapTransitioning || isEntering)
            //{
            //    SpriteHandler.Update(gameTime);
            //    Position += Velocity;

            //    if (isEntering)
            //    {
            //        if ((Velocity.X != 0 && (Math.Abs(Position.X - enterPosition.X) > ENTER_DISTANCE)) ||
            //            (Velocity.Y != 0 && (Math.Abs(Position.Y - enterPosition.Y) > ENTER_DISTANCE)))
            //        {
            //            isEntering = false;
            //            Velocity = Vector2.Zero;
            //        }
            //    }

            //    return;
            //}

            if (isInvincible)
            {
                invincibilityTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (invincibilityTimer >= INVINCIBILITY_TIME)
                {
                    isInvincible = false;
                    invincibilityTimer = 0;

                    SpriteHandler.StopBlinking();
                }
            }
            //if (isKnockedBack)
            //{
            //    knockBackTimer++;
            //    if (isKnockedBack && knockBackTimer >= KNOCKBACK_TIME)
            //    {
            //        isKnockedBack = false;
            //        //Velocity = Vector2.Zero;
            //        knockBackTimer = 0;
            //    }
            //}

            input.Update();

            //if (CanWalk)
            //{
            //    updateMovement();

            //    //pushAroundWalls();
            //}
            updateMovement(gameTime);

            SpriteHandler.Update(gameTime);

            input.Handle();

            stateHandler.Update(gameTime);
        }

        private void updateMovement(GameTime gameTime)
        {
            //Vector2 motion = DirectionsHelper.GetDirectionVector(input.InputDirection);

            //Velocity.X = motion.X * (IsAiming ? AIMING_WALK_SPEED : NORMAL_WALK_SPEED);
            //Velocity.Y = motion.Y * (IsAiming ? AIMING_WALK_SPEED : NORMAL_WALK_SPEED);
            movementHandler.Update(gameTime);

            if (stateHandler.CanChangeFaceDirection)
            {
                updateFaceDirection();
            }
        }

        //private void pushAroundWalls()
        //{
        //    // push player around impassable tiles
        //    Vector2 pushVel1 = Vector2.Zero;
        //    Vector2 pushVel2 = Vector2.Zero;
        //    Vector2 expectedPosition1 = Vector2.Zero;
        //    Vector2 expectedPosition2 = Vector2.Zero;
        //    Vector2 expectedPosition3 = Vector2.Zero;
        //    bool expectedPosition1Collides = false;
        //    bool expectedPosition2Collides = false;
        //    bool expectedPosition3Collides = false;
        //    List<Entity> entitiesToCheck = new List<Entity>();
        //    bool needToCheck = false;

        //    if (Velocity.X > 0 && Velocity.Y == 0)
        //    {
        //        expectedPosition1 = new Vector2(BoundingBox.ActualX + Width + 1, BoundingBox.ActualY);
        //        expectedPosition2 = new Vector2(BoundingBox.ActualX + Width + 1, Center.Y);
        //        expectedPosition3 = new Vector2(BoundingBox.ActualX + Width + 1, BoundingBox.ActualY + Height - 1);
        //        pushVel1 = new Vector2(0, 2);
        //        pushVel2 = new Vector2(0, -2);
        //        entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathX(this));
        //        needToCheck = true;
        //    }
        //    else if (Velocity.X < 0 && Velocity.Y == 0)
        //    {
        //        expectedPosition1 = new Vector2(BoundingBox.ActualX - 1, BoundingBox.ActualY);
        //        expectedPosition2 = new Vector2(BoundingBox.ActualX - 1, Center.Y);
        //        expectedPosition3 = new Vector2(BoundingBox.ActualX - 1, BoundingBox.ActualY + Height - 1);
        //        pushVel1 = new Vector2(0, 2);
        //        pushVel2 = new Vector2(0, -2);
        //        entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathX(this));
        //        needToCheck = true;
        //    }
        //    else if (Velocity.X == 0 && Velocity.Y > 0)
        //    {
        //        expectedPosition1 = new Vector2(BoundingBox.ActualX, BoundingBox.ActualY + Height + 1);
        //        expectedPosition2 = new Vector2(Center.X, BoundingBox.ActualY + Height + 1);
        //        expectedPosition3 = new Vector2(BoundingBox.ActualX + Width - 1, BoundingBox.ActualY + Height + 1);
        //        pushVel1 = new Vector2(2, 0);
        //        pushVel2 = new Vector2(-2, 0);
        //        entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathY(this));
        //        needToCheck = true;
        //    }
        //    else if (Velocity.X == 0 && Velocity.Y < 0)
        //    {
        //        expectedPosition1 = new Vector2(BoundingBox.ActualX, BoundingBox.ActualY - 1);
        //        expectedPosition2 = new Vector2(Center.X, BoundingBox.ActualY - 1);
        //        expectedPosition3 = new Vector2(BoundingBox.ActualX + Width - 1, BoundingBox.ActualY - 1);
        //        pushVel1 = new Vector2(2, 0);
        //        pushVel2 = new Vector2(-2, 0);
        //        entitiesToCheck.AddRange(game.CurrentArea.GetObstacleEntitiesInCollisionPathY(this));
        //        needToCheck = true;
        //    }
        //    if (needToCheck)
        //    {
        //        expectedPosition1Collides = game.CurrentArea.GetCollisionAtCell(Area.ConvertPositionToCell(expectedPosition1)) == TileCollision.Wall;
        //        expectedPosition2Collides = game.CurrentArea.GetCollisionAtCell(Area.ConvertPositionToCell(expectedPosition2)) == TileCollision.Wall;
        //        expectedPosition3Collides = game.CurrentArea.GetCollisionAtCell(Area.ConvertPositionToCell(expectedPosition3)) == TileCollision.Wall;

        //        foreach (Entity entity in entitiesToCheck)
        //        {
        //            expectedPosition1Collides = expectedPosition1Collides || entity.Contains(expectedPosition1);
        //            expectedPosition2Collides = expectedPosition2Collides || entity.Contains(expectedPosition2);
        //            expectedPosition3Collides = expectedPosition3Collides || entity.Contains(expectedPosition3);
        //        }

        //        if (expectedPosition1Collides && !expectedPosition2Collides && !expectedPosition3Collides)
        //            Velocity += pushVel1;
        //        else if (!expectedPosition1Collides && !expectedPosition2Collides && expectedPosition3Collides)
        //            Velocity += pushVel2;
        //    }
        //}

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

        public void StartLifting(CarriableEntity carriableEntity)
        {
            EnterState(new CarryingStateHandler(this, carriableEntity));
        }

        public void StartGrabbing(MovableBlock movableBlock)
        {
            EnterState(new GrabbingStateHandler(this, movableBlock));
        }

        public void StartOpening(Chest chest)
        {
            EnterState(new OpeningChestStateHandler(this, chest));
        }

        public void StartAttacking()
        {
            EnterState(new AttackingStateHandler(this));
        }

        public void StartUsingBow(int itemButtonNumber)
        {
            EnterState(new UsingBowStateHandler(this, itemButtonNumber));
        }

        public void StartLeavingMap(MapTransition mapTransition)
        {
            EnterState(new LeavingMapStateHandler(this, mapTransition));
        }

        public void StartEnteringArea(Directions direction)
        {
            isEntering = true;
            enterPosition = new Vector2(Position.X, Position.Y);

            //Velocity = Vector2.Zero;
            //if (direction == Directions.Up)
            //    Velocity.Y = -NORMAL_WALK_SPEED;
            //else if (direction == Directions.Down)
            //    Velocity.Y = NORMAL_WALK_SPEED;
            //else if (direction == Directions.Left)
            //    Velocity.X = -NORMAL_WALK_SPEED;
            //else if (direction == Directions.Right)
            //    Velocity.X = NORMAL_WALK_SPEED;
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            base.Draw(spriteBatch, changeColorsEffect);

            //spriteBatch.DrawString(gameWorld.Font, State.ToString(), new Vector2(Position.X - 30, Position.Y - 70), Color.White);
            //spriteBatch.DrawString(gameWorld.Font, Position.ToString(), new Vector2(Position.X - 30, Position.Y - 90), Color.White);

            //foreach (HitBox hitBox in GetActiveHitBoxes())
            //{
            //    spriteBatch.Draw(game.SquareTexture,
            //        hitBox.ToRectangle(),
            //        Color.Red);
            //}
        }


        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            if (thisHitBox.IsId(SWORD_HITBOX_ID))
            {
                if (other is Enemy)
                {
                    Enemy enemy = (Enemy)other;
                    if (enemy.TakesDamageFromPlayerSword(otherHitBox))
                        enemy.TakeDamage(this, KnockBackType.HitAngle);
                }
                if (other is Breakable && otherHitBox.IsId(BOUNDING_BOX_ID))
                {
                    Breakable breakable = (Breakable)other;
                    if (breakable.CanBeBroken)
                        breakable.StartBreaking();
                }
            }

            else if (thisHitBox.IsId(DAMAGE_HITBOX_ID))
            {
                KnockBackType knockBackType;
                if (other.DamagesPlayer(otherHitBox, out knockBackType) && CanGetHurt)
                {
                    TakeDamage(other, knockBackType);
                }
            }
        }

        public void CollectPickup(Pickup pickup)
        {
            if (pickup.PickupType == PickupType.Heart)
            {
                increaseHealth(pickup.Value);
            }
            else
            {
                inventory.CollectPickup(pickup);
            }
        }

        public override void TakeDamage(Entity other, KnockBackType knockBackType)
        {
            base.TakeDamage(other, knockBackType);

            EnterState(new HurtStateHandler(this, other, knockBackType));
        }

        public void StartInvincibility()
        {
            isInvincible = true;
            invincibilityTimer = 0;

            SpriteHandler.StartBlinking();
        }

        //public void KnockBack(Entity entity)
        //{
        //    if (!isKnockedBack)
        //    {
        //        isKnockedBack = true;
        //        knockBackTimer = 0;

        //        Vector2 direction;
        //        if (entity is Spikes)
        //            direction = ((Spikes)entity).GetKnockBackDirection();
        //        else
        //            direction = this.Center - entity.Center;

        //        float angle = (float)Math.Atan2(direction.Y, direction.X);
        //        Velocity.X = (float)Math.Cos(angle) * KNOCKBACK_SPEED;
        //        Velocity.Y = (float)Math.Sin(angle) * KNOCKBACK_SPEED;

        //        if (entity is RobotEnemy && State == PlayerState.Attacking)
        //        {
        //            swordTink.Play(0.75f, 0, 0);
        //        }
        //        else
        //        {
        //            spriteHandler.SetSpriteStill();
        //        }
        //        SpriteHandler.SetSpriteStill();
        //    }
        //}

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
            Vector2 inFrontPosition = Vector2.Zero;
            inFrontPosition.X = this.Center.X + (DirectionsHelper.GetDirectionVector(this.FaceDirection).X * (this.Width / 2 + 1));
            inFrontPosition.Y = this.Center.Y + (DirectionsHelper.GetDirectionVector(this.FaceDirection).Y * (this.Height / 2 + 1));

            return entity.Contains(inFrontPosition);
        }

        public Interactable GetInteractableEntityInFrontOf()
        {
            foreach (Entity entity in gameWorld.CurrentArea.Entities)
            {
                if (entity is Interactable && IsInFrontOf(entity))
                    return (Interactable)entity;
            }
            return null;
        }

        public void EnterState(PlayerStateHandler newStateHandler)
        {
            this.stateHandler.End(newStateHandler.State);
            this.stateHandler = newStateHandler;
            this.stateHandler.Start();
        }

        public void StartMovement(MovementHandler newMovementHandler)
        {
            this.movementHandler = newMovementHandler;
            this.movementHandler.Start();
        }

        public void OnButtonPressed(PlayerButtons button)
        {
            stateHandler.OnButtonPressed(button);

            if (button == PlayerButtons.Interact && stateHandler.CanInteract)
            {
                Interactable interactable = GetInteractableEntityInFrontOf();
                if (interactable != null && interactable.CanStartInteraction)
                {
                    if ((interactable.MustBeAllignedWithToInteract && IsAllignedWith((Entity)interactable)) || 
                        !interactable.MustBeAllignedWithToInteract)
                        interactable.StartInteraction();
                }
            }
            if (button == PlayerButtons.Attack && stateHandler.CanStartAttacking)
            {
                StartAttacking();
            }
            if (button == PlayerButtons.EquippedItem1 || button == PlayerButtons.EquippedItem2)
            {
                int itemButtonNumber = button == PlayerButtons.EquippedItem1 ? 0 : 1;
                EquippableItem equippableItem = inventory.EquippedItemAtIndex(itemButtonNumber);

                if (equippableItem != null && stateHandler.CanStartUsingEquippableItem(equippableItem))
                    equippableItem.StartUsing(itemButtonNumber);
            }
        }

        public void OnButtonReleased(PlayerButtons button)
        {
            stateHandler.OnButtonReleased(button);
        }

        public void PlaySwordSwingSound()
        {
            swordSwingSound.Play(0.5f, 0, 0);
        }

        public void PlayHitSound()
        {
            hitSound.Play(0.75f, 0f, 0f);
        }

        public void PlayArrowFireSound()
        {
            arrowFireSound.Play(0.9f, 0f, 0f);
        }
    }
}
