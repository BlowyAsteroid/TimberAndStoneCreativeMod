using System;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class ModSettings
    {
        private static ModSettings instance = new ModSettings();
        public static ModSettings getInstance() { return instance; }

        public static void setPlayerSetting(String name, bool value)
        {
            PlayerPrefs.SetInt(name, Convert.ToInt32(value));
        }

        public static bool getPlayerSetting(String name)
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(name));
        }

        public static bool isPreventDeathEnabled { get { return instance.isCheatsEnabled && instance.isPeacefulEnabled; } }
        public static bool isPreventInvasionsEnabled { get { return instance.isCheatsEnabled && instance.isPeacefulEnabled; } }

        public bool isPeacefulEnabled { get; set; }
        public bool isCheatsEnabled { get; set; }

        public bool isAlwaysDayTimeEnabled { get; set; }
        public bool isRealTimeEnabled { get; set; }       

        private ModSettings() { }
    }
}
