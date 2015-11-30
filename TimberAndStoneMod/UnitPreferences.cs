using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class UnitPreferences
    {
        public const String NPC_FRIENDLY = "blowy.npc.friendly";
        public const String NPC_ARCHER = "blowy.npc.archer";

        public static bool isFriendlyNPC(ALivingEntity entity)
        {
            if (!(entity is APlayableEntity)) return false;

            return entity.GetComponent<APlayableEntity>().preferences[UnitPreferences.NPC_FRIENDLY];
        }

        public static bool isArcherNPC(ALivingEntity entity)
        {
            if (!(entity is APlayableEntity)) return false;

            return entity.GetComponent<APlayableEntity>().preferences[UnitPreferences.NPC_ARCHER];
        }
    }
}
