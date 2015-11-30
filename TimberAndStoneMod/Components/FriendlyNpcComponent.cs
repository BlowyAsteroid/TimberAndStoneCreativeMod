using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Tasks;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class FriendlyNpcComponent : ModComponent
    {        
        private UnitManager unitManager = UnitManager.getInstance();
        private WorldManager worldManager = WorldManager.getInstance();
        private static UnitService unitService = UnitService.getInstance();

        public void Start()
        {
            setUpdatesPerMinute(30);
        }

        public void Update()
        {
            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            foreach (ALivingEntity entity in unitManager.allUnits.Where(u => UnitPreferences.isFriendlyNPC(u)))
            {
                if(!entity.taskStackContains(typeof(TaskAttack)) 
                    && !entity.taskStackContains(typeof(TaskAttackTarget)))
                {
                    entity.interruptTask(new TaskWait(10));
                }
                
                entity.spottedTimer = 15f;

                entity.inventory.Clear();
                unitService.equipNPC(entity, UnitPreferences.isArcherNPC(entity));
            }
        }

        public static void updateNPCsNear(ALivingEntity entity)
        {
            foreach (ALivingEntity unit in entity.getNearbyUnits(3f, false).Where(u => UnitPreferences.isFriendlyNPC(u)))
            {
                unit.interruptTask(new TaskWait(10));

                unit.inventory.Clear();
                unitService.equipNPC(unit, UnitPreferences.isArcherNPC(unit));
            }
        }
    }
}
