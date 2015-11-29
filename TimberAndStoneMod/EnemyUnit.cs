using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timber_and_Stone;
using Timber_and_Stone.Invasion;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class EnemyUnit
    {
        public const String GOBLIN_INFANTRY = "Goblin Infantry";
        public const String GOBLIN_ARCHER = "Goblin Archer";
        public const String SKELETON_INFANTRY = "Skeleton Infantry";
        public const String SKELETON_ARCHER = "Skeleton Archer";
        public const String NECROMANCER = "Necromancer";

        public const String HUMAN_INFANTRY = "Human Infantry";
        public const String HUMAN_ARCHER = "Human Archer";

        public static EnemyUnit GoblinInfantry = new EnemyUnit(GOBLIN_INFANTRY);
        public static EnemyUnit GoblinArcher = new EnemyUnit(GOBLIN_ARCHER);
        public static EnemyUnit SkeletonInfantry = new EnemyUnit(SKELETON_INFANTRY);
        public static EnemyUnit SkeletonArcher = new EnemyUnit(SKELETON_ARCHER);
        public static EnemyUnit Necromancer = new EnemyUnit(NECROMANCER);

        public static EnemyUnit HumanInfantry = new EnemyUnit(HUMAN_INFANTRY);
        public static EnemyUnit HumanArcher = new EnemyUnit(HUMAN_ARCHER);

        public static EnemyUnit[] List = new EnemyUnit[] 
        {
            HumanInfantry, HumanArcher, GoblinInfantry, GoblinArcher, SkeletonInfantry, SkeletonArcher, Necromancer
        };

        public static EnemyUnit Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }


        private String name;

        private EnemyUnit(String name)
        {
            this.name = name;
        }

        public String Name { get { return this.name; } }
    }
}
