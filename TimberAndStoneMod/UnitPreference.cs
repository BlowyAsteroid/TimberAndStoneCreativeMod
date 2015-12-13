using System;
using System.Collections.Generic;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public static class UnitPreference
    {
        public const String WAIT_IN_HALL_WHILE_IDLE = "preference.waitinhallwhileidle";
        public const String TRAIN_UNDER_LEVEL_3 = "preference.train";
        public const String CHOP_CLOSEST_TREE = "preference.ChopClosestTree";
        public const String HUNT_BERRIES = "preference.hunt.berries";
        public const String HUNT_GRASS = "preference.hunt.grass";
        public const String HUNT_CHICKEN = "preference.hunt.chicken";
        public const String HUNT_BOAR = "preference.hunt.boar";
        public const String HUNT_SHEEP = "preference.hunt.sheep";
        public const String CORPSE_LOOT = "preference.corpse.loot";
        public const String CORPSE_DISPOSE = "preference.corpse.dispose";
        public const String PATROL = "preference.patrol";
        public const String GUARD = "preference.guard";
        public const String ATTACK_CHARGE_TARGETS = "preference.attackchargetargets";
        public const String OPERATE_SIEGE = "preference.operatesiege";
        public const String TRAIN_INFANTRY = "preference.train.infantry";
        public const String TRAIN_ARCHER = "preference.train.archer";
        public const String ARCHER_PURSUE = "preference.archer.pursue";

        public static void setPreference(ALivingEntity entity, String preferenceName, bool value)
        {
            if (!isPlayableEntity(entity)) return;

            entity.GetComponent<APlayableEntity>().preferences[preferenceName] = value;
        }

        public static bool getPreference(ALivingEntity entity, String preferenceName)
        {
            if (!isPlayableEntity(entity)) return false;

            return entity.GetComponent<APlayableEntity>().preferences[preferenceName];
        }

        public static bool isPlayableEntity(ALivingEntity entity)
        {
            return entity != null && entity is APlayableEntity;
        }
    }
}
