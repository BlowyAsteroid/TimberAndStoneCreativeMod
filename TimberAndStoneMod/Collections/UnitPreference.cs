using System;
using System.Collections.Generic;

namespace Plugin.BlowyAsteroid.Collections.TimberAndStoneMod
{
    public sealed class UnitPreference : IModCollectionItem
    {
        public const String IS_PLAYER_UNIT = "blowy.playerunit";

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

        public static readonly UnitPreference WaitInHallWhileIdle = new UnitPreference(WAIT_IN_HALL_WHILE_IDLE, "Wait in hall while idle", UnitHuman.All);
        public static readonly UnitPreference TrainUnderLevel3 = new UnitPreference(TRAIN_UNDER_LEVEL_3, "Train under Lvl.3", UnitHuman.Crafter);
        public static readonly UnitPreference ChopNearestTrees = new UnitPreference(CHOP_CLOSEST_TREE, "Chop nearest trees", UnitHuman.WoodChopper);
        public static readonly UnitPreference HuntBerries = new UnitPreference(HUNT_BERRIES, "Gather berries", UnitHuman.Forager);
        public static readonly UnitPreference HuntGrass = new UnitPreference(HUNT_GRASS, "Gather wild wheat", UnitHuman.Forager);
        public static readonly UnitPreference HuntBoar = new UnitPreference(HUNT_BOAR, "Hunt boar", UnitHuman.Forager);
        public static readonly UnitPreference HuntChicken = new UnitPreference(HUNT_CHICKEN, "Hunt chicken", UnitHuman.Forager);
        public static readonly UnitPreference HuntSheep = new UnitPreference(HUNT_SHEEP, "Hunt sheep", UnitHuman.Forager);
        public static readonly UnitPreference LootCorpses = new UnitPreference(CORPSE_LOOT, "Loot dead", UnitHuman.Forager);
        public static readonly UnitPreference DisposeCorpses = new UnitPreference(CORPSE_DISPOSE, "Dispose dead", UnitHuman.Forager);
        public static readonly UnitPreference Patrol = new UnitPreference(PATROL, "Walk patrol routes", UnitHuman.Military);
        public static readonly UnitPreference Guard = new UnitPreference(GUARD, "Stand at guard positions", UnitHuman.Military);
        public static readonly UnitPreference ChargeTargets = new UnitPreference(ATTACK_CHARGE_TARGETS, "Attack charged enemies", UnitHuman.Infantry);
        public static readonly UnitPreference PursueTargets = new UnitPreference(OPERATE_SIEGE, "Pursue enemies", UnitHuman.Archer);
        public static readonly UnitPreference OperateSiege = new UnitPreference(OPERATE_SIEGE, "Operate siege equipment", UnitHuman.Military);
        public static readonly UnitPreference TrainInfantry = new UnitPreference(TRAIN_INFANTRY, "Train using dummies", UnitHuman.Infantry);
        public static readonly UnitPreference TrainArcher = new UnitPreference(TRAIN_ARCHER, "Train using targets", UnitHuman.Archer);

        public static readonly UnitPreference[] List = new UnitPreference[] 
        { 
            WaitInHallWhileIdle, TrainUnderLevel3, ChopNearestTrees, HuntBerries, HuntGrass, HuntChicken, HuntBoar, HuntSheep, 
            LootCorpses, DisposeCorpses, Patrol, Guard, ChargeTargets, PursueTargets, OperateSiege, TrainInfantry, TrainArcher
        };

        public static readonly List<UnitPreference> ArcherGroupList = new List<UnitPreference>()
        {
            Patrol, Guard, OperateSiege, TrainArcher
        };

        public static readonly List<UnitPreference> InfantryGroupList = new List<UnitPreference>()
        {
            Patrol, Guard, OperateSiege, TrainInfantry
        };

        public static bool isPartOfGroup(UnitPreference preference)
        {
            return ArcherGroupList.Contains(preference) || InfantryGroupList.Contains(preference);
        }

        private static List<UnitPreference> typePreferences = new List<UnitPreference>();
        private static List<UnitPreference> getPreferences(UnitHuman unitType)
        {
            typePreferences.Clear();

            foreach (UnitPreference preference in List)
            {
                if (UnitHuman.isRelatedTo(preference.unitType, unitType))
                {
                    typePreferences.Add(preference);
                }
            }

            return typePreferences;
        }

        public static UnitPreference[] getPreferences(APlayableEntity entity)
        {
            return getPreferences(UnitHuman.valueOf(entity.getProfession().getProfessionName())).ToArray();
        }

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

        private String preferenceName;
        private String description;
        private UnitHuman unitType;
        private UnitPreference(String preferenceName, String description, UnitHuman unitType)
        {
            this.preferenceName = preferenceName;
            this.description = description;
            this.unitType = unitType;
        }
        public String Name { get { return this.preferenceName; } }
        public String Description { get { return this.description; } }
        public UnitHuman UnitType { get { return this.unitType; } }
    }
}
