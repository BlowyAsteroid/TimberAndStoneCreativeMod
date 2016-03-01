using Plugin.BlowyAsteroid.TimberAndStoneMod.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class PlayerUnitSettings
    {
        public List<UnitSetting> UnitSettings { get; set; }

        public PlayerUnitSettings() 
        {
            this.UnitSettings = new List<UnitSetting>();
        }

        public static PlayerUnitSettings fromEntity(APlayableEntity entity, IUnitSettingCollectionItem[] collection)
        {
            List<UnitSetting> tempSettings = new List<UnitSetting>();
            UnitSetting tempSetting;
            foreach (IUnitSettingCollectionItem item in collection)
            {
                tempSetting = new UnitSetting();
                tempSetting.Item = item;
                tempSetting.Enabled = UnitPreference.getPreference(entity, item.Name);

                tempSettings.Add(tempSetting);
            }
            
            PlayerUnitSettings playerUnitSettings = new PlayerUnitSettings();
            playerUnitSettings.UnitSettings = tempSettings;

            return playerUnitSettings;
        }

        public UnitSetting getSetting(IUnitSettingCollectionItem item)
        {
            return this.UnitSettings.Where(s => s.Item == item).FirstOrDefault();
        }

        public void setSetting(IUnitSettingCollectionItem item, bool enabled)
        {
            getSetting(item).Enabled = enabled;
        }

        public sealed class UnitSetting
        {
            public IUnitSettingCollectionItem Item { get; set; }
            public bool Enabled;
        }

        public static void updatePlayerUnitSettings(ref PlayerUnitSettings playerUnitSettings, APlayableEntity entity, IUnitSettingCollectionItem[] collection)
        {
            if (UnitPreference.isPlayableEntity(entity))
            {
                playerUnitSettings = PlayerUnitSettings.fromEntity(entity, collection);
            }
            else
            {
                playerUnitSettings = null;
            }
        }

        public static void setPlayerUnitSettings(PlayerUnitSettings playerUnitSettings, APlayableEntity entity, IUnitSettingCollectionItem[] collection)
        {
            if (playerUnitSettings != null)
            {
                foreach (IUnitSettingCollectionItem item in collection)
                {
                    UnitPreference.setPreference(entity, item.Name, playerUnitSettings.getSetting(item).Enabled);
                }
            }
        }
    }
}
