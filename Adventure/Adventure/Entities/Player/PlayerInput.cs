using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public class PlayerInput
    {
        public Directions InputDirection { get { return inputDirection; } }

        private static Dictionary<PlayerButtons, Buttons> buttonMappings = new Dictionary<PlayerButtons, Buttons>()
        {
            {PlayerButtons.Interact, Buttons.A},
            {PlayerButtons.Attack, Buttons.B},
            {PlayerButtons.EquippedItem1, Buttons.X},
            {PlayerButtons.EquippedItem2, Buttons.Y},
            {PlayerButtons.Up, Buttons.LeftThumbstickUp},
            {PlayerButtons.Down, Buttons.LeftThumbstickDown},
            {PlayerButtons.Left, Buttons.LeftThumbstickLeft},
            {PlayerButtons.Right, Buttons.LeftThumbstickRight}
        };
        private Dictionary<PlayerButtons, bool> playerButtonDownState = new Dictionary<PlayerButtons, bool>();
        private Dictionary<PlayerButtons, bool> previousPlayerButtonDownState = new Dictionary<PlayerButtons, bool>();
        private GamePadState gamePadState;
        private Player player;
        private Directions inputDirection;

        public PlayerInput(Player player)
        {
            foreach (PlayerButtons button in Enum.GetValues(typeof(PlayerButtons)))
            {
                playerButtonDownState.Add(button, false);
                previousPlayerButtonDownState.Add(button, false);
            }
            gamePadState = GamePad.GetState(PlayerIndex.One);
            this.player = player;
        }

        public static Buttons PlayerButtonsToButtons(PlayerButtons playerButton)
        {
            return buttonMappings[playerButton];
        }

        /// <summary>
        /// Updates the current state of the player input.
        /// </summary>
        public void Update()
        {
            foreach (PlayerButtons playerButton in buttonMappings.Keys)
            {
                previousPlayerButtonDownState[playerButton] = playerButtonDownState[playerButton];
            }

            gamePadState = GamePad.GetState(PlayerIndex.One);

            // determine which buttons are currently down
            foreach (PlayerButtons playerButton in buttonMappings.Keys)
            {
                playerButtonDownState[playerButton] = gamePadState.IsButtonDown(buttonMappings[playerButton]);
            }

            // determine player's desired direction based on input
            inputDirection = Directions.None;
            if (IsButtonDown(PlayerButtons.Up))
                inputDirection |= Directions.Up;
            if (IsButtonDown(PlayerButtons.Down))
                inputDirection |= Directions.Down;
            if (IsButtonDown(PlayerButtons.Left))
                inputDirection |= Directions.Left;
            if (IsButtonDown(PlayerButtons.Right))
                inputDirection |= Directions.Right;
        }

        /// <summary>
        /// Notifies the player about button presses or releases so it can handle them.
        /// </summary>
        public void Handle()
        {
            foreach (PlayerButtons playerButton in Enum.GetValues(typeof(PlayerButtons)))
            {
                if (!previousPlayerButtonDownState[playerButton] && playerButtonDownState[playerButton])
                    player.OnButtonPressed(playerButton);
                else if (previousPlayerButtonDownState[playerButton] && !playerButtonDownState[playerButton])
                    player.OnButtonReleased(playerButton);
            }
        }

        public bool IsButtonDown(PlayerButtons button)
        {
            return playerButtonDownState[button];
        }

        public bool IsEquippedItemButtonDown(int itemButtonNumber)
        {
            if (itemButtonNumber == 0)
                return playerButtonDownState[PlayerButtons.EquippedItem1];
            else if (itemButtonNumber == 1)
                return playerButtonDownState[PlayerButtons.EquippedItem2];
            return false;
        }

        public bool IsButtonUp(PlayerButtons button)
        {
            return !playerButtonDownState[button];
        }

        public bool IsEquippedItemButtonUp(int itemButtonNumber)
        {
            if (itemButtonNumber == 0)
                return !playerButtonDownState[PlayerButtons.EquippedItem1];
            else if (itemButtonNumber == 1)
                return !playerButtonDownState[PlayerButtons.EquippedItem2];
            return false;
        }
    }

    public enum PlayerButtons
    {
        None,
        Interact,
        Attack,
        EquippedItem1,
        EquippedItem2,
        Up,
        Down,
        Left,
        Right
    }
}
