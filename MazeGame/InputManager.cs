using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace MazeGame
{
    public class InputManager
    {
        private static InputManager _instance;

        private readonly Dictionary<Keys, Action> _keyHandlers = new Dictionary<Keys, Action>();
        private readonly List<Keys> _pressedKeys = new List<Keys>();

        /// <summary>
        /// Gets the instance of the InputManager if it exists, else creates an instance.
        /// </summary>
        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InputManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Adds the specified Key-Action pair to the _keyHandlers Dictionary.
        /// </summary>
        /// <param name="key">The Key handled by the added Action.</param>
        /// <param name="action">The Action that handles the Key.</param>
        public void AddKeyHandler(Keys key, Action action)
        {
            if (!_keyHandlers.ContainsKey(key))
            {
                _keyHandlers[key] = action;
            }
            else
            {
                _keyHandlers[key] += action;
            }
        }

        /// <summary>
        /// Updates the states of the _pressedKeys and _keyHandlers dictionaries.
        /// </summary>
        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            foreach (Keys key in _keyHandlers.Keys)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    if (!_pressedKeys.Contains(key))
                    {
                        _pressedKeys.Add(key);
                        _keyHandlers[key]?.Invoke();
                    }
                }
                else
                {
                    _pressedKeys.Remove(key);
                }
            }
        }

        /// <summary>
        /// Returns whether the specified Key is currently pressed.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <returns>True if the Key is currently pressed, else false.</returns>
        public bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }
    }
}
