using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    class ModSettingsComponent : ModComponent
    {
        public void Start()
        {
            setUpdatesPerSecond(1);
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape) && modSettings.isHasSettings && Time.timeScale <= 0)
            {
                modSettings.saveSettings(worldManager.settlementName);
                log("Settings saved for: " + worldManager.settlementName);
            }

            if (modSettings.isHasSettings || !isTimeToUpdate(DateTime.Now.Ticks)) return;

            if (modSettings.isValidSettlementName(worldManager.settlementName))
            {
                modSettings.loadSettings(worldManager.settlementName);
                log("Settings loaded for: " + worldManager.settlementName);
            }
        }
    }
}
