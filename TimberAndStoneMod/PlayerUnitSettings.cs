using Plugin.BlowyAsteroid.Collections.TimberAndStoneMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class PlayerUnitSettings
    {
        public List<UnitSetting> UnitSettings { get; set; }

        public PlayerUnitSettings() 
        {
            this.UnitSettings = new List<UnitSetting>();
        }

        public static PlayerUnitSettings fromEntity(APlayableEntity entity, IModCollectionItem[] collection)
        {
            List<UnitSetting> tempSettings = new List<UnitSetting>();
            UnitSetting tempSetting;
            foreach (IModCollectionItem item in collection)
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

        public UnitSetting getSetting(IModCollectionItem item)
        {
            return this.UnitSettings.Where(s => s.Item == item).FirstOrDefault();
        }

        public void setSetting(IModCollectionItem item, bool enabled)
        {
            getSetting(item).Enabled = enabled;
        }

        public sealed class UnitSetting
        {
            public IModCollectionItem Item { get; set; }
            public bool Enabled;
        }
    }
}
