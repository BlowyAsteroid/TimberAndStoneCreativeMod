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
        public const String IS_SHOW_ENEMIES_ENABLED = "blowy.cheats.showenemies";
        public const String IS_AUTO_BACKUP_ENABLED = "blowy.autobackup";

        private const String NEW_SETTLEMENT = "New Settlement";

        public bool isCreativeEnabled;
        public bool isCheatsEnabled;
        public bool isPeacefulEnabled;
        public bool isAlwaysDaytimeEnabled;
        public bool isInfiniteMaterialsEnabled;
        public bool isUnlimitedStorageEnabled;
        public bool isShowEnemiesEnabled;
        public bool isAutoBackupsEnabled;

        public bool isHasSettings { get; private set; }

        private ModSettings() { }

        public void loadSettings(String settlementName = NEW_SETTLEMENT)
        {
            this.isAutoBackupsEnabled = getPlayerSetting(IS_AUTO_BACKUP_ENABLED);

            if (!isValidSettlementName(settlementName)) return;

            String prefix = settlementName + ".";

            this.isCreativeEnabled = getPlayerSetting(prefix + IS_CREATIVE_ENABLED);
            this.isCheatsEnabled = getPlayerSetting(prefix + IS_CHEATS_ENABLED);
            this.isPeacefulEnabled = getPlayerSetting(prefix + IS_PEACEFUL_ENABLED);
            this.isAlwaysDaytimeEnabled = getPlayerSetting(prefix + IS_ALWAYS_DAYTIME_ENABLED);
            this.isInfiniteMaterialsEnabled = getPlayerSetting(prefix + IS_INFINITE_MATERIALS_ENABLED);
            this.isUnlimitedStorageEnabled = getPlayerSetting(prefix + IS_UNLIMITED_STORAGE_ENABLED);
            this.isShowEnemiesEnabled = getPlayerSetting(prefix + IS_SHOW_ENEMIES_ENABLED);


            this.isHasSettings = true;
        }

        public void saveSettings(String settlementName = NEW_SETTLEMENT)
        {
            setPlayerSetting(IS_AUTO_BACKUP_ENABLED, this.isAutoBackupsEnabled);

            if (!isValidSettlementName(settlementName)) return;

            String prefix = settlementName + ".";

            setPlayerSetting(prefix + IS_CREATIVE_ENABLED, this.isCreativeEnabled);
            setPlayerSetting(prefix + IS_CHEATS_ENABLED, this.isCheatsEnabled);
            setPlayerSetting(prefix + IS_PEACEFUL_ENABLED, this.isPeacefulEnabled);
            setPlayerSetting(prefix + IS_ALWAYS_DAYTIME_ENABLED, this.isAlwaysDaytimeEnabled);
            setPlayerSetting(prefix + IS_INFINITE_MATERIALS_ENABLED, this.isInfiniteMaterialsEnabled);
            setPlayerSetting(prefix + IS_UNLIMITED_STORAGE_ENABLED, this.isUnlimitedStorageEnabled);
            setPlayerSetting(prefix + IS_SHOW_ENEMIES_ENABLED, this.isShowEnemiesEnabled);
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
