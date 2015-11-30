using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timber_and_Stone;
using Timber_and_Stone.Invasion;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class UnitEnemy
    {
        public const String GOBLIN_INFANTRY = "Goblin Infantry";
        public const String GOBLIN_ARCHER = "Goblin Archer";
        public const String SKELETON_INFANTRY = "Skeleton Infantry";
        public const String SKELETON_ARCHER = "Skeleton Archer";
        public const String NECROMANCER = "Necromancer";

        public const String HUMAN_INFANTRY = "Human Infantry";
        public const String HUMAN_ARCHER = "Human Archer";

        public static UnitEnemy GoblinInfantry = new UnitEnemy(GOBLIN_INFANTRY);
        public static UnitEnemy GoblinArcher = new UnitEnemy(GOBLIN_ARCHER);
        public static UnitEnemy SkeletonInfantry = new UnitEnemy(SKELETON_INFANTRY);
        public static UnitEnemy SkeletonArcher = new UnitEnemy(SKELETON_ARCHER);
        public static UnitEnemy Necromancer = new UnitEnemy(NECROMANCER);

        public static UnitEnemy HumanInfantry = new UnitEnemy(HUMAN_INFANTRY);
        public static UnitEnemy HumanArcher = new UnitEnemy(HUMAN_ARCHER);

        public static UnitEnemy[] List = new UnitEnemy[] 
        {
            HumanInfantry, HumanArcher, GoblinInfantry, GoblinArcher, SkeletonInfantry, SkeletonArcher, Necromancer
        };

        public static UnitEnemy Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }


        private String name;

        private UnitEnemy(String name)
        {
            this.name = name;
        }

        public String Name { get { return this.name; } }
    }
}
