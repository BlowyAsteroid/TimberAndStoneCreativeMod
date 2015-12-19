using System;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class GameSpeedComponent : PluginComponent
    {
        private const float MAX_GAME_SPEED = 5f;
        private const KeyCode INCREASE_KEY = KeyCode.Equals;
        private const KeyCode DECREASE_KEY = KeyCode.Minus;

        public override void OnStart() 
        {
            log(String.Format("Press {0} to increase game speed.", INCREASE_KEY));
            log(String.Format("Press {0} to decrease game speed.", DECREASE_KEY));
        }

        public override void OnInput()
        {
            if (Input.GetKeyUp(INCREASE_KEY))
            {
                updateGameSpeed(1);
            }
            else if (Input.GetKeyUp(DECREASE_KEY))
            {
                if (Time.timeScale <= 1f)
                {
                    timeManager.pause();
                }
                else updateGameSpeed(-1);
            }
        }

        private void updateGameSpeed(int direction)
        {
            if (direction > 0 ? Time.timeScale < MAX_GAME_SPEED : Time.timeScale > 1)
            {
                timeManager.play(Time.timeScale + direction);
                log("Speed: x" + Time.timeScale.ToString());
            }
        }
    }
}
