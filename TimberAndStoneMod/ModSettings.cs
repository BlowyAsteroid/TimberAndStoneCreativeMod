using System;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class ModSettings
    {
        private static ModSettings instance = new ModSettings();
        public static ModSettings getInstance() { return instance; }

        public const String IS_CREATIVE_ENABLED = "blowy.creative";
        public const String IS_CHEATS_ENABLED = "blowy.cheats";
        public const String IS_PEACEFUL_ENABLED = "blowy.cheats.peaceful";
        public const String IS_ALWAYS_DAYTIME_ENABLED = "blowy.cheats.daytime";
        public const String IS_INFINITE_MATERIALS_ENABLED = "blowy.cheats.infinitematerials";
        public const String IS_UNLIMITED_STORAGE_ENABLED = "blowy.cheats.unlimitedstorage";

        private const String NEW_SETTLEMENT = "New Settlement";

        public bool isCreativeEnabled;
        public bool isCheatsEnabled;
        public bool isPeacefulEnabled;
        public bool isAlwaysDaytimeEnabled;
        public bool isInfiniteMaterialsEnabled;
        public bool isUnlimitedStorageEnabled;

        public bool hasSettings { get { return this.isSettingsLoaded; } }
        private bool isSettingsLoaded = false;

        private ModSettings() { }

        public void loadSettings(String settlementName)
        {
            if (!isValidSettlementName(settlementName)) return;

            String prefix = settlementName + ".";

            isCreativeEnabled = getPlayerSetting(prefix + IS_CREATIVE_ENABLED);
            isCheatsEnabled = getPlayerSetting(prefix + IS_CHEATS_ENABLED);
            isPeacefulEnabled = getPlayerSetting(prefix + IS_PEACEFUL_ENABLED);
            isAlwaysDaytimeEnabled = getPlayerSetting(prefix + IS_ALWAYS_DAYTIME_ENABLED);
            isInfiniteMaterialsEnabled = getPlayerSetting(prefix + IS_INFINITE_MATERIALS_ENABLED);
            isUnlimitedStorageEnabled = getPlayerSetting(prefix + IS_UNLIMITED_STORAGE_ENABLED);

            isSettingsLoaded = true;
        }

        public void saveSettings(String settlementName)
        {
            if (!isValidSettlementName(settlementName)) return;

            String prefix = settlementName + ".";

            setPlayerSetting(prefix + IS_CREATIVE_ENABLED, this.isCreativeEnabled);
            setPlayerSetting(prefix + IS_CHEATS_ENABLED, this.isCheatsEnabled);
            setPlayerSetting(prefix + IS_PEACEFUL_ENABLED, this.isPeacefulEnabled);
            setPlayerSetting(prefix + IS_ALWAYS_DAYTIME_ENABLED, this.isAlwaysDaytimeEnabled);
            setPlayerSetting(prefix + IS_INFINITE_MATERIALS_ENABLED, this.isInfiniteMaterialsEnabled);
            setPlayerSetting(prefix + IS_UNLIMITED_STORAGE_ENABLED, this.isUnlimitedStorageEnabled);
        }

        public bool isValidSettlementName(String settlementName)
        {
            return settlementName != String.Empty && settlementName != NEW_SETTLEMENT;
        }

        private void setPlayerSetting(String name, bool value)
        {
            PlayerPrefs.SetInt(name, Convert.ToInt32(value));
        }

        private bool getPlayerSetting(String name)
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(name));
        }
    }
}
