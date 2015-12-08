using System;
using System.Collections.Generic;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public static class UnitPreference
    {
        public const String NPC_FRIENDLY = "blowy.npc.friendly";
        public const String NPC_ARCHER = "blowy.npc.archer";

        public static void set(ALivingEntity entity, String prefernceName, bool value)
        {
            if (!isPlayableEntity(entity)) return;

            entity.GetComponent<APlayableEntity>().preferences[prefernceName] = value;
        }

        public static bool isFriendlyNPC(ALivingEntity entity)
        {
            if (!isPlayableEntity(entity)) return false;

            return entity.GetComponent<APlayableEntity>().preferences[NPC_FRIENDLY];
        }

        public static bool isArcherNPC(ALivingEntity entity)
        {
            if (!isPlayableEntity(entity)) return false;

            return entity.GetComponent<APlayableEntity>().preferences[NPC_ARCHER];
        }

        public static bool isPlayableEntity(ALivingEntity entity)
        {
            return entity != null && entity is APlayableEntity;
        }

        private static Dictionary<APlayableEntity, APlayableEntity.Preferences> originalTraits
            = new Dictionary<APlayableEntity, APlayableEntity.Preferences>();

        private static APlayableEntity.Preferences originalPreferences;
        public static void setBestTraits(APlayableEntity entity)
        {
            originalPreferences = null;

            if (!originalTraits.ContainsKey(entity))
            {
                originalPreferences = new APlayableEntity.Preferences();
            }

            foreach (UnitTrait trait in UnitTrait.List)
            {
                if (originalPreferences != null)
                {
                    originalPreferences[trait.Name] = entity.preferences[trait.Name];
                }

                entity.preferences[trait.Name] = trait.Type == UnitTraitType.GOOD;
            }

            if (originalPreferences != null)
            {
                originalTraits.Add(entity, originalPreferences);
            }
        }

        private static APlayableEntity.Preferences existingPreferences;
        public static void restoreTraits(APlayableEntity entity)
        {
            if (originalTraits.TryGetValue(entity, out existingPreferences))
            {
                foreach (UnitTrait trait in UnitTrait.List)
                {
                    entity.preferences[trait.Name] = existingPreferences[trait.Name];
                }

                originalTraits.Remove(entity);
            }
        }
    }
}
