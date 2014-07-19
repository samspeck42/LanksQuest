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
        const int ATTACK_ANIMATION_DELAY = 2;
        const int ATTACK_TIME = (ATTACK_ANIMATION_DELAY + 1) * 3 - 1;
        const int HURT_TIME = 60;
        const int KNOCKBACK_TIME = 10;
        const float KNOCKBACK_SPEED = 8.0f;
        const int BLINK_DELAY = 2;
        const int SWORD_LENGTH = 38;
        const int SWORD_HITBOX_LENGTH = 6;
        const int MAX_HEALTH = 8;
        const int DAMAGE = 1;
        const int START_TO_PUSH_TIME = 30;
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

        private SoundEffect swordSwingSound;
        private SoundEffect swordTink;
        private SoundEffect hitSound;

        public Rectangle SwordHitBox
        {
            get
            {
                return new Rectangle((int)Math.Round(swordHitPoint.X) - (SWORD_HITBOX_LENGTH / 2),
                    (int)Math.Round(swordHitPoint.Y) - (SWORD_HITBOX_LENGTH / 2), SWORD_HITBOX_LENGTH, SWORD_HITBOX_LENGTH);
            }
        }

        public Vector2 FootPosition
        {
            get
            {
                return new Vector2(HitBoxPosition.X + (Width / 2), HitBoxPosition.Y + Height);
            }
        }

        public bool CanMove
        {
            get
            {
                return !(State == PlayerState.Attacking || isKnockedBack ||
                  State == PlayerState.Pushing || State == PlayerState.OpeningChest);
            }
        }

        private Inventory inventory;
        public Inventory Inventory { get { return inventory; } }
        private bool isHurt = false;
        public bool IsHurt { get { return isHurt; } }
        private bool isReadyToPush = false;
        public bool IsReadyToPush { get { return isReadyToPush; } }
        private bool isTryingToInteract = false;
        public bool IsTryingToInteract { get { return isTryingToInteract; } }
        private PlayerState state = PlayerState.Normal;
        public PlayerState State { get { return state; } }

        private GamePadState gamepadState;
        private GamePadState previousGamepadState;
        private Vector2 swordHitPoint = Vector2.Zero;
        private int attackTimer = 0;
        private Vector2 forwardSwordOrigin = new Vector2(20, 18);
        private Vector2 backwardSwordOrigin = new Vector2(1, 13);
        private Vector2 leftSwordOrigin = new Vector2(10, 17);
        private Vector2 rightSwordOrigin = new Vector2(11, 17);
        private bool isKnockedBack = false;
        private bool isAiming = false;
        private int hurtTimer = 0;
        private int knockBackTimer = 0;
        private int startToPushTimer = 0;
        private MovableBlock blockBeingPushed = null;
        private CarriableEntity entityBeingCarried = null;
        private EquippableItem equippableItemBeingUsed = null;
        private Chest chestBeingOpened = null;
        private bool isEntering = false;
        private Vector2 enterPosition = Vector2.Zero;
        private Vector2 previousVelocity= Vector2.Zero;

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

            FaceDirection = Directions.Down;
            CurrentSprite = GetStandingSprite(FaceDirection);

            MaxHealth = MAX_HEALTH;
            Health = MaxHealth;
            Damage = DAMAGE;

            gamepadState = GamePad.GetState(PlayerIndex.One);
            previousGamepadState = gamepadState;

            inventory = new Inventory();

            //Bow bow = new Bow(game, null);
            //bow.LoadContent();
            //inventory.CollectEquippableItem(bow);
            //inventory.EquipItem(bow, Buttons.B);
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

            swordSwingSound = game.Content.Load<SoundEffect>("Audio/Player/sword_swing");
            swordTink = game.Content.Load<SoundEffect>("Audio/sword_tink");
            hitSound = game.Content.Load<SoundEffect>("Audio/player_hit");
        }

        public override void Update()
        {
            if (game.IsMapTransitioning || isEntering)
            {
                CurrentSprite.UpdateAnimation();
                Origin += Velocity;

                if (isEntering)
                {
                    if ((Velocity.X != 0 && (Math.Abs(Origin.X - enterPosition.X) > ENTER_DISTANCE)) ||
                        (Velocity.Y != 0 && (Math.Abs(Origin.Y - enterPosition.Y) > ENTER_DISTANCE)))
                    {
                        isEntering = false;
                        Velocity = Vector2.Zero;
                    }
                }

                return;
            }

            gamepadState = GamePad.GetState(PlayerIndex.One);

            if (IsHurt)
            {
                hurtTimer++;
                if (hurtTimer >= HURT_TIME)
                {
                    isHurt = false;
                    hurtTimer = 0;
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
            if (CanMove)
            {
                updateMovement();

                pushAroundWalls();
            }

            handleInput();

            checkReadyToPush();


            CanLeaveArea = State == PlayerState.Normal;
            

            if (State == PlayerState.Attacking)
            {
                doAttack();
            }
            if (State == PlayerState.Pushing)
            {
                doPush();
            }
            if (State == PlayerState.Carrying)
            {
                doCarry();
            }
            if (State == PlayerState.UsingEquippableItem)
            {
                useEquippableItem();
            }
            if (State == PlayerState.OpeningChest)
            {
                doOpenChest();
            }

            base.Update();

            previousGamepadState = gamepadState;
        }

        private void checkReadyToPush()
        {
            // update startToPushTimer if player is pushing against wall and check if player is ready to push
            if (JustCollidedWithWall)
            {
                if ((Velocity.Y == 0f && previousVelocity.Y == 0f && Velocity.X != 0f && Velocity.X == previousVelocity.X) ||
                    (Velocity.X == 0f && previousVelocity.X == 0f && Velocity.Y != 0f && Velocity.Y == previousVelocity.Y))
                    startToPushTimer++;
                else
                    startToPushTimer = 0;
            }
            else
            {
                startToPushTimer = 0;
            }

            isReadyToPush = startToPushTimer >= START_TO_PUSH_TIME ? true : false;
            previousVelocity = new Vector2(Velocity.X, Velocity.Y);
        }

        private void handleInput()
        {
            if (CanEnterState(PlayerState.Attacking))
            {
                if (gamepadState.IsButtonDown(Buttons.X) && previousGamepadState.IsButtonUp(Buttons.X))
                {
                    startAttack();
                }
            }

            // handle input for equippable items
            foreach (Buttons button in Inventory.EQUIPPED_ITEM_BUTTONS)
            {
                EquippableItem equippedItem = inventory.GetEquippedItemForButton(button);

                if (CanEnterState(PlayerState.UsingEquippableItem) && gamepadState.IsButtonDown(button) && equippedItem != null)
                {
                    startUsingEquippableItem(equippedItem);
                }

                if (State == PlayerState.UsingEquippableItem && gamepadState.IsButtonUp(button) && equippedItem == equippableItemBeingUsed)
                {
                    if (equippedItem is Bow)
                    {
                        Bow bow = (Bow)equippedItem;
                        bow.FireArrow();
                    }
                }
            }

            isTryingToInteract = false;
            if (gamepadState.IsButtonDown(Buttons.A) && previousGamepadState.IsButtonUp(Buttons.A))
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

            Velocity.X = motion.X * (isAiming ? AIMING_WALK_SPEED : WALK_SPEED);
            Velocity.Y = motion.Y * (isAiming ? AIMING_WALK_SPEED : WALK_SPEED);

            if (!isAiming)
            {
                updateFaceDirection();
            }
            else if (isAiming)
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
                isAiming = false;
            }

            state = PlayerState.Normal;
            equippableItemBeingUsed = null;
        }

        private void startUsingEquippableItem(EquippableItem equippableItem)
        {
            state = PlayerState.UsingEquippableItem;
            equippableItemBeingUsed = equippableItem;
            equippableItem.StartBeingUsed();

            if (equippableItem is Bow)
            {
                isAiming = true;
            }
        }

        private void startThrowing()
        {
            entityBeingCarried.StartBeingThrown(FaceDirection);
            game.CurrentArea.Entities.Add(entityBeingCarried);
            entityBeingCarried = null;
            state = PlayerState.Normal;
        }

        public void StartLifting(CarriableEntity entity)
        {
            entityBeingCarried = entity;
            entityBeingCarried.StartBeingLifted();
            game.CurrentArea.Entities.Remove(entityBeingCarried);
            state = PlayerState.Carrying;
        }

        public void StartPushing(MovableBlock block)
        {
            blockBeingPushed = block;
            Velocity = Entity.GetDirectionVector(FaceDirection) * PUSH_SPEED;

            blockBeingPushed.StartBeingPushed(FaceDirection);
            state = PlayerState.Pushing;
            isReadyToPush = false;
        }

        public void StartOpening(Chest chest)
        {
            chestBeingOpened = chest;
            chestBeingOpened.StartOpening();
            state = PlayerState.OpeningChest;
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
            state = PlayerState.Normal;
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
            startToPushTimer = 0;
            blockBeingPushed = null;
            state = PlayerState.Normal;
            isReadyToPush = false;
        }

        private void startAttack()
        {
            state = PlayerState.Attacking;
            attackTimer = 0;
            Velocity = Vector2.Zero;

            if (FaceDirection == Directions.Down)
            {
                swordHitPoint = new Vector2(HitBoxPosition.X + forwardSwordOrigin.X - SWORD_LENGTH, HitBoxPosition.Y + forwardSwordOrigin.Y);
            }
            else if (FaceDirection == Directions.Up)
            {
                swordHitPoint = new Vector2(HitBoxPosition.X + backwardSwordOrigin.X + SWORD_LENGTH, HitBoxPosition.Y + backwardSwordOrigin.Y);
            }
            else if (FaceDirection == Directions.Left)
            {
                swordHitPoint = new Vector2(HitBoxPosition.X + leftSwordOrigin.X, HitBoxPosition.Y + leftSwordOrigin.Y - SWORD_LENGTH);
            }
            else if (FaceDirection == Directions.Right)
            {
                swordHitPoint = new Vector2(HitBoxPosition.X + rightSwordOrigin.X, HitBoxPosition.Y + leftSwordOrigin.Y - SWORD_LENGTH);
            }

            CurrentSprite = GetAttackingSprite(FaceDirection);

            CurrentSprite.ResetAnimation();
            swordSwingSound.Play(0.75f, 0f, 0f);
        }

        private void doAttack()
        {
            attackTimer++;
            if (attackTimer >= ATTACK_TIME)
            {
                endAttack();
            }
            else
            {
                float angle = ((float)attackTimer / (float)(ATTACK_TIME - 1)) * MathHelper.PiOver2;
                if (FaceDirection == Directions.Left || FaceDirection == Directions.Right)
                    angle = MathHelper.PiOver2 - angle;
                int x = (int)(Math.Cos(angle) * SWORD_LENGTH);
                int y = (int)(Math.Sin(angle) * SWORD_LENGTH);
                
                if (FaceDirection == Directions.Down)
                {
                    swordHitPoint = new Vector2(HitBoxPosition.X + forwardSwordOrigin.X - x, HitBoxPosition.Y + forwardSwordOrigin.Y + y);
                }
                else if (FaceDirection == Directions.Up)
                {
                    swordHitPoint = new Vector2(HitBoxPosition.X + backwardSwordOrigin.X + x, HitBoxPosition.Y + backwardSwordOrigin.Y - y);
                }
                else if (FaceDirection == Directions.Left)
                {
                    swordHitPoint = new Vector2(HitBoxPosition.X + leftSwordOrigin.X - x, HitBoxPosition.Y + leftSwordOrigin.Y - y);
                }
                else if (FaceDirection == Directions.Right)
                {
                    swordHitPoint = new Vector2(HitBoxPosition.X + rightSwordOrigin.X + x, HitBoxPosition.Y + leftSwordOrigin.Y - y);
                }

            }
        }

        private void endAttack()
        {
            state = PlayerState.Normal;
            attackTimer = 0;
            swordHitPoint = Vector2.Zero;

            //if (FaceDirection == Directions.Down)
            //    CurrentSprite = forwardSprite;
            //else if (FaceDirection == Directions.Up)
            //    CurrentSprite = backwardSprite;
            //else if (FaceDirection == Directions.Left)
            //    CurrentSprite = leftSprite;
            //else if (FaceDirection == Directions.Right)
            //    CurrentSprite = rightSprite;
            CurrentSprite = GetStandingSprite(FaceDirection);
        }

        public void StartEnteringArea(Directions direction)
        {
            isEntering = true;
            enterPosition = new Vector2(Origin.X, Origin.Y);

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
            if (IsHurt)
            {
                int n = hurtTimer % (BLINK_DELAY * 2);
                if (n < BLINK_DELAY)
                    shouldDraw = false;
            }

            Entity equippableItemBeingUsedEntity = (Entity)equippableItemBeingUsed;

            if (state == PlayerState.UsingEquippableItem && FaceDirection != Directions.Down)
                equippableItemBeingUsedEntity.Draw(spriteBatch, changeColorsEffect);
            if (shouldDraw)
                base.Draw(spriteBatch, changeColorsEffect);
            if (state == PlayerState.UsingEquippableItem && FaceDirection == Directions.Down)
                equippableItemBeingUsedEntity.Draw(spriteBatch, changeColorsEffect);
            if (state == PlayerState.Carrying && entityBeingCarried != null)
                entityBeingCarried.Draw(spriteBatch, changeColorsEffect);

            //if (IsAttacking)
            //    spriteBatch.Draw(wallTexture, SwordHitBox, Color.Red);
        }


        public override void OnEntityCollision(Entity other)
        {
            if (other is Enemy)
            {
                Enemy enemy = (Enemy)other;
                if (!IsHurt && enemy.DoesDamageOnContact)
                {
                    takeDamageFrom(other);
                }
            }

            if (other is Spikes)
            {
                Spikes spikes = (Spikes)other;
                if (spikes.AreActivated && !IsHurt)
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
                if (!IsHurt)
                    takeDamageFrom(other);
            }
        }

        private void takeDamageFrom(Entity other)
        {
            isHurt = true;
            KnockBack(other);
            hurtTimer = 0;
            Health -= other.Damage;
            hitSound.Play(0.75f, 0, 0);
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
                    //if (FaceDirection == Directions.Down)
                    //    CurrentSprite = forwardSprite;
                    //else if (FaceDirection == Directions.Up)
                    //    CurrentSprite = backwardSprite;
                    //else if (FaceDirection == Directions.Left)
                    //    CurrentSprite = leftSprite;
                    //else if (FaceDirection == Directions.Right)
                    //    CurrentSprite = rightSprite;
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
            else if (state == PlayerState.UsingEquippableItem)
                return this.State == PlayerState.Normal;
            else if (state == PlayerState.OpeningChest)
                return this.State == PlayerState.Normal;
            return true;
        }

        public void EnterState(PlayerState state)
        {
            EnterState(state, null);
        }

        public void EnterState(PlayerState state, Entity associatedEntity)
        {

        }

    }

    public enum PlayerState
    {
        Normal,
        Attacking,
        Pushing,
        Carrying,
        UsingEquippableItem,
        OpeningChest
    }
}
