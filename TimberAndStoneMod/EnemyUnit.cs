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
        public const String GOBLIN = "Goblin";
        public const String MOUNTED_GOBLIN = "Mounted Goblin";
        public const String SKELETON = "Skeleton";
        public const String NECROMANCER = "Necromancer";
        public const String WOLF = "Wolf";
        public const String SPIDER = "Spider";
        public const String SPIDER_LORD = "Spider Lord";

        public static EnemyUnit Goblin = new EnemyUnit(GOBLIN);
        //public static EnemyUnit MountedGoblin = new EnemyUnit(MOUNTED_GOBLIN);
        public static EnemyUnit Skeleton = new EnemyUnit(SKELETON);
        public static EnemyUnit Necromancer = new EnemyUnit(NECROMANCER);
        //public static EnemyUnit Wolf = new EnemyUnit(WOLF);
        //public static EnemyUnit Spider = new EnemyUnit(SPIDER);
        //public static EnemyUnit SpiderLord = new EnemyUnit(SPIDER_LORD);

        public static EnemyUnit[] List = new EnemyUnit[] 
        {
            Goblin, Skeleton, Necromancer
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
