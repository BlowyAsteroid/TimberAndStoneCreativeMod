using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Tasks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class FriendlyNpcComponent : ModComponent
    {        
        private UnitManager unitManager = UnitManager.getInstance();
        private WorldManager worldManager = WorldManager.getInstance();
        private static EquipmentService equipmentService = EquipmentService.getInstance();

        public void Start()
        {
            setUpdatesPerMinute(30);
        }

        public void Update()
        {
            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            foreach (ALivingEntity entity in unitManager.allUnits.Where(u => UnitPreference.isFriendlyNPC(u)))
            {
                if(!entity.taskStackContains(typeof(TaskAttack)) 
                    && !entity.taskStackContains(typeof(TaskAttackTarget)))
                {
                    entity.interruptTask(new TaskWait(10));
                }
                
                entity.spottedTimer = 15f;
                entity.hitpoints = entity.maxHP;

                entity.inventory.Clear();
                equipmentService.equipNPCWeapons(entity, UnitPreference.isArcherNPC(entity));
            }
        }

        public static void updateNPCsNear(ALivingEntity entity)
        {
            if (entity == null) return;

            foreach (ALivingEntity unit in entity.getNearbyUnits(3f, false).Where(u => UnitPreference.isFriendlyNPC(u)))
            {
                if (unit == null) continue;

                unit.interruptTask(new TaskWait(10));

                unit.inventory.Clear();
                equipmentService.equipNPCWeapons(unit, UnitPreference.isArcherNPC(unit));
            }
        }
    }
}
