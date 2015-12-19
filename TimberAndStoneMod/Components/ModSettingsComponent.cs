using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    class ModSettingsComponent : PluginComponent
    {
        private ModSettings modSettings = ModSettings.getInstance();

        public override void OnStart()
        {
            setUpdatesPerSecond(1);
        }

        public override void OnInput()
        {
            if (Input.GetKeyUp(KeyCode.Escape) && modSettings.isHasSettings && Time.timeScale <= 0)
            {
                modSettings.saveSettings(worldManager.settlementName);
                log("Settings saved for: " + worldManager.settlementName);
            }            
        }

        public override void OnUpdate()
        {
            if (modSettings.isHasSettings) return;

            if (modSettings.isValidSettlementName(worldManager.settlementName))
            {
                modSettings.loadSettings(worldManager.settlementName);
                log("Settings loaded for: " + worldManager.settlementName);
            }
        }
    }
}
