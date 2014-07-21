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
        const float WALK_SPEED = 3.0f;
        const float AIMING_WALK_SPEED = 1.6f;
        const int WALK_ANIMATION_DELAY = 6;
        public const int ATTACK_ANIMATION_DELAY = 2;
        public const int ATTACK_TIME = (ATTACK_ANIMATION_DELAY + 1) * 3 - 1;
        const int INVINCIBILITY_TIME = 60;
        const int KNOCKBACK_TIME = 10;
        const float KNOCKBACK_SPEED = 8.0f;
        const int BLINK_DELAY = 2;
        public const int SWORD_LENGTH = 38;
        const int SWORD_HITBOX_LENGTH = 6;
        const int MAX_HEALTH = 8;
        const int DAMAGE = 1;
        public const int START_TO_PUSH_TIME = 30;
        const int ENTER_DISTANCE = 80;
        public const float PUSH_SPEED = 1.0f;
        public const float MAP_TRANSITION_WALK_SPEED = 1.0f;

        private const String FORWARD_SPRITE_IMAGE_NAME = "Sprites/Player/player_forward_still2";
        private const String FORWARD_WALKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_forward_walking2";
        private const String BACKWARD_SPRITE_IMAGE_NAME = "Sprites/Player/player_backward_still2";
        private const String BACKWARD_WALKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_backward_walking2";
        private const String LEFT_SPRITE_IMAGE_NAME = "Sprites/Player/player_left_still";
        private const String LEFT_WALKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_left_walking";
        private const String RIGHT_SPRITE_IMAGE_NAME = "Sprites/Player/player_right_still";
        private const String RIGHT_WALKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_right_walking";
        private const String FORWARD_ATTACKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_forward_sword2";
        private const String BACKWARD_ATTACKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_backward_sword2";
        private const String LEFT_ATTACKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_left_sword";
        private const String RIGHT_ATTACKING_SPRITE_IMAGE_NAME = "Sprites/Player/player_right_sword";

        //private AnimatedSprite forwardSprite;
        //private AnimatedSprite forwardWalkingSprite;
        //private AnimatedSprite backwardSprite;
        //private AnimatedSprite backwardWalkingSprite;
        //private AnimatedSprite leftSprite;
        //private AnimatedSprite leftWalkingSprite;
        //private AnimatedSprite rightSprite;
        //private AnimatedSprite rightWalkingSprite;
        //private AnimatedSprite forwardAttackingSprite;
        //private AnimatedSprite backwardAttackingSprite;
        //private AnimatedSprite leftAttackingSprite;
        //private AnimatedSprite rightAttackingSprite;

        private Sprite[] standingSprites = new Sprite[4];
        private Sprite[] walkingSprites = new Sprite[4];
        private Sprite[] attackingSprites = new Sprite[4];

        public SoundEffect SwordSwingSound;
        private SoundEffect swordTink;
        public SoundEffect HitSound;

        public Rectangle SwordHitBox
        {
            get
            {
                return new Rectangle((int)Math.Round(SwordHitPoint.X) - (SWORD_HITBOX_LENGTH / 2),
                    (int)Math.Round(SwordHitPoint.Y) - (SWORD_HITBOX_LENGTH / 2), SWORD_HITBOX_LENGTH, SWORD_HITBOX_LENGTH);
            }
        }

        public Vector2 FootPosition
        {
            get
            {
                return new Vector2(HitBoxPosition.X + (Width / 2), HitBoxPosition.Y + Height);
            }
        }

        public bool CanWalk { get { return stateHandler.CanWalk && !isKnockedBack; } }

        public bool CanGetHurt { get { return stateHandler.CanGetHurt && !isInvincible; } }

        private Inventory inventory;
        public Inventory Inventory { get { return inventory; } }
        private bool isInvincible = false;
        public bool IsInvincible { get { return isInvincible; } }
        public bool IsReadyToPush = false;
        //public bool IsReadyToPush { get { return isReadyToPush; } }
        private bool isTryingToInteract = false;
        public bool IsTryingToInteract { get { return isTryingToInteract; } }
        private PlayerStateHandler stateHandler;
        public PlayerStateHandler StateHandler { get { return stateHandler; } }
        public PlayerState State { get { return stateHandler.State; } }

        private GamePadState gamepadState;
        private GamePadState previousGamepadState;
        public Vector2 SwordHitPoint = Vector2.Zero;
        public Vector2 ForwardSwordOrigin = new Vector2(20, 18);
        public Vector2 BackwardSwordOrigin = new Vector2(1, 13);
        public Vector2 LeftSwordOrigin = new Vector2(10, 17);
        public Vector2 RightSwordOrigin = new Vector2(11, 17);
        private bool isKnockedBack = false;
        public bool IsKnockedBack = false;
        public bool IsAiming = false;
        private int invincibilityTimer = 0;
        private int knockBackTimer = 0;
        public int StartToPushTimer = 0;
        private MovableBlock blockBeingPushed = null;
        private CarriableEntity entityBeingCarried = null;
        private EquippableItem equippableItemBeingUsed = null;
        private Chest chestBeingOpened = null;
        private bool isEntering = false;
        private Vector2 enterPosition = Vector2.Zero;
        public Vector2 PreviousVelocity= Vector2.Zero;

        public Player(GameWorld game)
            : base(game, null)
        {
            hitBoxOffset = new Vector2(-11, -23);
            hitBoxWidth = 22;
            hitBoxHeight = 25;

            Vector2 origin = new Vector2(16, 38);
            standingSprites[0] = new Sprite(origin);
            walkingSprites[0] = new Sprite(origin, 4, WALK_ANIMATION_DELAY);
            standingSprites[1] = new Sprite(origin);
            walkingSprites[1] = new Sprite(origin, 4, WALK_ANIMATION_DELAY);
            standingSprites[2] = new Sprite(origin);
            walkingSprites[2] = new Sprite(origin, 4, WALK_ANIMATION_DELAY);
            standingSprites[3] = new Sprite(origin);
            walkingSprites[3] = new Sprite(origin, 4, WALK_ANIMATION_DELAY);

            origin = new Vector2(16, 58);
            attackingSprites[0] = new Sprite(origin, 3, ATTACK_ANIMATION_DELAY);

            origin = new Vector2(36, 38);
            attackingSprites[1] = new Sprite(origin, 3, ATTACK_ANIMATION_DELAY);

            origin = new Vector2(44, 58);
            attackingSprites[2] = new Sprite(origin, 3, ATTACK_ANIMATION_DELAY);

            origin = new Vector2(16, 58);
            attackingSprites[3] = new Sprite(origin, 3, ATTACK_ANIMATION_DELAY);

            stateHandler = new NormalStateHandler(this);
            FaceDirection = Directions.Down;
            CurrentSprite = GetStandingSprite(FaceDirection);

            MaxHealth = MAX_HEALTH;
            Health = MaxHealth;
            Damage = DAMAGE;

            gamepadState = GamePad.GetState(PlayerIndex.One);
            previousGamepadState = gamepadState;

            inventory = new Inventory();

            Bow bow = new Bow(game, null);
            bow.LoadContent();
            inventory.CollectEquippableItem(bow);
            inventory.EquipItem(bow, Buttons.B);
        }

        public Sprite GetStandingSprite(Directions direction)
        {
            return standingSprites[(int)direction];
        }

        public Sprite GetWalkingSprite(Directions direction)
        {
            return walkingSprites[(int)direction];
        }

        public Sprite GetAttackingSprite(Directions direction)
        {
            return attackingSprites[(int)direction];
        }

        public bool IsStandingSprite(Sprite sprite)
        {
            return standingSprites.Contains(sprite);
        }

        public bool IsWalkingSprite(Sprite sprite)
        {
            return walkingSprites.Contains(sprite);
        }

        public bool IsAttackingSprite(Sprite sprite)
        {
            return attackingSprites.Contains(sprite);
        }

        public override void LoadContent()
        {
            standingSprites[0].Texture = game.Content.Load<Texture2D>(BACKWARD_SPRITE_IMAGE_NAME);
            walkingSprites[0].Texture = game.Content.Load<Texture2D>(BACKWARD_WALKING_SPRITE_IMAGE_NAME);
            attackingSprites[0].Texture = game.Content.Load<Texture2D>(BACKWARD_ATTACKING_SPRITE_IMAGE_NAME);
            standingSprites[1].Texture = game.Content.Load<Texture2D>(FORWARD_SPRITE_IMAGE_NAME);
            walkingSprites[1].Texture = game.Content.Load<Texture2D>(FORWARD_WALKING_SPRITE_IMAGE_NAME);
            attackingSprites[1].Texture = game.Content.Load<Texture2D>(FORWARD_ATTACKING_SPRITE_IMAGE_NAME);
            standingSprites[2].Texture = game.Content.Load<Texture2D>(LEFT_SPRITE_IMAGE_NAME);
            walkingSprites[2].Texture = game.Content.Load<Texture2D>(LEFT_WALKING_SPRITE_IMAGE_NAME);
            attackingSprites[2].Texture = game.Content.Load<Texture2D>(LEFT_ATTACKING_SPRITE_IMAGE_NAME);
            standingSprites[3].Texture = game.Content.Load<Texture2D>(RIGHT_SPRITE_IMAGE_NAME);
            walkingSprites[3].Texture = game.Content.Load<Texture2D>(RIGHT_WALKING_SPRITE_IMAGE_NAME);
            attackingSprites[3].Texture = game.Content.Load<Texture2D>(RIGHT_ATTACKING_SPRITE_IMAGE_NAME);

            SwordSwingSound = game.Content.Load<SoundEffect>("Audio/Player/sword_swing");
            swordTink = game.Content.Load<SoundEffect>("Audio/sword_tink");
            HitSound = game.Content.Load<SoundEffect>("Audio/player_hit");
        }

        public override void Update()
        {
            if (game.IsMapTransitioning || isEntering)
            {
                CurrentSprite.UpdateAnimation();
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

            gamepadState = GamePad.GetState(PlayerIndex.One);

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
            if (CanWalk)
            {
                updateMovement();

                pushAroundWalls();
            }

            handleInput();

            checkReadyToPush();


            CanLeaveArea = State == PlayerState.Normal;

            stateHandler.Update(gamepadState, previousGamepadState);

            //if (State == PlayerState.Attacking)
            //{
            //    doAttack();
            //}
            //if (State == PlayerState.Pushing)
            //{
            //    doPush();
            //}
            //if (State == PlayerState.Carrying)
            //{
            //    doCarry();
            //}
            //if (State == PlayerState.UsingBow)
            //{
            //    useEquippableItem();
            //}
            //if (State == PlayerState.OpeningChest)
            //{
            //    doOpenChest();
            //}

            base.Update();

            previousGamepadState = gamepadState;
        }

        private void checkReadyToPush()
        {
            // update startToPushTimer if player is pushing against wall and check if player is ready to push
            if (JustCollidedWithWall)
            {
                if ((Velocity.Y == 0f && PreviousVelocity.Y == 0f && Velocity.X != 0f && Velocity.X == PreviousVelocity.X) ||
                    (Velocity.X == 0f && PreviousVelocity.X == 0f && Velocity.Y != 0f && Velocity.Y == PreviousVelocity.Y))
                    StartToPushTimer++;
                else
                    StartToPushTimer = 0;
            }
            else
            {
                StartToPushTimer = 0;
            }

            IsReadyToPush = StartToPushTimer >= START_TO_PUSH_TIME ? true : false;
            PreviousVelocity = new Vector2(Velocity.X, Velocity.Y);
        }

        private void handleInput()
        {
            if (stateHandler.CanStartAttacking)
            {
                if (gamepadState.IsButtonDown(Buttons.X) && previousGamepadState.IsButtonUp(Buttons.X))
                {
                    //startAttack();
                    EnterState(new AttackingStateHandler(this));
                }
            }

            // handle input for equippable items
            foreach (Buttons button in Inventory.EQUIPPED_ITEM_BUTTONS)
            {
                EquippableItem equippedItem = inventory.GetEquippedItemForButton(button);

                if (stateHandler.CanStartUsingEquippableItem(equippedItem) && gamepadState.IsButtonDown(button) && equippedItem != null)
                {
                    startUsingEquippableItem(equippedItem);
                }

                if (State == PlayerState.UsingBow && gamepadState.IsButtonUp(button) && equippedItem == equippableItemBeingUsed)
                {
                    if (equippedItem is Bow)
                    {
                        Bow bow = (Bow)equippedItem;
                        bow.FireArrow();
                    }
                }
            }

            isTryingToInteract = false;
            if (stateHandler.CanInteract && gamepadState.IsButtonDown(Buttons.A) && previousGamepadState.IsButtonUp(Buttons.A))
            {
                isTryingToInteract = true;
            }
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
                expectedPosition1 = new Vector2(HitBoxPosition.X + Width + 1, HitBoxPosition.Y);
                expectedPosition2 = new Vector2(HitBoxPosition.X + Width + 1, Center.Y);
                expectedPosition3 = new Vector2(HitBoxPosition.X + Width + 1, HitBoxPosition.Y + Height - 1);
                pushVel1 = new Vector2(0, 2);
                pushVel2 = new Vector2(0, -2);
                entitiesToCheck.AddRange(game.CurrentArea.GetImpassableEntitiesInCollisionPathX(this));
                needToCheck = true;
            }
            else if (Velocity.X < 0 && Velocity.Y == 0)
            {
                expectedPosition1 = new Vector2(HitBoxPosition.X - 1, HitBoxPosition.Y);
                expectedPosition2 = new Vector2(HitBoxPosition.X - 1, Center.Y);
                expectedPosition3 = new Vector2(HitBoxPosition.X - 1, HitBoxPosition.Y + Height - 1);
                pushVel1 = new Vector2(0, 2);
                pushVel2 = new Vector2(0, -2);
                entitiesToCheck.AddRange(game.CurrentArea.GetImpassableEntitiesInCollisionPathX(this));
                needToCheck = true;
            }
            else if (Velocity.X == 0 && Velocity.Y > 0)
            {
                expectedPosition1 = new Vector2(HitBoxPosition.X, HitBoxPosition.Y + Height + 1);
                expectedPosition2 = new Vector2(Center.X, HitBoxPosition.Y + Height + 1);
                expectedPosition3 = new Vector2(HitBoxPosition.X + Width - 1, HitBoxPosition.Y + Height + 1);
                pushVel1 = new Vector2(2, 0);
                pushVel2 = new Vector2(-2, 0);
                entitiesToCheck.AddRange(game.CurrentArea.GetImpassableEntitiesInCollisionPathY(this));
                needToCheck = true;
            }
            else if (Velocity.X == 0 && Velocity.Y < 0)
            {
                expectedPosition1 = new Vector2(HitBoxPosition.X, HitBoxPosition.Y - 1);
                expectedPosition2 = new Vector2(Center.X, HitBoxPosition.Y - 1);
                expectedPosition3 = new Vector2(HitBoxPosition.X + Width - 1, HitBoxPosition.Y - 1);
                pushVel1 = new Vector2(2, 0);
                pushVel2 = new Vector2(-2, 0);
                entitiesToCheck.AddRange(game.CurrentArea.GetImpassableEntitiesInCollisionPathY(this));
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
            Vector2 motion = Vector2.Zero;
            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickUp))
                motion.Y--;
            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickDown))
                motion.Y++;
            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickLeft))
                motion.X--;
            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickRight))
                motion.X++;

            if (motion != Vector2.Zero)
                motion.Normalize();

            Velocity.X = motion.X * (IsAiming ? AIMING_WALK_SPEED : WALK_SPEED);
            Velocity.Y = motion.Y * (IsAiming ? AIMING_WALK_SPEED : WALK_SPEED);

            if (!IsAiming)
            {
                updateFaceDirection();
            }
            else if (IsAiming)
            {
                if (Velocity != Vector2.Zero)
                {
                    //if (CurrentSprite == backwardSprite)
                    //    CurrentSprite = backwardWalkingSprite;
                    //else if (CurrentSprite == forwardSprite)
                    //    CurrentSprite = forwardWalkingSprite;
                    //else if (CurrentSprite == leftSprite)
                    //    CurrentSprite = leftWalkingSprite;
                    //else if (CurrentSprite == rightSprite)
                    //    CurrentSprite = rightWalkingSprite;
                    if (IsStandingSprite(CurrentSprite))
                        CurrentSprite = GetWalkingSprite(FaceDirection);
                }
            }

            if (Velocity == Vector2.Zero)
            {
                //if (CurrentSprite == backwardWalkingSprite)
                //    CurrentSprite = backwardSprite;
                //else if (CurrentSprite == forwardWalkingSprite)
                //    CurrentSprite = forwardSprite;
                //else if (CurrentSprite == leftWalkingSprite)
                //    CurrentSprite = leftSprite;
                //else if (CurrentSprite == rightWalkingSprite)
                //    CurrentSprite = rightSprite;
                if (IsWalkingSprite(CurrentSprite))
                    CurrentSprite = GetStandingSprite(FaceDirection);
            }
        }

        private void updateFaceDirection()
        {
            if (Velocity.Y < 0 && Velocity.X == 0 && FaceDirection != Directions.Up)
            {
                FaceDirection = Directions.Up;
            }
            else if (Velocity.Y > 0 && Velocity.X == 0 && FaceDirection != Directions.Down)
            {
                FaceDirection = Directions.Down;
            }
            else if ((Velocity.X < 0 && Velocity.Y == 0 && FaceDirection != Directions.Left) ||
                (Velocity.X < 0 && Velocity.Y < 0 && FaceDirection != Directions.Left && FaceDirection != Directions.Up) ||
                (Velocity.X < 0 && Velocity.Y > 0 && FaceDirection != Directions.Left && FaceDirection != Directions.Down))
            {
                FaceDirection = Directions.Left;
            }
            else if ((Velocity.X > 0 && Velocity.Y == 0 && FaceDirection != Directions.Right) ||
                (Velocity.X > 0 && Velocity.Y < 0 && FaceDirection != Directions.Right && FaceDirection != Directions.Up) ||
                (Velocity.X > 0 && Velocity.Y > 0 && FaceDirection != Directions.Right && FaceDirection != Directions.Down))
            {
                FaceDirection = Directions.Right;
            }

            if (Velocity != Vector2.Zero && CurrentSprite != GetWalkingSprite(FaceDirection))
            {
                CurrentSprite = GetWalkingSprite(FaceDirection);
            }
        }

        private void doOpenChest()
        {
            if (chestBeingOpened.IsOpened)
            {
                endOpeningChest();
            }
        }

        private void doCarry()
        {
            entityBeingCarried.Update();
            if (gamepadState.IsButtonDown(Buttons.A) && previousGamepadState.IsButtonUp(Buttons.A))
            {
                startThrowing();
            }
        }

        private void useEquippableItem()
        {
            Entity equippableItemBeingUsedEntity = (Entity)equippableItemBeingUsed;
            equippableItemBeingUsedEntity.Update();

            if (equippableItemBeingUsed.IsDoneBeingUsed)
            {
                stopUsingEquippableItem();
            }
        }

        private void stopUsingEquippableItem()
        {
            if (equippableItemBeingUsed is Bow)
            {
                IsAiming = false;
            }

            //state = PlayerState.Normal;
            equippableItemBeingUsed = null;
        }

        private void startUsingEquippableItem(EquippableItem equippableItem)
        {
            //state = PlayerState.UsingBow;
            //equippableItemBeingUsed = equippableItem;
            //equippableItem.StartBeingUsed();

            if (equippableItem is Bow)
            {
                //IsAiming = true;
                EnterState(new UsingBowStateHandler(this), (Bow)equippableItem);
            }
        }

        private void startThrowing()
        {
            entityBeingCarried.StartBeingThrown(FaceDirection);
            game.CurrentArea.Entities.Add(entityBeingCarried);
            entityBeingCarried = null;
            //state = PlayerState.Normal;
        }

        public void StartLifting(CarriableEntity entity)
        {
            EnterState(new CarryingStateHandler(this), entity);
            //entityBeingCarried = entity;
            //entityBeingCarried.StartBeingLifted();
            //game.CurrentArea.Entities.Remove(entityBeingCarried);
            //state = PlayerState.Carrying;
        }

        public void StartPushing(MovableBlock block)
        {
            EnterState(new PushingStateHandler(this), block);
            //blockBeingPushed = block;
            //Velocity = Entity.GetDirectionVector(FaceDirection) * PUSH_SPEED;

            //blockBeingPushed.StartBeingPushed(FaceDirection);
            //state = PlayerState.Pushing;
            //IsReadyToPush = false;
        }

        public void StartOpening(Chest chest)
        {
            EnterState(new OpeningChestStateHandler(this), chest);
            //chestBeingOpened = chest;
            //chestBeingOpened.StartOpening();
            //state = PlayerState.OpeningChest;
        }

        private void endOpeningChest()
        {
            if (chestBeingOpened.Treasure != null)
            {
                Entity treasure = chestBeingOpened.Treasure;

                if (treasure is Pickup)
                    inventory.CollectPickup((Pickup)treasure);
                else if (treasure is EquippableItem)
                    inventory.CollectEquippableItem((EquippableItem)treasure);
            }

            chestBeingOpened = null;
            //state = PlayerState.Normal;
        }

        private void doPush()
        {
            if (blockBeingPushed.ReachedPushDestination())
                endPush();
        }

        private void endPush()
        {
            blockBeingPushed.EndPush();

            if (FaceDirection == Directions.Left)
                HitBoxPositionX = blockBeingPushed.HitBoxPosition.X + blockBeingPushed.Width;
            else if (FaceDirection == Directions.Right)
                HitBoxPositionX = blockBeingPushed.HitBoxPosition.X - Width;
            else if (FaceDirection == Directions.Up)
                HitBoxPositionY = blockBeingPushed.HitBoxPosition.Y + blockBeingPushed.Height;
            else if (FaceDirection == Directions.Down)
                HitBoxPositionY = blockBeingPushed.HitBoxPosition.Y - Height;

            Velocity = Vector2.Zero;
            StartToPushTimer = 0;
            blockBeingPushed = null;
            //state = PlayerState.Normal;
            IsReadyToPush = false;
        }

        private void startAttack()
        {
            //state = PlayerState.Attacking;
            //AttackTimer = 0;
            //Velocity = Vector2.Zero;

            //if (FaceDirection == Directions.Down)
            //{
            //    SwordHitPoint = new Vector2(HitBoxPosition.X + ForwardSwordOrigin.X - SWORD_LENGTH, HitBoxPosition.Y + ForwardSwordOrigin.Y);
            //}
            //else if (FaceDirection == Directions.Up)
            //{
            //    SwordHitPoint = new Vector2(HitBoxPosition.X + BackwardSwordOrigin.X + SWORD_LENGTH, HitBoxPosition.Y + BackwardSwordOrigin.Y);
            //}
            //else if (FaceDirection == Directions.Left)
            //{
            //    SwordHitPoint = new Vector2(HitBoxPosition.X + LeftSwordOrigin.X, HitBoxPosition.Y + LeftSwordOrigin.Y - SWORD_LENGTH);
            //}
            //else if (FaceDirection == Directions.Right)
            //{
            //    SwordHitPoint = new Vector2(HitBoxPosition.X + RightSwordOrigin.X, HitBoxPosition.Y + LeftSwordOrigin.Y - SWORD_LENGTH);
            //}

            //CurrentSprite = GetAttackingSprite(FaceDirection);

            //CurrentSprite.ResetAnimation();
            //SwordSwingSound.Play(0.75f, 0f, 0f);
        }

        private void doAttack()
        {
            //attackTimer++;
            //if (attackTimer >= ATTACK_TIME)
            //{
            //    endAttack();
            //}
            //else
            //{
            //    float angle = ((float)attackTimer / (float)(ATTACK_TIME - 1)) * MathHelper.PiOver2;
            //    if (FaceDirection == Directions.Left || FaceDirection == Directions.Right)
            //        angle = MathHelper.PiOver2 - angle;
            //    int x = (int)(Math.Cos(angle) * SWORD_LENGTH);
            //    int y = (int)(Math.Sin(angle) * SWORD_LENGTH);
                
            //    if (FaceDirection == Directions.Down)
            //    {
            //        SwordHitPoint = new Vector2(HitBoxPosition.X + ForwardSwordOrigin.X - x, HitBoxPosition.Y + ForwardSwordOrigin.Y + y);
            //    }
            //    else if (FaceDirection == Directions.Up)
            //    {
            //        SwordHitPoint = new Vector2(HitBoxPosition.X + BackwardSwordOrigin.X + x, HitBoxPosition.Y + BackwardSwordOrigin.Y - y);
            //    }
            //    else if (FaceDirection == Directions.Left)
            //    {
            //        SwordHitPoint = new Vector2(HitBoxPosition.X + LeftSwordOrigin.X - x, HitBoxPosition.Y + LeftSwordOrigin.Y - y);
            //    }
            //    else if (FaceDirection == Directions.Right)
            //    {
            //        SwordHitPoint = new Vector2(HitBoxPosition.X + RightSwordOrigin.X + x, HitBoxPosition.Y + LeftSwordOrigin.Y - y);
            //    }

            //}
        }

        private void endAttack()
        {
            //state = PlayerState.Normal;
            //attackTimer = 0;
            //SwordHitPoint = Vector2.Zero;

            //if (FaceDirection == Directions.Down)
            //    CurrentSprite = forwardSprite;
            //else if (FaceDirection == Directions.Up)
            //    CurrentSprite = backwardSprite;
            //else if (FaceDirection == Directions.Left)
            //    CurrentSprite = leftSprite;
            //else if (FaceDirection == Directions.Right)
            //    CurrentSprite = rightSprite;
            //CurrentSprite = GetStandingSprite(FaceDirection);
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
                base.Draw(spriteBatch, changeColorsEffect);
            //if (State == PlayerState.UsingBow && FaceDirection == Directions.Down)
            //    equippableItemBeingUsedEntity.Draw(spriteBatch, changeColorsEffect);
            //if (State == PlayerState.Carrying && entityBeingCarried != null)
            //    entityBeingCarried.Draw(spriteBatch, changeColorsEffect);

            //if (IsAttacking)
            //    spriteBatch.Draw(wallTexture, SwordHitBox, Color.Red);

            spriteBatch.DrawString(game.Font, State.ToString(), new Vector2(Position.X, Position.Y - 60), Color.White);
        }


        public override void OnEntityCollision(Entity other)
        {
            if (other is Enemy)
            {
                Enemy enemy = (Enemy)other;
                //if (!IsHurt && enemy.DoesDamageOnContact)
                if (CanGetHurt && enemy.DoesDamageOnContact)
                {
                    takeDamageFrom(other);
                }
            }

            if (other is Spikes)
            {
                Spikes spikes = (Spikes)other;
                //if (spikes.AreActivated && !IsHurt)
                if (CanGetHurt && spikes.AreActivated)
                {
                    takeDamageFrom(other);
                }
            }

            if (other is Pickup)
            {
                Pickup pickup = (Pickup)other;
                if (pickup.Type == PickupType.Heart)
                {
                    Health += pickup.Value;
                    if (Health > MaxHealth)
                        Health = MaxHealth;
                }
                else
                {
                    inventory.CollectPickup(pickup);
                }

                pickup.PlayCollectionSound();
            }

            if (other is KeyDoor)
            {
                if (IsInFrontOf(other) && inventory.NumKeys > 0)
                {
                    inventory.UseKey();
                    KeyDoor keyDoor = (KeyDoor)other;
                    keyDoor.StartOpening();
                }
            }

            if (other is EnemyProjectile)
            {
                //if (!IsHurt)
                if (CanGetHurt)
                    takeDamageFrom(other);
            }
        }

        private void takeDamageFrom(Entity other)
        {
            //isHurt = true;
            //KnockBack(other);
            //invincibilityTimer = 0;
            //Health -= other.Damage;
            //HitSound.Play(0.75f, 0, 0);
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
                if (entity is Spikes)
                    direction = ((Spikes)entity).GetKnockBackDirection();
                else
                    direction = this.Center - entity.Center;

                float angle = (float)Math.Atan2(direction.Y, direction.X);
                Velocity.X = (float)Math.Cos(angle) * KNOCKBACK_SPEED;
                Velocity.Y = (float)Math.Sin(angle) * KNOCKBACK_SPEED;

                if (entity is RobotEnemy && State == PlayerState.Attacking)
                {
                    swordTink.Play(0.75f, 0, 0);
                }
                else
                {
                    CurrentSprite = GetStandingSprite(FaceDirection);
                }
            }
        }

        public bool IsAllignedWith(Entity entity)
        {
            Vector2 expectedPosition1 = Vector2.Zero;
            Vector2 expectedPosition2 = Vector2.Zero;

            if (FaceDirection == Directions.Left)
            {
                expectedPosition1 = new Vector2(HitBoxPosition.X - 1, HitBoxPosition.Y);
                expectedPosition2 = new Vector2(HitBoxPosition.X - 1, HitBoxPosition.Y + Height - 1);
            }
            else if (FaceDirection == Directions.Right)
            {
                expectedPosition1 = new Vector2(HitBoxPosition.X + Width, HitBoxPosition.Y);
                expectedPosition2 = new Vector2(HitBoxPosition.X + Width, HitBoxPosition.Y + Height - 1);
            }
            else if (FaceDirection == Directions.Up)
            {
                expectedPosition1 = new Vector2(HitBoxPosition.X, HitBoxPosition.Y - 1);
                expectedPosition2 = new Vector2(HitBoxPosition.X + Width - 1, HitBoxPosition.Y - 1);
            }
            else if (FaceDirection == Directions.Down)
            {
                expectedPosition1 = new Vector2(HitBoxPosition.X, HitBoxPosition.Y + Height);
                expectedPosition2 = new Vector2(HitBoxPosition.X + Width - 1, HitBoxPosition.Y + Height);
            }

            return entity.HitBox.Contains(new Point((int)Math.Round(expectedPosition1.X), (int)Math.Round(expectedPosition1.Y))) &&
                entity.HitBox.Contains(new Point((int)Math.Round(expectedPosition2.X), (int)Math.Round(expectedPosition2.Y)));
        }

        public bool IsInFrontOf(Entity entity)
        {
            if (entity is Chest && FaceDirection != Directions.Up)
                return false;

            Vector2 inFrontPosition = Vector2.Zero;
            inFrontPosition.X = this.Center.X + (Entity.GetDirectionVector(this.FaceDirection).X * (this.Width / 2 + 1));
            inFrontPosition.Y = this.Center.Y + (Entity.GetDirectionVector(this.FaceDirection).Y * (this.Height / 2 + 1));

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

    }

    public enum PlayerState
    {
        Normal,
        Hurt,
        Attacking,
        Pushing,
        Carrying,
        UsingBow,
        OpeningChest
    }
}
